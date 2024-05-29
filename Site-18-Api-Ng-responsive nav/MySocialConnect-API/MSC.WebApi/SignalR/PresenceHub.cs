using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MSC.Core.Extensions;
using MSC.Core.SignalR;

namespace MSC.WebApi.SignalR;

[Authorize]
/// <summary>
/// Displays the presence of users like user login and online users
/// Starting with local presence tracker. The elaborate one would be with redis or in database
/// PresenceHub, it derives from Hub and then override the virtual methods
/// No package to install for SignalR
/// Has methods when the user connects to the Hub and again when the user Disconnects from the Hub
/// </summary>
public class PresenceHub : Hub
{
    //NOTE: NewMessageReceived event is being sent by the MessageBub

    private const string _userIsOnline = "UserIsOnline";
    private const string _userIsOffline = "UserIsOffline";
    private const string _getOnlineUsers = "GetOnlineUsers";
    private readonly PresenceTrackerMemory _tracker;

    public PresenceHub(PresenceTrackerMemory tracker)
    {
        _tracker = tracker;
    }

    /// <summary>
    /// Implement OnConnectedAsync to tell other users when the current user goes online
    /// </summary>
    /// <returns></returns>
    public override async Task OnConnectedAsync()
    {
        var userName =  Context.User.GetUserName();
        var connectionId = Context.ConnectionId;
        var isOnline = await _tracker.UserConnected(userName, connectionId);
        if(isOnline){
            //invoke messages to clients other than logging in user that a user is logging in
            await Clients.Others.SendAsync(_userIsOnline, userName);
        }

        var onlineUsers = await _tracker.GetOnlineUsers();
        //send to all the users
        //await Clients.All.SendAsync(_getOnlineUsers, onlineUsers);
        //only send to the current user 
        await Clients.Caller.SendAsync(_getOnlineUsers, onlineUsers);

    }

    /// <summary>
    /// Implmenent OnDisconnectedAsync to tell other users when the current user goes offline
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public override async  Task OnDisconnectedAsync(Exception exception)
    {
        var userName =  Context.User.GetUserName();
        var connectionId = Context.ConnectionId;
        var isOffLine = await _tracker.UserDisconnected(userName, connectionId);
        if(isOffLine){
            //invoke messages to clients other than logging out user that a user is logging out
            await Clients.Others.SendAsync(_userIsOffline, userName);
        }

        //original: get the users online and send to every one who is connected
        //update: not sending the list
        var onlineUsers = await _tracker.GetOnlineUsers();
        //await Clients.All.SendAsync(_getOnlineUsers, onlineUsers);
        await Clients.Caller.SendAsync(_getOnlineUsers, onlineUsers);

        await base.OnDisconnectedAsync(exception);
    }
}
