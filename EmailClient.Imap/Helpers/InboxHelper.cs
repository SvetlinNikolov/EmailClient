namespace EmailClient.Services.Helpers;

using EmailClient.Imap.Dtos;
using EmailClient.Services.Helpers;
using System.Globalization;
using System.Text.RegularExpressions;

public static class InboxExtensions
{
    public static string ExtractEmail(this string? from)
    {
        if (string.IsNullOrWhiteSpace(from))
            return "(Unknown)";

        var match = Regex.Match(from, @"<([^>]+)>");
        return match.Success ? match.Groups[1].Value : from.Trim();
    }

    public static string DecodeAndTrimSubject(this string? rawSubject)
    {
        return MimeHeaderHelper.DecodeMimeHeader(rawSubject);
    }

    public static string FormatDate(this string? rawDate)
    {
        if (string.IsNullOrWhiteSpace(rawDate))
            return "(Unknown Date)";

        try
        {
            var cleaned = rawDate.Split('(')[0].Trim();
            var localTime = DateTimeOffset
                .Parse(cleaned, CultureInfo.InvariantCulture)
                .ToLocalTime()
                .DateTime;

            return localTime.ToString("g", CultureInfo.CurrentCulture);
        }
        catch
        {
            return rawDate.Trim();
        }
    }

    public static IEnumerable<EmailHeaderDto> FormatAndTrimEmailData(this IEnumerable<EmailHeaderDto> headers)
    {
        if (headers == null || !headers.Any())
        {
            return Enumerable.Empty<EmailHeaderDto>();
        }

        return headers
                   .OrderByDescending(h => TryParseRawDate(h.Date))
                   .Select(h => new EmailHeaderDto(
                       h.Subject.DecodeAndTrimSubject(),
                       h.From.ExtractEmail(),
                       h.Date.FormatDate()))
                   .ToList();
    }

    private static DateTime TryParseRawDate(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return DateTime.MinValue;

        try
        {
            var cleaned = raw.Split('(')[0].Trim();
            return DateTimeOffset
                .Parse(cleaned, CultureInfo.InvariantCulture)
                .ToLocalTime()
                .DateTime;
        }
        catch
        {
            return DateTime.MinValue;
        }
    }
}