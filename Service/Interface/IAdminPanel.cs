using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IAdminPanel
    {
        Task<List<User>> GettAllLockedUser();

        Task<bool> IsUserExist(string userName);


        Task<string> ResetPassword(string userName,string Password);
    }
}
