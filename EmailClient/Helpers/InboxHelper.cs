namespace EmailClient.Helpers;

using EmailClient.ViewModels.Email;
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
        return rawSubject; //TODO implement
    }
    public static string FormatDate(this string? rawDate)
    {
        if (string.IsNullOrWhiteSpace(rawDate))
            return "(Unknown Date)";

        if (DateTime.TryParse(rawDate, out var dt))
        {
            return dt.ToString("dd MMM yyyy, HH:mm");
        }

        return rawDate;
    }
    public static IEnumerable<EmailHeader> FormatAndTrimEmailData(this IEnumerable<EmailHeader> headers)
    {
        if (headers == null || !headers.Any())
        {
            return Enumerable.Empty<EmailHeader>();
        }

        return headers.Select(h => new EmailHeader(
            h.From.ExtractEmail(),
            h.Subject.DecodeAndTrimSubject(),
            h.Date.FormatDate()
        )).ToList();
    }
}
