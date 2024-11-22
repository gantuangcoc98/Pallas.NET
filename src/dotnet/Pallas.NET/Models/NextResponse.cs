using Pallas.NET.Models.Enums;

namespace Pallas.NET.Models;

public record NextResponse(
    NextResponseAction Action,
    Point Tip,
    byte[] BlockCbor
);