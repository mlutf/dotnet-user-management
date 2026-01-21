using Microsoft.AspNetCore.Mvc;

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
        var privilege = await _service.CreatePrivilege(dto.NameSpace, dto.Description, dto.Module, dto.Submodule);
        return Ok(privilege);
    }

    [HttpGet]
    [PermissionAuthorize("privileges.read")]
    public async Task<IActionResult> GetAll()
    {
        var privileges = await _service.GetPrivileges();
        return Ok(privileges);
    }

    [HttpPut("{id}")]
    [PermissionAuthorize("privileges.update")]
    public async Task<IActionResult> Update(int id, [FromBody] PrivilegeRequestDto dto)
    {
        await _service.UpdatePrivilege(id, dto.NameSpace, dto.Description, dto.Module, dto.Submodule);
        return Ok();
    }

    [HttpDelete("{id}")]
    [PermissionAuthorize("privileges.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeletePrivilege(id);
        return Ok();
    }
}

