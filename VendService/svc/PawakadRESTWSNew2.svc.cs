using Newtonsoft.Json;
using pawakadApp;
using pawakadApp.Admin;
using pawakadApp.Cls;
using pawakadApp.ClsAdmin;
using pawakadApp.ClsCard;
using pawakadApp.Crypt;
using pawakadApp.Log;
using pawakadApp.Mailer;
using pawakadApp.WS;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Web;

namespace PwkRESTWS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PawakadRESTWS" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select PawakadRESTWS.svc or PawakadRESTWS.svc.cs at the Solution Explorer and start debugging.
    public class PawakadRESTWSNew2 : IPawakadRESTWSNew2
    {
        public string SayWassup(string toWho)
        {
            return "Wassup " + toWho;
        }

        public string ValidateCustomerAndEPIN(string acctNoOrMeterNo, string amt, string dealerName, string password, string mtid, string businessUnit = "")
        {
            ClsDealer dl = new ClsDealer();
            Guid guid = Guid.NewGuid();

            string tDate = DateTime.Today.ToString("yyMMdd").Replace("/", "");
            string paymentType = new PaymentType().GetPaymentType(acctNoOrMeterNo);
            string channelRefNo = (paymentType == "PREPAID" ? "MT" : "MB") + tDate + dl.GetDealerCode(dealerName).ToString() + "_" + guid.ToString();

            if (RequestIsValid(dealerName, password))
            {

                string reqParams = "acctNoOrMeterNo:" + acctNoOrMeterNo + "|" +
                    "amt:" + amt + "|" +
                    "dealerName:" + dealerName + "|" +
                    "businessUnit:" + businessUnit
                    ;


                //log the mobile request
                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid + "|REQ", "ValidateCustomerAndEPIN", reqParams, channelRefNo);

                mtid += "|RESP";

                string customerName = "", address = "", fixedCharge = "";

                string dcode = new ClsDealer().GetDealerCode(dealerName).ToString();

                //log progress -- channelRefNo
                ClsCards cls = new ClsCards();
                //cls.RunNonQuery("insert into LogPaymentAction(channelRefNo) values('" + channelRefNo + "')");

                string getVars = acctNoOrMeterNo.Length == 11
                ? "vendAmount=" + amt + "&meterNo=" + acctNoOrMeterNo + "&paymentChannel=" + dcode
                : "meterNumber=" + (acctNoOrMeterNo.Length == 12 ? "PP" : "") + acctNoOrMeterNo + "&paymentChannel=" + dcode;

                WSCall wsc = new WSCall();

                //RealImplementation
                string returnString = "";

                if (acctNoOrMeterNo != "0000000000")
                {
                    returnString = wsc.GetJSON(String.Format(acctNoOrMeterNo.Length == 11 ? "/TrialCreditVend{0}" : "/confirmcustomer{0}", "?" + Misc.GetClientId + "&" + getVars + "&channelRefNo=" + channelRefNo));
                }
                else
                {
                    //returnString = "1|||||||";

                    string returnString33 = "1|customerName_address_fixedCharge_conlogTID_warningNsg_flag" + (mtid.IndexOf("|") != -1 ? "_channelRefNo" : "") + "||||||0|" + (mtid.IndexOf("|") != -1 ? "|" + channelRefNo : "");
                    //log the mobile request
                    ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "ValidateCustomerAndEPIN", returnString33 + "|" + amt, channelRefNo);

                    return returnString33;
                }
                //string returnString = wsc.GetJSON(String.Format("/confirmcustomer{0}", getVars)/*, new ClsDealer().GetDealerCode(dealerName).ToString()*/);

                //startTest
                //string returnString = "1|TransactionId_CustomerName_CustomerAddress_CustomerphoneNumber_CustomerAccountNumer_MinimunVendingAmount_Units|20170511115559~1000011|PAWAKAD1, TS|||04040406714|70.00|25.5";
                //endTest

                string warning = "", flag = "0";
                if (acctNoOrMeterNo != "")
                {
                    string[] lastPayment = dl.GetLastPayment(acctNoOrMeterNo, new ClsDealer().GetDealerCode(dealerName).ToString()).Split('_');

                    if (!lastPayment[0].ToString().Equals("") && !lastPayment[1].ToString().Equals("")) // lastPayment returnString ""
                    {
                        warning = "You have made a payment of N" + lastPayment[1] + " for this customer today at " + lastPayment[0].Substring(0, 8) + ".######Are you sure you want to proceed?###### Request for reversal may not be honoured.###";
                        flag = "1";
                    }
                }

                //this IF is necessary for new meter nums that fails on first vend by returning 'Amount is insufficient' error
                //when a value lesser than the required amount is paid, or null when the required amount or more is paid
                if (returnString == null || returnString == "")
                    returnString = wsc.GetJSON(String.Format("/confirmcustomer{0}", "?" + Misc.GetClientId + "&" + "meterNumber=" + acctNoOrMeterNo + "&paymentChannel=" + dcode + "&channelRefNo=" + channelRefNo));

                if (returnString == null || returnString == "")
                {
                    returnString = "0|Unable to get customer details";
                    //log the mobile request
                    ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "ValidateCustomerAndEPIN", returnString + "|" + amt, channelRefNo);

                    return returnString;
                }

                string[] tokens = returnString.Split('|');

                if (tokens[0] == "1")
                {
                    customerName = tokens[3];
                    address = tokens[4];
                    fixedCharge = tokens[7];
                }
                else
                {
                    string returnString2 = "0|" + tokens[1];
                    //log the mobile request
                    ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "ValidateCustomerAndEPIN", returnString2 + "|" + amt, channelRefNo);

                    return returnString2;
                }

                string returnString3 = "1|customerName_address_fixedCharge_conlogTID_warningNsg_flag" + (mtid.IndexOf("|") != -1 ? "_channelRefNo" : "") + "|" + customerName + "|" + address + "|" + fixedCharge + "|" + tokens[2] + "|" + warning + "|" + flag + (mtid.IndexOf("|") != -1 ? "|" + channelRefNo : "");
                //log the mobile request
                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "ValidateCustomerAndEPIN", returnString3 + "|" + amt, channelRefNo);

                return returnString3;
            }
            else
            {
                string returnString = "0|Invalid Request";
                //log the mobile request
                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "ValidateCustomerAndEPIN", returnString + "|" + amt, channelRefNo);

                return returnString;
            }
        }

        public string SubmitPayment(string meterNo, string amt, string dealerName, string custName, string custAddress, string crCode, string fixedCharge2,
            string conlogTID, string password, string mtid, string channelRefNo, string collectionType = "NRG", string invoiceNo = "", string businessUnit = "")
        {
            string errorMsg = "";
            ClsLog cl = new ClsLog();
            ClsDealer dl = new ClsDealer();

            if (dl.ChannelRefExists(channelRefNo))
            {
                return "0|Get receipt from payment history";
            }

            if (RequestIsValid(dealerName, password))
            {
                return pawakadApp.PwkRESTWS.utilClass.SubmitPayment.PerformPayment(meterNo, amt, dealerName, custName, custAddress, crCode, fixedCharge2,
             conlogTID, password, mtid, channelRefNo, collectionType, invoiceNo, businessUnit);
            }
            else
            {
                errorMsg = "0|Invalid_Request";
                cl.LogPaymentError(meterNo, "", int.Parse(amt), errorMsg, "PAYMENT", DateTime.Now /*, dCode, DateTime.Now, false*/);

                //dl.AddLoginLocation(dealerName, longitude, latitude, address, mtid);
                return errorMsg;
            }
        }

        public string Login(string dealerUserName, string dealerPassword, string mtid)
        {
            string userName = dealerUserName;
            // MAKE THE PASSWORD MD5 ENCRYPTION
            string passWord = MD5Crypt.MDee5(dealerPassword);
            ClsDealer dl = new ClsDealer();

            string reqParams = "dealerUserName:" + dealerUserName;

            //log the mobile request
            ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid + "|REQ", "Login", reqParams);

            mtid += "|RESP";

            //FIRST CHECK THAT USER IS NOT LOGGED IN ELSWHERE
            //if logged in elsewhere then inform user and  terminate login proces 
            int isDuplicateLogin = 0;// dl.CheckMultiLogin(userName);

            if (isDuplicateLogin == 1)
            {
                string errorMsg = "0|You Have Loged in elsewhere and cannot login at this terminal. Please log out from the other terminal.";
                //log the mobile request
                ClsPayment.LogMobileRequest2(dl.GetDealerCode(userName).ToString(), DateTime.Now, mtid, "Login", errorMsg);

                return errorMsg;
            }
            else
            {
                int isLoginValid = dl.IsDealerLoginValid(userName, passWord);

                if (isLoginValid == 1)
                {
                    SqlDataReader dr = dl.GetDealerDetails(userName);
                    string fullName = "", availableCredit = "", dealerEmail = "";

                    while (dr.Read())
                    {
                        fullName = dr["fullName"].ToString();
                        availableCredit = dr["availableCredit"].ToString();
                        dealerEmail = dr["dealerEmail"].ToString();
                    }

                    string verNum = dl.GetLatestMobileVersion().ToString();

                    dl.AddMultiLogin(userName);
                    //dl.AddLoginLocation(userName, longitude, latitude, address, mtid);

                    string format = "username_fullname_availableCredit_versionNum_apkLink_brandMsg|"; //vernum = @vernum + | + apkLink

                    string returnString = "1|" + format + userName + "|" + fullName + "|" + availableCredit + "|" + verNum + "|" + dl.GetPushMsg() + "|";

                    //log the mobile request
                    ClsPayment.LogMobileRequest2(dl.GetDealerCode(userName).ToString(), DateTime.Now, mtid, "Login", returnString);

                    return returnString;
                }
                else
                {
                    string errorMsg = "0|Your Login Is not VALID  ";
                    //log the mobile request
                    ClsPayment.LogMobileRequest2(dl.GetDealerCode(userName).ToString(), DateTime.Now, mtid, "Login", errorMsg);

                    return errorMsg;
                }
            }
        }

        public string GetPaymentLog(string acctNo, string dealerUserName, string dealerPassword, string mtid)
        {
            if (RequestIsValid(dealerUserName, dealerPassword))
            {
                //first things first, get any UGR for the dealer on that account number

                ClsPayment.RegenAnyUGR(acctNo, dealerUserName, DateTime.Now);

                //

                ClsDealer dl = new ClsDealer();

                string reqParams = "acctNo:" + acctNo + "|" +
                    "dealerUserName:" + dealerUserName
                    ;

                //log the mobile request
                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid + "|REQ", "GetPaymentLog", reqParams);

                mtid += "|RESP";

                string jsonString = "";

                if (acctNo.Length > 0)
                {
                    string results = "";

                    ClsPayment cp = new ClsPayment();
                    SqlDataReader dr = cp.GetPaymentLog(acctNo, "", DateTime.Now.AddDays(-365).ToString(), DateTime.Now.ToString());

                    if (dr.HasRows)
                    {
                        //dr = cp.GetPaymentLog(acctNo, "", DateTime.Now.AddDays(-365).ToString(), DateTime.Now.ToString());

                        while (dr.Read())
                        {
                            results += ("|" + dr["amount"].ToString() + "^" + dr["transactionDate"].ToString() + "^" + dr["channelRefNo"].ToString());
                        }

                        //log the mobile request
                        ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid, "GetPaymentLog", "1" + results);
                        jsonString = "1" + results;
                    }
                    else
                    {
                        //log the mobile request
                        ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid, "GetPaymentLog", "0|The account number has never been used on our platform");
                        jsonString = "0|The account number has never been used on our platform";
                    }
                }


                return jsonString;
            }
            else
            {
                ClsDealer dl = new ClsDealer();
                //log the mobile request
                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid, "GetPaymentLog", "0|Invalid_Request");

                return "0|Invalid_Request";
            }
        }

        public string GetReceipt(string channelRefNo, string dealerUserName, string dealerPassword, string mtid)
        {
            ClsDealer dl = new ClsDealer();
            string bottonMsg = dl.GetBottomMsg();

            if (RequestIsValid(dealerUserName, dealerPassword))
            {
                string reqParams = "channelRefNo:" + channelRefNo + "|" +
                    "dealerUserName:" + dealerUserName
                    ;

                //log the mobile request
                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid + "|REQ", "GetReceipt", reqParams);

                mtid += "|RESP";

                ClsPayment cp = new ClsPayment();
                DataTable dt = cp.GetPaymentLog2(channelRefNo);

                string acctNo = "", meterNo = "", address = "", sgc = "", tariff = "", purchasedAs = "", token = "", customer = "", time = "", colectionType = "", invoiceNo = "", bu = "", agent = "";

                foreach (DataRow dr in dt.Rows)
                {
                    string receiptNo = dr["receiptNo"].ToString();

                    if (channelRefNo.StartsWith("WT") || channelRefNo.StartsWith("MT") || channelRefNo.StartsWith("PT") || channelRefNo.StartsWith("NT"))
                    {
                        acctNo = dr["accountNumber"].ToString();
                        meterNo = dr["meterNumber"].ToString();
                        address = dr["address"].ToString();
                        sgc = dr["sgc"].ToString();
                        tariff = dr["tariff"].ToString();
                        purchasedAs = dr["units"].ToString() + "KWH at N" + dr["rate"].ToString() + " per KWH";
                        token = dr["token"].ToString();
                        customer = dr["customerName"].ToString();
                        time = dr["transactionDate"].ToString();

                        decimal txnAmt = decimal.Parse(dr["amount"].ToString());
                        decimal fixedCharge = dr["fixedCharge"].ToString() != "" ? decimal.Parse(dr["fixedCharge"].ToString()) : 0;

                        string rate = dr["rate"].ToString();
                        string units = dr["units"].ToString();
                        string cou = dr["costOfUnits"].ToString();

                        string vat = dr["vat"].ToString();
                        colectionType = dr["collectionType"].ToString();
                        invoiceNo = dr["receiptNo"].ToString();
                        bu = dr["businessUnit"].ToString();
                        agent = dr["dealerName"].ToString();

                        string returnString = "1|time_receiptNo_reprint_cdu_sgc_tariff_meterNo_custName_acctNo_address_transactionAmt_amtTendered_costOfUnit_fixedCharge_vat_unit_purchaseAs_tokens_bottomMsg_collectionType_invoiceNo_bu_agent|"
                        + time + "|" + receiptNo + "|" + dr["reprint"].ToString() + "|Operator Pawakad|" + sgc + "|" + tariff + "|" + meterNo + "|" + customer + "|" + acctNo + "|" + address + "|" + txnAmt + "|" + txnAmt + "|" + cou + "|" + fixedCharge + "|" + vat + "|" + dr["units"].ToString() + "|" + purchasedAs + "|" + token + "|" + bottonMsg + "|" + colectionType + "|" + invoiceNo + "|" + bu + "|" + agent + "|";// +creditBalance;

                        //log the mobile request
                        ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid, "GetReceipt", returnString);

                        return returnString;
                    }
                    else if (channelRefNo.StartsWith("WB") || channelRefNo.StartsWith("MB") || channelRefNo.StartsWith("PB") || channelRefNo.StartsWith("NB"))
                    {
                        string txnAmt = "", balance = "";
                        acctNo = dr["accountNumber"].ToString();
                        address = dr["address"].ToString();

                        txnAmt = string.Format("{0:#,###}", dr["amount"].ToString());

                        customer = dr["customerName"].ToString();
                        time = dr["transactionDate"].ToString();
                        balance = "N" + string.Format("{0:#,###}", dr["balance"].ToString());

                        colectionType = dr["collectionType"].ToString();
                        invoiceNo = dr["receiptNo"].ToString();
                        bu = dr["businessUnit"].ToString();
                        agent = dr["dealerName"].ToString();

                        string returnString = "1|time_receiptNo_reprint_cdu_sgc_tariff_meterNo_custName_acctNo_address_transactionAmt_amtTendered_costOfUnit_fixedCharge_vat_unit_purchaseAs_tokens_balance_bottomMsg_collectionType_invoiceNo_bu_agent|"
                        + time + "|" + receiptNo + "|" + dr["reprint"].ToString() + "|Operator Pawakad|" + sgc + "|" + tariff + "|" + meterNo + "|" + customer + "|" + acctNo + "|" + address + "|" + txnAmt.ToString().Substring(0, txnAmt.ToString().IndexOf('.') + 3) + "|" + txnAmt.ToString().Substring(0, txnAmt.ToString().IndexOf('.') + 3) + "|||||||" + balance + "|" + bottonMsg + "|" + colectionType + "|" + invoiceNo + "|" + bu + "|" + agent + "|";

                        //log the mobile request
                        ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid, "GetReceipt", returnString);


                        return returnString;

                    }
                }

                //log the mobile request
                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid, "GetReceipt", "0|No receipt");

                return "0|No receipt";
            }
            else
            {
                //log the mobile request
                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid, "GetReceipt", "0|Invalid_Request");

                return "0|Invalid_Request";
            }
        }

        public string GetFaq(string dealerUserName, string dealerPassword, string mtid)
        {
            if (RequestIsValid(dealerUserName, dealerPassword))
            {
                string jsonString = "";

                string results = "";

                ClsPayment cp = new ClsPayment();
                DataTable dr = cp.GetFaq();

                if (dr.Rows.Count > 0)
                {
                    foreach (DataRow row in dr.Rows)
                    {
                        results += ("|" + row["id"].ToString() + "^" + row["question"].ToString());
                    }

                    //ClsDealer dl = new ClsDealer();
                    ////log the mobile request
                    //ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid, "GetPaymentLog", "1" + results);
                    jsonString = "1" + results;
                }
                else
                {
                    //ClsDealer dl = new ClsDealer();
                    ////log the mobile request
                    //ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid, "GetPaymentLog", "0|The account number has never been used on our platform");
                    jsonString = "0|No question available";
                }



                return jsonString;
            }
            else
            {
                ClsDealer dl = new ClsDealer();
                //log the mobile request
                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid, "GetFaq", "0|Invalid_Request");

                return "0|Invalid_Request";
            }
        }

        public string GetAnsweredFaq(string id, string dealerUserName, string dealerPassword, string mtid)
        {
            ClsDealer dl = new ClsDealer();
            string bottonMsg = dl.GetBottomMsg();

            if (RequestIsValid(dealerUserName, dealerPassword))
            {
                ClsPayment cp = new ClsPayment();
                DataTable dt = cp.GetAnsweredFaq(id);

                string qid = "", question = "", answer = "";

                foreach (DataRow row in dt.Rows)
                {
                    qid = row["id"].ToString();
                    question = row["question"].ToString();
                    answer = row["answer"].ToString();

                    return "1|qid_question_answer|" + qid + "|" + question + "|" + answer;
                }

                //log the mobile request
                //ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid, "GetReceipt", "0|No receipt");

                return "0|Not yet Answered";
            }
            else
            {
                //log the mobile request
                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid, "GetAnsweredFaq", "0|Invalid_Request");

                return "0|Invalid_Request";
            }
        }

        public string GetMsgs(string dealerUserName, string dealerPassword, string mtid)
        {
            ClsDealer dl = new ClsDealer();

            string reqParams = "dealerUserName:" + dealerUserName;

            //log the mobile request
            ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid + "|REQ", "GetMsgs", reqParams);

            mtid += "|RESP";

            if (RequestIsValid(dealerUserName, dealerPassword))
            {
                string jsonString = "";

                string results = "";

                ClsPayment cp = new ClsPayment();
                DataTable dr = cp.GetMsgs();

                if (dr.Rows.Count > 0)
                {
                    foreach (DataRow row in dr.Rows)
                    {
                        results += ($"|{row["id"].ToString()}^{row["title"].ToString()}^{row["date"].ToString()}");
                    }

                    jsonString = "1" + results;

                }
                else
                {
                    jsonString = "0|No question available";
                }

                //log the mobile resp
                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid, "GetMsgs", jsonString);


                return jsonString;
            }
            else
            {
                //log the mobile resp
                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid, "GetMsgs", "0|Invalid_Request");

                return "0|Invalid_Request";
            }
        }

        public string GetMsg(string id, string dealerUserName, string dealerPassword, string mtid)
        {
            ClsDealer dl = new ClsDealer();

            string reqParams = $"id:{id}|dealerUserName:{dealerUserName}";

            //log the mobile request
            ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid + "|REQ", "GetMsg", reqParams);

            mtid += "|RESP";

            string jsonString = "";

            if (RequestIsValid(dealerUserName, dealerPassword))
            {
                ClsPayment cp = new ClsPayment();
                DataTable dt = cp.GetMsg(id);

                string title = "", message = "", date = "";

                foreach (DataRow row in dt.Rows)
                {
                    title = row["title"].ToString();
                    message = row["message"].ToString();
                    date = row["date"].ToString();

                    return "1|title_message_date|" + title + "|" + message + "|" + date;
                }
                
                jsonString = "0|Invalid Msg Id";
            }
            else
            {
                jsonString = "0|Invalid_Request";
            }

            //log the mobile request
            ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerUserName).ToString(), DateTime.Now, mtid, "GetMsg", jsonString); 

            return jsonString;
        }

        public string Logout(string dealerUserName)
        {
            ClsDealer dl = new ClsDealer();
            dl.RemoveMultiLoginByUserName(dealerUserName);

            return "1";
        }

        private bool RequestIsValid(string dealerUserName, string dealerPassword)
        {
            string userName = dealerUserName;
            // MAKE THE PASSWORD MD5 ENCRYPTION
            string passWord = MD5Crypt.MDee5(dealerPassword);
            ClsDealer dl = new ClsDealer();

            int isLoginValid = dl.IsDealerLoginValid(userName, passWord);

            return isLoginValid == 1 ? true : false;
        }
    }
}
