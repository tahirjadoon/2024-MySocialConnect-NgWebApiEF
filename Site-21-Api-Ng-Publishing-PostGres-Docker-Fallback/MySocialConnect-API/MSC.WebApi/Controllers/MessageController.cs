using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSC.Core.BusinessLogic;
using MSC.Core.Dtos;
using MSC.Core.Dtos.Pagination;
using MSC.Core.Extensions;

namespace MSC.WebApi.Controllers;

[Authorize]
public class MessageController : BaseApiController
{
    private readonly IMessageBusinessLogic _msgBl;

    public MessageController(IMessageBusinessLogic msgBl)
    {
        _msgBl = msgBl;
    }

    [HttpPost("create")]
    public async Task<ActionResult<MessageDto>> CreateMessage([FromBody] MessageCreateDto msg)
    {
        //get the claims
        var claims = User.GetUserClaims();
        if(claims == null)
            return BadRequest("User issue");

        var result = await _msgBl.AddMessage(msg, claims.Id);
        if(result == null)
            return BadRequest("Unable to send message");

        var message = result.ConvertDataToType<MessageDto>();
        
        ActionResult actionResult = result.HttpStatusCode switch
        {
            HttpStatusCode.OK => Ok(message),
            HttpStatusCode.BadRequest => BadRequest(result.Message),
            HttpStatusCode.NotFound => NotFound(result.Message),
            _ => BadRequest("Unable to send message")
        };

        return actionResult;
    }

    [HttpGet("user/get/messages")]
    public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageSearchParamDto search)
    {
        //get the claims
        var claims = User.GetUserClaims();
        if(claims == null)
            return BadRequest("User issue");

        search.UserId = claims.Id;

        var messages = await _msgBl.GetMessagesForUser(search);
        if(messages == null)
            return NotFound("No messages found");

        Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

        return Ok(messages);        
    }

    [HttpGet("thread/{recipientId:int}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread([FromRoute] int recipientId)
    {
        //get the claims
        var claims = User.GetUserClaims();
        if(claims == null)
            return BadRequest("User issue");

        var messages = await _msgBl.GetMessageThread(claims.Id, recipientId);
        if(messages == null)
            NotFound("No messages found");

        return Ok(messages);
    }

    [HttpDelete("delete/{msgGuid:Guid}")]
    public async Task<ActionResult> DeleteMessage([FromRoute] Guid msgGuid)
    {
        //get the claims
        var claims = User.GetUserClaims();
        if(claims == null)
            return BadRequest("User issue");

        var result = await _msgBl.DeleteMessage(claims.Id, msgGuid);
        if(result == null)
            BadRequest("Unable to delete message");

        ActionResult actionResult = result.HttpStatusCode switch
        {
            HttpStatusCode.OK => Ok(),
            HttpStatusCode.BadRequest => BadRequest(result.Message),
            HttpStatusCode.NotFound => NotFound(result.Message),
            _ => BadRequest("Unale to delete message")
        };

        return actionResult;        
    }

}
