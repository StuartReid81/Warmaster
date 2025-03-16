using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class AppUserController(WarmasterContext db) : ControllerBase
    {
        public WarmasterContext db { get; } = db;

        #region Get Requests

        [HttpGet("GetUsers")]
        public async Task<ActionResult> GetUsers() {
            var users = await db.Users.Where(x => !x.IsDeleted).ToListAsync();

            if(users is null) return BadRequest("No live users found.");

            return Ok(users);
        }

        [HttpGet("GetDeletedUsers")]
        public async Task<ActionResult> GetDeletedUsers() {
            var users = await db.Users.Where(x => x.IsDeleted).ToListAsync();

            if(users is null) return BadRequest("No live users found.");

            return Ok(users);
        }

        [HttpGet("GetUser/{id}")]
        public async Task<ActionResult> GetUser(Guid id) {
            var user = await db.Users.Where(x => x.Id.ToString() == id.ToString()).FirstOrDefaultAsync();
            
            if(user is null) return BadRequest("We couldn't find this user.");

            return Ok(user);
        }

        #endregion
    }
}
