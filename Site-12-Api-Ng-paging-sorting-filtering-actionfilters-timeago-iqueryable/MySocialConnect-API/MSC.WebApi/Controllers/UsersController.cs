using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSC.Core.BusinessLogic;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;
using MSC.Core.Dtos.Pagination;
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userParams">Passed as query string</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<PagedList<UserDto>>> GetUsers([FromQuery]UsersSearchParamDto userParams)
    {
        //get current user
        var currentUser = await _userBusinessLogic.GetUserAMQEAsync(User.GetUserName());
        if(currentUser == null)
            return BadRequest("User issue");

        //var users = await _userBusinessLogic.GetUsersAsync();
        if(string.IsNullOrWhiteSpace(userParams.Gender))
            userParams.Gender = currentUser.Gender.ToLower() == "male" ? "female" : "male";

        var users = await _userBusinessLogic.GetUsersAMQEAsync(userParams, currentUser.GuId);
        if (users == null || !users.Any())
        {
            return NotFound("No users found!");
        }

        //write pagination header
        Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

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

    [HttpPost("add/photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        //get the user claims
        var userClaims = User.GetUserClaims();
        if(userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
            return BadRequest("User Issue");

        var photoDto = await _userBusinessLogic.AddPhotoAsync(file, userClaims);
        if(photoDto == null)
            return BadRequest("Problem adding photo");

        //this is to tell from where to pick the info. Point to GetUserByGuid and pass the guid to it
        //this will return 401
        //look at the location headers for the for url to the action that gets the user by guid
        return CreatedAtRoute("GetUserByGuid", new {guid = userClaims.Guid.ToString() }, photoDto);
    }

    [HttpDelete("delete/{photoId}/photo")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        //get the user claims
        var userClaims = User.GetUserClaims();
        if(userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
            return BadRequest("User Issue");
        
        var result = await _userBusinessLogic.DeletePhotoAsync(photoId, userClaims);

        ActionResult actionResult = BadRequest("Unable to delete photo");
        if (result != null)
        {
            switch (result.HttpStatusCode)
            {
                case HttpStatusCode.OK:
                    actionResult = Ok();
                    break;
                case HttpStatusCode.BadRequest:
                    actionResult = BadRequest(result.Message ?? "Unable to delete photo");
                    break;
                case HttpStatusCode.NotFound:
                    actionResult = NotFound(result.Message ?? "Photo not found");
                    break;
                default:
                    actionResult = BadRequest("Unable to delete photo");
                    break;
            }
        }

        return actionResult;
    }

    [HttpPut("set/photo/{photoId}/main")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        //get claims
        var userClaims = User.GetUserClaims();
        if (userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
            return BadRequest("User issue");

        var result = await _userBusinessLogic.SetPhotoMainAsync(photoId, userClaims);

        if (result)
            return NoContent();

        return BadRequest("Unable to set photo to main");
    }

}
