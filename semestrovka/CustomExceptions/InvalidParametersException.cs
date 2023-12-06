namespace semestrovka.CustomExceptions;

public class InvalidParametersException : CustomException
{
    public InvalidParametersException(string exceptionMessage) : base(exceptionMessage)
    {
    }
}