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
        private readonly IHouseRepository _houseRepository;
        private readonly IGenericHomeRepository<HomHouse> _houseRepo;
        private readonly IGenericHomeRepository<HomRoomType> _roomTypeRepo;
        private readonly IGenericHomeRepository<HomRoomImage> _houseImageRepo;
        private readonly IGenericHomeRepository<HomReview> _reviewRepo;
        private readonly IGenericHomeRepository<HomRoomTypeAmenity> _roomAmenityRepo;
        private readonly IGenericHomeRepository<HomAmenity> _amenityRepo;
        private readonly IGenericHomeRepository<UserUser> _userRepo;
        private readonly SalterDbContext _context;
        public HomService
            (
            IHouseRepository houseRepository,
            IGenericHomeRepository<UserUser> userRepo,
            IGenericHomeRepository<HomHouse> houseRepo,
            IGenericHomeRepository<HomRoomType> roomTypeRepo,
            IGenericHomeRepository<HomRoomImage> houseImageRepo,
            IGenericHomeRepository<HomReview> reviewRepo,
            IGenericHomeRepository<HomRoomTypeAmenity> roomAmenityRepo,
            IGenericHomeRepository<HomAmenity> amenityRepo,
            SalterDbContext context
            )
        {
            _houseRepository = houseRepository;
            _houseRepo = houseRepo;
            _roomTypeRepo = roomTypeRepo;
            _houseImageRepo = houseImageRepo;
            _reviewRepo = reviewRepo;
            _context = context;
            _amenityRepo = amenityRepo;
            _roomAmenityRepo = roomAmenityRepo;
                _userRepo = userRepo;
        }

        //取得所有房子及其房型的基本資訊
        public async Task<IEnumerable<HouseDetailViewDTO>> GetAllHousesAsync()
        {
            var rooms = await _roomTypeRepo.GetAll();
            var houses = await _houseRepo.GetAll();
            var allImages = await _houseImageRepo.GetAll();

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
                             RoomDescription = r.Description,
                             HouseDescription = h.Description,

                             AllImages = allImages.
                             Where(i => i.RoomTypeId == r.RoomTypeId)
                             .Select(i => i.ImagePath)
                             .ToList()
                         };
            return result.ToList();
        }

        // 根據搜尋條件（城市、可容納人數）篩選房子及其房型的基本資訊
        public async Task<IEnumerable<HouseDetailViewDTO>> SearchHousesAsync(HouseSearchDTO search)
        {
            // 1. 抓取所有基礎資料
            var houses = await _houseRepo.GetAll(); // 拿 HomHouse
            var rooms = await _roomTypeRepo.GetAll(); // 拿 HomRoomType
            var images = await _houseImageRepo.GetAll(); // 拿 HomRoomImage
            var roomAmenities = await _roomAmenityRepo.GetAll(); // 拿 HomRoomTypeAmenity
            var amenities = await _amenityRepo.GetAll(); // 拿 HomAmenity (為了拿設備名稱)

            // 2. 開始 Join 與篩選
            var query = from h in houses
                        join r in rooms on h.HouseId equals r.HouseId
                        select new { h, r };

            // 篩選城市
            if (!string.IsNullOrEmpty(search.Citie))
            {
                query = query.Where(x => x.h.Citie.Contains(search.Citie));
            }
            // 篩選人數
            if (search.PeopleCount.HasValue)
            {
                query = query.Where(x => x.r.Capacity >= search.PeopleCount.Value);
                 //房型容量 >= 需求人數
            }

            // 組裝完整的 DTO
            var result = query.Select(x => new HouseDetailViewDTO
            {
                RoomTypeId = x.r.RoomTypeId,
                Name = x.r.Name, // 確保欄位名正確
                PricePerNight = x.r.PricePerNight,
                Capacity = x.r.Capacity,
                Location = x.h.Location,
                Citie = x.h.Citie,
                District = x.h.District,
                RoomDescription = x.r.Description,
                HouseDescription = x.h.Description,
                //這裡補上圖片：過濾出屬於這個 RoomType 的所有網址
                AllImages = images
                    .Where(img => img.RoomTypeId == x.r.RoomTypeId)
                    .Select(img => img.ImagePath)
                    .ToList(),

                // 這裡補上設備：透過關聯表找到對應的設備名稱
                Amenities = (from ra in roomAmenities
                             join a in amenities on ra.AmenityId equals a.AmenityId
                             where ra.RoomTypeId == x.r.RoomTypeId
                             select new AmenityItemDTO
                             {
                                    Name = a.Name,
                                    IconCode = a.IconCode
                             }).ToList()
            });

            return result.ToList();
        }

        // 根據房型ID取得該房型的詳細資訊（包含房子資訊、所有圖片、評論等）
        public async Task<HouseDetailViewDTO> SerchHouseDetailAsync(int roomTypeId)
        {
            // 1. 取得基本資料 (維持原樣)
            var rooms = await _roomTypeRepo.GetAll();
            var roomAmenities = await _roomAmenityRepo.GetAll();
            var amenities = await _amenityRepo.GetAll();
            var r = rooms.FirstOrDefault(x => x.RoomTypeId == roomTypeId);
            if (r == null) return null;

            var houses = await _houseRepo.GetAll();
            var h = houses.FirstOrDefault(x => x.HouseId == r.HouseId);

            // 2. 取得圖片
            var allImages = await _houseImageRepo.GetAll();
            var images = allImages.Where(x => x.RoomTypeId == roomTypeId).Select(x => x.ImagePath).ToList();

            // 3. 取得評論與使用者資訊 (把兩段合併成這一段)
            var reviews = await _reviewRepo.GetAll();
            var allUsers = await _userRepo.GetAll(); 

            var roomReviews = reviews.Where(rv => rv.RoomTypeId == roomTypeId)
                .Select(rv =>
                {
                    var user = allUsers.FirstOrDefault(u => u.Id == rv.UserId);
                    return new ReviewItemDTO
                    {
                        Rating = rv.Rating,
                        Comment = rv.Comment,
                        CreatedTime = rv.CreatedTime,
                        Name = user?.UserName ?? "匿名使用者",
                        Picture = user?.ProfilePicture ?? "default-profile.png"
                    };
                }).ToList(); 

            // 取得設備清單
            var amenitiesList = (from ra in roomAmenities
                                 join a in amenities on ra.AmenityId equals a.AmenityId
                                 where ra.RoomTypeId == roomTypeId
                                 select new AmenityItemDTO
                                 {
                                     Name = a.Name,
                                     IconCode = a.IconCode
                                 }).ToList();

            var amenityIds = roomAmenities
                              .Where(ra => ra.RoomTypeId == roomTypeId)
                              .Select(ra => ra.AmenityId)
                              .ToList();

            // 組合成 DTO 
            return new HouseDetailViewDTO
            {
                RoomTypeId = r.RoomTypeId,
                Name = r.Name,
                Capacity = r.Capacity,
                PricePerNight = r.PricePerNight,
                ViewCount = r.ViewCount,
                RoomDescription = r.Description,
                HouseDescription = h?.Description,
                Location = h?.Location,
                District = h?.District,
                Citie = h?.Citie,
                Amenities = amenitiesList,
                AmenityIds = amenityIds,
                AllImages = images,
                Reviews = roomReviews 
            };
        }

        // 取得所有房子所在的城市列表（去重復後）
        public async Task<IEnumerable<string?>> GetAllCityAsync()
        {
            var houses = await _houseRepo.GetAll();

            return houses.Where(h => !string.IsNullOrEmpty(h.Citie)).
                Select(h => h.Citie)
                .Distinct()
                .OrderBy(c => c) // 可以選擇性地排序
                .ToList();
        }

        // 取得所有房子所在的城市列表（）
        public async Task<List<CityGroupDTO>> GetCityGroupPreviewsAsync(string? city = null)
        {
            var query = _context.HomRoomTypes
                .Include(rt => rt.House)
                .Include(rt => rt.HomRoomImages)
                .Where(rt => rt.IsActive == true); 

            // 如果前端有傳城市，且不是 "全部"，就在資料庫層級過濾
            if (!string.IsNullOrEmpty(city) && city != "全部")
            {
                query = query.Where(rt => rt.House.Citie == city);
            }

            // 執行查詢並撈回記憶體 (這時候才真的去資料庫拿資料)
            var roomTypes = await query.ToListAsync();

            // 進行分組處理
            return roomTypes
                .GroupBy(rt => rt.House.Citie)
                .Select(g => new CityGroupDTO
                {
                    CityName = g.Key ?? "未分類",
                    Houses = g.Take(6).Select(rt => new HousePreviewDTO
                    {
                        HouseId = rt.RoomTypeId,
                        Title = rt.Name ?? "精選房源",
                        Price = (int)(rt.PricePerNight ?? 0),
                        District = rt.House.District ?? "",
                        ImageUrl = rt.HomRoomImages.FirstOrDefault()?.ImagePath ?? ""
                    }).ToList()
                }).ToList();
        }
        // 取得瀏覽次數最高的前幾個房型的基本資訊
        public async Task<IEnumerable<HouseDetailViewDTO>> GetTopRoomsAsync(int count)
        {
            var rooms = await _roomTypeRepo.GetAll();
            var houses = await _houseRepo.GetAll();

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
                            RoomDescription = r.Description,
                            HouseDescription = h.Description
                        };
            return query.Take(count).ToList();
        }

        //查詢所有設備
        public async Task<IEnumerable<HomAmenity>> GetAllAmenitiesAsync()
        {
            return await _amenityRepo.GetAll();

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
                        await _roomAmenityRepo.AddAsync(roomAmenity);
                    }
                    // 最後統一儲存所有設備關聯
                    await _roomAmenityRepo.SaveAsync();
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

        //更新房型API
        public async Task<bool> UpdateFullHouseAsync(HouseUpdateDTO dto)
        {
            try
            {
                // 抓取資料庫中現有的房型資料
                var room = await _roomTypeRepo.GetByIdAsync(dto.RoomTypeId);
                if (room == null) return false;

                // 更新房型基本欄位 (將 DTO 的值賦給 Entity)
                room.Name = dto.RoomName;
                room.PricePerNight = dto.PricePerNight;
                room.Capacity = dto.Capacity;
                room.Description = dto.RoomDescription;

                // 執行 Repository 的 Update
                await _roomTypeRepo.UpdateAsync(room);

                // 更新房屋基本資料 (地址資訊通常在 HomHouse 表)
                var house = await _houseRepo.GetByIdAsync(room.HouseId);
                if (house != null)
                {
                    house.Citie = dto.Citie;
                    house.District = dto.District;
                    house.Description = dto.HouseDescription;
                    house.Location = dto.Location;
                    await _houseRepo.UpdateAsync(house);
                }

                //先刪除該房型所有舊圖，再插入 DTO 帶來的新圖
                var allImages = await _houseImageRepo.GetAll();
                var oldImages = allImages.Where(x => x.RoomTypeId == dto.RoomTypeId);
                foreach (var img in oldImages)
                {
                    await _houseImageRepo.DeleteByIdAsync(img.ImageId);
                }

                foreach (var path in dto.ImageUrls) // 這是前端 textarea 傳回來的網址陣列
                {
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        await _houseImageRepo.AddAsync(new HomRoomImage
                        {
                            RoomTypeId = dto.RoomTypeId,
                            ImagePath = path,
                            CreatedTime = DateTime.Now
                        });
                    }
                }

                // 先刪除舊關聯，再插入新勾選的 ID
                var allRoomAmenities = await _roomAmenityRepo.GetAll();
                var oldAmenities = allRoomAmenities.Where(x => x.RoomTypeId == dto.RoomTypeId);
                foreach (var am in oldAmenities)
                {
                    await _roomAmenityRepo.DeleteByIdAsync(am.RoomTypeAmenityId);
                }

                foreach (var amId in dto.AmenityIds)
                {
                    await _roomAmenityRepo.AddAsync(new HomRoomTypeAmenity
                    {
                        RoomTypeId = dto.RoomTypeId,
                        AmenityId = amId
                    });
                }

                return true; // 全部順利完成
            }
            catch (Exception ex)
            {
                // 這裡可以記錄錯誤日誌 (Log)
                return false;
            }
        }

        //查詢顯示房型
        public async Task<List<HousePreviewDTO>> SearchHousesAsync(string? city, string? keyword, int? guests)
        {
            var query = _context.HomRoomTypes
                .Include(rt => rt.House)
                .Include(rt => rt.HomRoomImages)
                .Where(re => re.IsActive == true)
                .AsQueryable();

            //篩選城市
            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(rt => rt.House.Citie == city);
            }
            //關鍵字篩選
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(rt => rt.Name.Contains(keyword) || rt.House.District.Contains(keyword));
            }
            //人數篩選
            if (guests.HasValue && guests > 0)
            {
                query = query.Where(rt => rt.Capacity >= guests);
            }

            return await query.Select(rt => new HousePreviewDTO
            {
                HouseId = rt.RoomTypeId,
                Title = rt.Name,
                Price = (int)(rt.PricePerNight ?? 0),
                District = rt.House.District,
                ImageUrl = rt.HomRoomImages.FirstOrDefault().ImagePath
            }).ToListAsync();
        }

        //篩選條件查詢房型(城市、日期、人數)
        public async Task<IEnumerable<HousePreviewDTO>> GetSearchHousesAsync(HouseSearchDTO searchDto)
        {
            // 處理日期型別轉換 (DateTime? -> DateOnly?)
            DateOnly? start = searchDto.StartDate.HasValue ? DateOnly.FromDateTime(searchDto.StartDate.Value) : null;
            DateOnly? end = searchDto.EndDate.HasValue ? DateOnly.FromDateTime(searchDto.EndDate.Value) : null;

            // 呼叫 Repository 執行資料庫篩選
            var roomEntities = await _houseRepository.SearchAvailableRoomsAsync(
                searchDto.Citie,
                searchDto.PeopleCount,
                start,
                end
            );

            // 3. 業務轉換：將 Entity 轉為 DTO
            return roomEntities.Select(rt => new HousePreviewDTO
            {
                HouseId = rt.HouseId,
                Title = rt.Name ?? string.Empty,
                Price = rt.PricePerNight ?? 0,
                District = rt.House?.District ?? string.Empty,
                // 取第一張圖片作為 ImageUrl
                ImageUrl = rt.HomRoomImages?.FirstOrDefault()?.ImagePath ?? "default.jpg"
            });
        }

        //查詢是否有資格評論
        public async Task<int?> GetAvailableBookingIdAsync(int userId, int roomTypeId)
        {
            // 尋找該用戶針對此房型，最新一筆狀態為 "Completed" 且尚未有評論的訂單
            // 這裡假設 Status 內容為 "Completed"
            var booking = await _context.HomBookings
                .Where(b => b.UserId == userId
                         && b.RoomTypeId == roomTypeId
                         && b.Status == "1"
                         && !b.HomReviews.Any()
                         ) // 確保還沒評論過
                .OrderByDescending(b => b.CheckOutDate)
                .Select(b => b.BookingId)
                .FirstOrDefaultAsync();

            return booking == 0 ? null : (int?)booking;
        }

        //新增評論到指定的房型(要有預約訂單的使用者才能評論!!)
        public async Task<bool> AddReviewAsync(ReviewCreateDTO dto)
        {
            var newReview = new HomReview
            {
                RoomTypeId = dto.RoomTypeId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                UserId = dto.MemberId,
                BookingId = dto.BookingId, // 關聯到訂單
                CreatedTime = DateTime.Now
            };

            _context.HomReviews.Add(newReview);
            return await _context.SaveChangesAsync() > 0;
        }

        
    }



}
