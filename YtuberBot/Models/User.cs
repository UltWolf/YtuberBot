using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YtuberBot.Models
{
    [Serializable]
    public class User
    {
        public string  email { get; set; }
        public string password { get; set; }
    }
}
