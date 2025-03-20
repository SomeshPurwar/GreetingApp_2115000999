using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Helper;
using BusinessLayer.Interface;
using ModelLayer.DTO;
using RepositoryLayer.DTO;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class UserBL :IUserBL
    {
        private readonly IUserRL _userRepository;
        private readonly JwtHelper _jwtHelper;
        private readonly EmailService _emailService;
        private readonly RabbitMQProducer _emailProducer;

        public UserBL(IUserRL userRepository , JwtHelper jwtHelper, EmailService emailService, RabbitMQProducer emailProducer)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
            _emailProducer = emailProducer;
        }

        public User Register(UserDTO userDTO)
        {
            var user = new User
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                PasswordHash = PasswordHasher.HashPassword(userDTO.Password) 
            };

            return _userRepository.Register(user);
        }

        public User Login(LoginDTO loginDTO)
        {
            var user = _userRepository.GetUserByEmail(loginDTO.Email);
            if (user == null || !PasswordHasher.VerifyPassword(loginDTO.Password, user.PasswordHash))
                return null; // Invalid credentials

            return user;
        }

        public bool ForgotPassword(ForgotPasswordDTO forgotPasswordDto)
        {
            var user = _userRepository.GetUserByEmail(forgotPasswordDto.Email);
            if (user == null) return false;

            // Generate Reset Token (JWT)
            string resetToken = _jwtHelper.GeneratePasswordResetToken(user.Email);

            // Send Email with Reset Token
            string resetLink = $"https://localhost:7150/reset-password?token={resetToken}";
            string emailBody = $"Click <a href='{resetLink}'>here</a> to reset your password.";

            _emailProducer.PublishEmail(user.Email, "Password Reset Request", emailBody);
            //return _emailService.SendEmail(user.Email, "Password Reset Request", emailBody);
            return true;
        }

        public bool ResetPassword(ResetPasswordDTO resetPasswordDto)
        {
            var claims = _jwtHelper.ValidateToken(resetPasswordDto.Token);
            if (claims == null) return false; // Invalid or expired token

            var email = claims.FindFirst(ClaimTypes.Email)?.Value
         ?? claims.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
            if (email == null) return false;

            var user = _userRepository.GetUserByEmail(email);
            if (user == null) return false;

            user.PasswordHash = PasswordHasher.HashPassword(resetPasswordDto.NewPassword);
            _userRepository.UpdateUser(user);
            return true;
        }
    }
}
