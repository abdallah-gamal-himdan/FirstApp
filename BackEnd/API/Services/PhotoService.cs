using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloud;
        public PhotoService(IOptions<CloudinarySettings>Config , Cloudinary cloud)
        {
            var acc = new Account(
Config.Value.CloudName ,
Config.Value.ApiKey,
Config.Value.ApiSecret
            );
            _cloud = cloud;  
        }

        public Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            throw new NotImplementedException();
        }

        public Task<DeletionResult> DeletePhotoAsync(string PublicId)
        {
            throw new NotImplementedException();
        }
    }
}