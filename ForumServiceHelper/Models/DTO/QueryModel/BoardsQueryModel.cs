using ForumServiceHelper.Models.DTO.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Models.DTO.QueryModel
{
    public class BoardsQueryModel
    {
        public string? SortBy { get; set; } = string.Empty;

        public int TakeSize { get; set; }

    }
}
