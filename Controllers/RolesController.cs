using Microsoft.AspNetCore.Mvc;
using UserManagement.Api.Helpers;

[ApiController]
[Route("api/roles")]
public class RolesController : ControllerBase
{
    private readonly RoleService _service;

    public RolesController(RoleService service) => _service = service;

    [HttpPost]
    [PermissionAuthorize("roles.create")]
    public async Task<IActionResult> Create([FromBody] RoleRequestDto dto)
    {
        var role = await _service.CreateRole(dto);
        return Ok(new ApiResponse<RoleResponseDto?>(role));
    }

    [HttpGet]
    [PermissionAuthorize("roles.list")]
    public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int limit = 10)
    {
        var (roles, count) = await _service.GetRolesWithPrivileges(skip, limit);
        return Ok(new ApiListResponse<RoleResponseDto>(roles, count));
    }

    [HttpGet("{id}")]
    [PermissionAuthorize("roles.read")]
    public async Task<IActionResult> GetById(int id)
    {
        var role = await _service.GetRoleByIdWithPrivileges(id);
        if (role == null) return NotFound();
        return Ok(new ApiResponse<RoleResponseDto?>(role));
    }

    [HttpPut("{id}")]
    [PermissionAuthorize("roles.update")]
    public async Task<IActionResult> Update(int id, [FromBody] RoleRequestDto dto)
    {
        var role = await _service.UpdateRole(id, dto);
        if (role == null) return NotFound();
        return Ok(new ApiResponse<RoleResponseDto?>(role));
    }

    [HttpDelete("{id}")]
    [PermissionAuthorize("roles.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var role = await _service.DeleteRole(id);
        if (role == null) return NotFound();
        return Ok(new ApiResponse<RoleResponseDto?>(role));
    }
}
