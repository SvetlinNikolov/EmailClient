namespace AbritesTask.Smtp;

using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class SmtpClient(string smtpServer, int port, string username, string password)
{
    private readonly string _smtpServer = !string.IsNullOrWhiteSpace(smtpServer) ? smtpServer : throw new ArgumentException("SMTP server cannot be null or empty.");
    private readonly int _port = port > 0 ? port : throw new ArgumentException("Port must be greater than zero.");
    private readonly string _username = !string.IsNullOrWhiteSpace(username) ? username : throw new ArgumentException("Username cannot be null or empty.");
    private readonly string _password = !string.IsNullOrWhiteSpace(password) ? password : throw new ArgumentException("Password cannot be null or empty.");

    public async Task SendEmailAsync(string from, string to, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(from)) throw new ArgumentException("From address cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(to)) throw new ArgumentException("To address cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException("Subject cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(body)) throw new ArgumentException("Body cannot be null or empty.");

        using var client = new TcpClient();
        await client.ConnectAsync(_smtpServer, _port);

        using var stream = new SslStream(client.GetStream());
        await stream.AuthenticateAsClientAsync(_smtpServer);

        using var writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };
        using var reader = new StreamReader(stream, Encoding.ASCII);

        await ReadResponse(reader);

        await SendCommand(writer, $"EHLO localhost");
        await ReadMultiLineResponse(reader);

        await SendCommand(writer, "AUTH LOGIN");
        await ReadResponse(reader);

        await SendCommand(writer, Convert.ToBase64String(Encoding.ASCII.GetBytes(_username)));
        await ReadResponse(reader);

        await SendCommand(writer, Convert.ToBase64String(Encoding.ASCII.GetBytes(_password)));
        await ReadResponse(reader);

        await SendCommand(writer, $"MAIL FROM:<{from}>");
        await ReadResponse(reader);

        await SendCommand(writer, $"RCPT TO:<{to}>");
        await ReadResponse(reader);

        await SendCommand(writer, "DATA");
        await ReadResponse(reader);

        var message = new StringBuilder()
        .AppendLine($"Subject: {subject}")
        .AppendLine("Content-Type: text/plain; charset=utf-8")
        .AppendLine()
        .AppendLine(body)
        .AppendLine(".");

        await SendCommand(writer, message.ToString());
        await ReadResponse(reader);

        await SendCommand(writer, "QUIT");
        await ReadResponse(reader);
    }

    private async Task SendCommand(StreamWriter writer, string command)
    {
        Console.WriteLine("C: " + command);
        await writer.WriteLineAsync(command);
    }

    private async Task ReadResponse(StreamReader reader)
    {
        var response = await reader.ReadLineAsync();
        Console.WriteLine("S: " + response);
    }

    private async Task ReadMultiLineResponse(StreamReader reader)
    {
        string? line;
        do
        {
            line = await reader.ReadLineAsync();
            Console.WriteLine("S: " + line);
        } while (line != null && line.Length >= 4 && line[3] == '-');
    }
}
