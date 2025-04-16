# Custom SMTP & IMAP Client in C#

This project is a custom SMTP and IMAP email client built in C#. It uses raw TCP and TLS connections, without relying on external libraries.

## Features

### SMTP
- Connects using TcpClient and SslStream
- Supports STARTTLS and full TLS
- AUTH LOGIN authentication
- Sends plain text emails with custom headers

### IMAP
- Connects using TcpClient and SslStream
- LOGIN-based authentication
- Lists folders and fetches inbox messages
- Supports pagination
- Fetches plain text and HTML message content
- Stateless login using encrypted cookie

## Technologies
- .NET 9
- TcpClient and SslStream
- Optional in-memory caching (IMemoryCache)
- Encrypted cookies for storing login credentials
