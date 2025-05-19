namespace SurveyBasket.Api.Abstractions;

public class Result
{
    public Result(bool isSucess,Error error) 
    {
        if ((isSucess && error != Error.None) || (!isSucess && error == Error.None)) {
            throw new InvalidOperationException();
        }
        IsSuccess = isSucess;
        Error = error;
    
    } 
    public bool IsSuccess { get;}
    public bool IsFailure =>!IsSuccess;
    public Error Error { get; } = default!;

    public static Result Sucess() => new Result(true, Error.None);
    public static Result Failure(Error error) => new Result(false, error);

    public static Result<TValue> Sucess<TValue>(TValue value) => new (value,true, Error.None);
    public static Result<TValue> Failure<TValue>(Error error) => new (default, false, error);

}
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result (TValue? value, bool isSucess, Error error) : base(isSucess, error)
    {
        if ((isSucess && error != Error.None) || (!isSucess && error == Error.None))
        {
            throw new InvalidOperationException();
        }
        _value = value;
    }

    public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("Failure Results Cannot have Value");
}
