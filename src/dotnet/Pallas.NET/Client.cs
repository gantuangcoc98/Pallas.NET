using Pallas.NET.Models;
using Pallas.NET.Models.Enums;
using PallasPoint = PallasDotnetRs.PallasDotnetRs.Point;
using ClientWrapper = PallasDotnetRs.PallasDotnetRs.ClientWrapper;
using NextResponseRs = PallasDotnetRs.PallasDotnetRs.NextResponse;
using Pallas.NET.Extensions;

namespace Pallas.NET;

public class Client
{
    private ClientWrapper? _clientWrapper;
    private ClientType _clientType = ClientType.Unknown;

    private string _connection { get; set; } = default!;
    private ulong _magicNumber = 0;

    private bool IsSyncing { get; set; }
    private bool IsConnected => _clientWrapper != null;
    public bool ShouldReconnect { get; set; } = true; 

    private ulong _lastSlot = 0;   
    private byte[] _lastHash = [];

    public event EventHandler? Disconnected;
    public event EventHandler? Reconnected;

    public async Task<Point> ConnectAsync(string connection, ulong magicNumber, ClientType clientType)
    {
        _clientWrapper = clientType switch
        {
            ClientType.N2C => PallasDotnetRs.PallasDotnetRs.Connect(connection, magicNumber, (byte)ClientType.N2C),
            ClientType.N2N => PallasDotnetRs.PallasDotnetRs.Connect(connection, magicNumber, (byte)ClientType.N2N),
            _ => default
        };

        if (_clientWrapper is null)
        {
            throw new Exception("Failed to connect to node");
        }

        _connection = connection;
        _magicNumber = magicNumber;
        _clientType = clientType;

        return await GetTipAsync();
    }

    public async IAsyncEnumerable<NextResponse> StartChainSyncAsync(List<Point> points)
    {
        if (_clientWrapper is null)
        {
            throw new Exception("Not connected to node");
        }

        if (points.Count > 0)
        {
            IReadOnlyCollection<PallasPoint> _points = points.Select(point => point.ToPallasPoint()).ToList().AsReadOnly();

            await Task.Run(() =>
            {
                PallasPoint _intersection = PallasDotnetRs.PallasDotnetRs.FindIntersect(_clientWrapper.Value, _points);

                _lastSlot = _intersection.slot;
                _lastHash = _intersection.hash is not null ? [.._intersection.hash] : [];
            });
        }

        IsSyncing = true;
        while (IsSyncing)
        {   
            NextResponseRs nextResponseRs = PallasDotnetRs.PallasDotnetRs.ChainSyncNext(_clientWrapper.Value);

            if ((NextResponseAction)nextResponseRs.action == NextResponseAction.Error)
            {
                if (ShouldReconnect)
                {
                    IReadOnlyCollection<PallasPoint> _points = points.Select(point => point.ToPallasPoint()).ToList().AsReadOnly();

                    _clientWrapper = PallasDotnetRs.PallasDotnetRs.Connect(_connection, _magicNumber, (byte)_clientType);
                    PallasPoint _intersection = PallasDotnetRs.PallasDotnetRs.FindIntersect(_clientWrapper.Value, _points);

                    _lastSlot = _intersection.slot;
                    _lastHash = [.._intersection.hash];

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
                Point tip = nextResponseRs.tip.ToPoint();

                NextResponse nextResponse = nextResponseAction switch
                {
                    NextResponseAction.RollForward => new(nextResponseAction, tip, [.. nextResponseRs.blockCbor]),
                    NextResponseAction.RollBack => new(nextResponseAction, tip, [.. nextResponseRs.blockCbor]),
                    _ => default!
                };
                
                yield return nextResponse;
            }
        }
    }

    public void StopSync()
    {
        IsSyncing = false;
    }

    public Task DisconnectAsync()
    {
        if (_clientWrapper is null)
        {
            throw new Exception("Not connected to node");
        }

        return Task.Run(() => PallasDotnetRs.PallasDotnetRs.Disconnect(_clientWrapper.Value));
    }

    public async Task<List<byte[]>> GetUtxoByAddressCborAsync(string address)
    {
        if (_clientWrapper is null)
        {
            throw new Exception("Not connected to node");
        }

        List<List<byte>> utxoByAddress = await Task.Run(() => PallasDotnetRs.PallasDotnetRs.GetUtxoByAddressCbor(_clientWrapper.Value, address));
        return utxoByAddress?.Select(utxo => utxo.ToArray()).ToList() ?? [];
    }

    public async Task<byte[]> FetchBlockAsync(Point? intersection = null)
    {
        if (_clientWrapper is null)
        {
            throw new Exception("Not connected to node");
        }

        if (intersection is null)
        {
            throw new Exception("Intersection not provided");
        }

        return await Task.Run(() => {
            return PallasDotnetRs.PallasDotnetRs.FetchBlock(_clientWrapper.Value, new PallasPoint
            {
                slot = intersection.Slot,
                hash = [.. Convert.FromHexString(intersection.Hash)]
            }).ToArray();
        });
    }

    public async Task<Point> GetTipAsync()
    {
        if (_clientWrapper is null)
        {
            throw new Exception("Not connected to node");
        }

        PallasPoint tip = PallasDotnetRs.PallasDotnetRs.GetTip(_clientWrapper.Value);
        
        return await Task.Run(() => {
            return tip.ToPoint();
        });
    }
}