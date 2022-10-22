using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace NEA_Project
{
    class CustomerUser : UserInfo
    {
        protected string username, password;
        public CustomerUser(string username, string password) : base(username, password)
        {
            this.username = username;
            this.password = password;
        }
    }
}


