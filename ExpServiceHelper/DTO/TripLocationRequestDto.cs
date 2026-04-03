using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class TripLocationRequestDto
    {
        public string LocationName { get; set; } = null!;
        public string? AddressText { get; set; }
        public string? GooglePlaceId { get; set; }
        public string? CityName { get; set; }
        public string? DistrictName { get; set; }
        public string? LocationRole { get; set; }
        public string? Note { get; set; }
        public int SortOrder { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
    }
}
