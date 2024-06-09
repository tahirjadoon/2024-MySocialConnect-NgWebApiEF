using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSC.Core.BusinessLogic;
using MSC.Core.Constants;
using MSC.Core.Dtos;
using MSC.Core.Extensions;

namespace MSC.WebApi.Controllers;

public class AdminController : BaseApiController
{
    private readonly IUserBusinessLogic _userBl;
    private readonly IPhotoBusinessLogic _photoBl;

    public AdminController(IUserBusinessLogic _userBl, IPhotoBusinessLogic _photoBl)
    {
        this._userBl = _userBl;
        this._photoBl = _photoBl;
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

    [Authorize(Policy = SiteIdentityConstants.AuthPolicy_Moderator_Photos)]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult<IEnumerable<PhotoForApprovalDto>>> GetPhotosForModeration()
    {
        var photos = await _photoBl.GetUnapprovedPhotosAsync();
        if(photos == null || !photos.Any())
            return NoContent();

        return Ok(photos);
    }

    [Authorize(Policy = SiteIdentityConstants.AuthPolicy_Moderator_Photos)]
    [HttpPut("photo-to-approve/{photoId}")]
    public async Task<ActionResult> ApprovePhoto([FromRoute]int photoId)
    {
        var result = await _photoBl.ApprovePhotoAsync(photoId);
        ActionResult actionResult = result.HttpStatusCode switch{
            HttpStatusCode.OK => Ok(),
            HttpStatusCode.BadRequest => BadRequest(result.Message),
            HttpStatusCode.NotFound => NotFound(result.Message),
            _ => BadRequest("Unable to approve photo")
        };
        return actionResult;
    }

    [Authorize(Policy = SiteIdentityConstants.AuthPolicy_Moderator_Photos)]
    [HttpPut("photo-to-reject/{photoId}")]
    public async Task<ActionResult> RejectPhoto([FromRoute]int photoId)
    {
        var result = await _photoBl.RemovePhotoAsync(photoId);
        ActionResult actionResult = result.HttpStatusCode switch{
            HttpStatusCode.OK => Ok(),
            HttpStatusCode.BadRequest => BadRequest(result.Message),
            HttpStatusCode.NotFound => NotFound(result.Message),
            _ => BadRequest("Unable to reject photo")
        };
        return actionResult;
    }
}
