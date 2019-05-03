namespace NugetAnalyzer.BLL.Entities.Enums
{
    public enum PackageVersionStatus : byte
    {
        Actual = 0,
        MajorChanged,
        MinorChanged,
        BuildChanged,
        RevisionChanged
    }
}
