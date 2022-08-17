using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.EventBus.Dashboard.Common
{
    public static class UtilHelper
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime MillisecondTimestampBoundaryDate = new DateTime(1978, 1, 11, 21, 31, 40, 799, DateTimeKind.Utc);
        private static readonly long MillisecondTimestampBoundary = 253402300799L;

        public static long ToTimestamp(DateTime value)
        {
            TimeSpan elapsedTime = value - Epoch;
            return (long)elapsedTime.TotalSeconds;
        }
    }
}
