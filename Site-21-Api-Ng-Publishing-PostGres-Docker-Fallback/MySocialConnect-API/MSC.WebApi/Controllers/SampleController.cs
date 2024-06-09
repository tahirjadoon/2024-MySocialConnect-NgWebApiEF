using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MSC.Core.BusinessLogic;
using MSC.Core.DB.Data;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos.Helper;
using MSC.Core.Extensions;

namespace MSC.WebApi.Controllers;

// /api/sample
public class SampleController : BaseApiController
{
    private readonly IOptions<EnvConfig> _envConfig;
    private readonly IConfiguration _config;
    private readonly IUserBusinessLogic _userBal;
    private readonly DataContext _context;

    public SampleController(IOptions<EnvConfig> envConfig, IConfiguration config, 
    IUserBusinessLogic userBal, DataContext context)
    {
        _envConfig = envConfig;
        _config = config;
        _userBal = userBal;
        _context = context;
    }

    [HttpPost("sample1")]
    public ActionResult<bool> Sample1()
    {
        //IConfiguration
        var config = _config;
        //using Ioptions
        var howdy = _envConfig;
        return Ok(true);
    }

    [HttpPost("sample2")]
    public ActionResult<bool> Sample2()
    {
        //IConfiguration with extension
        var config = _config;
        var tokenKey = _config.GetTokenKey();
        //using Ioptions
        var howdy = _envConfig;
        return Ok(true);
    }

    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetSecret()
    {
        return "secret text";
    }

    [HttpGet("usernotfound")]
    public ActionResult<AppUser> GetUserNotFound()
    {
        var user = _context.Users.Find(-1);
        if(user == null)
            return NotFound();
        return user;
    }

    [HttpGet("servererror")]
    public ActionResult<string> GetServerError()
    {
        var user = _context.Users.Find(-1);
        var userReturn = user.ToString(); //will throw null reference exception
        return userReturn;
    }

    [HttpGet("badrequest")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("This was not a good request");
    }

    [HttpPost("samplelogin")]
    public ActionResult<bool> SampleLogin([FromBody] SampleLoginDto sampleLoginDto)
    {
        if(sampleLoginDto == null)
            return BadRequest("Sample login dto empty");
        return true;
    }
}

public class SampleLoginDto
{
    [Required(ErrorMessage = "UserName is empty")]
    [MinLength(5, ErrorMessage = "UserName length must be atleast 5 chars")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is empty")]
    [StringLength(16, MinimumLength = 10)]
    [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\\d)(?=.*?[@#$&()<>]).+$", ErrorMessage = "Password must have an upper case, a lower case, a number and one special character from the set @#$&()<>")]
    public string Password { get; set; }
}
