using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MSC.Core.BusinessLogic;
using MSC.Core.DB.Entities.SignalR;
using MSC.Core.Dtos;
using MSC.Core.Extensions;
using MSC.Core.SignalR;

namespace MSC.WebApi.SignalR;

[Authorize]
/// <summary>
/// MessageHub, it derives from Hub and then override the virtual methods 
/// No package to install for SignalR
/// Messages will be sent using the MessageHub and not MessageController.CreateMessage
/// </summary>
public class MessageHub : Hub
{
    private const string _keyReceiveMessageThread = "ReceiveMessageThread";
    private const string _keyNewMessage = "NewMessage";
    private const string _keyUpdatedGroup = "UpdatedGroup";

    //when the recipient is not on the message page, is online then send the new message received event
    private const string _keyNewMessageReceived = "NewMessageReceived";

    private readonly IMessageBusinessLogic _msgBl;
    private readonly IUserBusinessLogic _userBl;
    private readonly ISignalRBusinessLogic _srBl;
    private readonly PresenceTrackerMemory _presenceTracker;
    private readonly IHubContext<PresenceHub> _presenceHub;
    private readonly IMapper _mapper;

    public MessageHub(IMessageBusinessLogic msgBl, 
                    IUserBusinessLogic userBl, 
                    ISignalRBusinessLogic srBl, 
                    PresenceTrackerMemory presenceTracker,
                    IHubContext<PresenceHub> presenceHub,
                    IMapper mapper)
    {
        _msgBl = msgBl;
        _userBl = userBl;
        _srBl = srBl;
        _presenceTracker = presenceTracker;
        _presenceHub = presenceHub;
        _mapper = mapper;
    }

    /// <summary>
    /// Implement OnConnectedAsync 
    /// </summary>
    /// <returns></returns>
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var connectionId = Context.ConnectionId;
        
        //current user
        var callerUserName = Context.User.GetUserName();
        var callerUserId = Context.User.GetId(); 

        //will be passing in the other users info via query string
        var otherUserName = httpContext.Request.Query["otherUserName"].ToString();
        var otherUserId = int.Parse(httpContext.Request.Query["otherUserId"].ToString());

        //build the group name, create a group of two users.
        var groupName = GetGroupName(callerUserName, otherUserName);

        //add to SignalR groups
        await Groups.AddToGroupAsync(connectionId, groupName);

        //add to database
        var group = await AddToGroup(groupName, connectionId, callerUserName, callerUserId);
        //send the message to the group
        await Clients.Group(groupName).SendAsync(_keyUpdatedGroup, group);

        //get the message thread from the message business logic just like MessgeController
        var messages = await _msgBl.GetMessageThread(callerUserId, otherUserId);

        //send the messages to the caller
        //await Clients.Group(groupName).SendAsync(_keyReceiveMessageThread, messages);
        await Clients.Caller.SendAsync(_keyReceiveMessageThread, messages);
    }

    /// <summary>
    /// Implmenent OnDisconnectedAsync 
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var group = await RemoveFromMessageGroup(Context.ConnectionId);
        await Clients.Group(group.GroupName).SendAsync(_keyUpdatedGroup, group);
        //users will be automatically removed from the group 
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Moved here from MessageController
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public async Task CreateMessage(MessageCreateDto msg)
    {
        //get the claims
        var claims = Context.User.GetUserClaims();
        if(claims == null || !claims.HasUserName || !claims.HasId || !claims.HasGuid) 
            throw new HubException("User issue");

        //check message
        if(msg == null || msg.RecipientId <= 0 || string.IsNullOrWhiteSpace(msg.MessageContent))
            throw new HubException("Message info invalid");

        //check that we have a connection for the recipient. If we have then the recipient is on message page
        bool isRecipientOnMessagePage = false;
        bool markMessageAsRead = false;
        var recipient = await _userBl.GetUserRawAsync(msg.RecipientId, includePhotos: true);
        if(recipient == null)   
            throw new HubException("Recipient not found");
        var groupName = GetGroupName(claims.UserName, recipient.UserName);
        SignalRGroup group = await _srBl.GetMessageGroup(groupName);
        if(group != null && group.Connections.Any(x => x.UserName == recipient.UserName)){
            isRecipientOnMessagePage = true;
            markMessageAsRead = true;
        }

        //add message , mark the message read if the recipient is in message group with sender
        var result = await _msgBl.AddMessageWithReadRecipt(msg, claims.Id, markMessageAsRead);
        if(result == null) 
            throw new HubException("Unable to send message");
        if(result.HttpStatusCode != HttpStatusCode.OK)
            throw new HubException(result.Message ?? "Unable to send message");
        
        //the message that got added 
        var messagedAdded = result.ConvertDataToType<MessageDto>();

        //when the recipient is not on the same message page and have connection then notify the recipient
        if(!isRecipientOnMessagePage){
            var connections = await _presenceTracker.GetConnectionsForUser(recipient.UserName);
            if(connections != null){
                var sender = await _userBl.GetUserAsync(Context.User.GetId());
                if(sender != null){
                    //to display to the logged in user the sender info
                    //since presenceHub us bing used to send the message, implement this on the presenceHub client
                    await _presenceHub.Clients.Clients(connections).SendAsync(_keyNewMessageReceived, sender);
                }
            }
        }

        //send the event for new message created
        await Clients.Group(groupName).SendAsync(_keyNewMessage, messagedAdded);
    }

    //sort in alphabatical order and build group name
    private string GetGroupName(string caller, string other)
    {
        //Less than zero –strA is less than strB.
        //Zero –strA and strB are equal.
        //Greater than zero –strA is greater than strB
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private async Task<SignalRGroup> AddToGroup(string groupName, string connectionId, string callerUserName, int callerUSerId)
    {
        //get the group from the DB and save it
        SignalRGroup group = await _srBl.GetMessageGroup(groupName);
        if(group == null){
            group = new SignalRGroup(groupName);
            //only saving the group when not found. 
            _srBl.AddGroup(group);
        }

        //create connection
        SignalRConnection connection = new SignalRConnection(connectionId, callerUserName, callerUSerId);

        //add connection to group and call save method
        group.Connections.Add(connection);

        //save
        if(await _srBl.SaveAllSync())
            return group;

        throw new HubException("Failed to join group");
    }

    private async Task<SignalRGroup> RemoveFromMessageGroup(string connectionId)
    {
        SignalRGroup group = await _srBl.GetGroupByConnection(connectionId);
        if(group == null)
            throw new HubException("Failed to get group for connection");

        SignalRConnection connection = group.Connections.FirstOrDefault(x => x.ConnectionId == connectionId);
        if (connection == null)
            throw new HubException("Failed to get connection");

        _srBl.RemoveConnection(connection);
        if(await _srBl.SaveAllSync()){
            //group.Connections.Remove(connection);
            return group;
        }

        throw new HubException("Failed to remove from group");
    }

}
