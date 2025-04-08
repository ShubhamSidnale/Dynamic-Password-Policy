using DynamicPasswordPolicy.Database;
using Microsoft.AspNetCore.Mvc;
using Model.Models;
using Service.Interface;
using Service.Repository;

namespace DynamicPasswordPolicy.Controllers
{
    public class AdminPanelController : Controller
    {

        private readonly IAdminPanel _adminPanel;

        //private const  IAdminPanel _adminPanel1=null;
        public AdminPanelController(IAdminPanel adminPanel)
        {
            _adminPanel = adminPanel;
        }

        public IActionResult Index()
        {
           
            return View();
        }


        public IActionResult List()
        {
            try
            {
                var list = _adminPanel.GettAllLockedUser();

                if(list.Result.Count > 0)
                {
                    return View(list.Result);
                }
                else
                {
                    var empty = new List<User>();
                    return View(empty);
                }
            }
            catch(Exception ex)
            {
                throw;
            }

        }


        public IActionResult Edit()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string Username, string password,string ConfirmPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(ConfirmPassword))
                {
                    TempData["ResetPasswordMsg"] = "Username & password should not be blank";
                    return View();
                }

                var result = await _adminPanel.IsUserExist(Username);

                if(result)
                {
                    if(string.Equals(password, ConfirmPassword, StringComparison.OrdinalIgnoreCase))
                    {
                        var Result = _adminPanel.ResetPassword(Username, password);
                        TempData["ResetPasswordMsg"] = Result.ToString();
                        return RedirectToAction("Login", "User");
                    }
                }
                else
                {
                    TempData["ResetPasswordMsg"] = "Invalid user";
                    return View();
                }

            }
            catch(Exception ex)
            {
                throw;
            }

            return View();
        }




    }
}
