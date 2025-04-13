using EmailClient.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace EmailClient.Domain.Results;

public class Result
{
    public bool IsSuccess { get; init; }
    public Error? Error { get; init; }
    public object? Data { get; init; }

    private Result(bool isSuccess, object? data = null, Error? error = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
    }

    public static Result Success(object data)
    {
        return new Result(isSuccess: true, data);
    }

    public static Result Success()
    {
        return new Result(isSuccess: true);
    }

    public static Result Failure(Error error)
    {
        return new Result(isSuccess: false, data: null, error);
    }

    public T GetData<T>()
    {
        if (IsSuccess && Data is T typedData)
        {
            return typedData;
        }

        throw new InvalidOperationException(Error?.Message ?? "Data is not available or of an incorrect type.");
    }
}