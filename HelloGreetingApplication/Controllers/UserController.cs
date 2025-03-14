using BusinessLayer.Helper;
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
        private readonly JwtHelper _jwtHelper;


        public UserController(IUserBL userBL, JwtHelper jwtHelper)
        {
            _userBL = userBL;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDTO userDTO)
        {
            var registeredUser = _userBL.Register(userDTO);
            if (registeredUser == null)
                return BadRequest(new { Success = false, Message = "User registration failed." });

            var token = _jwtHelper.GenerateToken(registeredUser.Email, registeredUser.Id); 

            return Ok(new { Success = true, Message = "User registered successfully.", Token = token });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginRequest)
        {
            var user = _userBL.Login(loginRequest);
            if (user == null)
                return Unauthorized(new { Success = false, Message = "Invalid credentials." });

            var token = _jwtHelper.GenerateToken(user.Email, user.Id); 

            return Ok(new { Success = true, Message = "Login successful.", Token = token });
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDto)
        {
            var success = _userBL.ForgotPassword(forgotPasswordDto);
            if (!success)
                return NotFound(new { Success = false, Message = "Email not found." });

            return Ok(new { Success = true, Message = "Password reset email sent." });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
        {
            var success = _userBL.ResetPassword(resetPasswordDto);
            if (!success)
                return BadRequest(new { Success = false, Message = "Invalid or expired reset token." });

            return Ok(new { Success = true, Message = "Password reset successful." });
        }
    }
}
