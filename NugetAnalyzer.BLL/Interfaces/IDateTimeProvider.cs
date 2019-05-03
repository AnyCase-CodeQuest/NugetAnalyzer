using System;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTime CurrentDateAndTime { get; }
    }
}