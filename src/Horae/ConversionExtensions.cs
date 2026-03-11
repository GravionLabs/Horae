namespace Horae;

public static class ConversionExtensions
{
    public static DateOnly AsDateOnly(this DateTimeOffset dto) =>
        DateOnly.FromDateTime(dto.UtcDateTime);

    public static DateTime AsDateTime(this DateTimeOffset dto) =>
        dto.UtcDateTime;

    public static DateTimeOffset AsDateTimeOffset(this DateOnly date) =>
        new(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);

    public static DateTimeOffset AsUtcDateTimeOffset(this DateOnly date) =>
        new(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
}
