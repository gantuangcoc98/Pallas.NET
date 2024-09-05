using PallasDotnet.Models;

namespace PallasDotnet;

public class Utils
{
    public static Point MapPallasPoint(PallasDotnetRs.PallasDotnetRs.Point rsPoint)
        => new(rsPoint.slot, Convert.ToHexString(rsPoint.hash.ToArray()));
}