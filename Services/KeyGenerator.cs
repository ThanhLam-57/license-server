using System.Security.Cryptography;
using System.Text;

namespace LicenseKeyServer.Services;

public class KeyGenerator : IKeyGenerator
{
    private const string AllowedChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public string Generate(string? prefix, int groupCount, int charsPerGroup)
    {
        var groups = new List<string>();

        if (!string.IsNullOrWhiteSpace(prefix))
        {
            groups.Add(prefix.Trim().ToUpperInvariant());
        }

        for (var g = 0; g < groupCount; g++)
        {
            groups.Add(RandomGroup(charsPerGroup));
        }

        return string.Join("-", groups);
    }

    private static string RandomGroup(int length)
    {
        var buffer = new byte[length];
        RandomNumberGenerator.Fill(buffer);
        var sb = new StringBuilder(length);

        for (var i = 0; i < length; i++)
        {
            sb.Append(AllowedChars[buffer[i] % AllowedChars.Length]);
        }

        return sb.ToString();
    }
}
