using System.Threading.Tasks;
using MSC.Core.Repositories;

namespace MSC.Core.DB.UnitOfWork;

public interface IUnitOfWork
{
    IUserRepository UserRepo {get;}
    ILikesRepository LikesRepo {get;}
    IMessageRepository MessageRepo {get;}
    ISignalRRepository SignalRRepo {get;}

    IPhotoRepository PhotoRepo {get;}

    Task<bool> SaveChangesAsync();
    bool HasChanges();
}
