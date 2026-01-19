using Microsoft.AspNetCore.Mvc;

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
        var user = await _service.CreateUser(dto.Username, dto.Password);
        return Ok(user);
    }

    [HttpGet]
    [PermissionAuthorize("users.read")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _service.GetUsers();
        return Ok(users);
    }

    [HttpPut("{id}")]
    [PermissionAuthorize("users.update")]
    public async Task<IActionResult> Update(int id, [FromBody] UserRequestDto dto)
    {
        await _service.UpdateUser(id, dto.Username);
        return Ok();
    }

    [HttpDelete("{id}")]
    [PermissionAuthorize("users.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteUser(id);
        return Ok();
    }
}

