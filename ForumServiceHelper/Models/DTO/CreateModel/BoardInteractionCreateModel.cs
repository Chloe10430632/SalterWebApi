using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Models.DTO.CreateModel
{
    public class BoardInteractionCreateModel
    {
        public int BoardId { get; set; }
        public string Type { get; set; } = string.Empty; //看板互動 View & Follow

    }
}
