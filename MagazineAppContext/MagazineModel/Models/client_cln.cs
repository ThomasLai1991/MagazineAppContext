using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MagazineAppContext.Models
{
    [DebuggerDisplay("ID:{id_cln} Name:{name_cln}")]
    public class client_cln_list
    {
        public int id_cln ;
        public int id_status_cln;
        public int id_region_cln;
        public DateTime lastedit_cln;
        public string name_cln;
        public string tags_cln;
        public string StatusName;
        public int id_user_usr_cln;
        public int id_magazine_cln_mag;
    }

    public class CFilters
    {
        public int UserID=0;
        public int MagazineID=0;
        public int RegionID=0;
        public int StatusID=0;
        public string SearchText="";
        public bool SearchKeywordInsteadOfName=false;
    }

    public class client_cln
    {
        public int id_cln;
        public int id_status_cln;
        public int id_lastnote_cln;
        public int id_region_cln;
        public DateTime date_cln;
        public DateTime lastedit_cln;
        public string name_cln;
        public string tags_cln;
        public string phone_cln;
        public string contact_cln;
        public string fax_cln;
        public string email_cln;
        public string website_cln;
        public string notes_cln;
        public string street_addr_cln;
        public string billing_addr_cln;
        public string mailing_addr_cln;
        public bool mailout_cln;
        public bool unassigned_cln;
        public bool hidden_cln;

        public string StatusName;
        public string RegionName;
        public int id_user_usr_cln;
        public int id_magazine_cln_mag;

    }
}
