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
        public PhotoService(IOptions<CloudinarySettings> Config)
        {
            var acc = new Account(
                        Config.Value.CloudName,
                        Config.Value.ApiKey,
                        Config.Value.ApiSecret);

            _cloud = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if(file.Length > 0)
            {
                await using var stream = file.OpenReadStream() ;
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloud.UploadAsync(uploadParams);

            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string PublicId)
        {
            var deletionResult = new DeletionResult();

            var deletionParams = new DeletionParams(PublicId);
            deletionResult = await _cloud.DestroyAsync(deletionParams);
            return deletionResult;
        }
    }
}