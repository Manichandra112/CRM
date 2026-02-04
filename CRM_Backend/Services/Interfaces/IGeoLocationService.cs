namespace CRM_Backend.Services.Interfaces;

public interface IGeoLocationService
{
    Task<string?> ResolveAsync(string ipAddress);
}
