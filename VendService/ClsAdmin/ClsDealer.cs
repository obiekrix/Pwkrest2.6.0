using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;


namespace pawakadApp.ClsAdmin
{
    public class ClsDealer
    {
        public int IsDealerLoginValid(string userName, string passWord)
        {
            //System.IO.TextWriter txtwrt = new System.IO.StreamWriter(@"C:\Users\Deji\Documents\WriteText.txt",true);
            //txtwrt.WriteLine(userName + "  , " + passWord);
            //txtwrt.Close();
            //System.IO.File.AppendText
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@passWord", SqlDbType.VarChar, ParameterDirection.Input, passWord);
            int isValidLogin = (int)da.ExecuteScalar("UP_LoginDealer", CommandType.StoredProcedure);

            return isValidLogin;
        }

        public int IsReporterLoginValid(string userName, string passWord)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@passWord", SqlDbType.VarChar, ParameterDirection.Input, passWord);
            int isValidLogin = (int)da.ExecuteScalar("UPR_LoginReporter", CommandType.StoredProcedure);

            return isValidLogin;
        }

        public int CheckMultiLogin(string userName)
        {

            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            int isDuplicateLogin = (int)da.ExecuteScalar("UP_CheckMultiLogin", CommandType.StoredProcedure);

            return isDuplicateLogin;
        }

        public void RemoveMultiLoginByUserName(string userName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.ExecuteScalar("UP_RemoveMultiLoginByUserName", CommandType.StoredProcedure);
        }


        public void UpdateNoMultiLoginDateTime(string userName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.ExecuteScalar("UP_UpdateNoMultiLoginDateTime", CommandType.StoredProcedure);
        }

        public void AddMultiLogin(string userName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.ExecuteScalar("UP_SaveLogin", CommandType.StoredProcedure);
        }

