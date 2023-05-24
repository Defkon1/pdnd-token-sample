namespace PDNDTokenSample.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static int ToUnixTimestamp(this DateTime dateTime)
        {
            int unixTimestamp = (int)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            return unixTimestamp;
        }
    }
}
