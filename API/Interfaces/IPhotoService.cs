using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;

namespace API.Interfaces
{
    public interface IPhotoService
    {
        //ImageUploadResult and DeletionResult are from cloudinary
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file); //IFormFile represent a file sent from httprequest
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}