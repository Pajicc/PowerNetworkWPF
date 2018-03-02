using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF1._5
{
    public class User
    {
        private string Username;
        private string Password;

        public User()
        {
            Username = String.Empty;
            Password = String.Empty;
        }

        public void setUserPass(string user, string pass)
        {
            Username = user;
            Password = pass;
        }

        public string getUser()
        {
            return Username;
        }

        public string getPass()
        {
            return Password;
        }
    }
}
