using PallasDotnet.Models;
using NextResponseRs = PallasDotnetRs.PallasDotnetRs.NextResponse;

namespace PallasDotnet;

public class N2cClient
{
    private PallasDotnetRs.PallasDotnetRs.ClientWrapper? _n2cClient;
    private string _socketPath = string.Empty;
    private ulong _magicNumber = 0;
    private byte[] _lastHash = [];
    private ulong _lastSlot = 0;
    private byte _client = 0;

    public bool IsConnected => _n2cClient != null;
    public bool IsSyncing { get; private set; }
    public bool ShouldReconnect { get; set; } = true;

    public event EventHandler? Disconnected;
    public event EventHandler? Reconnected;

    public async Task<Point> ConnectAsync(string socketPath, ulong magicNumber)
    {
        return await Task.Run(() =>
        {
            _n2cClient = PallasDotnetRs.PallasDotnetRs.Connect(socketPath, magicNumber, (byte)Client.N2C);

            if (_n2cClient is null)
            {
                throw new Exception("Failed to connect to node");
            }
            
            _magicNumber = magicNumber;
            _socketPath = socketPath;
            _client = (byte)Client.N2C;

            var pallasPoint = PallasDotnetRs.PallasDotnetRs.GetTip(_n2cClient.Value);
            return Utils.MapPallasPoint(pallasPoint);
        });
    }

    public async IAsyncEnumerable<NextResponse> StartChainSyncAsync(Point? intersection = null)
    {
        if (_n2cClient is null)
        {
            throw new Exception("Not connected to node");
        }

        if (intersection is not null)
        {
            await Task.Run(() =>
            {
                PallasDotnetRs.PallasDotnetRs.FindIntersect(_n2cClient.Value, new PallasDotnetRs.PallasDotnetRs.Point
                {
                    slot = intersection.Slot,
                    hash = new List<byte>(Convert.FromHexString(intersection.Hash))
                });
            });
        }

        IsSyncing = true;
        while (IsSyncing)
        {   
            NextResponseRs nextResponseRs = PallasDotnetRs.PallasDotnetRs.ChainSyncNext(_n2cClient.Value);

            if ((NextResponseAction)nextResponseRs.action == NextResponseAction.Error)
            {
                if (ShouldReconnect)
                {
                    _n2cClient = PallasDotnetRs.PallasDotnetRs.Connect(_socketPath, _magicNumber, _client);
                    PallasDotnetRs.PallasDotnetRs.FindIntersect(_n2cClient.Value, new PallasDotnetRs.PallasDotnetRs.Point
                    {
                        slot = _lastSlot,
                        hash = [.. _lastHash]
                    });
                    Reconnected?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    IsSyncing = false;
                    Disconnected?.Invoke(this, EventArgs.Empty);
                }
            }
            else if ((NextResponseAction)nextResponseRs.action == NextResponseAction.Await)
            {
                yield return new
                (
                    NextResponseAction.Await,
                    default!,
                    default!
                );
            }
            else
            {
                NextResponseAction nextResponseAction = (NextResponseAction)nextResponseRs.action;
                Point tip = Utils.MapPallasPoint(nextResponseRs.tip);

                NextResponse nextResponse = nextResponseAction switch
                {
                    NextResponseAction.RollForward => new(nextResponseAction, tip, [.. nextResponseRs.blockCbor]),
                    NextResponseAction.RollBack => new(nextResponseAction, tip, default!),
                    _ => default!
                };
                
                yield return nextResponse;
            }
        }
    }

    public async Task<List<byte[]>> GetUtxoByAddressCborAsync(string address)
    {
        if (_n2cClient is null)
        {
            throw new Exception("Not connected to node");
        }
        var utxoByAddress = await Task.Run(() => PallasDotnetRs.PallasDotnetRs.GetUtxoByAddressCbor(_n2cClient.Value, address));
        return utxoByAddress?.Select(utxo => utxo.ToArray()).ToList() ?? [];
    }

    public void StopSync()
    {
        IsSyncing = false;
    }

    public Task DisconnectAsync()
    {
        if (_n2cClient is null)
        {
            throw new Exception("Not connected to node");
        }
        return Task.Run(() => PallasDotnetRs.PallasDotnetRs.Disconnect(_n2cClient.Value));
    }
}
