using System.Linq;

namespace MSC.Core.Extensions;

public static class ByteExtension
{
    public static bool AreEqual(this byte[] a, byte[] b)
    {
        var areEqual = a.SequenceEqual(b);
        return areEqual;
    }
}
