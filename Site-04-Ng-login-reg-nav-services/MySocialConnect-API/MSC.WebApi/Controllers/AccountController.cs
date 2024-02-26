using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSC.Core.BusinessLogic;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;

namespace MSC.WebApi.Controller;

// /api/account
public class AccountController : BaseApiController
{
    private readonly IUserBusinessLogic _userBusinessLogic;

    public AccountController(IUserBusinessLogic userBusinessLogic)
    {
        _userBusinessLogic = userBusinessLogic;
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoggedInUserDto>> Register([FromBody] UserRegisterDto registerUser)
    {
        if(await _userBusinessLogic.UserExists(registerUser.UserName))
            return BadRequest("Username already taken");

        var loggedInUser = await _userBusinessLogic.RegisterUserAsync(registerUser);
        if(loggedInUser == null || string.IsNullOrWhiteSpace(loggedInUser.UserName))
            return BadRequest("Unable to create registration");

        return loggedInUser;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoggedInUserDto>> Login([FromBody] LoginDto login)
    {
        var user = await _userBusinessLogic.LoginAsync(login);
        if (user == null || string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Token))
            return Unauthorized("Unable to login user");

        return Ok(user);
    }
}