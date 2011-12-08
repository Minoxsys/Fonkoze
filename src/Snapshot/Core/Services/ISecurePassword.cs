using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Services
{
    public interface ISecurePassword
    {
         string EncryptPassword(string password);
         string DecryptPassword(string cryptedPassword);
    }
}
