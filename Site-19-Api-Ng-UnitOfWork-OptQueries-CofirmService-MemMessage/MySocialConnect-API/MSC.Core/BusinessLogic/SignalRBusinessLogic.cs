using System.Threading.Tasks;
using MSC.Core.DB.Entities.SignalR;
using MSC.Core.DB.UnitOfWork;

namespace MSC.Core.BusinessLogic;

public class SignalRBusinessLogic : ISignalRBusinessLogic
{
    private readonly IUnitOfWork _uow;

    public SignalRBusinessLogic(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _uow.SaveChangesAsync();
    }

    public void AddGroup(SignalRGroup group)
    {
        _uow.SignalRRepo.AddGroup(group);
    }

    public void RemoveConnection(SignalRConnection connection)
    {
        _uow.SignalRRepo.RemoveConnection(connection);
    }

    public async Task<SignalRGroup> GetMessageGroup(string groupName)
    {
       var group = await _uow.SignalRRepo.GetMessageGroup(groupName);
       return group;
    }

    public async Task<SignalRGroup> GetGroupByConnection(string connectionId)
    {
        var group = await _uow.SignalRRepo.GetGroupByConnection(connectionId);
        return group;
    }

    public Task<SignalRConnection> GetConnection(string connectionId)
    {
        var connection = _uow.SignalRRepo.GetConnection(connectionId);
        return connection;
    }
}
