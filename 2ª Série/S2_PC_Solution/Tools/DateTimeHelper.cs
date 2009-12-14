using System;

namespace Tools
{
    internal class DateTimeHelper
    {
        DateTimeHelper() { }

        public static long GetCurrentTicks() { return DateTime.UtcNow.Ticks; }
    }
}
