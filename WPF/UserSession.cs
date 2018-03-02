using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF1._5
{
    public class UserSession
    {
        private static volatile User instance;

        public static User getInstance()
        {
            if (instance == null)
            {
                instance = new User();
            }
            return instance;
        }
    }
}
