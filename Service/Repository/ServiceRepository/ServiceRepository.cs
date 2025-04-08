using DynamicPasswordPolicy.Database;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Service.Repository
{
    public class ServiceRepository :  ControllerBase ,IRepository
    {
        public readonly IEncryptionDecryption _encryptionDecryption;
        public readonly AppDatabaseEntity _appDatabase;
        public int PassqordAttemptCount = 1;
        public ServiceRepository(IEncryptionDecryption encryptionDecryption, AppDatabaseEntity appDatabase)
        {
            _encryptionDecryption = encryptionDecryption;
            _appDatabase = appDatabase;
        }


        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                // Validate if username or password is empty
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return BadRequest("Username and password are required.");
                }

                // Encrypt the entered password
                var encryptedPassword = await _encryptionDecryption.PasswordEncryption(password);

                // Fetch user details by username
                var user = _appDatabase._user.FirstOrDefault(p => p.UserName == username);

                // Check if user exists
                if (user == null)
                {
                    return BadRequest("Invalid username & password.");
                }

                // Check if user account is locked
                if (user.IsLocked)
                {
                    return BadRequest("Your account is locked. Please contact the Admin team.");
                }

                // Verify encrypted password
                if (user.Password != encryptedPassword)
                {
                    // Increment failed login attempts
                    user.FailedAttempts++;

                    // Get maximum allowed login attempts from password policy
                    int maxAttempts = _appDatabase._passwordPolicies
                                        .Select(p => p.PasswordAttemptCount)
                                        .FirstOrDefault();

                    // Lock the account if failed attempts exceed policy limit
                    if (user.FailedAttempts >= maxAttempts)
                    {
                        user.IsLocked = true;
                    }

                    // Save changes (failed attempt or lock status) to database
                    await _appDatabase.SaveChangesAsync();

                    return BadRequest($"Invalid credentials. Remaining attempts: {maxAttempts - user.FailedAttempts}.");
                }

                // Reset failed attempts on successful login
                user.FailedAttempts = 0;
                await _appDatabase.SaveChangesAsync();

                // Return success response
                return Ok("Login Successfully");
            }
            catch (Exception ex)
            {
                // Log the exception if logging is configured (optional)
                return StatusCode(500, "Internal Server Error");
            }
        }


    }
}
