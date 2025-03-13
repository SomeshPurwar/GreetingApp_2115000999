using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Helper;
using BusinessLayer.Interface;
using RepositoryLayer.DTO;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class UserBL :IUserBL
    {
        private readonly IUserRL _userRepository;

        public UserBL(IUserRL userRepository)
        {
            _userRepository = userRepository;
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
    }
}
