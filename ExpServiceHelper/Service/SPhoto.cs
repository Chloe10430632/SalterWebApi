using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.Service
{
    public class SPhoto
    {
            private readonly Cloudinary _cloudinary;
            public SPhoto(IConfiguration _config)
            {

                string? cloudName = _config["Cloudinary_Exp:CloudName"];
                string? apiKey = _config["Cloudinary_Exp:ApiKey"];
                string? apiSecret = _config["Cloudinary_Exp:ApiSecret"];

                if (string.IsNullOrWhiteSpace(cloudName) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
                {
                    throw new InvalidOperationException("Cloudinary_Exp 設定缺少必要欄位。");
                }

                var acc = new Account(cloudName, apiKey, apiSecret);
                _cloudinary = new Cloudinary(acc);
            }

            //新增照片
            public async Task<List<ImageUploadResult>> AddPhotoAsync(List<IFormFile> file)
            {
                var uploadResult = new ImageUploadResult();
                var resultCollection = new List<ImageUploadResult>();

                if (file != null && file.Any())
                {
                    foreach (var f in file)
                    {
                        using var stream = f.OpenReadStream();
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(f.FileName, stream),
                            Transformation = new Transformation().Width(1000).Crop("fit")
                        };
                        uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        resultCollection.Add(uploadResult);
                    }
                }
                return resultCollection;
            }


            //刪除照片
            public async Task DeletePhotoAsync(List<string>? publicIds)
            {

                if (publicIds == null || !publicIds.Any()) return;

                var delParams = new DelResParams()
                {
                    PublicIds = publicIds,
                    Type = "upload",
                    ResourceType = ResourceType.Image
                };

                var result = await _cloudinary.DeleteResourcesAsync(delParams);

            }
        
    }
}
