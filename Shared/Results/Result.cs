public class Result<T>
{
    public T Value { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }

    public static Result<T> Success(T value)
    {
        return new Result<T> { Value = value, IsSuccess = true };
    }

    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T> { ErrorMessage = errorMessage, IsSuccess = false };
    }
}