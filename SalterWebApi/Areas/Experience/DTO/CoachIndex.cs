namespace SalterWebApi.Areas.Experience.DTO
{
    public class CoachIndex
    {
        /**評論*/
        public int? CoachId { get; set; }
        public int? Rating { get; set; }
        /**地區*/
        public int? DistrictId { get; set; }
        public int? CityId { get; set; }
        /**專業*/
        public string SportsName { get; set; } = null!;
        /**收藏*/
        public DateTime? FavoritedAt { get; set; }
        /**教練*/
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public DateTime? CreatedAt { get; set; }


    }
}
