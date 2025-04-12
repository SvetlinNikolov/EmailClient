namespace EmailClient.Services;

using EmailClient.Domain.Errors;
using EmailClient.Domain.Results;
using EmailClient.Helpers;
using EmailClient.Services.Contracts;
using EmailClient.ViewModels.Email;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class ImapClient(string host, int port, string username, string password) : IImapClient
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

    public async Task<Result> ConnectAsync()
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
    public async Task<Result> ConnectAndLoginAsync()
    {
        var connect = await ConnectAsync();
        if (!connect.IsSuccess) return connect;

        var login = await LoginAsync();
        if (!login.IsSuccess) return login;

        return Result.Success();
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

    public async Task<Result> GetInboxAsync(int page = 1, int pageSize = 10)
    {
        var selectResult = await SelectInboxAsync();
        if (!selectResult.IsSuccess) return selectResult;

        var searchResult = await SearchAllMessageIdsAsync();
        if (!searchResult.IsSuccess) return searchResult;

        var allIds = searchResult.GetData<List<int>>();
        var totalPages = (int)Math.Ceiling((double)allIds.Count / pageSize);

        var pageIds = allIds
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        if (!pageIds.Any())
            return Result.Success(new InboxViewModel
            {
                Emails = [],
                CurrentPage = page,
                TotalPages = totalPages
            });

        var emailsResult = await FetchHeadersByIdsAsync(pageIds);
        if (!emailsResult.IsSuccess) return emailsResult;

        var emails = emailsResult.GetData<List<EmailHeader>>()
            .OrderByDescending(x => x.Date)
            .ToList();

        var inboxVm = new InboxViewModel
        {
            Emails = emails.FormatAndTrimEmailData(),
            CurrentPage = page,
            TotalPages = totalPages
        };

        return Result.Success(inboxVm);
    }


    private async Task<Result> SelectInboxAsync()
    {
        var tag = NextTag();
        var sendResult = await SendCommandAsync($"{tag} SELECT INBOX");
        if (!sendResult.IsSuccess) return sendResult;

        return await ReadUntilTaggedResponseAsync(tag);
    }

    private async Task<Result> SearchAllMessageIdsAsync()
    {
        var tag = NextTag();
        var sendResult = await SendCommandAsync($"{tag} SEARCH ALL");
        if (!sendResult.IsSuccess) return Result.Failure(ImapErrors.ImapListMailboxesFailed());

        var response = await ReadUntilTaggedResponseAsync(tag, collect: true);
        if (!response.IsSuccess) return Result.Failure(ImapErrors.ImapListMailboxesFailed());

        var lines = response.GetData<(List<string>, string)>().Item1;
        var searchLine = lines.FirstOrDefault(l => l.StartsWith("* SEARCH"));
        if (searchLine is null) return Result.Success(new List<int>());

        var ids = searchLine.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Skip(2)
                            .Select(x => int.TryParse(x, out var i) ? i : -1)
                            .Where(i => i > 0)
                            .Reverse()
                            .ToList();

        return Result.Success(ids);
    }

    private async Task<Result> FetchHeadersByIdsAsync(IEnumerable<int> ids)
    {
        var tag = NextTag();
        var idString = string.Join(",", ids);
        var sendResult = await SendCommandAsync($"{tag} FETCH {idString} (BODY.PEEK[HEADER.FIELDS (Subject From Date)])");
        if (!sendResult.IsSuccess) return Result.Failure(ImapErrors.ImapListMailboxesFailed());

        var fetchResult = await ReadUntilTaggedResponseAsync(tag, collect: true);
        if (!fetchResult.IsSuccess) return Result.Failure(ImapErrors.ImapListMailboxesFailed());

        var lines = fetchResult.GetData<(List<string>, string)>().Item1;
        var headers = new List<EmailHeader>();
        var buffer = new Dictionary<string, string>();

        foreach (var line in lines)
        {
            if (line.StartsWith("Subject:", StringComparison.OrdinalIgnoreCase))
                buffer["Subject"] = line["Subject:".Length..].Trim();
            else if (line.StartsWith("From:", StringComparison.OrdinalIgnoreCase))
                buffer["From"] = line["From:".Length..].Trim();
            else if (line.StartsWith("Date:", StringComparison.OrdinalIgnoreCase))
                buffer["Date"] = line["Date:".Length..].Trim();
            else if (line == ")")
            {
                headers.Add(new EmailHeader(
                    buffer.GetValueOrDefault("Subject", "?"),
                    buffer.GetValueOrDefault("From", "?"),
                    buffer.GetValueOrDefault("Date", "?")
                ));
                buffer.Clear();
            }
        }

        return Result.Success(headers);
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
        _reader?.Dispose();
        _writer?.Dispose();
        _sslStream?.Dispose();
        _tcpClient?.Close();
    }
}
