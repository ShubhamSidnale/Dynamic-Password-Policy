using DynamicPasswordPolicy.Database;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using Service.Interface;
using Service.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.ServiceImplementation
{
    public class ServiceAdminPanel : IAdminPanel
    {
        public readonly IUser _user;
        public readonly AppDatabaseEntity _appDatabase;
        public readonly IEncryptionDecryption _encryptionDecryption;

        public ServiceAdminPanel(AppDatabaseEntity appDatabase, IEncryptionDecryption encryptionDecryption)
        {
            _appDatabase = appDatabase;
            _encryptionDecryption = encryptionDecryption;
        }

        // Get all locked users from the database
        public async Task<List<User>> GettAllLockedUser()
        {
            List<User> ListofLockedUSer = new List<User>();

            try
            {
                ListofLockedUSer = _appDatabase._user
                    .Where(p => p.IsLocked) // Filter only locked users
                    .Select(item => new User
                    {
                        Guid = item.Guid,
                        UserName = item.UserName,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        IsLocked = item.IsLocked
                    })
                    .ToList(); // Convert query to list

                if (ListofLockedUSer.Count() > 0)
                {
                    return ListofLockedUSer;
                }
            }
            catch (Exception ex)
            {
                throw; // Re-throw the exception for higher-level handling/logging
            }

            return ListofLockedUSer;
        }

        // Check if user exists by username
        public async Task<bool> IsUserExist(string UserName)
        {
            bool output = false;

            try
            {
                if (string.IsNullOrEmpty(UserName))
                {
                    return output;
                }

                // Check if any user exists with the given username
                output = _appDatabase._user.Any(p => p.UserName == UserName) ? true : false;

                return output;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Reset user's password
        public async Task<string> ResetPassword(string Username, string password)
        {
            string result = string.Empty;

            try
            {
                // Check password history before resetting
                var Data = await PasswordHistory(Username, password);
                result = Data;
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        // Check if the new password already exists in the user's password history
        public async Task<string> PasswordHistory(string Username, string password)
        {
            string output = string.Empty;

            try
            {
                // Encrypt the new password
                var EncryptedPassword = await _encryptionDecryption.PasswordEncryption(password)!;

                // Get the user Guid
                var UserID = _appDatabase._user
                    .Where(p => p.UserName == Username)
                    .Select(p => p.Guid)
                    .FirstOrDefault();

                // Check if encrypted password exists in the user's password history
                var PasswordExistorNot = _appDatabase.PasswordHistories
                    .Where(p => p.Guid == UserID && p.Password.Equals(EncryptedPassword));

                if (PasswordExistorNot.Count() >= 1)
                {
                    // Password reuse not allowed
                    return output = $"You cannot reuse any of your last used passwords";
                }
                else
                {
                    // Record the new password in the history
                    var Result = await RecordPasswordChange(UserID, EncryptedPassword);

                    if (Result)
                    {
                        output = "Password change successfully";
                    }
                    else
                    {
                        output = "Something went wrong";
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return output;
        }

        // Record a new password in the history and update the user's current password
        public async Task<bool> RecordPasswordChange(Guid guid, string Password)
        {
            bool result = false;

            try
            {
                // Get the allowed password history count from policy
                var PasswordHistoryCount = _appDatabase._passwordPolicies
                    .Select(p => p.HistoryCount)
                    .FirstOrDefault();

                // Get current password history entries sorted by date (ascending)
                var CurrentPasswordCount = _appDatabase.PasswordHistories
                    .Where(p => p.Guid == guid)
                    .OrderBy(p => p.PasswordChangedDate);

                if (CurrentPasswordCount.Count() <= PasswordHistoryCount)
                {
                    // Update current password in User table
                    _appDatabase._user
                        .Where(u => u.Guid == guid)
                        .ExecuteUpdate(setters => setters
                            .SetProperty(u => u.Password, u => Password)
                            .SetProperty(u => u.CreatedDate, u => DateTime.Now));

                    // Add new password entry in history table
                    var TempData = new PasswordHistory
                    {
                        Guid = guid,
                        Password = Password,
                        PasswordChangedDate = DateTime.Now
                    };

                    _appDatabase.PasswordHistories.Add(TempData);
                    _appDatabase.SaveChanges();
                    result = true;
                }
                else
                {
                    // If history count exceeds, replace the oldest entry
                    var updatePassword = CurrentPasswordCount.FirstOrDefault(); // Get oldest record

                    // Update the oldest record with the new password
                    _appDatabase.PasswordHistories
                        .Where(u => u.Guid == guid && u.ID == updatePassword.ID)
                        .ExecuteUpdate(setters => setters
                            .SetProperty(u => u.Password, Password)
                            .SetProperty(u => u.PasswordChangedDate, DateTime.UtcNow));

                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }
    }
}
