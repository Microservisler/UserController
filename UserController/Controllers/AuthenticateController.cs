using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserController.Auth;
using UserController.Contexts;
using UserController.Models;
using UserController.DTOs;
using UserController.General;
using UserController.Services.Interfaces;

namespace UserController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        private MapperConfiguration _mapper => new(cfg =>
        {
            cfg.CreateMap<CreateUserDto, User>();
            cfg.CreateMap<UpdateUserDto, User>();
        });

        public AuthenticateController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            DataContext context, 
            IUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _userService = userService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(typeof(User), 200)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                PhoneNumber = model.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            var newUser = await _userService.Register(model);
            if (!result.Succeeded || newUser == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            var newUser = await _userService.Register(model);
            if (!result.Succeeded || newUser == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetUsers([FromQuery] RequestParameters parameters)
        {
            if (parameters.Offset > 100) parameters.Offset = 100;

            var response = new Response<PaginationResponse<List<User>>>()
            {
                Status = 0,
                Cached = 0,
                Count = 0
            };

            try
            {
                IQueryable<User> query = _context.Users
                    .AsSplitQuery()
                    .AsNoTracking()
                    .OrderByDescending(u => u.Id);

                if (!string.IsNullOrEmpty(parameters.Text))
                {
                    query = query
                        .Where(u => EF.Functions.Like("FirstName", $"%{parameters.Text}%") ||
                                    EF.Functions.Like("LastName", $"%{parameters.Text}%") ||
                                    EF.Functions.Like("Email", $"%{parameters.Text}%") ||
                                    EF.Functions.Like("UserName", $"%{parameters.Text}%"));
                }

                if (!string.IsNullOrEmpty(parameters.Code))
                {
                    query = query.Where(u => u.Code == parameters.Code);
                }

                if (!query.Any())
                {
                    var notFoundError = new ErrorResponse()
                    {
                        Status = 404,
                        RequestUrl = Request.Path,
                        Title = "Not Found",
                        Message = "Not users found"
                    };

                    response.Errors = new[] { notFoundError };
                    return NotFound(response);
                }

                var userCount = await query.CountAsync();

                var users = await query
                    //.Include(p => p.Addresses)
                    //.Skip((parameters.Page - 1) * parameters.Offset)
                    //.Take(parameters.Offset)
                    .ToListAsync();

                var pagination = new PaginationResponse<List<User>>()
                {
                    Page = parameters.Page,
                    Offset = parameters.Offset,
                    TotalCount = userCount,
                    TotalPage = (int)Math.Ceiling((double)userCount / parameters.Offset),
                    NextPageUrl = NextPageUrl(parameters, userCount),
                    PreviousPageUrl = PreviousPageUrl(parameters, userCount),
                    FirstPageUrl = FirstPageUrl(parameters, userCount),
                    LastPageUrl = LastPageUrl(parameters, userCount),
                    Items = users
                };

                response.Count = userCount;
                response.Message = $"Found {userCount} user entries in the database";
                response.Data = pagination;
                return Ok(response);
            }
            catch (Exception e)
            {
                var error = new ErrorResponse()
                {
                    Status = Request.HttpContext.Response.StatusCode,
                    RequestUrl = Request.Path,
                    Title = "Internal Server Error",
                    Message = e.ToString()
                };

                response.Errors = new[] { error };
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Route("user")]
        public async Task<IActionResult> GetUser(string id)
        {
            var response = new Response<User>()
            {
                Status = 1,
                Cached = 0,
                Count = 0
            };

            try
            {
                var user = await _context.Users
                    .AsSplitQuery()
                    .AsNoTracking()
                    .Where(x => x.Id == id)
                    .Include(u => u.Addresses)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    var notFoundError = new ErrorResponse()
                    {
                        Status = Request.HttpContext.Response.StatusCode,
                        RequestUrl = Request.Path,
                        Title = "Not Found",
                        Message = $"User with id of {id} not found"
                    };

                    response.Errors = new[] { notFoundError };
                    return NotFound(response);
                }

                response.Count = 1;
                response.Message = "Found 1 user entry in the database";
                response.Data = user;
                return Ok(response);
            }
            catch (Exception e)
            {
                var error = new ErrorResponse()
                {
                    Status = Request.HttpContext.Response.StatusCode,
                    RequestUrl = Request.Path,
                    Title = "Internal Server Error",
                    Message = e.InnerException?.Message ?? e.Message
                };

                response.Errors = new[] { error };
                return StatusCode(500, response);
            }
        }

        [HttpPut]
        [Route("user/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto userDto)
        {
            var user = _mapper.CreateMapper().Map<User>(userDto);

            var response = new Response<User>()
            {
                Status = 0,
                Cached = 0,
                Count = 0
            };

            if (id != userDto.Id)
            {
                var badRequestError = new ErrorResponse()
                {
                    Status = Request.HttpContext.Response.StatusCode,
                    RequestUrl = Request.Path,
                    Title = "IDs not match",
                    Message =
                        "Requested id and the user id does not match. Please double-check the both id and the body"
                };

                response.Errors = new[] { badRequestError };
                return BadRequest(response);
            }

            var userToUpdate = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            var identityUserToUpdate = await _context.AspNetUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == userToUpdate.Email);

            if (userToUpdate == null)
            {
                var notFoundError = new ErrorResponse()
                {
                    Status = Request.HttpContext.Response.StatusCode,
                    RequestUrl = Request.Path,
                    Title = "user not found",
                    Message = $"User with id \"{id}\" not found"
                };

                response.Errors = new[] { notFoundError };
                return NotFound(response);
            }

            try
            {
                // Identity User Update
                IdentityUser identityUser = new()
                {
                    UserName = userDto.UserName ?? identityUserToUpdate.UserName,
                    NormalizedUserName = userDto.UserName.Normalize() ?? identityUserToUpdate.UserName.Normalize(),
                    Email = userDto.Email ?? identityUserToUpdate.Email,
                    NormalizedEmail = userDto.Email.Normalize() ?? identityUserToUpdate.Email.Normalize(),
                    EmailConfirmed = identityUserToUpdate.EmailConfirmed,
                    PasswordHash = identityUserToUpdate.PasswordHash,
                    SecurityStamp = identityUserToUpdate.SecurityStamp,
                    ConcurrencyStamp = identityUserToUpdate.ConcurrencyStamp,
                    PhoneNumber = userDto.PhoneNumber ?? identityUserToUpdate.PhoneNumber,
                    PhoneNumberConfirmed = identityUserToUpdate.PhoneNumberConfirmed,
                    TwoFactorEnabled = identityUserToUpdate.TwoFactorEnabled,
                    LockoutEnd = identityUserToUpdate.LockoutEnd,
                    LockoutEnabled = identityUserToUpdate.LockoutEnabled,
                    AccessFailedCount = identityUserToUpdate.AccessFailedCount
                };

                _context.Entry(identityUser).State = EntityState.Modified;

                // Users user update
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


            }
            catch (Exception e)
            {
                var error = new ErrorResponse()
                {
                    Status = Request.HttpContext.Response.StatusCode,
                    RequestUrl = Request.Path,
                    Title = "Internal Server Error",
                    Message = e.InnerException?.Message ?? e.Message
                };

                response.Errors = new[] { error };
                return StatusCode(500, response);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var error = new ErrorResponse()
                {
                    Status = Request.HttpContext.Response.StatusCode,
                    RequestUrl = Request.Path,
                    Title = "Internal Server Error",
                    Message = e.InnerException?.Message ?? e.Message
                };

                response.Errors = new[] { error };
                return StatusCode(500, response);
            }

            response.Status = 1;
            response.Count = 1;
            response.Message = $"User with id \"{user.Id}\" updated successfully";
            response.Data = user;
            return Ok(response);
        }

        private string NextPageUrl(RequestParameters parameters, int total)
        {
            var result = $"http://api.akilliphone.com/users?page=1&offset={parameters.Offset}";

            if (parameters.Page * parameters.Offset < total)
                result = $"http://api.akilliphone.com/users?page={parameters.Page + 1}&offset={parameters.Offset}";

            if (!string.IsNullOrEmpty(parameters.Text))
                result += $"&text={parameters.Text}";
            if (!string.IsNullOrEmpty(parameters.Code))
                result += $"&code={parameters.Code}";

            return result;
        }

        private string PreviousPageUrl(RequestParameters parameters, int total)
        {
            var result = $"http://api.akilliphone.com/users?page=1&offset={parameters.Offset}";

            if (parameters.Page * parameters.Offset < total)
            {
                result = (parameters.Page - 1) < 1
                    ? result
                    : $"http://api.akilliphone.com/users?page={parameters.Page - 1}&offset={parameters.Offset}";

                if (!string.IsNullOrEmpty(parameters.Text))
                    result += $"&text={parameters.Text}";
                if (!string.IsNullOrEmpty(parameters.Code))
                    result += $"&code={parameters.Code}";

                return result;
            }

            if (!string.IsNullOrEmpty(parameters.Text))
                result += $"&text={parameters.Text}";
            if (!string.IsNullOrEmpty(parameters.Code))
                result += $"&code={parameters.Code}";

            return result;
        }

        private string FirstPageUrl(RequestParameters parameters, int total)
        {
            var result = $"http://api.akilliphone.com/users?page=1&offset={parameters.Offset}";

            if (!string.IsNullOrEmpty(parameters.Text))
                result += $"&text={parameters.Text}";
            if (!string.IsNullOrEmpty(parameters.Code))
                result += $"&code={parameters.Code}";

            return result;
        }

        private string LastPageUrl(RequestParameters parameters, int total)
        {
            var result = $"http://api.akilliphone.com/users?page={(int)Math.Ceiling((double)total / parameters.Offset)}&offset={parameters.Offset}";

            if (!string.IsNullOrEmpty(parameters.Text))
                result += $"&text={parameters.Text}";
            if (!string.IsNullOrEmpty(parameters.Code))
                result += $"&code={parameters.Code}";

            return result;
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
