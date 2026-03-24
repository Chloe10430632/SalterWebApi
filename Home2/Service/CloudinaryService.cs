using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServiceHelper.Service
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(IConfiguration config)
        {
            var acc = new Account(
                config["CloudinaryHouse:CloudName"],
                config["CloudinaryHouse:ApiKey"],
                config["CloudinaryHouse:ApiSecret"]
            );
            _cloudinary = new Cloudinary(acc);
        }

        // 支援多張上傳
        public async Task<List<string>> UploadImagesAsync(List<IFormFile> files, string folder)
        {
            var urls = new List<string>();
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = "HouseImage",
                        PublicId = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString()[..4]}",
                        Transformation = new Transformation().Quality("auto").FetchFormat("auto")
                    };
                    var result = await _cloudinary.UploadAsync(uploadParams);

                    if (result != null && result.SecureUrl != null)
                    {
                        urls.Add(result.SecureUrl.ToString());
                    }
                    else
                    {
                        // 如果失敗了，印出原因到控制台
                        Console.WriteLine($"Upload Error: {result?.Error?.Message}");
                    }
                }
            }
            return urls;
        }
    }
}

