using Microsoft.AspNetCore.Http;

namespace CRM_Backend.Exceptions;

public class ForbiddenException : AppException
{
    public ForbiddenException(string message)
        : base(message, StatusCodes.Status403Forbidden)
    {
    }
}
