namespace LicenseKeyServer.Services;

public interface IKeyGenerator
{
    string Generate(string? prefix, int groupCount, int charsPerGroup);
}
