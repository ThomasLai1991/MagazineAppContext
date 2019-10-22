using System;
using System.Collections.Generic;
using System.Reflection;
using MagazineAppContext.Models;

namespace MagazineAppContext
{
    public class MagazineModels
    {
        public List<user_usr> Users;
        public List<magazine_mag> Magazines;
        public List<client_cln> Client;  
        public List<client_cln_list> Clients;
        public int Count;
        public List<status_sts> Status;
        public List<region_rgn> Regions;
        public List<note_nte> Notes;

        public MagazineModels()
        {
            Magazines = new List<magazine_mag>();
            Users = new List<user_usr>();
            Client = new List<client_cln>();
            Clients = new List<client_cln_list>();
            Status = new List<status_sts>();
            Regions = new List<region_rgn>();
            Notes = new List<note_nte>();
        }

    }
}