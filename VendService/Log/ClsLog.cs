using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;


namespace pawakadApp.Log
{
    public class ClsLog
    {

        public void LogPaymentError(string accountNumber, string channelRefNo, int totalPinAmount, string aggErrorMsg, string operation, DateTime transactionDate/*, string dCode, DateTime transactionDate, bool refunded*/)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@accountNumber", SqlDbType.VarChar, ParameterDirection.Input, accountNumber);
            da.AddParameter("@channelRefNo", SqlDbType.VarChar, ParameterDirection.Input, channelRefNo);
            da.AddParameter("@totalPinAmount", SqlDbType.Int, ParameterDirection.Input, totalPinAmount);
            da.AddParameter("@aggErrorMsg", SqlDbType.VarChar, ParameterDirection.Input, aggErrorMsg);
            da.AddParameter("@operation", SqlDbType.VarChar, ParameterDirection.Input, operation);
            //da.AddParameter("@dCode", SqlDbType.VarChar, ParameterDirection.Input, dCode);
            da.AddParameter("@transactionDate", SqlDbType.DateTime, ParameterDirection.Input, transactionDate);
            //da.AddParameter("@refunded", SqlDbType.VarChar, ParameterDirection.Input, refunded.ToString());
            da.ExecuteNonQuery("UP_InsertPaymentErrorLog", CommandType.StoredProcedure);
        }

    }
}