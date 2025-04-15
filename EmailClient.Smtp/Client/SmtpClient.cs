namespace EmailClient.Smtp.Client;


using EmailClient.Smtp.Client.Contracts;
using EmailClient.Smtp.Errors;
using EmailService.Domain.Results;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class SmtpClient(string smtpServer, int port, string username, string password) : ISmtpClient
{
    private readonly string _smtpServer = smtpServer;
    private readonly int _port = port;
    private readonly string _username = username;
    private readonly string _password = password;

    public async Task<Result> SendEmailAsync(string from, string to, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(from)) return Result.Failure(SmtpErrors.InvalidMessage("From"));
        if (string.IsNullOrWhiteSpace(to)) return Result.Failure(SmtpErrors.InvalidMessage("To"));
        if (string.IsNullOrWhiteSpace(subject)) return Result.Failure(SmtpErrors.InvalidMessage("Subject"));
        if (string.IsNullOrWhiteSpace(body)) return Result.Failure(SmtpErrors.InvalidMessage("Body"));

        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_smtpServer, _port);

            using var stream = new SslStream(client.GetStream());
            await stream.AuthenticateAsClientAsync(_smtpServer);

            using var writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };
            using var reader = new StreamReader(stream, Encoding.ASCII);

            return await ExecuteSmtpSequence(writer, reader, from, to, subject, body);
        }
        catch (Exception ex)
        {
            return Result.Failure(SmtpErrors.ConnectionFailed(ex.Message));
        }
    }

    private async Task<Result> ExecuteSmtpSequence(StreamWriter writer, StreamReader reader, string from, string to, string subject, string body)
    {
        async Task<Result> Step(string command, bool read = true)
        {
            Console.WriteLine("C: " + command);
            await writer.WriteLineAsync(command);
            return read ? await ReadResponse(reader, command) : Result.Success();
        }

        async Task<Result> All(params Func<Task<Result>>[] steps)
        {
            foreach (var step in steps)
            {
                var result = await step();
                if (!result.IsSuccess) return result;
            }
            return Result.Success();
        }

        return await All(
            () => ReadResponse(reader),
            () => Step($"EHLO {Dns.GetHostName()}", read: false),
            () => ReadMultiLineResponse(reader),
            () => Step("AUTH LOGIN"),
            () => Step(Convert.ToBase64String(Encoding.ASCII.GetBytes(_username))),
            () => Step(Convert.ToBase64String(Encoding.ASCII.GetBytes(_password))),
            () => Step($"MAIL FROM:<{from}>"),
            () => Step($"RCPT TO:<{to}>"),
            () => Step("DATA"),
            () => Step(FormatEmail(subject, body)),
            () => Step("QUIT", read: false)
        );
    }

    private static string FormatEmail(string subject, string body) =>
        new StringBuilder()
            .AppendLine($"Subject: {subject}")
            .AppendLine("Content-Type: text/plain; charset=utf-8")
            .AppendLine()
            .AppendLine(body)
            .AppendLine(".")
            .ToString();

    private async Task<Result> ReadResponse(StreamReader reader, string command = "unknown")
    {
        try
        {
            var response = await reader.ReadLineAsync();
            Console.WriteLine("S: " + response);

            return string.IsNullOrWhiteSpace(response) || !response.StartsWith("2") && !response.StartsWith("3")
                ? Result.Failure(SmtpErrors.CommandFailed(command, response))
                : Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(SmtpErrors.UnexpectedResponse(ex.Message));
        }
    }

    private async Task<Result> ReadMultiLineResponse(StreamReader reader)
    {
        try
        {
            string? line;
            do
            {
                line = await reader.ReadLineAsync();
                Console.WriteLine("S: " + line);
            } while (line != null && line.Length >= 4 && line[3] == '-');

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(SmtpErrors.UnexpectedResponse(ex.Message));
        }
    }
}
