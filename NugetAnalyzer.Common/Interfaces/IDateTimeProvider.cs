using System;

namespace NugetAnalyzer.Common.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTime CurrentUtcDateTime { get; }
    }
}
