using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Users
{
    internal interface IUser
    {
        public void SetPassword(string raw, IPasswordHasher hasher); 
    }
}
