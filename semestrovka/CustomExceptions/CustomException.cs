namespace semestrovka.CustomExceptions;

public class CustomException : Exception
{
    protected CustomException(string exceptionMessage) : base(exceptionMessage)
    {
    }
}