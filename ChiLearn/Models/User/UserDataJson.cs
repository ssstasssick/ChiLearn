using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiLearn.Models.User
{
    public class UserDataJson
    {
        public string Name { get; set; } = "Неизвестный";
        public string Email { get; set; }
        public int LastLevelNum { get; set; } = 1;
        public bool isAuth { get; set; } = false;
    }
}
