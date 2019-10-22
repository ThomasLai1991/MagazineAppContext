
using System;
using System.Collections.Generic;
using System.Text;

namespace MagazineAppContext
{
    public static class Data
    {

        public static string PackData(string sPackage, string sLabel, object oData, bool bNewLine = false)
        {
            char FieldSeparator = Convert.ToChar(20);
            string sData = oData.ToString();

            if (bNewLine)
            {
                char LineTerminator = Convert.ToChar(21);
                sPackage = sPackage + LineTerminator;
            }

            return sPackage + "[" + sLabel + "]" + sData + FieldSeparator;
        }

        public static string PackDate(string sPackage, string sLabel, DateTime oDate, bool bNewLine = false)
        {
            char FieldSeparator = Convert.ToChar(20);
            long lDate = oDate.Ticks;
            string sData = lDate.ToString();

            if (bNewLine)
            {
                char LineTerminator = Convert.ToChar(21);
                sPackage = sPackage + LineTerminator;
            }

            return sPackage + "[" + sLabel + "]" + sData + FieldSeparator;
        }

        public static object GetData(string Data, Type LabelType, string LabelName)
        {
            //extract the value of correspondent label
            string field = GetField(Data, LabelName);

            //determin the final type of value and output it in correct type
            switch (Type.GetTypeCode(LabelType))
            {
                case TypeCode.String:
                    string strResult = "N/A";
                    if (field != "")
                    {
                        strResult = field;
                    }
                    return strResult;
                case TypeCode.Int32:
                        int intResult=-1;
                    Int32.TryParse(field, out intResult);
                    return intResult;
                case TypeCode.Boolean:
                    bool boolResult=false;
                    Boolean.TryParse(field, out boolResult);
                    return boolResult;
                case TypeCode.DateTime:
                    long longResult=0;
                    long.TryParse(field, out longResult);
                    var datetime = new DateTime(longResult);
                    return datetime;
                case TypeCode.Single:
                    Single singleResult;
                    Single.TryParse(field, out singleResult);
                    return singleResult;
                default:
                    return "Try get data failed";

            }
            //extract value with a given label
            string GetField(string data, string label)
            {
                int iField, iSplit, iStart, iLength;
                char FieldSeparator = Convert.ToChar(20);

                // Extract the data
                label = "[" + label + "]";
                //Debug.Print("GetField: Label = " + Label);

                iField = data.IndexOf(label);
                //Debug.Print("GetField: iField = " + iField.ToString());

                //if field(index of a label) exsist
                if (iField > -1)
                {
                    //return the length, from start of label to fieldSeperator
                    iSplit = data.IndexOf(FieldSeparator, iField);
                    //if there is one 
                    if (iSplit > iField)
                    {
                        //then start from right after label
                        iStart = iField + label.Length;
                        //length equals to total length minus start of the content
                        iLength = iSplit - iStart;
                        //return that string
                        return data.Substring(iStart, iLength);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }

        }

        public static string CreateStandardQuery(string sQuery)
        {
            string sData = "";

            sData = PackData(sData, "DBIndex", "1");
            sData = PackData(sData, "Module", "000");
            sData = PackData(sData, "Function", "GetQuery");
            sData = PackData(sData, "Query", sQuery);

            return AddDataTerminator(sData);
        }

        public static string CreatePagedQuery(string sQuery, int PageNumber, int ResultsPerPage)
        {
            string sData = "";

            sData = PackData(sData, "DBIndex", "1");
            sData = PackData(sData, "Module", "000");
            sData = PackData(sData, "Function", "GetQueryByPage");
            sData = PackData(sData, "PageNumber", PageNumber.ToString());
            sData = PackData(sData, "ResultsPerPage", ResultsPerPage.ToString());
            sData = PackData(sData, "Query", sQuery);

            return AddDataTerminator(sData);
        }



        public static string CreateSaveQuery(string sQuery, string sData)
        {
            sData = PackData(sData, "DBIndex", "1");
            sData = PackData(sData, "Module", "000");
            sData = PackData(sData, "Function", "SaveRecord");
            sData = PackData(sData, "Query", sQuery);

            return AddDataTerminator(sData);

        }

        public static string CreateDeleteQuery(string sQuery)
        {
            string sData;

            sData = PackData("", "DBIndex", "1");
            sData = PackData(sData, "Module", "000");
            sData = PackData(sData, "Function", "DeleteRecord");
            sData = PackData(sData, "Query", sQuery);

            return AddDataTerminator(sData);

        }

        public static string CreateRecordCountQuery(string sQuery)
        {
            string sData = "";

            sData = PackData("", "DBIndex", "1");
            sData = PackData(sData, "Module", "000");
            sData = PackData(sData, "Function", "GetRecordCount");
            sData = PackData(sData, "Query", sQuery);

            return AddDataTerminator(sData);
        }

        public static string AddDataTerminator(string sData)
        {
            char DataTerminator = Convert.ToChar(4);
            return sData + DataTerminator;
        }

    }
}
