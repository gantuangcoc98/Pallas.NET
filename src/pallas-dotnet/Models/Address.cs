namespace PallasDotnet.Models;

public class Address(byte[] addressBytes)
{
    public byte[] Raw => addressBytes;
    
    public string ToBech32()
        => PallasDotnetRs.PallasDotnetRs
                .AddressBytesToBech32(addressBytes);
}