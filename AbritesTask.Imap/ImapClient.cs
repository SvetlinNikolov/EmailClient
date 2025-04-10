namespace AbritesTask.Imap;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class ImapClient(string host, int port, string username, string password) : IDisposable
{
    private readonly string _host = host;
    private readonly int _port = port;
    private readonly string _username = username;
    private readonly string _password = password;

    private TcpClient _tcpClient = new();
    private SslStream? _sslStream;
    private StreamReader? _reader;
    private StreamWriter? _writer;
    private int _tagCounter = 0;

    public async Result ConnectAsync()
    {
        try
        {
            await _tcpClient.ConnectAsync(_host, _port);
            _sslStream = new SslStream(_tcpClient.GetStream(), false);
            await _sslStream.AuthenticateAsClientAsync(_host);

            _reader = new StreamReader(_sslStream, Encoding.ASCII);
            _writer = new StreamWriter(_sslStream, Encoding.ASCII) { AutoFlush = true };

            var greeting = await _reader.ReadLineAsync();
            Console.WriteLine($"[Server] {greeting}");

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ImapErrors.ImapConnectionFailed(ex.Message));
        }
    }

    public async Task<Result> LoginAsync()
    {
        var tag = NextTag();
        var sendResult = await SendCommandAsync($"{tag} LOGIN {_username} {_password}");
        if (!sendResult.IsSuccess) return sendResult;

        var responseResult = await ReadUntilTaggedResponseAsync(tag);
        if (!responseResult.IsSuccess) return responseResult;

        var taggedResponse = responseResult.GetData<(List<string>, string)>().Item2;

        return taggedResponse.Contains("OK", StringComparison.OrdinalIgnoreCase)
            ? Result.Success()
            : Result.Failure(ImapErrors.ImapLoginFailed(taggedResponse));
    }

    public async Task<Result> LogoutAsync()
    {
        var tag = NextTag();
        var sendResult = await SendCommandAsync($"{tag} LOGOUT");
        if (!sendResult.IsSuccess) return sendResult;

        var responseResult = await ReadUntilTaggedResponseAsync(tag);
        if (!responseResult.IsSuccess) return responseResult;

        var taggedResponse = responseResult.GetData<(List<string>, string)>().Item2;

        return taggedResponse.Contains("OK", StringComparison.OrdinalIgnoreCase)
            ? Result.Success()
            : Result.Failure(ImapErrors.ImapLogoutFailed(taggedResponse));
    }

    public async Task<Result> ListMailboxesAsync()
    {
        var tag = NextTag();
        var sendResult = await SendCommandAsync($"{tag} LIST \"\" \"*\"");
        if (!sendResult.IsSuccess) return sendResult;

        var responseResult = await ReadUntilTaggedResponseAsync(tag, collect: true);
        if (!responseResult.IsSuccess) return responseResult;

        var lines = responseResult.GetData<(List<string>, string)>().Item1;
        return Result.Success(lines);
    }

    private async Task<Result> SendCommandAsync(string command)
    {
        try
        {
            if (_writer is null)
                return Result.Failure(ImapErrors.ImapWriterNotInitialized());

            Console.WriteLine($"[Client] {command.Trim()}");
            await _writer.WriteAsync(command + "\r\n");
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ImapErrors.ImapConnectionFailed(ex.Message));
        }
    }

    private async Task<Result> ReadUntilTaggedResponseAsync(string tag, bool collect = false)
    {
        try
        {
            if (_reader is null)
                return Result.Failure(ImapErrors.ImapReaderNotInitialized());

            var result = new List<string>();
            string? line;

            while ((line = await _reader.ReadLineAsync()) != null)
            {
                Console.WriteLine($"[Server] {line}");
                if (collect) result.Add(line);
                if (line.StartsWith(tag))
                    return Result.Success((result, line));
            }

            return Result.Failure(ImapErrors.ImapNoTaggedResponse());
        }
        catch (Exception ex)
        {
            return Result.Failure(ImapErrors.ImapListMailboxesFailed(ex.Message));
        }
    }

    private string NextTag() => $"A{++_tagCounter}";

    public void Dispose()
    {
        try
        {
            _reader?.Dispose();
            _writer?.Dispose();
            _sslStream?.Dispose();
            _tcpClient?.Close();
        }
        catch
        {
            // Swallow dispose errors
        }
    }
}
