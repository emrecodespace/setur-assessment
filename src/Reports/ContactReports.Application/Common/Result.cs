namespace ContactReports.Application.Common;
public class Result
{
    protected Result(bool isSuccess, string? errorMessage = null, int? statusCode = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }

    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public int? StatusCode { get; }


    public static Result Success() => new(true);
    public static Result Failure(string errorMessage, int? statusCode = null)
        => new(false, errorMessage, statusCode);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true)
    => Value = value;
    
    private Result(string errorMessage, int? statusCode = null) 
        : base(false, errorMessage, statusCode) { }

    public static Result<T> Success(T value) => new(value);
    public new static Result<T> Failure(string errorMessage, int? statusCode = null) 
        => new(errorMessage, statusCode);
}