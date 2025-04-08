using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IUser
    {
        Task<bool> CreateUser(User model);

        Task<bool> PasswordValidation(string model);

    }
}
