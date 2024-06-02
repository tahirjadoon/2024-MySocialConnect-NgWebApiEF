using System.Threading.Tasks;
using MSC.Core.DB.Data;
using MSC.Core.Repositories;

namespace MSC.Core.DB.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;
    private readonly IUserRepository _userRepo;
    private readonly ILikesRepository _likesRepo;
    private readonly IMessageRepository _msgRepo;
    private readonly ISignalRRepository _sigrRepo;

    public UnitOfWork(DataContext context, 
                        IUserRepository userRepo, ILikesRepository likesRepo, 
                        IMessageRepository msgRepo, ISignalRRepository sigrRepo)
    {
        _context = context;
        _userRepo = userRepo;
        _likesRepo = likesRepo;
        _msgRepo = msgRepo;
        _sigrRepo = sigrRepo;
    }

    public IUserRepository UserRepo => _userRepo;

    public ILikesRepository LikesRepo => _likesRepo;

    public IMessageRepository MessageRepo => _msgRepo;

    public ISignalRRepository SignalRRepo => _sigrRepo;

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }
}
