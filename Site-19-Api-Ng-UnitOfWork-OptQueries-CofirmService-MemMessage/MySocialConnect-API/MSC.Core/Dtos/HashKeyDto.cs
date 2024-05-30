namespace MSC.Core.Dtos;

public class HashKeyDto
{
    public byte[] Salt { get; set; }
    public byte[] Hash { get; set; }
}
