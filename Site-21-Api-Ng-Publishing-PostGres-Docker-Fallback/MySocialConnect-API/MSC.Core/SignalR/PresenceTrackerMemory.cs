using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSC.Core.SignalR;

/// <summary>
/// starting with local presence tracker. The elaborate one would be with redis or in database
/// add as a singleton in services
/// </summary>
public class PresenceTrackerMemory
{
    //dictionary to track the users when logging in and logging out
    //key will be the user name 
    //value will be the list of connection ids for that user, like from phone and desktop
    private static readonly Dictionary<string, List<string>> _onlineUsers = new Dictionary<string, List<string>>();

    public Task<bool> UserConnected(string userName, string connectionId)
    {
        var isOnline = false;
        lock (_onlineUsers)
        {
            if(_onlineUsers.ContainsKey(userName))
                _onlineUsers[userName].Add(connectionId); 
            else{
                //user has come online
                _onlineUsers.Add(userName, new List<string>{ connectionId});
                isOnline = true;
            }               
        }
        //return Task.CompletedTask;
        return Task.FromResult(isOnline);
    }

    public Task<bool> UserDisconnected(string userName, string connectionId)
    {
        var isOffLine = false;
        lock(_onlineUsers){
            if(!_onlineUsers.ContainsKey(userName)){
                //return Task.CompletedTask;
                return Task.FromResult(isOffLine);
            }

            //remove the connections
            _onlineUsers[userName].Remove(connectionId);
            if(_onlineUsers[userName].Count == 0){
                //remove the user
                _onlineUsers.Remove(userName);
                isOffLine = true;
            }
        }
        //return Task.CompletedTask;
        return Task.FromResult(isOffLine);
    }

    public Task<string[]> GetOnlineUsers(){
        string[] onlineUsers;
        lock (_onlineUsers){
            onlineUsers = _onlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
        }
        return Task.FromResult(onlineUsers);
    }

    public Task<string[]> GetConnectionsForUser(string userName){
        List<string> connectionsIds = new List<string>();
        lock (_onlineUsers){
            if(_onlineUsers.ContainsKey(userName))
            {
                connectionsIds = _onlineUsers.GetValueOrDefault(userName);
                 return Task.FromResult(connectionsIds.ToArray());
            }
        }
        //return Task.CompletedTask;
        return Task.FromResult<string[]>(null);
    }
}
