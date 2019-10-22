using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using MagazineAppContext.Models;


namespace MagazineAppContext
{
    public class Context
    {
        public MagazineModels Model;

        public Context()
        {
            Model = new MagazineModels();
        }
    }


    public static class Extensions
    {
        public  static TCPConnection Connection;

        static Extensions()
        {
            Connection = new TCPConnection();
        }

        public static string Login(this Context context,string UserName, string PassWord)
        {

            string Query = createLoginQuery(UserName, PassWord);
            Query = Data.CreateStandardQuery(Query);
            string response = Connection.SendStream(Query);

            return response;

            string createLoginQuery(string userName, string passWord)
            {
                string query = $"SELECT * FROM user_usr WHERE username_usr=\"{userName}\" AND password_usr=\"{passWord}\" AND disabled_usr=False;";
                return query;
            }

        }

        public static int GetCount(this List<client_cln_list> list,CFilters Filter)
        {

            string Query = BuildClientListQuery(Filter);


            Query = Data.CreateRecordCountQuery(Query);
            string response = Connection.SendStream(Query);
            int count = Convert.ToInt32(Data.GetData(response, typeof(string), "RecordCount"));
            return count;

            string createSelect(string tableName)
            {
                string query = $"SELECT * FROM {tableName};";
                return query;
            }

            string BuildClientListQuery(CFilters filter)
            {
                string sQuery;
                string sOperator = " WHERE ";

                // Select correct stored query
                if (filter.UserID == 0 && filter.MagazineID == 0)
                {
                    sQuery = "SELECT * FROM client_cln_List0";
                }
                else if (filter.UserID > 0 && filter.MagazineID == 0)
                {
                    sQuery = "SELECT * FROM client_cln_List1 WHERE id_user_usr_cln={1}";
                    sQuery = sQuery.Replace("{1}", filter.UserID.ToString());
                }
                else if (filter.UserID == 0 && filter.MagazineID > 0)
                {
                    sQuery = "SELECT * FROM client_cln_List2 WHERE id_magazine_cln_mag={2}";
                    sQuery = sQuery.Replace("{2}", filter.MagazineID.ToString());
                }
                else // (UserID > 0 && MagazineID > 0)
                {
                    sQuery = "SELECT * FROM client_cln_List3 WHERE (id_user_usr_cln={1} AND id_magazine_cln_mag={2})";
                    sQuery = sQuery.Replace("{1}", filter.UserID.ToString());
                    sQuery = sQuery.Replace("{2}", filter.MagazineID.ToString());
                }

                // Check if WHERE has been used yet
                if (sQuery.IndexOf("WHERE") > -1)
                {
                    sOperator = " AND ";
                }

                // Add Region and Status criteria
                if (filter.RegionID == 0 && filter.StatusID == 0)
                {
                    // 
                }
                else if (filter.RegionID > 0 && filter.StatusID == 0)
                {
                    sQuery += sOperator + "id_region_cln={1}";
                    sQuery = sQuery.Replace("{1}", filter.RegionID.ToString());
                }
                else if (filter.RegionID == 0 && filter.StatusID > 0)
                {
                    sQuery += sOperator + "id_status_cln={2}";
                    sQuery = sQuery.Replace("{2}", filter.StatusID.ToString());
                }
                else // (RegionID > 0 && StatusID > 0)
                {
                    sQuery += sOperator + "(id_region_cln={1} AND id_status_cln={2})";
                    sQuery = sQuery.Replace("{1}", filter.RegionID.ToString());
                    sQuery = sQuery.Replace("{2}", filter.StatusID.ToString());
                }

                // Check if WHERE has been used yet
                if (sQuery.IndexOf("WHERE") > -1)
                {
                    sOperator = " AND ";
                }

                // Add text search criteria
                if (filter.SearchText != "")
                {
                    string sFind = filter.SearchText;
                    if (filter.SearchKeywordInsteadOfName)
                    {
                        sQuery += sOperator + $"InStr([tags_cln],\"{sFind}\")>0";
                    }
                    else
                    {
                        sQuery += sOperator + $"InStr([name_cln],\"{sFind}\")>0";
                    }
                }
                sQuery += ";";
                return sQuery;
            }
        }
        //SINGLE ITEM METHODS
        public static void CreateOrUpdateSingleItem<T>(this List<T> list, T NewItem)
        {
            //get Entity type and its field
            Type EntityType = typeof(T);
            FieldInfo[] fields = EntityType.GetFields();

            //write the queries and send out
            string Package = PackRecord(NewItem);
            string Query = CreateCreateSingleQuery(NewItem,EntityType.Name,fields);
            string Stream = Data.CreateSaveQuery(Query, Package);

            //read the response,parse and set ID 
            string Response = Connection.SendStream(Stream);
            int ID = (int)Data.GetData(Response, typeof(int), "RecordID");
            fields[0].SetValue(NewItem, ID);
            //depending on the return ID, add the item as new one(ID returned not in list), or update the item in list(ID returned is in the list)
            AddNewItemToList(list,NewItem);

            string PackRecord(object item)
            {
                string package = "";
                foreach (var field in fields)
                {
                    package = Data.PackData(package, field.Name, field.GetValue(item));
                }
                return package;
            }
            string CreateCreateSingleQuery(object newItem,string tableName, FieldInfo[] fieldInfos)
            {
                string query = $"SELECT * FROM {tableName} WHERE {fieldInfos[0].Name}={fieldInfos[0].GetValue(newItem)};";
                return query;
            }
        }

