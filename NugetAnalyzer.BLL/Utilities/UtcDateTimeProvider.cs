using System;
using NugetAnalyzer.BLL.Interfaces;

namespace NugetAnalyzer.BLL.Utilities
{
    public class UtcDateTimeProvider : IDateTimeProvider
    {
        public DateTime CurrentDateAndTime => DateTime.UtcNow;
    }
}