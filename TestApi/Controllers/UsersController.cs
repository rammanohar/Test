using Microsoft.AspNetCore.Mvc;
using Test.Core;
using Test.Interface;

namespace TestApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {

        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id, CancellationToken ct)
        {
           var user = await _userService.GetUserByIdAsync(id, ct);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost("bulk-test")]
        public async Task<IActionResult> TestBulkInsert([FromBody] List<User> payload, CancellationToken ct)
        {
            var result = await _userService.ExecuteBulkInsertAsync(payload, ct);

            if (result)
            {
                return Ok(new { Message = $"Successfully inserted {payload.Count} records using EF Core via MediatR!" });
            }

            return StatusCode(500, new { Message = "Failed to insert batch payload. Verify table access." });
        }




    }
}


