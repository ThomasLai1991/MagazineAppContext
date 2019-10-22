using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MagazineAppContext.Models
{
    [DebuggerDisplay("ID:{id_rgn} Name:{name_rgn}")]
    public class region_rgn
    {
            public int id_rgn;
            public string name_rgn;
            public bool hidden_rgn;
    }
}
