using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DCourseOrder
    {
        //教練資料
        public int? CourseSessionId { get; set; }
        public int? UserId { get; set; }
        public int? CoachId { get; set; }
        public string? CoachName { get; set; }
        public string? AvatarUrl { get; set; }
        //評論
        public int? ReviewId { get; set; }
        public DateTime? CreatReviewAt { get; set; }
        public DateTime? UpdateReviewAt { get; set; }
        public string? ReviewContent {  get; set; }
        public int? Rating { get; set; }
        //課程
        public decimal? Price { get; set; }
        public string? Title { get; set; }
        public DateOnly? StartDate { get; set; }
        public string? TimeSlot { get; set; }
        public string? Location { get; set; }
        public string? Difficulty { get; set; }
        //交易
        public int? ExpTransactionId { get; set; }
        public DateTime? ReservedAt { get; set; }
        public DateTime? UpdatedTransacAt { get; set; }
        public int? Status { get; set; }
        //
        public int? CourseOrderId { get; set; }
    }
}
