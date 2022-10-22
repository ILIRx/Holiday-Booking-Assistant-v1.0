using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA_Project
{
    class UserInfo
    {
        protected string username, password, agentKey;
        protected int passwordKey;
        private bool isAgent;
        private static Random rnd = new Random();
        public UserInfo(string username, string password)
        {
            this.username = username;
            this.password = password;
            this.agentKey = generateKey();
        }
        public void setIsAgent(bool isAgent)
        {
            this.isAgent = isAgent;
        }
        public bool getIsAgent()
        {
            return isAgent;
        }
        public void setPassword(string passwordIN)
        {
            password = passwordIN;
        }
        public void setUserName(string usernameIN)
        {
            username = usernameIN;
        }
        public void setPassKey(int key)
        {
            passwordKey = key;
        }

        public string getUserName()
        {
            return username;
        }
        public int getPasswordKey()
        {
            return passwordKey;
        }
        private string generateKey()
        {
            for (int i = 0; i < 20; i++)
            {
                agentKey += (char)rnd.Next(33, 94);
            }
            File.WriteAllText("agentKey.txt", agentKey);
            return agentKey;
        }
        public string getAgentKey()
        {
            return agentKey;
        }
    }
}
