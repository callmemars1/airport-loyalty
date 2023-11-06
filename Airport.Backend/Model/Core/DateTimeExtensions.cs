namespace Airport.Backend.Model.Core;

public static class DateTimeExtensions
{
    public static DateTime AsUtc(this DateTime dateTime)
        => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
}