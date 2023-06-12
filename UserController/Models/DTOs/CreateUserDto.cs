namespace UserController.Models.DTOs;

public class CreateUserDto
{
    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public string Code { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? TcKimlik { get; set; }

    public string Telefon { get; set; }

    public DateTime BirthDate { get; set; }

    public int Newsletter { get; set; }

    public int PrivateDiscountType { get; set; }

    public bool? PhoneNumberConfirmed { get; set; } = false;

    public bool? TwoFactorEnabled { get; set; } = false;

    public bool? EmailConfirmed { get; set; } = false;

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int? AccessFailedCount { get; set; } = 0;
}