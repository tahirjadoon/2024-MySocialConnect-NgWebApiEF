using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;

namespace MSC.Core.Repositories;

public interface IPhotoRepository
{
    Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotosAsync();
    Task<Photo> GetPhotoByIdAsync(int id);
    void RemovePhoto(Photo photo);
}
