using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Selu383.SP24.Api.Data;
using Selu383.SP24.Api.Features;
using System.Reflection.Metadata;
using System.Transactions;

namespace Selu383.SP24.Api.Controllers;

    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly DbSet<User> users;
        private readonly DataContext dataContext;
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager, DataContext dataContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.dataContext = dataContext;
            users = dataContext.Set<User>();
        }

        [HttpGet]
        public IQueryable<UserDto> GetAllRoles()
        {
            return GetUserDtos(users);
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<UserDto> GetUserById(int id)
        {
            var result = GetUserDtos(users.Where(x => x.Id == id)).FirstOrDefault();
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserCreateDto>> CreateUserAsync(UserCreateDto userCreateDto)
        {
            if (userCreateDto == null)
            {
                return BadRequest();
            }
            if (userCreateDto.UserName.IsNullOrEmpty())
            {
                return BadRequest();
            }
            if (userCreateDto.Password.IsNullOrEmpty())
            {
                return BadRequest();
            }
            if (userCreateDto.UserName == dataContext.Users.First().UserName)
            {
                return BadRequest();
            }
            if (userCreateDto.Roles.IsNullOrEmpty())
            {
                return BadRequest();
            }
            if (userCreateDto.Password.IsNullOrEmpty())
            {
                return BadRequest();
            }
            
            if (!((userCreateDto.Roles.All("Admin".Contains)) || (userCreateDto.Roles.All("User".Contains))))
            {
                return BadRequest();
            }

            var userToCreate = new User
            {
                UserName = userCreateDto.UserName,
            };

            var createdResult = await _userManager.CreateAsync(userToCreate
            , userCreateDto.Password);

            if (!createdResult.Succeeded)
            {
                return BadRequest();
            }

            var temp = dataContext.Users.First(x => x.UserName == userToCreate.UserName);


            var roleResult = await _userManager.AddToRoleAsync(temp, userCreateDto.Roles[0]);


            var rolesList = await _userManager.GetRolesAsync(userToCreate);


            var userToReturn = new UserDto
            {
                Id = userToCreate.Id,
                UserName = userCreateDto.UserName,
                Roles = rolesList
            };

            return Ok(userToReturn);
        }




        private static IQueryable<UserDto> GetUserDtos(IQueryable<User> users)
        {
            return users
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Roles = x.Roles.Select(y => y.Role!.Name).ToArray()
                });
        }
    }
