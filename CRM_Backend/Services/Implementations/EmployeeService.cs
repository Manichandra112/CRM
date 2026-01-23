using CRM_Backend.Data;
using CRM_Backend.DTOs.Common;
using CRM_Backend.DTOs.Employees;
using Microsoft.EntityFrameworkCore;

public class EmployeeService : IEmployeeService
{
    private readonly CrmAuthDbContext _context;

    public EmployeeService(CrmAuthDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<EmployeeDto>> GetVisibleEmployeesAsync(
        long currentUserId,
        IEnumerable<string> roles,
        IEnumerable<string> permissions,
        EmployeeFilterDto filter)
    {
        var query = _context.Users.AsQueryable();

        // 🔐 VISIBILITY RULES (Zoho-style)
        if (permissions.Contains("CRM_FULL_ACCESS"))
        {
            // Admin → all employees
        }
        else if (roles.Any(r => r.EndsWith("_MANAGER")))
        {
            // Manager → self + direct reports
            query = query.Where(u =>
                u.UserId == currentUserId ||
                u.ManagerId == currentUserId);
        }
        else
        {
            // Normal user → self only
            query = query.Where(u => u.UserId == currentUserId);
        }

        // 🌍 DOMAIN FILTER
        if (!string.IsNullOrWhiteSpace(filter.DomainCode))
        {
            query = query.Where(u => u.Domain.DomainCode == filter.DomainCode);
        }

        // 👤 MANAGER FILTER (optional)
        if (filter.ManagerId.HasValue)
        {
            query = query.Where(u => u.ManagerId == filter.ManagerId);
        }

        // 🔍 SEARCH
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(u =>
                u.Username.Contains(filter.Search) ||
                u.Email.Contains(filter.Search));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.Username)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(u => new EmployeeDto
            {
                UserId = u.UserId,
                Username = u.Username,
                Department = u.Department,
                Designation = u.Designation,
                ManagerId = u.ManagerId,
                DomainId = u.DomainId
            })
            .ToListAsync();

        return new PagedResult<EmployeeDto>
        {
            Items = items,
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }
}
