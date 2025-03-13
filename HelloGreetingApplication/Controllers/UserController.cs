using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;
using RepositoryLayer.DTO;

namespace HelloGreetingApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL;

        public UserController(IUserBL userBL)
        {
            _userBL = userBL;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDTO userDTO)
        {
            var registeredUser = _userBL.Register(userDTO);
            return Ok(new { Success = true, Message = "User registered successfully." });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginRequest)
        {
            var user = _userBL.Login(loginRequest);
            if (user == null)
                return Unauthorized(new { Success = false, Message = "Invalid credentials." });

            return Ok(new { Success = true, Message = "Login successful." });
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            return Ok(new { Success = true, Message = "Password reset email sent." });
        }


        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            return Ok(new { Success = true, Message = "Password reset successful." });
        }
    }
}
