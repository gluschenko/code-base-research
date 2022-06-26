using System;

namespace CodeBase.Core
{
    public static class UnixTime
    {
        private static readonly DateTime _unixZero = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static long UtcNow()
        {
            return (long)(DateTime.UtcNow - _unixZero).TotalSeconds;
        }

        public static long Now()
        {
            return (long)(DateTime.Now - _unixZero).TotalSeconds;
        }

        public static long ToTimestamp(DateTime dateTime)
        {
            return (long)Math.Floor((dateTime - _unixZero).TotalSeconds);
        }

        public static DateTime ToDateTime(long timestamp)
        {
            return _unixZero.AddSeconds(timestamp);
        }
    }
}
