namespace PallasDotnet.Models;

public enum RedeemerTag
{
    Spend = 0,
    Mint = 1,
    Cert = 2,
    Reward = 3
}

public record Redeemer(RedeemerTag Tag, uint Index, byte[] Data, ExUnits ExUnits);