
public record UserRequestDto(string Username, string? Password, List<int> RoleIds);
public record UserResponseDto(int Id);
public record UserDetailResponseDto(int Id, string Username, List<string> Roles);
