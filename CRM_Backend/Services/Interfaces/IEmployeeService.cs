using CRM_Backend.DTOs.Common;
using CRM_Backend.DTOs.Employees;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeDto>> GetVisibleEmployeesAsync(
        long currentUserId,
        IEnumerable<string> roles,
        IEnumerable<string> permissions,
        EmployeeFilterDto filter
    );
}
