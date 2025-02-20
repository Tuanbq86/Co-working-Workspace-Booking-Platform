using WorkHive.BuildingBlocks.Exceptions;

namespace WorkHive.Services.Exceptions;

public class BadRequestEmailOrPhoneException : BadRequestException
{
    public BadRequestEmailOrPhoneException(string message) : base(message)
    {
        
    }
}
