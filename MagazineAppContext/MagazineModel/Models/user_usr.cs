using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace MagazineAppContext.Models
{
    [DebuggerDisplay("ID:{id_usr} Name:{name_usr}")]
    public class user_usr
    {
        public int id_usr;
        public string name_usr;
        public string username_usr;
        public string password_usr;
        public string security_usr;
        public bool admin_usr;
        public bool notlisted_usr;
        public bool disabled_usr;
        public bool hidden_usr;

    }
}
