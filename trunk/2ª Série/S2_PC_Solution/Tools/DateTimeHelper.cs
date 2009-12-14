using System;

namespace Tools
{
    internal class DateTimeHelper
    {
        DateTimeHelper() { }

        public static long CurrentTicks { get { return DateTime.UtcNow.Ticks; } }
    }
}
