using UserController.Auth;
using UserController.DTOs;
using UserController.Models;

namespace UserController.Services.Interfaces;

public interface IUserService
{
    Task<User> Register(RegisterModel register);

    Task<User> UpdateUser(UpdateUserDto userDto, string email);
}