using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSC.Core.BusinessLogic;
using MSC.Core.Constants;
using MSC.Core.Extensions;

namespace MSC.WebApi.Controller;

public class AdminController : BaseApiController
{
    private readonly IUserBusinessLogic _userBl;

    public AdminController(IUserBusinessLogic _userBl)
    {
        this._userBl = _userBl;
    }

    [Authorize(Policy = SiteIdentityConstants.AuthPolicy_Admin)]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult<IEnumerable<object>>> GetUsersWithRoles()
    {
        var users = await _userBl.GetUSersWithRoles();
        if(users == null || !users.Any())
            return NotFound("No users found");
        return Ok(users);
    }

    [Authorize(Policy = SiteIdentityConstants.AuthPolicy_Moderator_Photos)]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Admins or moderators can see this");
    }

    [Authorize(Policy = SiteIdentityConstants.AuthPolicy_Admin)]
    [HttpPost("edit-roles/{guid:Guid}")]
    public async Task<ActionResult<IEnumerable<string>>> EditRoles([FromRoute] Guid guid, [FromQuery] string roles)
    {
        if(string.IsNullOrWhiteSpace(roles))
            return BadRequest("No roles provided to update");

        //roles are comma seperated list so split
        var rolesList = roles.StringSplitToType<string>();
        if(rolesList == null || !rolesList.Any())
            return BadRequest("Unable to parse the roles passed");

        //edit the roles
        var result = await _userBl.EditRolesForUser(User.GetId(), guid, rolesList);

        ActionResult actionResult = result.HttpStatusCode switch{
            HttpStatusCode.OK => Ok(result.ConvertDataToType<IEnumerable<string>>()),
            HttpStatusCode.BadRequest => BadRequest(result.Message),
            HttpStatusCode.NotFound => NotFound(result.Message),
            _ => BadRequest("Unable to edit roles")
        };

        return actionResult;
    }
}
