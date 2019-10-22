using System;
using System.Collections.Generic;
using System.Text;
using MagazineAppContext.Models;

namespace MagazineAppContext
{
    class Program
    {
        static void Main(string[] arugs)
        {
            Context context = new Context();
            //Console.WriteLine( context.Model.Magazines.GetCount());
            //Console.WriteLine(context.Model.Users.GetCount());
            //Console.WriteLine(context.Model.Status.GetCount());

            //Console.WriteLine(context.Model.Regions.GetCount());
            //Console.WriteLine(context.Model.Notes.GetCount());

            //Console.WriteLine(context.Model.Clients.GetCount());
            var filter = new CFilters();
            filter.UserID = 1;
            filter.MagazineID =1;
            filter.SearchText = "Thai";

            Console.WriteLine(context.Model.Clients.GetCount(filter));


            //context.Model.Clients.LoadListItems(50, 1,filter);





            Console.ReadLine();
        }
    }
}
