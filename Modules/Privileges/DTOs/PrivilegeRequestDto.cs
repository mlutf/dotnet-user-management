public record PrivilegeRequestDto(
    string NameSpace,
    string Module,
    string Submodule,
    string Description
);

public record PrivilegeResponseDto(
    int Id,
    string NameSpace,
    string? Module,
    string? Submodule,
    string? Description
);

public record PrivilegeCreateResponseDto(int Id);

public record PrivilegeDetailResponseDto(
    int Id,
    string NameSpace,
    string? Module,
    string? Submodule,
    string? Description
);
