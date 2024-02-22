using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSC.Core.BusinessLogic;
using MSC.Core.DB.Entities;

namespace MSC.WebApi.Controller;

// /api/users
public class UsersController : BaseApiController
{
    private readonly IUserBusinessLogic _userBusinessLogic;

    public UsersController(IUserBusinessLogic userBusinessLogic)
    {
        _userBusinessLogic = userBusinessLogic;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await _userBusinessLogic.GetUsersAsync();
        if (users == null || !users.Any())
        {
            return NotFound("No users found!");
        }
        return users.ToList();
    }

    [HttpGet("{id}", Name = "GetUserById")] // /api/users/2
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        var user = await _userBusinessLogic.GetUserAsync(id);
        if (user == null)
        {
            return NotFound($"No user found by id {id}");
        }

        return user;
    }

    [HttpGet("{userName}/name", Name = "GetUserByName")] // /api/users/Bob/name
    public async Task<ActionResult<AppUser>> GetUser(string userName)
    {
        var user = await _userBusinessLogic.GetUserAsync(userName);
        if (user == null)
        {
            return NotFound($"No user found by name {userName}");
        }
        return user;
    }

    [HttpGet("{guid}/guid", Name = "GetUserByGuid")] // /api/users/---/guid
    public async Task<ActionResult<AppUser>> GetUser(Guid guid)
    {
        var user = await _userBusinessLogic.GetUserAsync(guid);
        if (user == null)
        {
            return NotFound($"No user found by guid {guid}");
        }
        return user;
    }
}
