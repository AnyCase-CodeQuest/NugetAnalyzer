using System;
using NugetAnalyzer.Common.Interfaces;

namespace NugetAnalyzer.Common
{
    public class UtcDateTimeProvider : IDateTimeProvider
    {
        public DateTime CurrentUtcDateTime => DateTime.UtcNow;
    }
}