        public void AddLoginLocation(string userName, string longitude, string latitude, string address, string mtid)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@longitude", SqlDbType.VarChar, ParameterDirection.Input, longitude);
            da.AddParameter("@latitude", SqlDbType.VarChar, ParameterDirection.Input, latitude);
            da.AddParameter("@address", SqlDbType.VarChar, ParameterDirection.Input, address);
            da.AddParameter("@mtid", SqlDbType.VarChar, ParameterDirection.Input, mtid);
            da.ExecuteScalar("UPM_SaveLoginLocation", CommandType.StoredProcedure);
        }


        public int ChangeDealerPassword(string userName, string passWord)
        {
            Cls.DataAccess da = new Cls.DataAccess();

            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@passWord", SqlDbType.VarChar, ParameterDirection.Input, passWord);
            int status = (int)da.ExecuteNonQuery("UP_ChangeDealerPassword", CommandType.StoredProcedure);

            return status;
        }

        public bool DealerEmailIsCorrect(string userName, string email)
        {
            Cls.DataAccess da = new Cls.DataAccess();

            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@email", SqlDbType.VarChar, ParameterDirection.Input, email);
            int status = (int)da.ExecuteScalar("UP_VerifyDealerEmail", CommandType.StoredProcedure);
      
            return status == 1;
        }

        //Christian: 25/04/14
        public int ChangeReporterPassword(string userName, string passWord)
        {
            Cls.DataAccess da = new Cls.DataAccess();

            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@passWord", SqlDbType.VarChar, ParameterDirection.Input, passWord);
            int status = (int)da.ExecuteNonQuery("UPR_ChangeReporterPassword", CommandType.StoredProcedure);

            return status;
        }

        internal SqlDataReader GetPaymentModes()
        {
            Cls.DataAccess da = new Cls.DataAccess();
            SqlDataReader dr = da.ExecuteReader("UP_getPaymentMode", CommandType.StoredProcedure);
            return dr;
        }


        public int CheckDealerCredit(string userName, Int32 payMentAmount)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@payMentAmount", SqlDbType.BigInt, ParameterDirection.Input, payMentAmount);
            int isCreditEnough = (int)da.ExecuteScalar("UP_CheckDealerCredit", CommandType.StoredProcedure);

            return isCreditEnough;
        }

        internal string GetNGN100Epins(int numberOfEpinsWanted, string userName)
        {
            string finalResult = "";
            Cls.DataAccess da = new Cls.DataAccess();
            SqlDataReader dr;
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@numberOfEpinsWanted", SqlDbType.Int, ParameterDirection.Input, numberOfEpinsWanted);
            dr = da.ExecuteReader("UP_GetNGN100Epins2", CommandType.StoredProcedure);
            HashSet<string> htPinItem = new HashSet<string>();
            while (dr.Read())
            {
                htPinItem.Add(dr[0].ToString());

            }
            finalResult = string.Join(",", htPinItem);
            dr.Close();
            return finalResult;
        }

        public void DeductDealerAvailableCredit(string userName, decimal amountToDeduct)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@amountToDeduct", SqlDbType.Decimal, ParameterDirection.Input, amountToDeduct);
            da.ExecuteScalar("UP_DeductDealerAvailableCredit", CommandType.StoredProcedure);
        }


        //Christian: method to deduct dealer's available EPIN as requested by Yinka
        public void DeductDealerAvailableEPIN(string userName, decimal amountToDeduct)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@amountToDeduct", SqlDbType.Decimal, ParameterDirection.Input, amountToDeduct);
            da.ExecuteScalar("UP_DeductDealerAvailableEPIN", CommandType.StoredProcedure);
        }

        //Christian: method to calculate dealer's available EPIN as requested by Yinka
        public void CalculateDealerAvailableEPIN(string userName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.ExecuteScalar("UP_CalculateDealerAvailableEPIN", CommandType.StoredProcedure);
        }

        // update card table with the account number of the customer who used the card(s) and mark the card(s) as used
        public int AddAccountNumberAndMakCardAsUsed(string cardPins, string accountNumber)
        {
            string[] arrCardpins = cardPins.Split(',');
            var arrCardpinsMD5 = arrCardpins.Select(element => Crypt.MD5Crypt.MDee5(element));
            //arrCardpins.
            string pinCommaSep = "'" + string.Join("','", arrCardpinsMD5) + "'";

            //System.IO.TextWriter txtwrt = new System.IO.StreamWriter(@"C:\Users\Deji\Documents\WriteText.txt", true);
            //txtwrt.WriteLine("update CardTable set customerAccountNumber ='" + accountNumber + "' ,  cardUsedStatus ='U' where cast(cardPin as varchar(50)) in(" + pinCommaSep + ")" + "\n");
            //txtwrt.WriteLine(pinCommaSep + "\n");
            //txtwrt.Close();

            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@cardPins", SqlDbType.VarChar, ParameterDirection.Input, cardPins.Trim());
            //int dr = da.ExecuteNonQuery("update CardTable set customerAccountNumber ='" + accountNumber + "' ,  cardUsedStatus ='U' where cast(cardPin as varchar(50)) in(" + pinCommaSep + ") and cardUsedStatus <>'U'", CommandType.Text);
            /*Christian: Added the preceeding extra 'AND' condition to avoid card reuseal and multiple receipt transaction */
            return 1; // dr;
        }

        public int AddAccountNumberAndMakCardAsUsedEpin100(string cardPins, string accountNumber)
        {
            string[] arrCardpins = cardPins.Split(',');
            //var arrCardpinsMD5 = arrCardpins.Select(element => Crypt.MD5Crypt.MDee5(element));
            //arrCardpins.
            string pinCommaSep = "'" + string.Join("','", arrCardpins) + "'";

            //System.IO.TextWriter txtwrt = new System.IO.StreamWriter(@"C:\Users\Deji\Documents\WriteText.txt", true);
            //txtwrt.WriteLine("update CardTable set customerAccountNumber ='" + accountNumber + "' ,  cardUsedStatus ='U' where cast(cardPin as varchar(50)) in(" + pinCommaSep + ")" + "\n");
            //txtwrt.WriteLine(pinCommaSep + "\n\n\n");
            //txtwrt.Close();

            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@cardPins", SqlDbType.VarChar, ParameterDirection.Input, cardPins.Trim());
            int dr = da.ExecuteNonQuery("update CardTable set customerAccountNumber ='" + accountNumber + "' ,  cardUsedStatus ='U' where cast(cardPin as varchar(50)) in(" + pinCommaSep + ")", CommandType.Text);
            return dr;
        }

        // Log payment to payment log  table
        public int ApplyPayment(string cardPins, string accountNumber, string meterNumber, int amount, string channelRefNo, /*christian:9/4/14 added the last param*/ DateTime transactionDate)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@cardPins", SqlDbType.VarChar, ParameterDirection.Input, cardPins.Trim());
            da.AddParameter("@accountNumber", SqlDbType.VarChar, ParameterDirection.Input, accountNumber);
            da.AddParameter("@meterNumber", SqlDbType.VarChar, ParameterDirection.Input, meterNumber);
            da.AddParameter("@amount", SqlDbType.Int, ParameterDirection.Input, amount);
            da.AddParameter("@channelRefNo", SqlDbType.VarChar, ParameterDirection.Input, channelRefNo);
            da.AddParameter("@transactionDate", SqlDbType.DateTime, ParameterDirection.Input, transactionDate);//christian: 9/4/14
            int num = da.ExecuteNonQuery("UP_ApplyPayment", CommandType.StoredProcedure);

            return num;
        }

        // Log payment to payment log  table
        public int ApplyPayment2(string accountType, string dtName, string dtNumber, string orgName, string orgNo, string phoneNo, string reprint, string accountNumber, string meterNumber, int amount, string channelRefNo, DateTime transactionDate,
            string token, string sgc, string tariff, string custName, string address, decimal cou, decimal fixedCharge, decimal vat,
            decimal units, decimal rate, decimal balance, string receipt, string uniqueTId, string collectionType = "NRG")
        {
            //Cls.DataAccess da = new Cls.DataAccess();
            ////da.AddParameter("@businessUnit", SqlDbType.VarChar, ParameterDirection.Input, businessUnit.ToString());
            //da.AddParameter("@reprint", SqlDbType.VarChar, ParameterDirection.Input, reprint.ToString());
            //da.AddParameter("@accountNumber", SqlDbType.VarChar, ParameterDirection.Input, accountNumber);
            //da.AddParameter("@meterNumber", SqlDbType.VarChar, ParameterDirection.Input, meterNumber);
            //da.AddParameter("@amount", SqlDbType.Int, ParameterDirection.Input, amount);
            //da.AddParameter("@channelRefNo", SqlDbType.VarChar, ParameterDirection.Input, channelRefNo);
            //da.AddParameter("@transactionDate", SqlDbType.DateTime, ParameterDirection.Input, transactionDate);//christian: 9/4/14
            //da.AddParameter("@token", SqlDbType.VarChar, ParameterDirection.Input, token);//christian: 9/4/14
            //da.AddParameter("@uniqueTId", SqlDbType.VarChar, ParameterDirection.Input, uniqueTId);//christian: 9/4/14


            //da.AddParameter("@sgc", SqlDbType.VarChar, ParameterDirection.Input, sgc.ToString());
            //da.AddParameter("@tariff", SqlDbType.VarChar, ParameterDirection.Input, tariff.ToString());
            //da.AddParameter("@custName", SqlDbType.VarChar, ParameterDirection.Input, custName.ToString());
            //da.AddParameter("@address", SqlDbType.VarChar, ParameterDirection.Input, address.ToString());
            //da.AddParameter("@cou", SqlDbType.Decimal, ParameterDirection.Input, cou.ToString());
            //da.AddParameter("@fixedCharge", SqlDbType.Decimal, ParameterDirection.Input, fixedCharge.ToString());
            //da.AddParameter("@vat", SqlDbType.Decimal, ParameterDirection.Input, vat.ToString());
            //da.AddParameter("@units", SqlDbType.Decimal, ParameterDirection.Input, units.ToString());
            //da.AddParameter("@rate", SqlDbType.Decimal, ParameterDirection.Input, rate.ToString());
            //da.AddParameter("@balance", SqlDbType.Decimal, ParameterDirection.Input, balance.ToString());
            //da.AddParameter("@receiptNo", SqlDbType.VarChar, ParameterDirection.Input, receipt.ToString());
            //da.AddParameter("@collectionType", SqlDbType.VarChar, ParameterDirection.Input, collectionType.ToString());
            //int num = da.ExecuteNonQuery("UP_ApplyPayment", CommandType.StoredProcedure);

            //return num;

            Cls.clsDataAccess da = new Cls.clsDataAccess();
            SqlCommand cmd = new SqlCommand();

            cmd.Parameters.Add("@phoneNo", SqlDbType.VarChar).Value = phoneNo.ToString();
            cmd.Parameters.Add("@reprint", SqlDbType.VarChar).Value = reprint.ToString();
            cmd.Parameters.Add("@accountNumber", SqlDbType.VarChar).Value = accountNumber;
            cmd.Parameters.Add("@meterNumber", SqlDbType.VarChar).Value = meterNumber;
            cmd.Parameters.Add("@amount", SqlDbType.Int).Value = amount;
            cmd.Parameters.Add("@channelRefNo", SqlDbType.VarChar).Value = channelRefNo;
            cmd.Parameters.Add("@transactionDate", SqlDbType.DateTime).Value = transactionDate;//christian: 9/4/14
            cmd.Parameters.Add("@token", SqlDbType.VarChar).Value = token;//christian: 9/4/14
            cmd.Parameters.Add("@uniqueTId", SqlDbType.VarChar).Value = uniqueTId;//christian: 9/4/14

            cmd.Parameters.Add("@sgc", SqlDbType.VarChar).Value = sgc.ToString();
            cmd.Parameters.Add("@tariff", SqlDbType.VarChar).Value = tariff.ToString();
            cmd.Parameters.Add("@custName", SqlDbType.VarChar).Value = custName.ToString();
            cmd.Parameters.Add("@address", SqlDbType.VarChar).Value = address.ToString();
            cmd.Parameters.Add("@cou", SqlDbType.Decimal).Value = cou.ToString();
            cmd.Parameters.Add("@fixedCharge", SqlDbType.Decimal).Value = fixedCharge.ToString();
            cmd.Parameters.Add("@vat", SqlDbType.Decimal).Value = vat.ToString();
            cmd.Parameters.Add("@units", SqlDbType.Decimal).Value = units.ToString();
            cmd.Parameters.Add("@rate", SqlDbType.Decimal).Value = rate.ToString();
            cmd.Parameters.Add("@balance", SqlDbType.Decimal).Value = balance.ToString();
            cmd.Parameters.Add("@receiptNo", SqlDbType.VarChar).Value = receipt.ToString();
            cmd.Parameters.Add("@collectionType", SqlDbType.VarChar).Value = collectionType.ToString();
            cmd.Parameters.Add("@accountType", SqlDbType.VarChar).Value = accountType.ToString();
            cmd.Parameters.Add("@dtName", SqlDbType.VarChar).Value = dtName.ToString();
            cmd.Parameters.Add("@dtNumber", SqlDbType.VarChar).Value = dtNumber.ToString();
            cmd.Parameters.Add("@orgName", SqlDbType.VarChar).Value = orgName.ToString();
            cmd.Parameters.Add("@orgNo", SqlDbType.VarChar).Value = orgNo.ToString();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = da.GetConnection();
            cmd.CommandText = "UP_ApplyPayment";

            int num = cmd.ExecuteNonQuery();

            da.CloseConnection(); da.Dispose();
            cmd = null;

            return num;
        }

        // Log payment to payment log  table
        public int RegeneratePayment(string accountNumber, string meterNumber, int amount, string channelRefNo,
            string token, string sgc, string tariff, string custName, string address, decimal cou, decimal fixedCharge, decimal vat,
            decimal units, decimal rate, decimal balance, string receipt, string uniqueTId, string collectionType = "NRG")
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@accountNumber", SqlDbType.VarChar, ParameterDirection.Input, accountNumber);
            da.AddParameter("@meterNumber", SqlDbType.VarChar, ParameterDirection.Input, meterNumber);
            da.AddParameter("@amount", SqlDbType.Int, ParameterDirection.Input, amount);
            da.AddParameter("@channelRefNo", SqlDbType.VarChar, ParameterDirection.Input, channelRefNo);
            da.AddParameter("@transactionDate", SqlDbType.DateTime, ParameterDirection.Input, DateTime.Now);//christian: 9/4/14
            da.AddParameter("@token", SqlDbType.VarChar, ParameterDirection.Input, token);//christian: 9/4/14
            da.AddParameter("@uniqueTId", SqlDbType.VarChar, ParameterDirection.Input, uniqueTId);//christian: 9/4/14


            da.AddParameter("@sgc", SqlDbType.VarChar, ParameterDirection.Input, sgc.ToString());
            da.AddParameter("@tariff", SqlDbType.VarChar, ParameterDirection.Input, tariff.ToString());
            da.AddParameter("@custName", SqlDbType.VarChar, ParameterDirection.Input, custName.ToString());
            da.AddParameter("@address", SqlDbType.VarChar, ParameterDirection.Input, address.ToString());
            da.AddParameter("@cou", SqlDbType.Decimal, ParameterDirection.Input, cou.ToString());
            da.AddParameter("@fixedCharge", SqlDbType.Decimal, ParameterDirection.Input, fixedCharge.ToString());
            da.AddParameter("@vat", SqlDbType.Decimal, ParameterDirection.Input, vat.ToString());
            da.AddParameter("@units", SqlDbType.Decimal, ParameterDirection.Input, units.ToString());
            da.AddParameter("@rate", SqlDbType.Decimal, ParameterDirection.Input, rate.ToString());
            da.AddParameter("@balance", SqlDbType.Decimal, ParameterDirection.Input, balance.ToString());
            da.AddParameter("@receiptNo", SqlDbType.VarChar, ParameterDirection.Input, receipt.ToString());
            da.AddParameter("@collectionType", SqlDbType.VarChar, ParameterDirection.Input, collectionType.ToString());
            int num = da.ExecuteNonQuery("UP_RegeneratePayment", CommandType.StoredProcedure);

            return num;
        }

        public int SaveDealerPayment(string dealer, decimal amount, DateTime paymentDate, string paymentMode, string enteredBy, string paymentRef, string remarks)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@dealer", SqlDbType.VarChar, ParameterDirection.Input, dealer);
            da.AddParameter("@amount", SqlDbType.Decimal, ParameterDirection.Input, amount);
            da.AddParameter("@paymentDate", SqlDbType.DateTime, ParameterDirection.Input, paymentDate);
            da.AddParameter("@paymentMode", SqlDbType.VarChar, ParameterDirection.Input, paymentMode);
            da.AddParameter("@enteredBy", SqlDbType.VarChar, ParameterDirection.Input, enteredBy);
            da.AddParameter("@paymentRef", SqlDbType.VarChar, ParameterDirection.Input, paymentRef);
            da.AddParameter("@remarks", SqlDbType.VarChar, ParameterDirection.Input, remarks);
            int num = da.ExecuteNonQuery("[UPR_DealerPaymentInsert]", CommandType.StoredProcedure);
            return num;
        }

        public int GetPinValue(string pin)
        {
            // System.IO.TextWriter txtwrt2 = new System.IO.StreamWriter(@"C:\Users\Deji\Documents\WriteText.txt", true);
            //txtwrt2.WriteLine(pin);
            //txtwrt2.Close();

            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@pin", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, pin);
            int cardAmount = Convert.ToInt32(da.ExecuteScalar("UP_GetPinValue", System.Data.CommandType.StoredProcedure));
            return cardAmount;
        }

        //public int ApplyPaymentMuliLine(string cardPins, string accountNumber,  string channelRefNo)
        //{
        //    try
        //    {

        //        //string[] arrCarpins = cardPins.Split(',');
        //        //string pinItemMD5;
        //        DateTime transactionDate = DateTime.Now;//christian: 9/4/14
        //        //foreach (string pinItem in arrCarpins)
        //        //{
        //            //pinItemMD5 = Crypt.MD5Crypt.MDee5(pinItem);
        //            // log to the payment table by calling this.ApplyPayment method

        //        int r= this.ApplyPayment(cardPins, accountNumber, 0, channelRefNo, /*christian: 9/4/14 added this last arg*/ transactionDate);
        //        return r;
        //        //}

        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        return 0;
        //    }

        //}

        //public void ApplyPaymentMuliLineEpin100(string cardPins, string accountNumber, string channelRefNo)
        //{
        //    try
        //    {

        //        string[] arrCarpins = cardPins.Split(',');
        //        //string pinItemMD5;
        //        DateTime transactionDate = DateTime.Now;//christian: 9/4/14

        //        foreach (string pinItem in arrCarpins)
        //        {
        //            //pinItemMD5 = Crypt.MD5Crypt.MDee5(pinItem);
        //            // log to the payment table by calling this.ApplyPayment method
        //            //System.IO.TextWriter txtwrt1 = new System.IO.StreamWriter(@"C:\Users\Deji\Documents\WriteText.txt", true);
        //            //txtwrt1.WriteLine(pinItem + "\n\n\n");
        //            //txtwrt1.WriteLine(accountNumber + "\n\n\n");
        //            //txtwrt1.WriteLine(this.GetPinValue(pinItem) + "\n\n\n");
        //            //txtwrt1.WriteLine(channelRefNo + "\n\n\n");
        //            //txtwrt1.Close();

        //            this.ApplyPayment(pinItem, accountNumber, this.GetPinValue(pinItem), channelRefNo, /*christian: 9/4/14 added this last arg*/ transactionDate);

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();

        //    }
        //}

        public SqlDataReader AllocatePinsToDealer(/*long startSerial, long endSerial, string denomination,*/ int amount, string subDealer, string allocateBy)
        {
            //System.IO.TextWriter txtwrt = new System.IO.StreamWriter(@"C:\Users\Deji\Documents\WriteText.txt", true);
            //txtwrt.WriteLine(startSerial);
            //txtwrt.WriteLine(endSerial);
            //txtwrt.WriteLine(denomination);
            //txtwrt.WriteLine(subDealer);
            //txtwrt.WriteLine(allocateBy);

            Cls.DataAccess da = new Cls.DataAccess();
            //Christian: commenting out Deji's code
            //da.AddParameter("@startSerial", SqlDbType.BigInt, ParameterDirection.Input, startSerial);
            //da.AddParameter("@endSerial", SqlDbType.BigInt, ParameterDirection.Input, endSerial);
            //da.AddParameter("@denomination", SqlDbType.VarChar, ParameterDirection.Input, denomination);

            da.AddParameter("@amount", SqlDbType.VarChar, ParameterDirection.Input, amount);
            da.AddParameter("@subDealerId", SqlDbType.VarChar, ParameterDirection.Input, subDealer);
            da.AddParameter("@allocatedBy", SqlDbType.VarChar, ParameterDirection.Input, allocateBy);
            SqlDataReader dr = da.ExecuteReader("UP_AllocatePinToDealer", CommandType.StoredProcedure);

            //txtwrt.WriteLine(num);
            //txtwrt.Close();

            return dr;
        }

        public int CheckIfEnoughNGN100Epins(string userName, Int32 numberOfPinsNeeded)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@numberOfPinsNeeded", SqlDbType.BigInt, ParameterDirection.Input, numberOfPinsNeeded);
            int isEnoughPins = (int)da.ExecuteScalar("UP_CheckIfEnoughNGN100Epins", CommandType.StoredProcedure);

            return isEnoughPins;
        }


        public int CheckDuplicateTransaction(string pinList)
        {

            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@pinList", SqlDbType.VarChar, ParameterDirection.Input, pinList);
            int isDuplicateTrans = (int)da.ExecuteScalar("UP_CheckDuplicateTransaction", CommandType.StoredProcedure);

            return isDuplicateTrans;
        }

        public decimal GetAvailableDealerCredit(string userName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            decimal availableDealerCredit = (decimal)da.ExecuteScalar("UP_GetAvailableDealerCredit", CommandType.StoredProcedure);

            return availableDealerCredit;
        }

        public string GetErrorCodeDescription(string errorCode)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@errorCode", SqlDbType.VarChar, ParameterDirection.Input, errorCode);
            string errorDescription = (string)da.ExecuteScalar("UP_GetErrorCodeDescription", CommandType.StoredProcedure);

            return errorDescription;
        }


        internal SqlDataReader GetMyCards(string userName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            SqlDataReader dr = da.ExecuteReader("UP_GetMyCards", CommandType.StoredProcedure);
            return dr;
        }


        public SqlDataReader GetDealerDetails(string userName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            SqlDataReader dr = da.ExecuteReader("UP_GetDealerForEdit", CommandType.StoredProcedure);
            return dr;
        }

        //Chriistian: 9/4/14
        public SqlDataReader GetReporterDetails(string userName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            SqlDataReader dr = da.ExecuteReader("UPR_GetReporterForEdit", CommandType.StoredProcedure);
            return dr;
        }

        internal SqlDataReader GetSubDealers(string parentDealer)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@parentDealer", SqlDbType.VarChar, ParameterDirection.Input, parentDealer);
            SqlDataReader dr = da.ExecuteReader("UP_GetSubDealer", CommandType.StoredProcedure);

            return dr;
        }

        internal SqlDataReader GetPayments(string parentDealer)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@parentDealer", SqlDbType.VarChar, ParameterDirection.Input, parentDealer);
            SqlDataReader dr = da.ExecuteReader("UPR_GetPayments", CommandType.StoredProcedure);
            return dr;
        }

        public int AddDealer(string userName, string passWord, string fullName, string email, string address, string parentDealer, decimal creditLimit, decimal availableCredit, string dCode, string dealerTier, string subDealerCount, bool optin = false)
        {
            if (int.Parse(subDealerCount.ToString()) > 58)
            {
                return 58;
            }
            else
            {
                Cls.DataAccess da = new Cls.DataAccess();
                da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
                da.AddParameter("@Password", SqlDbType.VarChar, ParameterDirection.Input, passWord);
                da.AddParameter("@FullName", SqlDbType.VarChar, ParameterDirection.Input, fullName);
                da.AddParameter("@email", SqlDbType.VarChar, ParameterDirection.Input, email);
                da.AddParameter("@address", SqlDbType.VarChar, ParameterDirection.Input, address);
                da.AddParameter("@parentDealer", SqlDbType.VarChar, ParameterDirection.Input, parentDealer);
                da.AddParameter("@creditLimit", SqlDbType.Decimal, ParameterDirection.Input, creditLimit);
                da.AddParameter("@dCode", SqlDbType.VarChar, ParameterDirection.Input, dCode);
                da.AddParameter("@dealerTier", SqlDbType.VarChar, ParameterDirection.Input, dealerTier);
                da.AddParameter("@optin", SqlDbType.Bit, ParameterDirection.Input, optin);
                // da.AddParameter("@creditLimit", SqlDbType.Decimal, ParameterDirection.Input, availableCredit);
                int result = da.ExecuteNonQuery("[UP_InsertDealers]", CommandType.StoredProcedure);
                return result;
            }

        }

        public int LogToAllocationTable(long startSerial, long endSerial, string denomination, string allocatedTo, string allocatedBy, string formDate, string formId)
        {
            char[] seperators = { '/', '-' };
            string[] dateNums = formDate.Split(seperators);

            string dt = (dateNums[2].Length == 4 ? dateNums[2] : dateNums[0]) + "-" + dateNums[1] + "-" + (dateNums[2].Length == 4 ? dateNums[0] : dateNums[2]);

            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@startSerial", SqlDbType.BigInt, ParameterDirection.Input, startSerial);
            da.AddParameter("@endSerial", SqlDbType.BigInt, ParameterDirection.Input, endSerial);
            da.AddParameter("@denomination", SqlDbType.VarChar, ParameterDirection.Input, denomination);
            da.AddParameter("@allocatedTo", SqlDbType.VarChar, ParameterDirection.Input, allocatedTo);
            da.AddParameter("@allocatedBy", SqlDbType.VarChar, ParameterDirection.Input, allocatedBy);
            da.AddParameter("@allocationFormDate", SqlDbType.DateTime, ParameterDirection.Input, dt);
            da.AddParameter("@formId", SqlDbType.VarChar, ParameterDirection.Input, formId);
            int result = da.ExecuteNonQuery("[UP_LogToAllocationTable]", CommandType.StoredProcedure);
            return result;
        }

        internal SqlDataReader GetDenomination()
        {
            Cls.DataAccess da = new Cls.DataAccess();
            SqlDataReader dr = da.ExecuteReader("[UP_GetCardDenomination]", CommandType.StoredProcedure);
            return dr;
        }



        public int rndd(string amt)
        {
            int rnd;
            int num = Convert.ToInt32(amt);
            //if (num < 100)
            //{
            //    num = 100;
            //    return num;
            //}

            //int remender = num % 100;
            //if (remender > 50)
            //{
            //    rnd = Round100(num);
            //}
            //else
            //{
            //    rnd = Round100(num) + Round100(remender);
            //}    

            //////////////Christian: replace Deji's code (above) with this (below)
            if (num % 100 != 0)
            {
                int rem = num % 100;
                int numToAdd = 100 - rem;

                rnd = num + numToAdd;
            }
            else
            {
                rnd = num;
            }
            return rnd;
        }

        //Christian: this method is no longer relevant
        //private static int Round100(double value)
        //{
        //    int result = (int)Math.Round(value / 100);
        //    if (value > 0 && result == 0)
        //    {
        //        result = 1;
        //    }
        //    return (int)result * 100;
        //}


        //Chritian: Created this method
        public string TopUpSubDealerAvailableCredit(decimal amountPaid, decimal topUpAmoumt, string subDealerUserName, string userName, decimal subDealerAvailableCredit, string allocationClass = "SALES", string aid = null)
        {

            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@amountPaid", SqlDbType.Decimal, ParameterDirection.Input, amountPaid);
            da.AddParameter("@topUpAmount", SqlDbType.Decimal, ParameterDirection.Input, topUpAmoumt);
            da.AddParameter("@subDealerUserName", SqlDbType.VarChar, ParameterDirection.Input, subDealerUserName);
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@subDealerAvailableCredit", SqlDbType.Decimal, ParameterDirection.Input, subDealerAvailableCredit);
            da.AddParameter("@allocationClass", SqlDbType.VarChar, ParameterDirection.Input, allocationClass);
            if (aid != null)
            {
                da.AddParameter("@aid", SqlDbType.VarChar, ParameterDirection.Input, aid);
            }
            string num = (string)da.ExecuteScalar("UP_TopUpSubDealerAvailableCredit", CommandType.StoredProcedure);
            return num;
        }

        //Chritian: Created this method
        public int AllocateScratchCardsToDealer(long startSerial, long endSerial, string denomination, string subDealer, string allocateBy, string formDate, string formId)
        {
            char[] seperators = { '/', '-' };
            string[] dateNums = formDate.Split(seperators);

            string dt = (dateNums[2].Length == 4 ? dateNums[2] : dateNums[0]) + "-" + dateNums[1] + "-" + (dateNums[2].Length == 4 ? dateNums[0] : dateNums[2]);

            Cls.DataAccess da = new Cls.DataAccess();

            da.AddParameter("@startSerial", SqlDbType.BigInt, ParameterDirection.Input, startSerial);
            da.AddParameter("@endSerial", SqlDbType.BigInt, ParameterDirection.Input, endSerial);
            da.AddParameter("@denomination", SqlDbType.VarChar, ParameterDirection.Input, denomination);

            da.AddParameter("@subDealerId", SqlDbType.VarChar, ParameterDirection.Input, subDealer);
            da.AddParameter("@allocatedBy", SqlDbType.VarChar, ParameterDirection.Input, allocateBy);
            da.AddParameter("@formId", SqlDbType.VarChar, ParameterDirection.Input, formId);
            da.AddParameter("@allocationFormDate", SqlDbType.VarChar, ParameterDirection.Input, dt);
            int num = (int)da.ExecuteScalar("UP_AllocateScratchCardToDealer", CommandType.StoredProcedure);

            return num;
        }

        //Chritian: 25/04/14
        public int DeactivateDealerCards(long startSerial, long endSerial, string denomination, string subDealer)
        {
            Cls.DataAccess da = new Cls.DataAccess();

            da.AddParameter("@startSerial", SqlDbType.BigInt, ParameterDirection.Input, startSerial);
            da.AddParameter("@endSerial", SqlDbType.BigInt, ParameterDirection.Input, endSerial);
            da.AddParameter("@denomination", SqlDbType.VarChar, ParameterDirection.Input, denomination);
            da.AddParameter("@subDealer", SqlDbType.VarChar, ParameterDirection.Input, subDealer);

            int num = (int)da.ExecuteScalar("UP_DeactivateDealerCards", CommandType.StoredProcedure);

            return num;
        }

        //Chritian: Created this method
        public SqlDataReader GetThisAllocationDetails(string allocatedBy, string subDealer, string formDate, string formId)
        {

            char[] seperators = { '/', '-' };
            string[] dateNums = formDate.Split(seperators);

            string dt = (dateNums[2].Length == 4 ? dateNums[2] : dateNums[0]) + "-" + dateNums[1] + "-" + (dateNums[2].Length == 4 ? dateNums[0] : dateNums[2]);


            Cls.DataAccess da = new Cls.DataAccess();

            da.AddParameter("@allocatedBy", SqlDbType.VarChar, ParameterDirection.Input, allocatedBy);
            da.AddParameter("@subDealer", SqlDbType.VarChar, ParameterDirection.Input, subDealer);
            da.AddParameter("@formDate", SqlDbType.DateTime, ParameterDirection.Input, dt);
            da.AddParameter("@formId", SqlDbType.VarChar, ParameterDirection.Input, formId);
            SqlDataReader dr = da.ExecuteReader("UP_GetThisAllocationDetails", CommandType.StoredProcedure);

            return dr;
        }

        //Chritian: Created this method
        public int GetCountOfPinWithinRange(int amount, string denomination, string allocatedBy)
        {
            Cls.DataAccess da = new Cls.DataAccess();

            da.AddParameter("@amount", SqlDbType.Int, ParameterDirection.Input, amount);
            da.AddParameter("@denomination", SqlDbType.VarChar, ParameterDirection.Input, denomination);
            da.AddParameter("@allocatedBy", SqlDbType.VarChar, ParameterDirection.Input, allocatedBy);

            int count = (int)da.ExecuteScalar("UP_GetCountOfScratchCardsWithinRange", CommandType.StoredProcedure);

            return count;
        }
        //Chritian: Created this method
        public object GetUsedValueByAllocation(long startSerial, long endSerial)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@startSerial", SqlDbType.BigInt, ParameterDirection.Input, startSerial);
            da.AddParameter("@endSerial", SqlDbType.BigInt, ParameterDirection.Input, endSerial);
            object usedValue = da.ExecuteScalar("UPR_GetUsedValueByAllocation", CommandType.StoredProcedure);

            return usedValue;
        }

        //Chritian: Created this method
        public object GetLatestMobileVersion()
        {
            Cls.DataAccess da = new Cls.DataAccess();
            object latestVersion = da.ExecuteScalar("UPM_GetLatestMobileVersion", CommandType.StoredProcedure);

            return latestVersion;
        }

        public void ClearMultiLogin()
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.ExecuteScalar("UP_ClearMultiLogin", CommandType.StoredProcedure);
        }

        //Christian: 26/03/13
        public string CheckIfUserIsReporter(string userName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            string status = (string)da.ExecuteScalar("UPR_CheckIfUserIsReporter", CommandType.StoredProcedure);

            return status;
        }

        //Christian: 26/03/13
        internal int AddReporter(string userName, string password, string parentDealer, string fullname, string email, string address)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@password", SqlDbType.VarChar, ParameterDirection.Input, password);
            da.AddParameter("@parentDealer", SqlDbType.VarChar, ParameterDirection.Input, parentDealer);
            da.AddParameter("@fullname", SqlDbType.VarChar, ParameterDirection.Input, fullname);
            da.AddParameter("@email", SqlDbType.VarChar, ParameterDirection.Input, email);
            da.AddParameter("@address", SqlDbType.VarChar, ParameterDirection.Input, address);
            // da.AddParameter("@creditLimit", SqlDbType.Decimal, ParameterDirection.Input, availableCredit);
            int result = da.ExecuteNonQuery("UPR_InsertReporter", CommandType.StoredProcedure);
            return result;
        }

        //Christian: 26/03/13
        internal SqlDataReader GetReporters(string parentDealer)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@parentDealer", SqlDbType.VarChar, ParameterDirection.Input, parentDealer);
            SqlDataReader dr = da.ExecuteReader("UPR_GetReporters", CommandType.StoredProcedure);
            return dr;
        }

        //Christian: 24/04/14
        internal int UpdateDealerDetails(string userName, string fullName, string email, string address)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@fullName", SqlDbType.VarChar, ParameterDirection.Input, fullName);
            da.AddParameter("@email", SqlDbType.VarChar, ParameterDirection.Input, email);
            da.AddParameter("@address", SqlDbType.VarChar, ParameterDirection.Input, address);

            return da.ExecuteNonQuery("UP_UpdateDealers", CommandType.StoredProcedure);
        }

        //Christian: 25/04/14
        internal int UpdateReporterDetails(string userName, string fullName, string email, string address)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@fullName", SqlDbType.VarChar, ParameterDirection.Input, fullName);
            da.AddParameter("@email", SqlDbType.VarChar, ParameterDirection.Input, email);
            da.AddParameter("@address", SqlDbType.VarChar, ParameterDirection.Input, address);

            return da.ExecuteNonQuery("UPR_UpdateReporter", CommandType.StoredProcedure);
        }

        //Christian: 25/04/14
        internal void DeleteDealer(string dealerName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@dealerName", SqlDbType.VarChar, ParameterDirection.Input, dealerName);
            da.ExecuteNonQuery("UP_DeleteDealer", CommandType.StoredProcedure);
        }

        //Christian: 25/04/14
        internal void DeletePayment(string paymentId)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@paymentId", SqlDbType.Int, ParameterDirection.Input, Int32.Parse(paymentId));
            da.ExecuteNonQuery("UPR_DeletePayment", CommandType.StoredProcedure);
        }

        //Christian: 25/04/14
        internal void DeleteReporter(string dealerName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@dealerName", SqlDbType.VarChar, ParameterDirection.Input, dealerName);
            da.ExecuteNonQuery("UPR_DeleteReporter", CommandType.StoredProcedure);
        }

        internal object GetLastTransactionDate()
        {
            Cls.DataAccess da = new Cls.DataAccess();
            return da.ExecuteScalar("UP_GetLastTransactionDate", CommandType.StoredProcedure);
        }

        public int RetrieveEPINsFromDealer(string superDealer, string subDealer)
        {
            Cls.DataAccess da = new Cls.DataAccess();

            da.AddParameter("@dealer", SqlDbType.VarChar, ParameterDirection.Input, superDealer);
            da.AddParameter("@subDealer", SqlDbType.VarChar, ParameterDirection.Input, subDealer);

            int code = (int)da.ExecuteScalar("UP_RetrieveEPINsFromDealer", CommandType.StoredProcedure);

            return code;
        }

        //Christian: 8/5/14
        internal int ResetDealerPassword(string userName, string password)
        {
            Cls.DataAccess da = new Cls.DataAccess();

            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@password", SqlDbType.VarChar, ParameterDirection.Input, password);

            return da.ExecuteNonQuery("UP_ResetDealerPassword", CommandType.StoredProcedure);
        }

        internal object GetDealerCode(string dealerName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@dealerName", SqlDbType.VarChar, ParameterDirection.Input, dealerName);
            return da.ExecuteScalar("UPM_GetDealerCode", CommandType.StoredProcedure);
        }

        internal SqlDataReader GetMobileVersions()
        {
            Cls.DataAccess da = new Cls.DataAccess();
            return da.ExecuteReader("UPM_GetMobileVersions", CommandType.StoredProcedure);
        }

        internal int RegisterVersion(string verNum, string revision, string appLink)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@VersionNo", SqlDbType.VarChar, ParameterDirection.Input, verNum);
            da.AddParameter("@description", SqlDbType.VarChar, ParameterDirection.Input, revision);
            da.AddParameter("@apkLink", SqlDbType.VarChar, ParameterDirection.Input, appLink);

            int result = da.ExecuteNonQuery("[UPM_RegisterVersion]", CommandType.StoredProcedure);

            return result;
        }

        internal SqlDataReader GetEPINsRetrievalLog(string userName, string startDate, string endDate)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@userName", SqlDbType.VarChar, ParameterDirection.Input, userName);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);

            return da.ExecuteReader("UPR_GetEPINSRetrievalLog", CommandType.StoredProcedure);
        }

        internal SqlDataReader BUPerformance(string startDate, string endDate)
        {
            Cls.DataAccess da = new Cls.DataAccess();

            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);

            return da.ExecuteReader("UPR_BUPerformance", CommandType.StoredProcedure);
        }

        internal DataTable GetBusinessUnits()
        {
            Cls.DataAccess da = new Cls.DataAccess();

            DataTable dt = new DataTable();
            dt.Load(da.ExecuteReader("UP_GetBusinessUnits", CommandType.StoredProcedure));

            return dt;
        }

        public bool IsEligible(string dealerName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@dealerName", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dealerName);

            DataTable dt = new DataTable();
            dt.Load(da.ExecuteReader("UP_CheckEligibility", CommandType.StoredProcedure));

            return dt.Rows.Count == 1;
        }

        internal SqlDataReader GetTransactionSummary(string bu, string startDate, string endDate, string collectionType)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@bu", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, bu);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);
            da.AddParameter("@collectionType", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, collectionType);

            SqlDataReader dr = da.ExecuteReader("UPR_GetTransactionSummary", CommandType.StoredProcedure);

            return dr;
        }

        internal SqlDataReader GetDealerTransactionSummary(string bu, string startDate, string endDate, string dealerName, string txnFrom)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@bu", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, bu);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);
            da.AddParameter("@subDealer", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dealerName);
            da.AddParameter("@txnFrom", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, txnFrom);

            SqlDataReader dr = da.ExecuteReader("UPR_GetDealerTxnSummary", CommandType.StoredProcedure);

            return dr;
        }

        internal SqlDataReader GetDailyTransactionSummary(string bu, string startDate, string endDate, string collectionType)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@bu", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, bu);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);
            da.AddParameter("@collectionType", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, collectionType);

            SqlDataReader dr = da.ExecuteReader("UPR_GetDailyTransactionSummary", CommandType.StoredProcedure);

            return dr;
        }

        internal SqlDataReader GetDealerDailyTransactionSummary(string bu, string startDate, string endDate, string dealerName, string txnFrom)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@bu", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, bu);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);
            da.AddParameter("@subDealer", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dealerName);
            da.AddParameter("@txnFrom", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, txnFrom);

            SqlDataReader dr = da.ExecuteReader("UPR_GetDealerDailyTxnSummary", CommandType.StoredProcedure);

            return dr;
        }

        internal SqlDataReader GetDealerDailyTransactionSummary(string bu, string startDate, string endDate, string dealerName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@bu", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, bu);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);
            da.AddParameter("@dealerName", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dealerName);

            SqlDataReader dr = da.ExecuteReader("UPR_GetDealerDailyTxnSummary", CommandType.StoredProcedure);

            return dr;
        }

        internal SqlDataReader GetTransactionDetails(string bu, string acctNo, string startDate, string endDate, string collectionType)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@bu", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, bu);
            da.AddParameter("@acctNo", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, acctNo);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);
            da.AddParameter("@collectionType", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, collectionType);

            SqlDataReader dr = da.ExecuteReader("UPR_GetTransactionDetails", CommandType.StoredProcedure);

            return dr;
        }



        internal string GetLastPayment(string acctNo, string dCode)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@acctNo", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, acctNo);
            da.AddParameter("@dCode", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dCode);

            SqlDataReader dr = da.ExecuteReader("UP_GetLastPayment", CommandType.StoredProcedure);

            while (dr.Read())
            {
                return dr["transactionDate"].ToString() + "_" + dr["amount"].ToString();
            }

            return "_";
        }

        internal int GetRequestUniqueIDCount(string mReqUID, string dCode)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@mReqUID", SqlDbType.VarChar, ParameterDirection.Input, mReqUID);
            da.AddParameter("@dCode", SqlDbType.VarChar, ParameterDirection.Input, dCode);

            int result = (int)da.ExecuteScalar("[UPM_GetRequestUniqueIDCount]", CommandType.StoredProcedure);

            return result;
        }


        internal string GetBottomMsg()
        {
            Cls.DataAccess da = new Cls.DataAccess();

            string result = (string)da.ExecuteScalar("[UPM_GetBottomMsg]", CommandType.StoredProcedure);

            return result;
        }

        internal string GetPushMsg()
        {
            Cls.DataAccess da = new Cls.DataAccess();

            string result = (string)da.ExecuteScalar("[UPM_GetPushMsg]", CommandType.StoredProcedure);

            return result;
        }

        internal SqlDataReader GetTransactionsToPush(string tabName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@tabName", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, tabName);

            SqlDataReader dr = da.ExecuteReader("PUSH_GetTransactionsToPush", CommandType.StoredProcedure);

            return dr;
        }

        internal string GetDealerInfo(string dcode)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@dcode", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dcode);

            string result = (string)da.ExecuteScalar("PUSH_GetDealerInfo", CommandType.StoredProcedure);

            return result;
        }

        internal void MarkAsDone(string utid, string url)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@utid", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, utid);
            da.AddParameter("@url", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, url);

            da.ExecuteScalar("PUSH_MarkAsDone", CommandType.StoredProcedure);
        }

        internal void MarkAsConfirmed(string utid, string ans)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@utid", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, utid);
            da.AddParameter("@ans", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, ans);

            da.ExecuteScalar("PUSH_MarkAsConfirmed", CommandType.StoredProcedure);
        }

        internal SqlDataReader GetDealerTransactionDetails(string subDealer, string bu, string txnFrom, string startDate, string endDate)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@subDealer", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, subDealer);
            da.AddParameter("@bu", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, bu);
            da.AddParameter("@txnFrom", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, txnFrom);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);

            SqlDataReader dr = da.ExecuteReader("UPR_GetDealerTransactionDetails", CommandType.StoredProcedure);

            return dr;
        }

        internal void ReverseTransaction(string dealer, decimal amount, string accountNumber, string errorMsg, decimal initialAC, string reversalId)
        {
            DeductDealerAvailableCredit(dealer, amount * -1);

            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@dealer", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dealer);
            da.AddParameter("@dCode", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, GetDealerCode(dealer));
            da.AddParameter("@amount", System.Data.SqlDbType.Decimal, System.Data.ParameterDirection.Input, amount);
            da.AddParameter("@accountNumber", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, accountNumber);
            da.AddParameter("@errorMsg", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, errorMsg);
            da.AddParameter("@initialAC", System.Data.SqlDbType.Decimal, System.Data.ParameterDirection.Input, initialAC);
            da.AddParameter("@reversalId", System.Data.SqlDbType.Decimal, System.Data.ParameterDirection.Input, reversalId);

            da.ExecuteScalar("UP_LogReversal", CommandType.StoredProcedure);

        }

        internal DataTable GetPendingAllocations(string dealer, string foundAmt = "0")
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@dealerName", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dealer);
            da.AddParameter("@foundAmt", System.Data.SqlDbType.Decimal, System.Data.ParameterDirection.Input, Decimal.Parse(foundAmt));

            SqlDataReader dr = da.ExecuteReader("UP_GetPendingAllocations", CommandType.StoredProcedure);

            DataTable dt = new DataTable();
            dt.Load(dr);

            dr.Close();

            return dt;
            //return dr;
        }

        internal SqlDataReader GetPendingManualAllocations()
        {
            Cls.DataAccess da = new Cls.DataAccess();

            SqlDataReader dr = da.ExecuteReader("UP_GetPendingManualAllocations", CommandType.StoredProcedure);

            return dr;
        }

        internal SqlDataReader GetPendingManualAllocations(string userName)
        {
            Cls.DataAccess da = new Cls.DataAccess();

            da.AddParameter("@userName", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, userName);
            SqlDataReader dr = da.ExecuteReader("UP_GetPendingManualAllocations", CommandType.StoredProcedure);

            return dr;
        }

        internal string AutoAllocate(string aid)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@aid", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, aid);

            return (string)da.ExecuteScalar("UP_AutoAllocate", CommandType.StoredProcedure);
        }

        internal int LogAllocationRequest(decimal amount, string bank, string dealer)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@subDealer", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dealer);
            da.AddParameter("@bank", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, bank);
            da.AddParameter("@amount", System.Data.SqlDbType.Decimal, System.Data.ParameterDirection.Input, amount);

            return da.ExecuteNonQuery("UP_LogAllocationRequest", CommandType.StoredProcedure);
        }

        internal string AddBankAccountName(string dealer, string accountName, string accountNum, string bankName, string dCode, string approved = null)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@dealerName", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dealer);
            da.AddParameter("@accountName", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, accountName);
            da.AddParameter("@accountNum", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, accountNum);
            da.AddParameter("@bankName", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, bankName);
            da.AddParameter("@dCode", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dCode);
            //da.AddParameter("@approved", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, approved);

            string resp = da.ExecuteScalar("UP_AddBankAccountName", CommandType.StoredProcedure).ToString();

            return resp;
        }

        internal SqlDataReader GetBankAccountNames(string dealerName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@dealerName", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dealerName);

            return da.ExecuteReader("UP_GetBankAccountNames", CommandType.StoredProcedure);
        }

        internal SqlDataReader GetBankAccountNames(string dealerName, string approved)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@dealerName", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dealerName);
            da.AddParameter("@approved", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, approved);

            return da.ExecuteReader("UP_GetBankAccountNames", CommandType.StoredProcedure);
        }

        internal string ChangeDealerCommission(string subdealer, decimal commission)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@subdealer", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, subdealer);
            da.AddParameter("@commission", System.Data.SqlDbType.Decimal, System.Data.ParameterDirection.Input, commission);

            return (string)da.ExecuteScalar("UP_ChangeDealerCommission", CommandType.StoredProcedure);
        }

        internal int CheckDealerOptin(string subdealer)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@subdealer", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, subdealer);

            return (int)da.ExecuteScalar("UP_CheckDealerOptin", CommandType.StoredProcedure);
        }


        internal DataTable GetFirstUGR()
        {
            Cls.DataAccess da = new Cls.DataAccess();

            DataTable dt = new DataTable();
            dt.Load(da.ExecuteReader("UP_GetUGR", CommandType.StoredProcedure));

            return dt;
        }

        internal bool ChannelRefExists(string channelRefNo)
        {
            Cls.DataAccess da = new Cls.DataAccess();

            DataTable dt = new DataTable();
            da.AddParameter("@channelRefNo", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, channelRefNo);
            dt.Load(da.ExecuteReader("UP_ChannelRefExists", CommandType.StoredProcedure));

            return dt.Rows.Count != 0;
        }
    }
}