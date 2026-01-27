using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.Services.Password
{
    public interface IPasswordService
    {
        string Hash(string password, object userContext);
        bool Verify(string hashedPassword, string password, object userContext);
    }
}