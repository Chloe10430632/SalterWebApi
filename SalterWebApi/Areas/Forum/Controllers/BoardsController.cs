using ForumServiceHelper.IService;
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
        public async Task<ActionResult<IList<BoardsViewModel>>> Get([FromQuery] string sortBy = "DEFAULT")
        {
            // 1. 調用 Service 的非同步方法
            var allBoardsList = await _boardsService.GetAllBoardsAsync(sortBy);

            // 2. 如果沒資料，回傳空的陣列或 NoContent (204)
            if (allBoardsList == null)
            {
                return NoContent();
            }

            // 3. 回傳 200 OK 與 JSON 格式的資料
            return Ok(allBoardsList);
        }

        // GET api/<BoardsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BoardsViewModel>> GetForumBoardCategory(int id)
        {

             var boardData = await _boardsService.GetDetailsAsync(id);

            if(boardData == null)
            {
                return NotFound();
            }

            return boardData;
        }

       
    }
}
