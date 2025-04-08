
using DynamicPasswordPolicy.Database;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Model.Models;
using Newtonsoft.Json.Linq;
using Service.Interface;
using Service.Repository;


namespace DynamicPasswordPolicy.Controllers
{
    public class UserController : Controller
    {
        public readonly IEncryptionDecryption _encryptionDecryption;
        public readonly IUser _user;
        public readonly IRepository _repository;
        public readonly AppDatabaseEntity _appDatabase;

        public UserController(IEncryptionDecryption encryptionDecryption, AppDatabaseEntity appDatabase, IUser user, IRepository repository)
        {
            _encryptionDecryption = encryptionDecryption;
            _appDatabase = appDatabase;
            _user = user;
            _repository = repository;
        }



        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }

       

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        public async Task<ActionResult> Create(User model)
        {
            try
            {
              
                if (model.ConfirmPassword.Equals(model.Password) && model.UserName is not null)
                {
                    var PasswordValidationResult = await _user.PasswordValidation(model.Password); // check password validation

                    if(PasswordValidationResult)
                    {
                        var result = await _user.CreateUser(model);
                        if (result)
                        {
                            TempData["Message"] = "User Created successfully";
                            return RedirectToAction("Login");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Password not matching with passowrd policy";
                           return View();
                    }
                   
                }
                else
                {
                    TempData["Message"] = "Kindly check credentials";
                    return View();
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
            return View();
        }




        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(string UserName, string Password)
        {
            try
            {
                if(Password is not null && UserName is not null)
                {
                    

                    var result= await _repository.Login (UserName,Password);

                    int statusCode = (result as ObjectResult)?.StatusCode ?? (result as StatusCodeResult)?.StatusCode ?? 200;
                    if (statusCode == 200)
                    {
                        HttpContext.Session.SetString("userId", UserName);
                        TempData["Message"] = "Login Successfully";
                        return RedirectToAction("List");
                    }
                    else
                    {
                        var Message = (result as ObjectResult)?.Value ?? (result as StatusCodeResult)?.StatusCode ?? 400;
                        TempData["Message"] = Message;
                    }


                 
                }
                else
                {
                    TempData["Message"] = "Kindly fill credentials";
                }

            }  
            catch(Exception ex)
            {
                throw ex;
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }


    }
}
