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
        public async Task<ActionResult<IList<BoardsViewModel>>> Get()
        {
            // 1. 調用 Service 的非同步方法
            var data = await _boardsService.GetAllBoardsAsync();

            // 2. 如果沒資料，回傳空的陣列或 NoContent (204)
            if (data == null)
            {
                return NoContent();
            }

            // 3. 回傳 200 OK 與 JSON 格式的資料
            return Ok(data);
        }

        // GET api/<BoardsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<BoardsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<BoardsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BoardsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
