using Microsoft.AspNetCore.Mvc;
using UserManagement.Api.Helpers;

[ApiController]
[Route("api/privileges")]
public class PrivilegesController : ControllerBase
{
    private readonly PrivilegeService _service;

    public PrivilegesController(PrivilegeService service) => _service = service;

    [HttpPost]
    [PermissionAuthorize("privileges.create")]
    public async Task<IActionResult> Create([FromBody] PrivilegeRequestDto dto)
    {
        var privilege = await _service.CreatePrivilege(dto);
        return Ok(new ApiResponse<PrivilegeDetailResponseDto?>(privilege));
    }

    [HttpGet]
    [PermissionAuthorize("privileges.read")]
    public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int limit = 10)
    {
        var (privileges, count) = await _service.GetPrivilegesWithFilter(skip, limit);
        return Ok(new ApiListResponse<PrivilegeDetailResponseDto>(privileges, count));
    }

    [HttpGet("{id}")]
    [PermissionAuthorize("privileges.read")]
    public async Task<IActionResult> GetById(int id)
    {
        var privilege = await _service.GetPrivilegeById(id);
        if (privilege == null) return NotFound();
        return Ok(new ApiResponse<PrivilegeDetailResponseDto?>(privilege));
    }

    [HttpPut("{id}")]
    [PermissionAuthorize("privileges.update")]
    public async Task<IActionResult> Update(int id, [FromBody] PrivilegeRequestDto dto)
    {
        var privilege = await _service.UpdatePrivilege(id, dto);
        if (privilege == null) return NotFound();
        return Ok(new ApiResponse<PrivilegeDetailResponseDto?>(privilege));
    }

    [HttpDelete("{id}")]
    [PermissionAuthorize("privileges.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var privilege = await _service.DeletePrivilege(id);
        if (privilege == null) return NotFound();
        return Ok(new ApiResponse<PrivilegeDetailResponseDto?>(privilege));
    }
}
