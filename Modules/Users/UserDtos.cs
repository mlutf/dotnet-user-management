
public record UserRequestDto(string Username, string Password);
public record UserResponseDto(int Id, string Username, List<string> Roles);
