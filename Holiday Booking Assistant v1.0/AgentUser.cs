using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NEA_Project
{
    internal class AgentUser : UserInfo
    {
        protected string username, password;
        public AgentUser(string username, string password) : base(username, password)
        {
            this.username = username;
            this.password = password;
        }
    }
}