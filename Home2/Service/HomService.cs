using HomeRepositoryHelper.IRepository;
using HomeServiceHelper.IService;
using HomeServiceHelper.Models.DTO.ViewModels;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HomeServiceHelper.Service
{
    public class HomService : IHomService
    {
        private readonly IGenericHomeRepository<HomHouse> _houseRepo;
        private readonly IGenericHomeRepository<HomRoomType> _roomTypeRepo;
        private readonly IGenericHomeRepository<HomRoomImage> _houseImageRepo;
        private readonly IGenericHomeRepository<HomReview> _reviewRepo;
        public HomService
            (IGenericHomeRepository<HomHouse> houseRepo,
            IGenericHomeRepository<HomRoomType> roomTypeRepo,
            IGenericHomeRepository<HomRoomImage> houseImageRepo,
            IGenericHomeRepository<HomReview> reviewRepo
            )
        {
            _houseRepo = houseRepo;
            _roomTypeRepo = roomTypeRepo;
            _houseImageRepo = houseImageRepo;
            _reviewRepo = reviewRepo;
        }

        //取得所有房子及其房型的基本資訊
        public async Task<IEnumerable<HouseDetailViewDTO>> GetAllHousesAsync()
        {
            var rooms = await _roomTypeRepo.GetAllHouse();
            var houses = await _houseRepo.GetAllHouse();

            var result = from h in houses
                         join r in rooms
                         on h.HouseId equals r.HouseId
                         select new HouseDetailViewDTO
                         {
                             RoomTypeId = r.RoomTypeId,
                             Name = r.Name,
                             PricePerNight = r.PricePerNight,
                             Capacity = r.Capacity,
                             Location = h.Location,
                             Citie = h.Citie,
                             District = h.District,
                             Description = r.Description
                         };
            return result.ToList();
        }

        // 根據搜尋條件（城市、可容納人數）篩選房子及其房型的基本資訊
        public async Task<IEnumerable<HouseDetailViewDTO>> SearchHousesAsync(HouseSearchDTO search)
        {
            var rooms = await _roomTypeRepo.GetAllHouse();
            var houses = await _houseRepo.GetAllHouse();

            var query = from h in houses
                        join r in rooms
                        on h.HouseId equals r.HouseId
                        select new { h, r };

            if (!string.IsNullOrEmpty(search.Citie))
            {
                query = query.Where(x => x.h.Citie.Contains(search.Citie));
            }
            if (search.PeopleCount.HasValue)
            {
                query = query.Where(x => x.r.Capacity <= search.PeopleCount.Value);
            }

            var result = query.Select(x => new HouseDetailViewDTO
            {
                RoomTypeId = x.r.RoomTypeId,
                Name = x.r.Name,
                PricePerNight = x.r.PricePerNight,
                Capacity = x.r.Capacity,
                Location = x.h.Location,
                Citie = x.h.Citie,
                District = x.h.District,
                Description = x.r.Description
            });
            return result.ToList();
        }

        // 根據房型ID取得該房型的詳細資訊（包含房子資訊、所有圖片、評論等）
        public async Task<HouseDetailViewDTO> SerchHouseDetailAsync(int roomTypeId)
        {
            // 先取得該房型的基本資訊
            var rooms = await _roomTypeRepo.GetAllHouse();
            var r = rooms.FirstOrDefault(x => x.RoomTypeId == roomTypeId);
            if (r == null) return null;

            // 取得該房型所屬的房子資訊
            var houses = await _houseRepo.GetAllHouse();
            var h = houses.FirstOrDefault(x => x.HouseId == r.HouseId);

            // 取得該房型的所有圖片
            var allImages = await _houseImageRepo.GetAllHouse();
            var images = allImages.Where(x => x.RoomTypeId == roomTypeId)
                .Select(x => x.ImagePath)
                .ToList();

            // 取得該房型的評論
            var reviews = await _reviewRepo.GetAllHouse();
            var roomReviews = reviews.Where(rv => rv.RoomTypeId == roomTypeId)
                .Select(rv => new ReviewItemDTO
                {
                    Rating = rv.Rating,
                    Comment = rv.Comment,
                    CreatedTime = rv.CreatedTime
                }).ToList();

            // 組合成完整的房型詳細資訊 DTO
            return new HouseDetailViewDTO
            {
                RoomTypeId = r.RoomTypeId,
                Name = r.Name,
                Capacity = r.Capacity,
                PricePerNight = r.PricePerNight,
                ViewCount = r.ViewCount,
                Description = r.Description,
                Location = h?.Location,
                District = h?.District,
                Citie = h?.Citie,
                AllImages = images,
                Reviews = roomReviews
            };
        }

        // 取得所有房子所在的城市列表（去重復後）
        public async Task<IEnumerable<string?>> GetAllCityAsync()
        {
            var houses = await _houseRepo.GetAllHouse();

            return houses.Where(h => !string.IsNullOrEmpty(h.Citie)).
                Select(h => h.Citie)
                .Distinct()
                .ToList();
        }

        // 取得瀏覽次數最高的前幾個房型的基本資訊
        public async Task<IEnumerable<HouseDetailViewDTO>> GetTopRoomsAsync(int count)
        {
            var rooms = await _roomTypeRepo.GetAllHouse();
            var houses = await _houseRepo.GetAllHouse();

            var query = from h in houses
                        join r in rooms
                        on h.HouseId equals r.HouseId
                        orderby r.ViewCount descending
                        select new HouseDetailViewDTO
                        {
                            RoomTypeId = r.RoomTypeId,
                            Name = r.Name,
                            PricePerNight = r.PricePerNight,
                            Capacity = r.Capacity,
                            Location = h.Location,
                            Citie = h.Citie,
                            District = h.District,
                            Description = r.Description
                        };
            return query.Take(count).ToList();
        }

        // 新增評論到指定的房型(要有預約訂單的使用者才能評論!!)
        public async Task<bool> AddReviewAsync(ReviewCreateDTO dto)
        {
            var newReview = new HomReview
            {
                RoomTypeId = dto.RoomTypeId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                UserId = dto.MemberId,
                CreatedTime = DateTime.Now,
                BookingId =1001
            };
            
            await _reviewRepo.AddAsync(newReview);
            
            return true;
        }


    }
}
