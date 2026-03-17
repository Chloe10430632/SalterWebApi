using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.ErrorMessage;
using ForumServiceHelper.Models.DTO.QueryModel;
using ForumServiceHelper.Models.DTO.ViewModel;
using Microsoft.AspNetCore.Mvc;
using SalterEFModels.EFModels;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SalterWebApi.Areas.Forum.Controllers
{
    [Area("Forum")]
    [Route("api/[area]/[controller]")] // 路由 api/Forum/Boards
    [ApiController]
    [Tags("社群討論版")] // Scalar 會用這個名字當分類標題
    public class BoardsController : ControllerBase
    {
        private readonly IBoardsService _boardsService;

        public BoardsController(IBoardsService boardsService)
        {
            _boardsService = boardsService;
        }

        // GET: api/<BoardsController>
        [HttpGet]
        public async Task<ActionResult<IList<BoardsViewModel>>> Get([FromQuery] BoardsQueryModel query)
        {
            var boardsList = await _boardsService.GetAllBoardsAsync(query);
            return Ok(boardsList);
        }

        // GET api/<BoardsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BoardsViewModel>> GetForumBoardCategory(int id)
        {
             var boardData = await _boardsService.GetDetailsAsync(id);

            if(boardData == null)
            {
                throw new KeyNotFoundException($"找不到 ID 為 {id} 的看板");
            }

            return boardData;
        }
       
    }
}
