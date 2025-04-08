using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IEncryptionDecryption
    {
        Task<string> PasswordEncryption(string password);

        Task<string> PasswordDecryption(string password);
    }
}
