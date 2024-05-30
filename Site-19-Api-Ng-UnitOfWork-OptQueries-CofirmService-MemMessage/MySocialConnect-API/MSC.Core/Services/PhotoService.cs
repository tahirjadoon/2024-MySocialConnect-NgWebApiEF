using System;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MSC.Core.Helper;

namespace MSC.Core.Services;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;
    private readonly string _cloudinaryFolder;

    public PhotoService(IOptions<CloudinaryConfig> cloudinaryConfig)
    {
        //cloudinary account object
        var account = new Account(
            cloudinaryConfig.Value.CloudName, 
            cloudinaryConfig.Value.ApiKey,
            cloudinaryConfig.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(account);
        _cloudinaryFolder = cloudinaryConfig.Value.Folder;
    }

    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();
        if(file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                //transformation for square image
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                Overwrite = false,
                //uploads to this specified folder showing as media library/folder in cloudinary
                Folder = _cloudinaryFolder  
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }
        return uploadResult;
    }

    public async Task<DeletionResult> DeletePhotoAync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        return result;
    }
}
