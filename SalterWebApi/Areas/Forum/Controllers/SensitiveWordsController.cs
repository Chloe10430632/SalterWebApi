using ForumServiceHelper.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SalterWebApi.Areas.Forum.Controllers
{
    [Area("Forum")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Tags("社群討論版")]
    public class SensitiveWordsController : ControllerBase
    {
        private readonly ISensitiveWordsService _wordsService;

        public SensitiveWordsController(ISensitiveWordsService wordsService)
        {
            _wordsService = wordsService;
        }

        [HttpPost("check")]
        public async Task<IActionResult> Check([FromBody] string content)
        {
            var result = await _wordsService.ValidateContentAsync(content);
            return Ok(result);
        }

    }
}
