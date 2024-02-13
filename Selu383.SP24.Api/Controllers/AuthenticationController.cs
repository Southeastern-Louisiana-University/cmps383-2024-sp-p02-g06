using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Selu383.SP24.Api.Data;

using Selu383.SP24.Api.Features;

using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace Selu383.SP24.Api.Controllers.AuthenticationController;

    [Route("/api/authentication/")]
    [ApiController]
    [Authorize]
    public class AuthenticationController : ControllerBase

    {
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
        private readonly DataContext _dataContext;
        public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager, DataContext dataContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dataContext = dataContext;
        }

        [HttpPost]
        [Route("/loginTest")]
        [AllowAnonymous]
        public async Task<ActionResult> LoginTest()
        {
            var user = _dataContext.Users.First(x => x.UserName == "bob");
            await _signInManager.SignInAsync(user, true);
            return Ok();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Login(LoginDto user)
        {
            var userFound = await _userManager.FindByNameAsync(user.UserName);

            if (userFound == null)
            {
                return BadRequest();
            }
            var rolesList = await _userManager.GetRolesAsync(userFound);

            var passwordIsValid = await _userManager.CheckPasswordAsync(userFound, user.Password);

            if (passwordIsValid)
            {
                await _signInManager.SignInAsync(userFound, false);
                //await _signInManager.CheckPasswordSignInAsync(userFound, user.Password, false);
                return Ok(new
                {
                    Id = userFound.Id,
                    UserName = userFound.UserName,
                    Roles = rolesList
                });
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> Me()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var rolesList = await _userManager.GetRolesAsync(user);



            if (user != null)
            {
                return Ok(new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Roles = rolesList
                });
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return StatusCode(200);
        }



    }
