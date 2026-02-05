using Microsoft.AspNetCore.Http;

namespace CRM_Backend.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base(message, StatusCodes.Status404NotFound)
    {
    }
}
