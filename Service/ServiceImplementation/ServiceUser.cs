using DynamicPasswordPolicy.Database;
using Model.Models;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Service.ServiceImplementation
{
    public class ServiceUser : IUser
    {

        public readonly IEncryptionDecryption _encryptionDecryption;
        public readonly AppDatabaseEntity _appDatabase;

        public ServiceUser(IEncryptionDecryption encryptionDecryption, AppDatabaseEntity appDatabase)
        {
            _encryptionDecryption = encryptionDecryption;
            _appDatabase = appDatabase;
        }

        public async Task<bool> CreateUser(User model)
        {
            bool output = false;
            try
            {
                if (model.ConfirmPassword.Equals(model.Password) && model.UserName is not null)
                {
                    var data = await _encryptionDecryption.PasswordEncryption(model.Password);

                    if (data is not null)
                    {
                        User modelData = new User();
                        modelData.Guid = Guid.NewGuid();
                        modelData.UserName = model.UserName;
                        modelData.Password = Convert.ToString(data)!;
                        modelData.CreatedDate = DateTime.Now;

                        _appDatabase._user.Add(modelData);
                        _appDatabase.SaveChanges();

                        output = true; ;

                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return output;
        }

        public async Task<bool> PasswordValidation(string password)
        {
            bool output = false;

            try
            {
                if (string.IsNullOrEmpty(password))
                {
                    return output;
                }
                
                var data = _appDatabase._passwordPolicies.FirstOrDefault(x => x.Id == 1);
                if(data is not null)
                {
                    string pattern = $@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{{8,{Math.Max(8, data.PasswordLegth)}}}$";

                    output = Regex.IsMatch(password, pattern);
                }

            }
            catch(Exception ex)
            {
                throw;
            }

            return output;
        }
    }
}
