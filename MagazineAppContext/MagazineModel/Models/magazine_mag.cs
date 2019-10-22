using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MagazineAppContext.Models
{
    [DebuggerDisplay("ID:{id_mag} Name:{name_mag}")]
    public class magazine_mag
    {
        public int id_mag;
        public string name_mag;
        public string description_mag;
        public bool hidden_mag;
    }


}
