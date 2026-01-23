using CRM_Backend.Data;
using CRM_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class UserVisibilityService : IUserVisibilityService
{
    private readonly CrmAuthDbContext _context;

    public UserVisibilityService(CrmAuthDbContext context)
    {
        _context = context;
    }

    public async Task<List<long>> GetVisibleUserIdsAsync(long viewerUserId)
    {
        var roles = await _context.UserRoles
            .Where(ur => ur.UserId == viewerUserId)
            .Select(ur => ur.Role.RoleCode)
            .ToListAsync();

        // ADMIN / HR → everyone
        if (roles.Contains("ADMIN") || roles.Contains("HR"))
        {
            return await _context.Users
                .Select(u => u.UserId)
                .ToListAsync();
        }

        // MANAGER → self + hierarchy
        if (roles.Contains("MANAGER"))
        {
            return await GetHierarchyAsync(viewerUserId);
        }

        // EMPLOYEE → self only
        return new List<long> { viewerUserId };
    }

    private async Task<List<long>> GetHierarchyAsync(long managerId)
    {
        var result = new List<long> { managerId };

        async Task Traverse(long id)
        {
            var children = await _context.Users
                .Where(u => u.ManagerId == id)
                .Select(u => u.UserId)
                .ToListAsync();

            foreach (var child in children)
            {
                if (!result.Contains(child))
                {
                    result.Add(child);
                    await Traverse(child);
                }
            }
        }

        await Traverse(managerId);
        return result;
    }
}
