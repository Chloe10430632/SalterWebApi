namespace ForumServiceHelper.Models.DTO.ViewModel
{
    public class BoardsViewModel
    {
        public int BoardId { get; set; }
        public string? BoardTitle { get; set; }
        public string? BoardImgUrl { get; set; }
        public int BoardSort { get; set; }
        public bool isFollowed { get; set; }
        public int ViewCount { get; set; }
        public int FollowCount { get; set; }
    }
}
