using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI2.APIToken;
using WebAPI2.DTO;

namespace WebAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ITokenAuthenticationService _authService;
        public AuthenticationController(ITokenAuthenticationService authService)
        {
            this._authService = authService;
        }

        [AllowAnonymous]
        [HttpPost, Route("requestToken")]
        public ActionResult RequestToken([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }

            string token;
            if (_authService.IsAuthenticated(request, out token))
            {
                return Ok("Bearer " + token);
            }

            return BadRequest("Invalid Request");
        }
    }
}
