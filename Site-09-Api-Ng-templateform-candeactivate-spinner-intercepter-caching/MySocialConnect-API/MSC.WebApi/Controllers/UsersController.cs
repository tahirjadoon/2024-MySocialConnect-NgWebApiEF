using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSC.Core.BusinessLogic;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;
using MSC.Core.Extensions;

namespace MSC.WebApi.Controller;

// /api/users
[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserBusinessLogic _userBusinessLogic;

    public UsersController(IUserBusinessLogic userBusinessLogic)
    {
        _userBusinessLogic = userBusinessLogic;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        //var users = await _userBusinessLogic.GetUsersAsync();
        var users = await _userBusinessLogic.GetUsersAMQEAsync();
        if (users == null || !users.Any())
        {
            return NotFound("No users found!");
        }
        return Ok(users);
    }

    [HttpGet("{id}", Name = "GetUserById")] // /api/users/2
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        //var user = await _userBusinessLogic.GetUserAsync(id);
        var user = await _userBusinessLogic.GetUserAMQEAsync(id);
        if (user == null)
        {
            return NotFound($"No user found by id {id}");
        }

        return Ok(user);
    }

    [HttpGet("{userName}/name", Name = "GetUserByName")] // /api/users/Bob/name
    public async Task<ActionResult<UserDto>> GetUser(string userName)
    {
        //var user = await _userBusinessLogic.GetUserAsync(userName);
        var user = await _userBusinessLogic.GetUserAMQEAsync(userName);
        if (user == null)
        {
            return NotFound($"No user found by name {userName}");
        }
        return Ok(user);
    }

    [HttpGet("{guid}/guid", Name = "GetUserByGuid")] // /api/users/---/guid
    public async Task<ActionResult<UserDto>> GetUser(Guid guid)
    {
        //var user = await _userBusinessLogic.GetUserAsync(guid);
        var user = await _userBusinessLogic.GetUserAMQEAsync(guid);
        if (user == null)
        {
            return NotFound($"No user found by guid {guid}");
        }
        return Ok(user);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser([FromBody] MemberUpdateDto member)
    {
        //get the user claims
        var userClaims = User.GetUserClaims();
        if(userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
            return BadRequest("User Issue");

        var isUpdate = await _userBusinessLogic.UpdateUserAsync(member, userClaims);
        if(!isUpdate)
            return BadRequest("User not updated");

        return NoContent(); //204
    }

}
