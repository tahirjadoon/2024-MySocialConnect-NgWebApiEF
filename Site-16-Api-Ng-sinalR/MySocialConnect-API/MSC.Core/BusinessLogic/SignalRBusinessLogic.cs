using System.Threading.Tasks;
using MSC.Core.DB.Entities.SignalR;
using MSC.Core.Repositories;

namespace MSC.Core.BusinessLogic;

public class SignalRBusinessLogic : ISignalRBusinessLogic
{
    private readonly ISignalRRepository _srRepo;

    public SignalRBusinessLogic(ISignalRRepository srRepo)
    {
        _srRepo = srRepo;
    }

    public async Task<bool> SaveAllSync()
    {
        return await _srRepo.SaveAllSync();
    }

    public void AddGroup(SignalRGroup group)
    {
        _srRepo.AddGroup(group);
    }

    public void RemoveConnection(SignalRConnection connection)
    {
        _srRepo.RemoveConnection(connection);
    }

    public async Task<SignalRGroup> GetMessageGroup(string groupName)
    {
       var group = await _srRepo.GetMessageGroup(groupName);
       return group;
    }

    public async Task<SignalRGroup> GetGroupByConnection(string connectionId)
    {
        var group = await _srRepo.GetGroupByConnection(connectionId);
        return group;
    }

    public Task<SignalRConnection> GetConnection(string connectionId)
    {
        var connection = _srRepo.GetConnection(connectionId);
        return connection;
    }
}
