using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSC.Core.BusinessLogic;
using MSC.Core.Dtos;
using MSC.Core.Dtos.Pagination;
using MSC.Core.Extensions;

namespace MSC.WebApi.Controller;

[Authorize]
public class LikesController : BaseApiController
{
    private readonly IUserBusinessLogic _userBl;
    private readonly ILikesBusinessLogic _likesBl;

    public LikesController(IUserBusinessLogic userBl, ILikesBusinessLogic likesBl)
    {
        _userBl = userBl;
        _likesBl = likesBl;
    }

    [HttpPost("{likedUserId:int}")]
    public async Task<ActionResult> AddLike(int likedUserId)
    {
        if(likedUserId <= 0)
            return BadRequest("The user to like userId is required");
        
        //get the claims
        var userClaims = User.GetUserClaims();
        if(userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName || !userClaims.HasId))
            return BadRequest("User issue");

        var result = await _likesBl.AddLike(likedUserId, userClaims);

        ActionResult actionResult =  result.HttpStatusCode switch
        {
            HttpStatusCode.OK => Created(),
            HttpStatusCode.BadRequest => BadRequest(result.Message),
            HttpStatusCode.NotFound => NotFound(result.Message),
            _ => BadRequest("Unable to add like")
        };
        
        return actionResult;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikeSearchParamDto search)
    {
        //get the claims
        var userClaims = User.GetUserClaims();
        if(userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName || !userClaims.HasId))
            return BadRequest("User issue");
        
        search.UserId = userClaims.Id;

        var users = await _likesBl.GetUserLikes(search);
        if(users == null)
            return NotFound("No users found");

        //write pagination header
        Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

        return Ok(users);
    }
}
