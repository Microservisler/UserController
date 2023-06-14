using Microsoft.EntityFrameworkCore;
using UserController.Auth;
using UserController.Contexts;
using UserController.DTOs;
using UserController.Models;
using UserController.Services.Interfaces;

namespace UserController.Services;

public class UserService : IUserService
{
    private readonly DataContext _context;

    public UserService(DataContext context)
    {
        _context = context;
    }

    public async Task<User> Register(RegisterModel register)
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = register.Username,
            NormalizedUserName = register.Username.Normalize(),
            Email = register.Email,
            NormalizedEmail = register.Email.Normalize(),
            EmailConfirmed = false,
            PhoneNumber = register.PhoneNumber,
            Code = register.Username.Normalize(),
            FirstName = register.FirstName,
            LastName = register.LastName,
            TcKimlik = null,
            Telefon = register.PhoneNumber,
            BirthDate = register.BirthDate,
            Newsletter = 0,
            PrivateDiscountType = 0,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = false,
            AccessFailedCount = 0,
            Addresses = null
        };

        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> UpdateUser(UpdateUserDto userDto, string email)
    {
        var userToUpdate = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        var user = new User();

        user.Code = userDto.Code ?? userToUpdate.Code;
        user.BirthDate = userDto.BirthDate ?? userToUpdate.BirthDate;
        user.Email = userDto.Email ?? userToUpdate.Email;
        user.AccessFailedCount = userDto.AccessFailedCount ?? userToUpdate.AccessFailedCount;
        user.ConcurrencyStamp = userDto.ConcurrencyStamp ?? userToUpdate.ConcurrencyStamp;
        user.EmailConfirmed = userDto.EmailConfirmed ?? userToUpdate.EmailConfirmed;
        user.FirstName = userDto.FirstName ?? userToUpdate.FirstName;
        user.LastName = userDto.LastName ?? userToUpdate.LastName;
        user.LockoutEnd = userDto.LockoutEnd ?? userToUpdate.LockoutEnd;
        user.Newsletter = userDto.Newsletter ?? userToUpdate.Newsletter;
        user.NormalizedEmail = userDto.NormalizedEmail ?? userToUpdate.NormalizedEmail;
        user.NormalizedUserName = userDto.NormalizedUserName ?? userToUpdate.NormalizedUserName;
        user.UserName = userDto.UserName ?? userToUpdate.UserName;
        user.Telefon = userDto.Telefon ?? userToUpdate.Telefon;
        user.TcKimlik = userDto.TcKimlik ?? userToUpdate.TcKimlik;
        user.SecurityStamp = userDto.SecurityStamp ?? userToUpdate.SecurityStamp;
        user.PrivateDiscountType = userDto.PrivateDiscountType ?? userToUpdate.PrivateDiscountType;
        user.PhoneNumber = userDto.PhoneNumber ?? userToUpdate.PhoneNumber;
        user.PasswordHash = userDto.PasswordHash ?? userToUpdate.PasswordHash;

        _context.Entry(user).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return user;
    }
}