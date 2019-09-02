using System;
using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos
{
  public class PhotoAccessor : IPhotoAccessor
  {
    private readonly Cloudinary cloudinary;
    public PhotoAccessor(IOptions<CloudinarySettings> config)
    {
      var account = new Account( config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret );
      cloudinary = new Cloudinary(account);
    }

    public PhotoUploadResult AddPhoto(IFormFile file)
    {
      var uploadResult = new ImageUploadResult();
      if (file.Length > 0) 
      {
        using(var stream = file.OpenReadStream())
        {
          var uploadParams = new ImageUploadParams 
          { 
            File = new FileDescription(file.FileName, stream), 
            Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face") 
          };
          uploadResult = cloudinary.Upload(uploadParams);
        }
      }
      
      if (uploadResult.Error != null) throw new Exception(uploadResult.Error.Message);
      return new PhotoUploadResult{ PublicID = uploadResult.PublicId, Url = uploadResult.SecureUri.AbsoluteUri };
    }

    public string DeletePhoto(string publicID)
    {
      var deleteParams = new DeletionParams(publicID);
      var result = cloudinary.Destroy(deleteParams);
      return (result.Result == "ok") ? result.Result : null;
    }
  }
}