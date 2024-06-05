using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MSC.Core.DB.Entities;
using MSC.Core.DB.UnitOfWork;
using MSC.Core.Dtos;
using MSC.Core.Services;

namespace MSC.Core.BusinessLogic;

public class PhotoBusinessLogic : IPhotoBusinessLogic
{
    private readonly IUnitOfWork _uow;
    private readonly IPhotoService _photoService;

    public PhotoBusinessLogic(IUnitOfWork uow, IPhotoService photoService)
    {
        _uow = uow;
        _photoService = photoService;
    }

    public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotosAsync()
    {
        var photos = await _uow.PhotoRepo.GetUnapprovedPhotosAsync();
        return photos;
    }

    public async Task<Photo> GetPhotoByIdAsync(int id)
    {
        var photo = await _uow.PhotoRepo.GetPhotoByIdAsync(id);
        return photo;
    }

    public async Task<BusinessResponse> ApprovePhotoAsync(int photoId)
    {
        var photo = await GetPhotoByIdAsync(photoId);
        if(photo == null) return new BusinessResponse(HttpStatusCode.NotFound, "Photo not found");
        photo.IsApproved = true;
        //get the user by the photo and if the user has no main photo applied then make the photo being approved as main
        var user = await _uow.UserRepo.GetUserByPhotoIdAsync(photoId);
        if(user == null) return new BusinessResponse(HttpStatusCode.BadRequest, "Unable to find the user");
        //make main if no other photo is main
        if(!user.Photos.Any(x => x.IsMain))
            photo.IsMain = true;
        if(await _uow.SaveChangesAsync())
            return new BusinessResponse(HttpStatusCode.OK, "Photo approved");

        return new BusinessResponse(HttpStatusCode.BadRequest, "Could not approve the photo");
    }

    public async Task<BusinessResponse> RemovePhotoAsync(int photoId)
    {
        var photo = await GetPhotoByIdAsync(photoId);
        if (photo == null) return new BusinessResponse(HttpStatusCode.NotFound, "Photo not found");

        if(!string.IsNullOrWhiteSpace(photo.PublicId))
        {
            //cloudinary
            var result = await _photoService.DeletePhotoAync(photo.PublicId);
            if(result.Error == null)
                _uow.PhotoRepo.RemovePhoto(photo);
        }
        else{
            _uow.PhotoRepo.RemovePhoto(photo);
        }

        //_uow.HasChanges() && 
        if(await _uow.SaveChangesAsync())
            return new BusinessResponse(HttpStatusCode.OK, "Photo Removed");

        return new BusinessResponse(HttpStatusCode.BadRequest, "Something went bad and could not remove photo");
    }
}
