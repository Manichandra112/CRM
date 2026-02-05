using Microsoft.AspNetCore.Http;

namespace CRM_Backend.Exceptions;

public class InternalServerException : AppException
{
    public InternalServerException(string message)
        : base(message, StatusCodes.Status500InternalServerError)
    {
    }
}