        public static void LoadOrUpdateSingleItem<T>(this List<T> list,int ID)
        {

            Type EntityType = typeof(T);
            FieldInfo[] fields = EntityType.GetFields();

            string Query = createLoadSingleQuery(ID, EntityType.Name,fields);
            Query = Data.CreateStandardQuery(Query);
            string response = Connection.SendStream(Query);

            //prepare new instance to be mapped
            var newItem = Activator.CreateInstance<T>();
            //given the received data and the newly created instance, map them together
            EntityMapping(response, newItem);

            AddNewItemToList(list,newItem);

            string createLoadSingleQuery(int id, string tableName, FieldInfo[] fieldInfos)
            {  
                //makesure it always chood the first field which should be id(the name of id is different in different table )
                string query = $"SELECT * FROM {tableName} WHERE  {fieldInfos[0].Name.ToString()}={id};";
                return query;
            }
        }

      
        public static void DeleteSingleItem<T>(this List<T> list, int ID)
        {
            Type EntityType = typeof(T);
            FieldInfo[] fields = EntityType.GetFields();
            string Query = createDeleteSingleQuery(ID,EntityType.Name,fields);
            Query = Data.CreateDeleteQuery(Query);
            string response = Connection.SendStream(Query);

            string createDeleteSingleQuery(int id, string tableName, FieldInfo[] fieldInfos)
            {
                //makesure it always chood the first field which should be id(the name of id is different in different table )
                string query = $"SELECT * FROM {tableName} WHERE  {fieldInfos[0].Name.ToString()}={id};";
                return query;
            }
        }
        //LIST ITEM METHOD
        public static void LoadListItems<T>(this List<T> list, int ResultPerPage, int PageNumber)
        {
            Type EntityType = typeof(T);
            list.Clear();
            //need to add page query
            string Query = createLoadListQuery(EntityType.Name);

            Query = Data.CreateStandardQuery(Query);
            string response = Connection.SendStream(Query);
            var obj = Activator.CreateInstance<T>();

            char separator = Convert.ToChar(21);
            string[] results = response.Split(separator);
            int resultNumber = results.Length - 1;
            //index 0 is reserved for server answer, so retrieved informations start from index  1.

            for (int count = 1; count <= resultNumber; count++)
            {
                var listObj = Activator.CreateInstance<T>();
                EntityMapping(results[count], listObj);
                list.Add(listObj);
            }

            var a = list.Count;

            //EntityMapping

            string createLoadListQuery(string tableName)
            {
                string query = $"SELECT * FROM {tableName}_List;";
                return query;
            }

             

        }
        public static void LoadListItems(this List<client_cln_list> list, int ResultsPerPage, int PageNumber, CFilters Filter)
        {
            string[] Results;
            char Separator = Convert.ToChar(21);
            list.Clear();
            string Query = BuildClientListQuery(Filter);
            string Response;

            if (ResultsPerPage > 0 && PageNumber > 0)
            {
                Query = Data.CreatePagedQuery(Query, PageNumber, ResultsPerPage);
            }
            else
            {
                Query = Data.CreateStandardQuery(Query);
            }
            Response = Connection.SendStream(Query);
            Results = Response.Split(Separator);
            int resultNumber = Results.Length - 1;

            for (int count = 1; count <= resultNumber; count++)
            {
                var listObj = new client_cln_list();
                EntityMapping(Results[count], listObj);
                list.Add(listObj);
            }
            var a = list.Count;

            string BuildClientListQuery(CFilters filter)
            {
                string sQuery;
                string sOperator = " WHERE ";

                // Select correct stored query
                if (filter.UserID == 0 && filter.MagazineID == 0)
                {
                    sQuery = "SELECT * FROM client_cln_List0";
                }
                else if (filter.UserID > 0 && filter.MagazineID == 0)
                {
                    sQuery = "SELECT * FROM client_cln_List1 WHERE id_user_usr_cln={1}";
                    sQuery = sQuery.Replace("{1}", filter.UserID.ToString());
                }
                else if (filter.UserID == 0 && filter.MagazineID > 0)
                {
                    sQuery = "SELECT * FROM client_cln_List2 WHERE id_magazine_cln_mag={2}";
                    sQuery = sQuery.Replace("{2}", filter.MagazineID.ToString());
                }
                else // (UserID > 0 && MagazineID > 0)
                {
                    sQuery = "SELECT * FROM client_cln_List3 WHERE (id_user_usr_cln={1} AND id_magazine_cln_mag={2})";
                    sQuery = sQuery.Replace("{1}", filter.UserID.ToString());
                    sQuery = sQuery.Replace("{2}", filter.MagazineID.ToString());
                }

                // Check if WHERE has been used yet
                if (sQuery.IndexOf("WHERE") > -1)
                {
                    sOperator = " AND ";
                }

                // Add Region and Status criteria
                if (filter.RegionID == 0 && filter.StatusID == 0)
                {
                    // 
                }
                else if (filter.RegionID > 0 && filter.StatusID == 0)
                {
                    sQuery += sOperator + "id_region_cln={1}";
                    sQuery = sQuery.Replace("{1}", filter.RegionID.ToString());
                }
                else if (filter.RegionID == 0 && filter.StatusID > 0)
                {
                    sQuery += sOperator + "id_status_cln={2}";
                    sQuery = sQuery.Replace("{2}", filter.StatusID.ToString());
                }
                else // (RegionID > 0 && StatusID > 0)
                {
                    sQuery += sOperator + "(id_region_cln={1} AND id_status_cln={2})";
                    sQuery = sQuery.Replace("{1}", filter.RegionID.ToString());
                    sQuery = sQuery.Replace("{2}", filter.StatusID.ToString());
                }

                // Check if WHERE has been used yet
                if (sQuery.IndexOf("WHERE") > -1)
                {
                    sOperator = " AND ";
                }

                // Add text search criteria
                if (filter.SearchText != "")
                {
                    string sFind = filter.SearchText;
                    if (filter.SearchKeywordInsteadOfName)
                    {
                        sQuery += sOperator + $"InStr([tags_cln],\"{sFind}\")>0";
                    }
                    else
                    {
                        sQuery += sOperator + $"InStr([name_cln],\"{sFind}\")>0";
                    }
                }
                sQuery += ";";
                return sQuery;
            }
        }




