using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MSC.Core.DB.Data;
using MSC.Core.DB.Entities.SignalR;

namespace MSC.Core.Repositories;

public class SignalRRepository : ISignalRRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public SignalRRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> SaveAllSync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
    
    public void AddGroup(SignalRGroup group)
    {
        _context.SignalRGroups.Add(group);
    }

    public void RemoveConnection(SignalRConnection connection)
    {
        _context.SignalRConnections.Remove(connection);
    }

    public async Task<SignalRGroup> GetMessageGroup(string groupName)
    {
        //also fill in the connections for the group
        var group = await _context.SignalRGroups
                                .Include(x => x.Connections)
                                .FirstOrDefaultAsync(x => x.GroupName == groupName);
        return group;
    }

    public async Task<SignalRGroup> GetGroupByConnection(string connectionId)
    {
        var group = await _context.SignalRGroups
                                .Include(x => x.Connections)
                                .Where(x => x.Connections.Any(x => x.ConnectionId == connectionId))
                                .FirstOrDefaultAsync();
        return group;
    }

    public async Task<SignalRConnection> GetConnection(string connectionId)
    {
       var connection = await _context.SignalRConnections.FindAsync(connectionId);
       return connection;
    }
}
