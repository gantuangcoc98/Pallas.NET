using PallasDotnet.Models;
using NextResponseRs = PallasDotnetRs.PallasDotnetRs.NextResponse;

namespace PallasDotnet;

public class N2nClient
{
    private PallasDotnetRs.PallasDotnetRs.ClientWrapper? _n2nClient;
    private string _server = string.Empty;
    private ulong _magicNumber = 0;
    private bool IsSyncing { get; set; }
    private bool IsConnected => _n2nClient != null;
    public bool ShouldReconnect { get; set; } = true; 
    private ulong _lastSlot = 0;   
    private byte[] _lastHash = [];
    private byte _client = 0;
    public event EventHandler? Disconnected;
    public event EventHandler? Reconnected;

    public async Task<Point> ConnectAsync(string server, ulong magicNumber)
    {
        _n2nClient = PallasDotnetRs.PallasDotnetRs.Connect(server, magicNumber, (byte)Client.N2N);

        if (_n2nClient is null)
        {
            throw new Exception("Failed to connect to node");
        }

        _server = server;
        _magicNumber = magicNumber;  
        _client = (byte)Client.N2N;

        return await GetTipAsync();
    }

    public async IAsyncEnumerable<NextResponse> StartChainSyncAsync(Point? intersection = null)
    {
        if (_n2nClient is null)
        {
            throw new Exception("Not connected to node");
        }

        if (intersection is not null)
        {
            await Task.Run(() =>
            {
                PallasDotnetRs.PallasDotnetRs.FindIntersect(_n2nClient.Value, new PallasDotnetRs.PallasDotnetRs.Point
                {
                    slot = intersection.Slot,
                    hash = new List<byte>(Convert.FromHexString(intersection.Hash))
                });
            });
        }

        IsSyncing = true;
        
        while (IsSyncing)
        {
            NextResponseRs nextResponseRs = PallasDotnetRs.PallasDotnetRs.ChainSyncNext(_n2nClient.Value);

            if ((NextResponseAction)nextResponseRs.action == NextResponseAction.Error)
            {
                if (ShouldReconnect)
                {
                    _n2nClient = PallasDotnetRs.PallasDotnetRs.Connect(_server, _magicNumber, _client);

                    PallasDotnetRs.PallasDotnetRs.FindIntersect(_n2nClient.Value, new PallasDotnetRs.PallasDotnetRs.Point
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

                NextResponse nextResponse = new(nextResponseAction, tip, [.. nextResponseRs.blockCbor]);

                yield return nextResponse;
            }
        }
    }

    public async Task<byte[]> FetchBlockAsync(Point? intersection = null)
    {
        if (_n2nClient is null)
        {
            throw new Exception("Not connected to node");
        }

        if (intersection is null)
        {
            throw new Exception("Intersection not provided");
        }

        return await Task.Run(() => {
            return PallasDotnetRs.PallasDotnetRs.FetchBlock(_n2nClient.Value, new PallasDotnetRs.PallasDotnetRs.Point
            {
                slot = intersection.Slot,
                hash = new List<byte>(Convert.FromHexString(intersection.Hash))
            }).ToArray();
        });
    }

    public async Task<Point> GetTipAsync()
    {
        if (_n2nClient is null)
        {
            throw new Exception("Not connected to node");
        }

        PallasDotnetRs.PallasDotnetRs.Point tip = PallasDotnetRs.PallasDotnetRs.GetTip(_n2nClient.Value);
        
        return await Task.Run(() => {
            return Utils.MapPallasPoint(tip);
        });
    }
}