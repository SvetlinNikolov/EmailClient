using System.Text.RegularExpressions;
using System.Text;

namespace EmailClient.Helpers;

public static class MimeHeaderHelper
{
    private static readonly Regex EncodedWordRegex = new(
        @"=\?([^?]+)\?([BbQq])\?([^?]+)\?=",
        RegexOptions.Compiled);

    public static string DecodeMimeHeader(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "(No Subject)";

        return EncodedWordRegex.Replace(input, match =>
        {
            var charset = match.Groups[1].Value;
            var encoding = match.Groups[2].Value.ToUpperInvariant();
            var encodedText = match.Groups[3].Value;

            try
            {
                byte[] bytes = encoding switch
                {
                    "B" => Convert.FromBase64String(encodedText),
                    "Q" => DecodeQuotedPrintable(encodedText),
                    _ => throw new NotSupportedException($"Unsupported encoding: {encoding}")
                };

                var encodingCharset = Encoding.GetEncoding(charset);
                return encodingCharset.GetString(bytes);
            }
            catch
            {
                return match.Value;
            }
        });
    }

    private static byte[] DecodeQuotedPrintable(string input)
    {
        var output = new List<byte>();
        for (int i = 0; i < input.Length;)
        {
            if (input[i] == '=' && i + 2 < input.Length && IsHex(input[i + 1]) && IsHex(input[i + 2]))
            {
                string hex = input.Substring(i + 1, 2);
                output.Add(Convert.ToByte(hex, 16));
                i += 3;
            }
            else
            {
                output.Add((byte)(input[i] == '_' ? ' ' : input[i]));
                i++;
            }
        }
        return output.ToArray();
    }

    private static bool IsHex(char c) =>
        (c >= '0' && c <= '9') ||
        (c >= 'A' && c <= 'F') ||
        (c >= 'a' && c <= 'f');
}