        //OTHERS
        //add or update an item in list
        public static void AddNewItemToList<T>(List<T> list, T NewItem)
        {
            Type EntityType = typeof(T);
            FieldInfo[] fields = EntityType.GetFields();
            int newItemID = Convert.ToInt32(fields[0].GetValue(NewItem));
            //if there is an item in the list which its ID is the same as the retreived item's ID
            object exsistedItem = list.Find(item => Convert.ToInt32(fields[0].GetValue(item)) == newItemID);
            if (exsistedItem != null)
            {
                //then that means they are the same item, so map them again to update values
                EntityMapping(NewItem, exsistedItem);
            }
            else
            {
                //otherwise, it means the retrieved item is not in the list, it is brand new, then add it to the list
                list.Add(NewItem);
            }
        }
        //mapping from text to fields
        private static void EntityMapping(string DataSource, object Entity)
        {
            Type entityType = Entity.GetType();
            FieldInfo[] Fields = entityType.GetFields();
            foreach (var field in Fields)
            {
                object value = Data.GetData(DataSource, field.FieldType, field.Name);
                field.SetValue(Entity, value);
            }
        }
        //mapping from fields to fields
        private static void EntityMapping(object DataSource,object Entity)
        {
            Type entityType = Entity.GetType();
            FieldInfo[] Fields = entityType.GetFields();
            

            foreach (var field in Fields)
            {
                field.SetValue(Entity, field.GetValue(DataSource));
            }
        }

       



    }
}
