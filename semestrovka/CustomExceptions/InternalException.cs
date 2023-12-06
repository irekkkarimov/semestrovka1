namespace semestrovka.CustomExceptions;

public class InternalException : CustomException
{
    public InternalException(string exceptionMessage) : base(exceptionMessage)
    {
    }
}