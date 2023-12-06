namespace semestrovka.CustomExceptions;

public class ForbiddenException : CustomException
{
    public ForbiddenException(string exceptionMessage) : base(exceptionMessage)
    {
    }
}