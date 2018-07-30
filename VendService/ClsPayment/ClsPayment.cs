using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace pawakadApp.Cls
{
    public class ClsPayment
    {
        public string SetPinUsedAfterPay(string pinList)
        {
            string[] arrPinList = pinList.Split(',');

            foreach (string pinItem in arrPinList)
            {
            //System.IO.File.AppendAllText(@"C:\Users\Public\test.txt", pinItem + "----" + arrPinList.Length);
               // using (System.IO.StreamWriter sw = System.IO.File.AppendText(@"C:\Users\Public\test.txt"))
               // {
                    //sw.WriteLine(pinItem + "----" + arrPinList.Length);
               // }

                MarkPinAsUsed(pinItem); 
            }
            
            return "Pins have been  marked as used";
        }

        public int ValidateAllPins(string pinList)
        {
            // (0 = Invalid, 1 = Valid)
            int isPinValid = 1;
            string[] arrPinList = pinList.Split(',');
            string pinItemMD5;
            foreach (string pinItem in arrPinList)
            {
                pinItemMD5 = Crypt.MD5Crypt.MDee5(pinItem);
                int validityStatus = CheckSinglePinValidity(pinItemMD5);

                //System.IO.StreamWriter sw = System.IO.File.AppendText(@"C:\Users\Public\debuger.txt");
                //sw.WriteLine(pinList + "---------pinlist");
                //sw.WriteLine(pinItem + "---------all");
                //sw.Close();

                if(validityStatus==0)
                {
                    isPinValid = 0;
                }
            }
            return isPinValid;
        }

        public int CheckAll4Deativation(string pinList)
        {
            // (2 = Invalid, 1 = Valid)
            int isPinValid = 1;
            string[] arrPinList = pinList.Split(',');
            string pinItemMD5;
            foreach (string pinItem in arrPinList)
            {
                pinItemMD5 = Crypt.MD5Crypt.MDee5(pinItem);
                int validityStatus = CheckSinglePinDeactivation(pinItemMD5);
                if (validityStatus == 2)
                {
                    isPinValid = 2;
                }
            }
            return isPinValid;
        }



        public int CheckSinglePinValidity(string pin)
        {

            //System.IO.StreamWriter sw2 = System.IO.File.AppendText(@"C:\Users\Public\debuger.txt");
            //sw2.WriteLine(pin + "---------single");
            //sw2.Close();

            int validityStatus = 0;                
            DataAccess da = new DataAccess();
            try
            {
                da.AddParameter("@pin", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, pin);
                validityStatus = (int)da.ExecuteScalar("UP_IsValidPin", System.Data.CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            finally
            {
                da.Dispose();
            }
            return validityStatus;
        }

        public int CheckSinglePinDeactivation(string pin)
        {
            int validityStatus=0;                
            DataAccess da = new DataAccess();

            try
            {
                da.AddParameter("@pin", System.Data.SqlDbType.BigInt, System.Data.ParameterDirection.Input, pin);
                validityStatus = (int)da.ExecuteScalar("UP_IsPinDeactivated", System.Data.CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            finally
            {
                da.Dispose();
            }
            return validityStatus;
        }

        internal SqlDataReader GetPaymentLog(string accNo, string receiptNo, string startDate, string endDate)
        {
            Cls.DataAccess da = new Cls.DataAccess();

            da.AddParameter("@accNo", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, accNo);
            da.AddParameter("@receiptNo", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, receiptNo);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);

            SqlDataReader dr = da.ExecuteReader("UP_GetPaymentLog", CommandType.StoredProcedure);

            return dr;
        }

        internal DataTable GetFaq()
        {
            Cls.DataAccess da = new Cls.DataAccess();

            DataTable dt = new DataTable();
            SqlDataReader dr = da.ExecuteReader("UP_GetFaq", CommandType.StoredProcedure);

            dt.Load(dr);

            dr.Close();

            return dt;
        }

        internal DataTable GetAnsweredFaq(string id)
        {
            Cls.DataAccess da = new Cls.DataAccess();

            DataTable dt = new DataTable();
            da.AddParameter("@id", System.Data.SqlDbType.Int, System.Data.ParameterDirection.Input, Int16.Parse(id));
            SqlDataReader dr = da.ExecuteReader("UP_GetAnsweredFaq", CommandType.StoredProcedure);

            dt.Load(dr);

            dr.Close();

            return dt;
        }

        internal DataTable GetMsgs()
        {
            Cls.DataAccess da = new Cls.DataAccess();

            DataTable dt = new DataTable();
            SqlDataReader dr = da.ExecuteReader("UPM_GetMsgs", CommandType.StoredProcedure);

            dt.Load(dr);

            dr.Close();

            return dt;
        }

        internal DataTable GetMsg(string id)
        {
            Cls.DataAccess da = new Cls.DataAccess();

            DataTable dt = new DataTable();
            da.AddParameter("@id", System.Data.SqlDbType.Int, System.Data.ParameterDirection.Input, Int16.Parse(id));
            SqlDataReader dr = da.ExecuteReader("UPM_GetMsg", CommandType.StoredProcedure);

            dt.Load(dr);

            dr.Close();

            return dt;
        }

        public int MarkPinAsUsed(string pin)
        {
            DataAccess da = new DataAccess();

            try
            {
                da.AddParameter("@pin", System.Data.SqlDbType.BigInt, System.Data.ParameterDirection.Input, pin);
                da.ExecuteNonQuery("UP_SetPinAsUsed", System.Data.CommandType.StoredProcedure);
            }

            catch(Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                da.Dispose();
            }
            return 1;
        }

        public int ProcessMultiPin(string pinList)
        {
            // split the string list to create an array
            string[] arrPinList = pinList.Split(',');

            // initialize an accumulator
            int totalPinValue = 0;

            // create a hashset to check for duplicates
            HashSet<string> htPinItem = new HashSet<string>();
            string pinItemMD5;
            // loop through the array
            foreach (string pinItem in arrPinList)
            {
                if(!htPinItem.Contains(pinItem))
                {
                    htPinItem.Add(pinItem);
                    // lookup each pin value and add to a total ////pinItemMD5 = Crypt.MD5Crypt.MDee5(pinItem);
                    pinItemMD5 = Crypt.MD5Crypt.MDee5(pinItem);
                    totalPinValue += GetPinValue(pinItemMD5); 
                }
                
                               
            }
            return totalPinValue;

        }

        //public string RemoveDuplicates(string pinList)
        //{
        //    // split the string list to create an array
        //    string[] arrPinList = pinList.Split(',');
        //    HashSet<string> htPinItem = new HashSet<string>();

        //    string strPinList = "";

        //    // loop through the array
        //    foreach (string pinItem in arrPinList)
        //    {
        //        if (!htPinItem.Contains(pinItem))
        //        {
        //            htPinItem.Add(pinItem);
        //            strPinList += pinItem+",";
        //        }
        //    }
        //    return strPinList;
        //}


        public int checkForDuplicatePin(string pinList)
        {
            // split the string list to create an array
            string[] arrPinList = pinList.Split(',');
            HashSet<string> htPinItem = new HashSet<string>();

            // (0 = Not duplicated, 1 = Duplicate pins exist)
            int pinDuplicate = 0;
            foreach (string pinItem in arrPinList)
            {
                
                if (htPinItem.Contains(pinItem))
                {
                    pinDuplicate = 1;
                }
                htPinItem.Add(pinItem);
            }
            return pinDuplicate;
        }

        public string RemoveDuplicates(string pinList)
        {
            // split the string list to create an array
            string[] arrPinList = pinList.Split(',');
            HashSet<string> htPinItem = new HashSet<string>();
            string msg="";
            foreach(string pinItem in arrPinList)
            {
                if (!htPinItem.Contains(pinItem))
                {
                    htPinItem.Add(pinItem);
                }
                else
                {
                    msg = "One or more of the pins you entered are duplicates";
                }
            }


            string strPinList = string.Join(",", htPinItem);
            if (msg.Length > 1)
            {
                return strPinList + "|" + msg;
            }
            else
            {
                return strPinList;
            }
            
        }

        public int GetPinValue(string pin)
        {
           /* System.IO.TextWriter txtwrt = new System.IO.StreamWriter(@"C:\Users\Deji\Documents\WriteText.txt", true);
            txtwrt.WriteLine(Convert.ToInt64(pin));
            txtwrt.Close();*/

            DataAccess da = new DataAccess();
            da.AddParameter("@pin", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, pin);
            int cardAmount = Convert.ToInt32(da.ExecuteScalar("UP_GetPinValue", System.Data.CommandType.StoredProcedure));
            return cardAmount;
        }

        public static int LogAggData(string customerName, string totalAmount, string customerAccountNumber, string transactionTimeStamp, string channelReferenceNumber, string receiptNumber, string code)
        {
            //ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

           // Cls.JSONConnect jc = new Cls.JSONConnect();
            //dynamic jsn = jc.SubmitPayment("705909094001", "SCR140111655dd014235hdK673dgd9052139091001", "100");

            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@customerName", SqlDbType.VarChar, ParameterDirection.Input, customerName);
            da.AddParameter("@totalAmount", SqlDbType.VarChar, ParameterDirection.Input, totalAmount);
            da.AddParameter("@customerAccountNumber", SqlDbType.VarChar, ParameterDirection.Input, customerAccountNumber);
            da.AddParameter("@transactionTimeStamp", SqlDbType.VarChar, ParameterDirection.Input, transactionTimeStamp);
            da.AddParameter("@channelReferenceNumber", SqlDbType.VarChar, ParameterDirection.Input, channelReferenceNumber);
            da.AddParameter("@receiptNumber", SqlDbType.VarChar, ParameterDirection.Input, receiptNumber);
            da.AddParameter("@transactionCode", SqlDbType.VarChar, ParameterDirection.Input, code);
            int num = da.ExecuteNonQuery("UP_LogAggSuccess", CommandType.StoredProcedure);

            return num;
        }


        internal DataTable GetPaymentLog2(string channelRefNo)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@channelRefNo", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, channelRefNo);
            SqlDataReader dr = da.ExecuteReader("UPR_GetPaymentLog", CommandType.StoredProcedure);
            DataTable dt = new DataTable();
            dt.Load(dr);

            dr.Close();

            return dt;
        }

        internal static int LogMobileRequest(string channelRefNo, DateTime reqDate, string meterNo, string amt)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@channelRefNo", SqlDbType.VarChar, ParameterDirection.Input, channelRefNo);
            da.AddParameter("@reqDate", SqlDbType.DateTime, ParameterDirection.Input, reqDate);
            da.AddParameter("@meterNo", SqlDbType.VarChar, ParameterDirection.Input, meterNo);
            da.AddParameter("@amt", SqlDbType.Decimal, ParameterDirection.Input, amt);

            int num = da.ExecuteNonQuery("UPM_LogMobileRequest", CommandType.StoredProcedure);

            return num;
        }

        internal static int LogMobileRequest2(string dCode, DateTime reqDate, /*string meterNo, string amt,*/ string mtid, string action, string success, string channelRefNo="")
        {
            //Cls.DataAccess da = new Cls.DataAccess();
            //da.AddParameter("@dCode", SqlDbType.VarChar, ParameterDirection.Input, dCode);
            //da.AddParameter("@reqDate", SqlDbType.DateTime, ParameterDirection.Input, reqDate);
            ////da.AddParameter("@meterNo", SqlDbType.VarChar, ParameterDirection.Input, meterNo);
            ////da.AddParameter("@amt", SqlDbType.Decimal, ParameterDirection.Input, amt);
            //da.AddParameter("@mtid", SqlDbType.VarChar, ParameterDirection.Input, mtid);
            //da.AddParameter("@action", SqlDbType.VarChar, ParameterDirection.Input, action);
            //da.AddParameter("@outResponseMsg", SqlDbType.VarChar, ParameterDirection.Input, success);
            //da.AddParameter("@channelRefNo", SqlDbType.VarChar, ParameterDirection.Input, channelRefNo);

            //int num = da.ExecuteNonQuery("UPM_LogMobileRequest2", CommandType.StoredProcedure);

            //return num;

            Cls.clsDataAccess da = new Cls.clsDataAccess();
            SqlCommand sqlcmd = new SqlCommand();

            try
            {
                sqlcmd.Connection = da.GetConnection();
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.CommandText = "UPM_LogMobileRequest2";

                sqlcmd.Parameters.Add("@dCode", SqlDbType.VarChar).Value = dCode;
                sqlcmd.Parameters.Add("@reqDate", SqlDbType.DateTime).Value = reqDate;
                sqlcmd.Parameters.Add("@mtid", SqlDbType.VarChar).Value = mtid;
                sqlcmd.Parameters.Add("@action", SqlDbType.VarChar).Value = action;
                sqlcmd.Parameters.Add("@outResponseMsg", SqlDbType.VarChar).Value = success;
                sqlcmd.Parameters.Add("@channelRefNo", SqlDbType.VarChar).Value = channelRefNo;

                int num = sqlcmd.ExecuteNonQuery();

                return num;
            }
            catch (Exception e)
            {
                return 0;
            }
            finally
            {
                sqlcmd.Dispose();
                da.Dispose();
            }

        }

        internal static void RegenAnyUGR(string acctNo, string dealerUserName, DateTime dateTime)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@accountNo", SqlDbType.VarChar, ParameterDirection.Input, acctNo);
            da.AddParameter("@txnDate", SqlDbType.DateTime, ParameterDirection.Input, dateTime);
            da.AddParameter("@dealerUserName", SqlDbType.VarChar, ParameterDirection.Input, dealerUserName);

            int num = da.ExecuteNonQuery("spGenUGR2", CommandType.StoredProcedure);

        }
    }
}