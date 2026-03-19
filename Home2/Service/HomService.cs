using HomeRepositoryHelper.IRepository;
using HomeServiceHelper.IService;
using HomeServiceHelper.Models.DTO.ViewModels;
using Microsoft.EntityFrameworkCore;
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
        private readonly IGenericHomeRepository<HomRoomTypeAmenity> _amenityRepo;
        private readonly SalterDbContext _context;
        public HomService
            (IGenericHomeRepository<HomHouse> houseRepo,
            IGenericHomeRepository<HomRoomType> roomTypeRepo,
            IGenericHomeRepository<HomRoomImage> houseImageRepo,
            IGenericHomeRepository<HomReview> reviewRepo,
            IGenericHomeRepository<HomRoomTypeAmenity> amenityRepo,
            SalterDbContext context
            )
        {
            _houseRepo = houseRepo;
            _roomTypeRepo = roomTypeRepo;
            _houseImageRepo = houseImageRepo;
            _reviewRepo = reviewRepo;
            _context = context;
            _amenityRepo = amenityRepo;
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
                BookingId = 1001
            };

            await _reviewRepo.AddAsync(newReview);

            return true;
        }

        //新增資料API：一次新增房子、房型、圖片等相關資料，並確保資料的一致性（使用交易）
        public async Task<bool> CreateFullHouseAsync(HouseCreateDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            //使用交易技術，保證資料的一致性

            try
            {
                // 1. 新增房屋 (HomHouse)
                var house = new HomHouse
                {
                    UserId = dto.UserID,
                    Description = dto.HouseDescription,
                    Location = dto.Location,
                    District = dto.District,
                    Citie = dto.Citie
                };
                await _houseRepo.AddAsync(house);
                // 必須先 Save 才能拿到自動產生的 houseID
                await _houseRepo.SaveAsync();

                // 2. 新增房型 (HomRoomType)
                var roomType = new HomRoomType
                {
                    HouseId = house.HouseId, // 關聯剛產生的 HouseID
                    Name = dto.RoomName,
                    Capacity = dto.Capacity,
                    PricePerNight = dto.PricePerNight,
                    Description = dto.RoomDescription,
                    CreatedTime = DateTime.Now,
                    IsActive = true,
                    ViewCount = 0
                };
                await _roomTypeRepo.AddAsync(roomType);
                await _roomTypeRepo.SaveAsync(); // 拿到 roomTypeID

                // 3. 批次新增圖片 (HomRoomImage)
                if (dto.ImageUrls != null && dto.ImageUrls.Any())
                {
                    foreach (var url in dto.ImageUrls)
                    {
                        var roomImage = new HomRoomImage
                        {
                            RoomTypeId = roomType.RoomTypeId, // 關聯剛產生的 RoomTypeID
                            ImagePath = url,
                            CreatedTime = DateTime.Now
                        };
                        await _houseImageRepo.AddAsync(roomImage);
                    }
                    await _houseImageRepo.SaveAsync();
                }
                // 4. 批次新增設備關聯 (HomRoomTypeAmenity)
                if (dto.AmenityIds != null && dto.AmenityIds.Any())
                {
                    foreach (var amenityId in dto.AmenityIds)
                    {
                        var roomAmenity = new HomRoomTypeAmenity
                        {
                            RoomTypeId = roomType.RoomTypeId, // 關聯剛剛產生的房型
                            AmenityId = amenityId             // 對應 HomAmenity 表的 ID
                        };
                        await _amenityRepo.AddAsync(roomAmenity);
                    }
                    // 最後統一儲存所有設備關聯
                    await _amenityRepo.SaveAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }

        }
    }
}
