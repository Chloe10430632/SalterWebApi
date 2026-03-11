using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceHelper.IService;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics.Eventing.Reader;

namespace UserServiceHelper.Service
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<string> UploadImageAsync(IFormFile file, string folderName)
        {
            // 如果檔案是空的，就回傳 string.Empty (或是 null，依你喜好)
            if (file == null || file.Length == 0) return string.Empty;

            // 1. 取得 wwwroot 的實體路徑
            string wwwrootPath = _env.WebRootPath;

            // 2. 串接資料夾實體路徑 (wwwroot/admin/imgs)
            // folderName 傳入 "admin/imgs"
            string folderPath = Path.Combine(wwwrootPath, folderName.TrimStart('/'));

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 3. 修改檔名邏輯：對齊你之前的 [時間戳記_原始檔名]
            // 使用 Path.GetFileName 確保抓到的是純檔名
            string fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Path.GetFileName(file.FileName)}";
            string fullPath = Path.Combine(folderPath, fileName);

            // 4. 實體存檔
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 5. 回傳資料庫路徑：對齊你之前的 "/admin/imgs/" + fileName
            // 這樣回傳的就會是 "/admin/imgs/20260311123000_photo.png"
            return $"/{folderName.Trim('/')}/{fileName}";
        }
    }
}
