using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repository
{
    public interface IRepository
    {
        Task<IActionResult> Login(string username, string password);
    }
}
