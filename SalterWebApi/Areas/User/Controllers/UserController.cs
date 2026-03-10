using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using UserServiceHelper.IService;
using UserServiceHelper.Models.DTO.ViewModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SalterWebApi.Areas.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfileViewModel>> Get(int id)
        {
            var profile = await _userService.GetUserProfileAsync(id);

            if (profile == null)
                return null;

            return Ok(profile);
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserEditViewModel model)
        {
            if(id != model.Id)
            {
                return BadRequest(new { message = "資料ID不符" });
            }


            var success = await _userService.UpdateProfileAsync(model);

            if (!success)
            {
                return BadRequest(new { message = "更新失敗，請檢查資料" });
            }

            return Ok(new { message = "更新成功" });

        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
