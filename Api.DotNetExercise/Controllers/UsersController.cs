using Api.Shared.Controllers;
using Business.Users;
using Microsoft.AspNetCore.Mvc;
using Shared.ViewModel.Users;

namespace Api.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {

        #region Attributes

        protected IUsersService _userService;

        #endregion

        #region Constructors

        public UsersController(IUsersService userService)
        {
            _userService = userService;
        }

        #endregion

        // GET api/users
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_userService.Get());
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _userService.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST api/users
        [HttpPost]
        public IActionResult Post([FromBody] UserModel value)
        {
            var result = _userService.Post(value);
            if (result == 0)
            {
                return BadRequest();
            }
            return Ok();
        }

        // PUT api/users/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UserModel value)
        {
            var result = _userService.Put(id, value);
            if (result == 0)
            {
                return NotFound();
            }
            return Ok();
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _userService.Delete(id);
            if (result == 0)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
