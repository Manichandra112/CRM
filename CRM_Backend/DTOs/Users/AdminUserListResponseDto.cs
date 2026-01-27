namespace CRM_Backend.DTOs.Users;

public class AdminUserListResponseDto
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<AdminUserListItemDto> Users { get; set; } = new();
}
