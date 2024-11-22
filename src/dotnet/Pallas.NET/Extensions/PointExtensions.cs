using Pallas.NET.Models;
using PallasPoint = PallasDotnetRs.PallasDotnetRs.Point;

namespace Pallas.NET.Extensions;

public static class PointExtensions
{
    public static Point ToPoint(this PallasPoint pallasPoint)
        => new(pallasPoint.slot, Convert.ToHexString(pallasPoint.hash.ToArray()));
    
    public static PallasPoint ToPallasPoint(this Point point)
        => new() { slot = point.Slot, hash = Convert.FromHexString(point.Hash).ToList() };
}