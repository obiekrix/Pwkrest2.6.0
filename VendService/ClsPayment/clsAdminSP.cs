using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace pawakadApp.Cls
{
    public class clsAdminSP
    {
        public static clsDataAccess dataConn = new clsDataAccess();

        public static DataTable getTransactionQuery(string startdate, string enddate, string transID, int includeNullresp)
        {
            DataTable dt = new DataTable();
            SqlCommand dc = new SqlCommand();
            DataSet ds = new DataSet();

            

            dc.Connection = dataConn.Connection;
            dc.CommandType = CommandType.StoredProcedure;
            dc.CommandText = "spGetTransactionQuery";
            dc.Parameters.Add(new SqlParameter("@StartDate", System.Data.SqlDbType.VarChar)).Value = startdate;
            dc.Parameters.Add(new SqlParameter("@EndDate", System.Data.SqlDbType.VarChar)).Value = enddate;
            dc.Parameters.Add(new SqlParameter("@TransactionID", System.Data.SqlDbType.VarChar)).Value = transID;
            dc.Parameters.Add(new SqlParameter("@IncludeNonNullResponse", System.Data.SqlDbType.Int)).Value = includeNullresp;          

            SqlDataAdapter da = new SqlDataAdapter(dc);
            da.Fill(ds);

            dt = ds.Tables[0];
            ds.Dispose();
            dc.Dispose();
            dataConn.CloseConnection();
            dataConn.Dispose();
            
            return dt;
        }
    }
}