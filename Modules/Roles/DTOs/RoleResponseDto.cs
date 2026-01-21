
public record RoleResponseDto(
    int Id,
    string Name,
    List<PrivilegeResponseDto> Privileges
);

public record RoleListResponseDto(
    int Id,
    string Name
);