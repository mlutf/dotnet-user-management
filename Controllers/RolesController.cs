using Microsoft.AspNetCore.Mvc;

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
        var role = await _service.CreateRole(dto.Name);
        return Ok(role);
    }

    [HttpGet]
    [PermissionAuthorize("roles.list")]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _service.GetRoles();
        return Ok(roles);
    }

    [HttpGet("{id}")]
    [PermissionAuthorize("roles.read")]
    public async Task<IActionResult> Read(int id)
    {
        var roles = await _service.GetOneById(id);
        return Ok(roles);
    }

    [HttpPut("{id}")]
    [PermissionAuthorize("users.update")]
    public async Task<IActionResult> Update(int id, [FromBody] RoleRequestDto dto)
    {
        await _service.UpdateRole(id, dto.Name);
        return Ok();
    }

    [HttpDelete("{id}")]
    [PermissionAuthorize("roles.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteRole(id);
        return Ok();
    }
}

