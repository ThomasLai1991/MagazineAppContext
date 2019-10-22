using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MagazineAppContext.Models
{
    [DebuggerDisplay("ID:{id_sts} Name:{name_sts}")]
    public class status_sts
    {
        public int id_sts;
        public string name_sts;
        public string hidden_sts;
    }
}
