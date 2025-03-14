using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        User Register(User user);
        User GetUserByEmail(string email);

        public void UpdateUser(User user);
    }
}
