using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTO;
using RepositoryLayer.DTO;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        User Register(UserDTO userDTO); 
        User Login(LoginDTO loginDTO);

        public bool ForgotPassword(ForgotPasswordDTO forgotPasswordDto);
        public bool ResetPassword(ResetPasswordDTO resetPasswordDto);
    }
}
