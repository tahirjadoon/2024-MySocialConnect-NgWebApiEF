using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;

namespace MSC.Core.BusinessLogic;

public interface IPhotoBusinessLogic
{
    Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotosAsync();
    Task<Photo> GetPhotoByIdAsync(int id);
    Task<BusinessResponse> ApprovePhotoAsync(int photoId);
    Task<BusinessResponse> RemovePhotoAsync(int photoId);
}
