using Microsoft.AspNetCore.Mvc;
using UserManagement.Api.Helpers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserService _service;

    public UsersController(UserService service) => _service = service;

    [HttpPost]
    [PermissionAuthorize("users.create")]
    public async Task<IActionResult> Create([FromBody] UserRequestDto dto)
    {
        var user = await _service.CreateUser(dto);
        return Ok(new ApiResponse<UserDetailResponseDto?>(user));
    }

    [HttpGet]
    [PermissionAuthorize("users.read")]
    public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int limit = 10, [FromQuery] string? roleName = null)
    {
        var (users, count) = await _service.GetUsersWithRoles(skip, limit, roleName);
        return Ok(new ApiListResponse<UserDetailResponseDto>(users, count));
    }

    [HttpGet("{id}")]
    [PermissionAuthorize("users.read")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _service.GetUserByIdWithRoles(id);
        if (user == null) return NotFound();
        return Ok(new ApiResponse<UserDetailResponseDto?>(user));
    }

    [HttpPut("{id}")]
    [PermissionAuthorize("users.update")]
    public async Task<IActionResult> Update(int id, [FromBody] UserRequestDto dto)
    {
        var user = await _service.UpdateUser(id, dto);
        if (user == null) return NotFound();
        return Ok(new ApiResponse<UserDetailResponseDto?>(user));
    }

    [HttpDelete("{id}")]
    [PermissionAuthorize("users.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _service.DeleteUser(id);
        if (user == null) return NotFound();
        return Ok(new ApiResponse<UserDetailResponseDto?>(user));
    }
}
