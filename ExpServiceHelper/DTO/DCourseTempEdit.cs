using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DCourseTempEdit
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Difficulty { get; set; }
        public decimal? Price { get; set; }
        public string? Location { get; set; }
       
        // 這裡存的是已經在資料庫/Cloudinary 裡的圖片資訊
        public string? ExistingPhotosJson { get; set; }

        // 這裡才是接收從前端選取、準備上傳的實體檔案
        public List<IFormFile>? NewImageFiles { get; set; }
        //
        public TripLocationRequestDto? LocationData { get; set; }
        public string? LocationName => LocationData?.LocationName;
    }
}
