namespace NugetAnalyzer.DTOs.Models.Enums
{
    // Don't change the order
    public enum PackageVersionStatus : byte
    {
        Undefined = 0,
        Actual = 1,
        Info = 2,
        Warning = 3,
        Error = 4
    }
}
