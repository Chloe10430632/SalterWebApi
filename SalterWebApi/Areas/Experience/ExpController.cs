using ExpServiceHelper.DTO;
using ExpServiceHelper.IService;
using ExpServiceHelper.Service;
using Microsoft.AspNetCore.Mvc;
using static ExpServiceHelper.DTO.DFavCoach;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SalterWebApi.Areas.Experience
{
    [Area("Exp")]
    [Route("api/[area]/[controller]")] // 路由 api/Forum/Posts
    [ApiController]
    [Tags("行程裝備預約")] // Scalar 會用這個名字當分類標題。
    public class ExpController : ControllerBase
    {
        private readonly ISCoachIndex _sCoachIndex;
        private readonly ISCoachMethods _sCoachMethods;
        public ExpController(ISCoachIndex sCoachIndex, ISCoachMethods sCoachMethods )
        {
            _sCoachIndex = sCoachIndex;
            _sCoachMethods = sCoachMethods;
        }
        [HttpPost("Favorites")]
        public async Task<IActionResult> MyFavCoach(DFavCoach dto) { 
            var result = await _sCoachIndex.MyFavCoach(dto);
            return Ok(result);
        }

        // GET: api/<ExpController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ExpController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ExpController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ExpController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ExpController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
