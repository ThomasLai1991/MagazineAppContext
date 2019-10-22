using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MagazineAppContext.Models
{
    [DebuggerDisplay("ID:{id_nte} Note:{text_nte}")]
    public class note_nte
    {
            public int id_nte;
            public int id_client_nte;
            public DateTime date_nte;
            public string text_nte;
            public bool hidden_nte;
    }
}
