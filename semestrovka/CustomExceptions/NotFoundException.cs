namespace semestrovka.CustomExceptions;

public class NotFoundException : CustomException
{
    public NotFoundException(string exceptionMessage) : base(exceptionMessage)
    {
    }
}