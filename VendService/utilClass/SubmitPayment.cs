using pawakadApp.Cls;
using pawakadApp.ClsAdmin;
using pawakadApp.ClsCard;
using pawakadApp.Log;
using pawakadApp.Mailer;
using pawakadApp.WS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pawakadApp.PwkRESTWS.utilClass
{
    public class SubmitPayment
    {
        public static string PerformPayment(string meterNo, string amt, string dealerName, string custName, string custAddress, string crCode, string fixedCharge2,
            string conlogTID, string password, string mtid, string channelRefNo, string phoneNo, string collectionType = "NRG",
            string accountType = "", string orgName = "", string orgNo = "", string invoiceNo = "", string businessUnit = "")
        {
            string errorMsg = "";
            ClsLog cl = new ClsLog();
            ClsDealer dl = new ClsDealer();


            string reqParams = "meterNo:" + meterNo + "|" +
                "amt:" + amt + "|" +
                "dealerName:" + dealerName + "|" +
                "custName:" + custName + "|" +
                "custAddress:" + custAddress + "|" +
                "crCode:" + crCode + "|" +
                "fixedCharge2:" + fixedCharge2 + "|" +
                "conlogTID:" + conlogTID + "|" +
                "channelRefNo:" + channelRefNo + "|" +
                "collectionType:" + collectionType + "|" +
                "invoiceNo:" + invoiceNo + "|" +
                "phoneNo:" + phoneNo + "|" +
                "businessUnit:" + businessUnit
                ;

            //log the mobile request
            int rowCount = ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid + "|REQ", "SubmitPayment", reqParams, channelRefNo);

            if (rowCount != 1)
            {
                return $"0|The MTID {mtid} cannot be used again";
            }

            mtid += "|RESP";

            custName = custName.Replace("$$$", " ");
            custAddress = custAddress.Replace("$$$", " ");

            custName = custName.Replace("___", "&");
            custAddress = custAddress.Replace("___", "&");

            string[] arrCrCodesForToken = { "MT", "PT", "NT", "CT" };
            List<string> lstCrCodesForToken = arrCrCodesForToken.ToList();

            string[] arrCrCodesForBill = { "MB", "PB", "NB", "CB" };
            List<string> lstCrCodesForBill = arrCrCodesForBill.ToList();

            string[] arrCrCodesForLocal = { "MB", "MT" };
            List<string> lstCrCodesForLocal = arrCrCodesForLocal.ToList();


            ClsDealer cd = new ClsDealer();

            string dCode = cd.GetDealerCode(dealerName).ToString();

            ClsCards cls = new ClsCards();

            if (channelRefNo == "" || channelRefNo == null)
            {
                Guid guid = Guid.NewGuid();

                string tDate = DateTime.Today.ToString("yyMMdd").Replace("/", "");
                channelRefNo = crCode + tDate + dCode + "_" + guid.ToString();// +"_" + mtid;

                //log progress -- channelRefNo

                //cls.RunNonQuery("insert into LogPaymentAction(channelRefNo) values('" + channelRefNo + "')");
            }

            if (!Misc.AlreadyTransacted(mtid, dCode))
            {
                if (lstCrCodesForToken.Contains(crCode) && meterNo.Length == 11 || lstCrCodesForBill.Contains(crCode) && (meterNo.Length == 12 || meterNo.Length == 10))
                {
                    if (int.Parse(amt) > 0)
                    {
                        string returnString = "";
                        decimal initialAC = -1;
                        bool reverseNotAllowed = false;

                        try
                        {
                            if (cd.CheckDealerCredit(dealerName, int.Parse(amt)) == 1)
                            {
                                string getVars = (lstCrCodesForToken.Contains(crCode) ? "vendAmount=" + amt.Replace(",", "") + "&meterNo=" + meterNo : "meterNumber=" + meterNo + "&amntToPay=" + amt.Replace(",", ""));

                                WSCall wsc = new WSCall();

                                //SqlDataReader dr = dl.GetDealerDetails(dealerName);

                                string bottonMsg = dl.GetBottomMsg();
                                //get initial AC
                                initialAC = cd.GetAvailableDealerCredit(dealerName);

                                //log progress -- initialAC
                                //cls.RunNonQuery("update LogPaymentAction set initialAC='" + initialAC + "' where channelRefNo='" + channelRefNo + "'");
                                cls.RunNonQuery("insert into LogPaymentActionNew (ChannelRefNo,DCode,Channel,InitialAC,Amount,ActionName,ActionDetails) values ('" + channelRefNo + "','" + dCode + "','MOBILE','" + initialAC + "','" + amt + "','INITIAL-BALANCE','SUCCESS')");

                                if (collectionType.Equals("NRG"))
                                {
                                    string sgc = "", tariff = "", receiptNo = "", token = "", acctNo = "", rate = "0", units = "0", balance = "0", accountDescription = "", uniqueTID = "";
                                    string /*accountType = "", */dtName = "", dtNumber = ""/*, orgName = "", orgNo = ""*/;

                                    decimal cou = 0, vat = 0, fixedCharge = 0;

                                    string[] tokens = null;
                                    bool acNotDeducted = true, conlogNotCalled = true, aggNotLogged = true;

                                    try
                                    {

                                        if (acNotDeducted)
                                        {
                                            //deduct dealer's credit
                                            cd.DeductDealerAvailableCredit(dealerName, Convert.ToInt32(amt));
                                            acNotDeducted = false;
                                            returnString = "DEALER_JUST_DEBITED";

                                            //log progress -- acDeducted
                                            //cls.RunNonQuery("update LogPaymentAction set acDeducted='YES' where channelRefNo='" + channelRefNo + "'");
                                            cls.RunNonQuery("insert into LogPaymentActionNew (ChannelRefNo,DCode,Channel,InitialAC,Amount,ActionName,ActionDetails) values ('" + channelRefNo + "','" + dCode + "','MOBILE','" + initialAC + "','" + amt + "','ACCOUNT-DEDUCTED','SUCCESS')");
                                        }

                                        if (conlogNotCalled)
                                        {
                                            //call conlog
                                            //startTest
                                            //returnString = "1|TransactionId_CustomerName_CustomerPhoneNumber_CustomerAddress_ReceiptNumber_VendAmount_Token_AccountNumber_TransactionDateTime_SGC_units_Tarrif_Rate_tx|20170511020548~1000033|PAWAKAD1, TS|||PH062/2429|100.00|01234567890123456789|04040406714|5/11/2017 2:04:56 PM|399999|25.42|NEW TARIFF 10|1.00|4.58";
                                            //endTest

                                            returnString = wsc.GetJSON(String.Format(lstCrCodesForToken.Contains(crCode) ? "/creditvend{0}" : "/accountpayment{0}", "?" + Misc.GetClientId + "&" + getVars + "&paymentChannel=" + (lstCrCodesForLocal.Contains(crCode) ? "LOCAL" : "THIRD") + "_" + dCode + "&channelRefNo=" + channelRefNo));

                                            if (returnString == null) { returnString = ""; }

                                            conlogNotCalled = false;
                                            reverseNotAllowed = returnString.StartsWith("1") ? true : false;
                                            tokens = returnString != null ? returnString.Split('|') : null;

                                            //log progress -- conlogCalled
                                            //cls.RunNonQuery("update LogPaymentAction set conlogCalled='YES', conlogResp='" + returnString.Replace("'", "''") + "' where channelRefNo='" + channelRefNo + "'");
                                            cls.RunNonQuery("insert into LogPaymentActionNew (ChannelRefNo,DCode,Channel,InitialAC,Amount,ActionName,ActionDetails) values ('" + channelRefNo + "','" + dCode + "','MOBILE','" + initialAC + "','" + amt + "','CONLOG-CALLED','" + returnString.Replace("'", "''") + "')");
                                        }

                                        if (returnString != null && !returnString.StartsWith("0") && tokens[0] == "1")
                                        {
                                            int retrials = 0; bool retry = true;
                                            string returnString2 = "";
                                            //retry:

                                            while (retry)
                                            {
                                                DateTime transactionDate = DateTime.Now;

                                                returnString2 = "";
                                                try
                                                {

                                                    if (lstCrCodesForToken.Contains(crCode))
                                                    {
                                                        uniqueTID = tokens[2].Replace("~", "");
                                                        sgc = tokens[11];
                                                        tariff = tokens[13];
                                                        receiptNo = tokens[6];
                                                        token = tokens[8].ToString().Replace("&", "\n");
                                                        acctNo = tokens[9].Replace("/", "").Replace("-", "");

                                                        rate = (tokens[14] != "") ? Math.Round(decimal.Parse(tokens[14]), 2).ToString() : "0";
                                                        units = (tokens[12] != "") ? Math.Round(decimal.Parse(tokens[12]), 2).ToString() : "0";

                                                        //new params begin
                                                        accountType = tokens[16];
                                                        dtName = tokens[17];
                                                        dtNumber = tokens[18];
                                                        orgName = tokens[19];
                                                        orgNo = tokens[20];
                                                        //new params end

                                                        cou = Math.Round(decimal.Parse(tokens[14] != "" ? tokens[14] : "0") * decimal.Parse(tokens[12] != "" ? tokens[12] : "0"), 2);

                                                        vat = Math.Round(decimal.Parse(tokens[15] != "" ? tokens[15] : "0"), 2);
                                                        fixedCharge = Math.Round(decimal.Parse(amt) - cou - vat, 2);

                                                        if (fixedCharge < 1 && fixedCharge > 0)
                                                        {
                                                            fixedCharge -= fixedCharge;
                                                            cou += fixedCharge;
                                                        }
                                                        else if (fixedCharge < 0)
                                                        {
                                                            fixedCharge = 0;
                                                        }

                                                        //int sd = 0;
                                                        //int az = 1 / sd; 
                                                        //log progress -- aboutToLogPayment
                                                        //cls.RunNonQuery("update LogPaymentAction set aboutToLogPayment='YES' where channelRefNo='" + channelRefNo + "'");
                                                        cls.RunNonQuery("insert into LogPaymentActionNew (ChannelRefNo,DCode,Channel,InitialAC,Amount,ActionName,ActionDetails) values ('" + channelRefNo + "','" + dCode + "','MOBILE','" + initialAC + "','" + amt + "','ABOUT-TO-LOG-PAYMENT','SUCCESS')");

                                                        cd.ApplyPayment2(accountType, dtName, dtNumber, orgName, orgNo, phoneNo, "", acctNo, meterNo, int.Parse(amt), channelRefNo, DateTime.Now, token, sgc, tariff, custName, custAddress, cou, fixedCharge, vat, decimal.Parse(units), decimal.Parse(rate), 0, receiptNo, uniqueTID);

                                                        //log progress -- paymentLogged
                                                        //cls.RunNonQuery("update LogPaymentAction set paymentLogged='YES' where channelRefNo='" + channelRefNo + "'");
                                                        cls.RunNonQuery("insert into LogPaymentActionNew (ChannelRefNo,DCode,Channel,InitialAC,Amount,ActionName,ActionDetails) values ('" + channelRefNo + "','" + dCode + "','MOBILE','" + initialAC + "','" + amt + "','PAYMENT-LOGGED','SUCCESS')");

                                                        int creditBalance = (int)cd.GetAvailableDealerCredit(dealerName);

                                                        returnString2 = "1|time_receiptNo_reprint_cdu_sgc_tariff_meterNo_custName_acctNo_address_transactionAmt_amtTendered_costOfUnit_fixedCharge_vat_unit_purchaseAs_tokens_creditBalance_bottomMsg_AccountType_DtName_DtNumber_OrgName_OrgNo|"
                                                                + transactionDate + "|" + receiptNo + "|T589 / 8080990|Operator Pawakad|" + sgc + "|" + tariff + "|" + meterNo
                                                                + "|" + custName + "|" + acctNo + "|" + custAddress + "|" + amt + "|" + amt + "|" + cou.ToString() + "|" + fixedCharge.ToString()
                                                                + "|" + vat.ToString() + "|" + units + "|" + units + "KWh at N" + rate + " per KWh|" + token + "|" + creditBalance + "|" + bottonMsg + "|"
                                                         + $"{accountType}|{dtName}|{dtNumber}|{orgName}|{orgNo}|";


                                                        //log the mobile request
                                                        ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "SubmitPayment", returnString2, channelRefNo);

                                                    }
                                                    else
                                                    {
                                                        uniqueTID = tokens[2].Replace("~", "");
                                                        receiptNo = tokens[7];
                                                        balance = tokens[6];
                                                        accountDescription = tokens[4];

                                                        //new params begin
                                                        accountType = tokens[8];
                                                        dtName = tokens[9];
                                                        dtNumber = tokens[10];
                                                        orgName = tokens[11];
                                                        orgNo = tokens[12];
                                                        //new params end

                                                        //log progress -- aboutToLogPayment
                                                        //cls.RunNonQuery("update LogPaymentAction set aboutToLogPayment='YES' where channelRefNo='" + channelRefNo + "'");
                                                        cls.RunNonQuery("insert into LogPaymentActionNew (ChannelRefNo,DCode,Channel,InitialAC,Amount,ActionName,ActionDetails) values ('" + channelRefNo + "','" + dCode + "','MOBILE','" + initialAC + "','" + amt + "','ABOUT-TO-LOG-PAYMENT','SUCCESS')");

                                                        cd.ApplyPayment2(accountType, dtName, dtNumber, orgName, orgNo, phoneNo, "", meterNo, "", int.Parse(amt), channelRefNo, DateTime.Now, "", "", "", custName, custAddress, 0,
                                                            0, 0, 0, 0, decimal.Parse(balance), receiptNo, uniqueTID, collectionType);

                                                        //log progress -- paymentLogged
                                                        //cls.RunNonQuery("update LogPaymentAction set paymentLogged='YES' where channelRefNo='" + channelRefNo + "'");
                                                        cls.RunNonQuery("insert into LogPaymentActionNew (ChannelRefNo,DCode,Channel,InitialAC,Amount,ActionName,ActionDetails) values ('" + channelRefNo + "','" + dCode + "','MOBILE','" + initialAC + "','" + amt + "','PAYMENT-LOGGED','SUCCESS')");

                                                        //get the new availableCredit
                                                        int creditBalance = (int)cd.GetAvailableDealerCredit(dealerName);

                                                        //int sd = 0;
                                                        //int az = 1 / sd; 
                                                        returnString2 = "1|time_receiptNo_reprint_cdu_sgc_tariff_meterNo_custName_acctNo_address_transactionAmt_amtTendered_costOfUnit_fixedCharge_vat_unit_purchaseAs_tokens_availableCredit_balance_bottomMsg_AccountType_DtName_DtNumber_OrgName_OrgNo|"
                                                        + transactionDate + "|" + receiptNo + "|T589 / 8080990|Operator Pawakad|" + sgc + "|" + tariff + "|" + meterNo
                                                        + "|" + custName + "|" + meterNo + "|" + custAddress + "|" + amt + "|" + amt + "|" + cou + "|" + fixedCharge2
                                                        + "|" + vat + "|" + units + "|" + "|" + token + "|" + creditBalance + "||" + bottonMsg
                                                         + $"|{accountType}|{dtName}|{dtNumber}|{orgName}|{orgNo}"; ;

                                                        //log the mobile request
                                                        ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "SubmitPayment", returnString2, channelRefNo);

                                                    }
                                                    retry = false;
                                                }
                                                catch (Exception e)
                                                {
                                                    //log progress -- errorMsg
                                                    //cls.RunNonQuery("update LogPaymentAction set errorMsg='" + e.Message + "\n\n\n" + e.Source + "' where channelRefNo='" + channelRefNo + "'");
                                                    cls.RunNonQuery("insert into LogPaymentActionNew (ChannelRefNo,DCode,Channel,InitialAC,Amount,ActionName,ActionDetails) values ('" + channelRefNo + "','" + dCode + "','MOBILE','" + initialAC + "','" + amt + "','EXCEPTION','" + e.Message + "\n\n\n" + e.Source + "')");

                                                    if (retrials < 4)
                                                    {
                                                        retrials++;

                                                        //log progress -- retrials
                                                        //cls.RunNonQuery("update LogPaymentAction set retrials='" + retrials + "' where channelRefNo='" + channelRefNo + "'");
                                                        cls.RunNonQuery("insert into LogPaymentActionNew (ChannelRefNo,DCode,Channel,InitialAC,Amount,ActionName,ActionDetails) values ('" + channelRefNo + "','" + dCode + "','MOBILE','" + initialAC + "','" + amt + "','RETRIALS','" + retrials + "')");

                                                        //goto retry;
                                                    }
                                                    else
                                                    {
                                                        returnString2 = "0|YOUR TRANSACTION IS SUCCESSFUL BUT THE RECEIPT IS NOT READY FOR DISPLAY, PLEASE CHECK BACK IN AN HOUR TIME.";

                                                        string insertQuery =
                                                            "<br/>Meter Number: " + meterNo +
                                                            "<br/>Amount: " + amt +
                                                            "<br/>Address: " + custAddress +
                                                            "<br/>UniqueTID: " + uniqueTID +
                                                            "<br/>Dealer Name: " + dealerName +
                                                            "<br/>CR Code: " + crCode; ;

                                                        PortalMailer mailer = new PortalMailer();

                                                        mailer.subject = "Un-generated Receipt.";
                                                        mailer.emailTo = "support@pawakad.com";
                                                        mailer.body = "Details for un-generated receipt<br/><br/>" + insertQuery;

                                                        mailer.SendMail();


                                                        cd.RegeneratePayment(acctNo, meterNo, int.Parse(amt), channelRefNo + "_MG", token, sgc, tariff, custName, custAddress, cou,
                                                           fixedCharge, vat, decimal.Parse(units), decimal.Parse(rate), decimal.Parse(balance), receiptNo, uniqueTID, collectionType);

                                                        retry = false;
                                                    }
                                                }
                                            }
                                            return returnString2;
                                        }
                                        else
                                        {
                                            //cd.ReverseTransaction(dealerName, Convert.ToInt32(amt), meterNo, tokens[1], initialAC, tokens[1].Split('~')[1]);
                                            cd.ReverseTransaction(dealerName, Convert.ToInt32(amt), meterNo, tokens[1].Split('^')[0], initialAC, tokens[1].Split('^')[1]);
                                            reverseNotAllowed = true;

                                            cl.LogPaymentError(meterNo, channelRefNo, int.Parse(amt), tokens[1], "PAYMENT", DateTime.Now/*, dCode, DateTime.Now, false*/);

                                            if (tokens[1].ToLower().Contains("block"))
                                            {
                                                errorMsg = "0|This account number is blocked, pls contact Ikeja Electric Customer Care on <br/>0800 022 5543 or 01 448 3900 or 0700 022 5543";
                                                //log the mobile request
                                                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "SubmitPayment", errorMsg, channelRefNo);

                                                return errorMsg;
                                            }
                                            else if (tokens[1].ToLower().Contains("arrears to be paid"))
                                            {
                                                errorMsg = "0|The amount you are trying to pay is too low, pls pay above " + fixedCharge2;
                                                //log the mobile request
                                                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "SubmitPayment", errorMsg, channelRefNo);

                                                return errorMsg;
                                            }
                                            else if (tokens[1].ToLower().Contains("meter not found"))
                                            {
                                                errorMsg = "0|This account number is invalid ";
                                                //log the mobile request
                                                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "SubmitPayment", errorMsg, channelRefNo);

                                                return errorMsg;
                                            }
                                            else if (tokens[1].ToLower().Contains("any instance"))
                                            {
                                                errorMsg = "0|This account number cannot be processed for payment, pls contact Ikeja Electric Customer Care on <br/>0800 022 5543 or 01 448 3900 or 0700 022 5543";

                                                //log the mobile request
                                                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "SubmitPayment", errorMsg, channelRefNo);

                                                return errorMsg;
                                            }
                                            else if (tokens[1].ToLower().Contains("no open account match"))
                                            {
                                                errorMsg = "0|This account number is invalid";

                                                //log the mobile request
                                                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "SubmitPayment", errorMsg, channelRefNo);

                                                return errorMsg;
                                            }
                                            else
                                            {
                                                errorMsg = "0|" + tokens[1];

                                                //log the mobile request
                                                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "SubmitPayment", errorMsg, channelRefNo);

                                                return errorMsg;
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        if (!reverseNotAllowed)
                                        {
                                            cd.ReverseTransaction(dealerName, Convert.ToInt32(amt), meterNo, tokens[1].Split('^')[0], initialAC, tokens[1].Split('^')[1]);
                                            reverseNotAllowed = true;
                                        }
                                        return "0|" + e.Message;
                                    }
                                }
                                else
                                {
                                    //rework the receipt to avoid issues where the agent enters an existing receipt no
                                    invoiceNo += ("-" + conlogTID.Replace("~", ""));

                                    //Boolean retry = true;
                                    //while (retry)
                                    //{
                                    try
                                    {
                                        //if LOR2, just write to LogTable
                                        if (meterNo.Equals("0000000000"))
                                        {
                                            string query = "insert into logTable(clientId, uniqueTransactionId, logMsgType, inRequestMsg, outResponseMsg, responseDateTime, paymentChannel, channelRefNo) " +
                                            "values('" +
                                            "Pawakad" + "','" +
                                            conlogTID.Replace("~", "") + "','" +
                                            "CONFIRM-CUSTOMER" + "','" +
                                            (meterNo.Length == 12 ? "PP" : "") + meterNo + "|Pawakad" + "','" +
                                            "" + "','" +
                                            DateTime.Now + "','" +
                                            "NER_LOCAL_" + dCode + "','" +
                                            channelRefNo +
                                            "')";

                                            cls.RunNonQuery(query);
                                        }
                                        else
                                        {
                                            new pawakadApp.ClsCard.ClsCards().RunNonQuery("update LogTable set PaymentChannel='NER_" + (lstCrCodesForLocal.Contains(crCode) ? "LOCAL" : "THIRD") + "_" + dCode + "' where UniqueTransactionId='" + conlogTID.Replace("~", "") + "'");
                                        }

                                        //deduct dealer balance
                                        cd.DeductDealerAvailableCredit(dealerName, Convert.ToInt32(amt));

                                        //int sd = 0;
                                        //int az = 1 / sd; 
                                        //log progress -- acDeducted
                                        //cls.RunNonQuery("update LogPaymentAction set acDeducted='YES',amount=" + int.Parse(amt) + " where channelRefNo='" + channelRefNo + "'");
                                        cls.RunNonQuery("insert into LogPaymentActionNew (ChannelRefNo,DCode,Channel,InitialAC,Amount,ActionName,ActionDetails) values ('" + channelRefNo + "','" + dCode + "','MOBILE','" + initialAC + "','" + amt + "','ACCOUNT-DEDUCTED','SUCCESS')");

                                        //get the new availableCredit
                                        int creditBalance = (int)cd.GetAvailableDealerCredit(dealerName);

                                        //log progress -- aboutToLogPayment
                                        //cls.RunNonQuery("update LogPaymentAction set aboutToLogPayment='YES' where channelRefNo='" + channelRefNo + "'");
                                        cls.RunNonQuery("insert into LogPaymentActionNew (ChannelRefNo,DCode,Channel,InitialAC,Amount,ActionName,ActionDetails) values ('" + channelRefNo + "','" + dCode + "','MOBILE','" + initialAC + "','" + amt + "','ABOUT-TO-LOG-PAYMENT','SUCCESS')");

                                        cd.ApplyPayment2(accountType,"","",orgName,orgNo,phoneNo, "", meterNo, "", int.Parse(amt), channelRefNo, DateTime.Now, "", "", "", custName, custAddress, 0,
                                            0, 0, 0, 0, 0, invoiceNo, conlogTID.Replace("~", ""), collectionType);

                                        //set this value to avoid accidental reversal after logging to paymentLog
                                        reverseNotAllowed = true;

                                        //log progress -- paymentLogged
                                        //cls.RunNonQuery("update LogPaymentAction set paymentLogged='YES' where channelRefNo='" + channelRefNo + "'");
                                        cls.RunNonQuery("insert into LogPaymentActionNew (ChannelRefNo,DCode,Channel,InitialAC,Amount,ActionName,ActionDetails) values ('" + channelRefNo + "','" + dCode + "','MOBILE','" + initialAC + "','" + amt + "','PAYMENT-LOGGED','SUCCESS')");

                                        string returnString2 = "1|time_receiptNo_reprint_cdu_sgc_tariff_meterNo_custName_acctNo_address_transactionAmt_amtTendered_costOfUnit_fixedCharge_vat_unit_purchaseAs_tokens_availableCredit_" + (meterNo.Length == 11 ? "" : "balance_") + "bottomMsg_AccountType_DtName_DtNumber_OrgName_OrgNo|"
                                                        + DateTime.Now + "|" + invoiceNo + "|T589 / 8080990|Operator Pawakad|" + "" + "|" + "" + "|" + meterNo
                                                        + "|" + custName + "|" + meterNo + "|" + custAddress + "|" + amt + "|" + amt + "|" + "" + "|" + ""
                                                        + "|" + "" + "|" + "" + "|" + "|" + "" + "|" + creditBalance + "|" + (meterNo.Length == 11 ? "" : "|") + bottonMsg + $"|{accountType}|||{orgName}|{orgNo}|";

                                        //log the mobile request
                                        ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "SubmitPayment", returnString2, channelRefNo);

                                        //retry = false;

                                        return returnString2;
                                    }
                                    catch (Exception e)
                                    {
                                        if (!reverseNotAllowed)
                                        {
                                            cd.ReverseTransaction(dealerName, Convert.ToInt32(amt), meterNo, e.Message, initialAC, collectionType + "_" + conlogTID);
                                            reverseNotAllowed = true;
                                        }
                                        return "0|" + e.Message;
                                    }
                                    //}
                                }
                            }
                            else
                            {
                                errorMsg = "0|Insufficient Available Credit";
                                cl.LogPaymentError(meterNo, channelRefNo, int.Parse(amt), errorMsg, "PAYMENT", DateTime.Now/*, dCode, DateTime.Now, false*/);
                                //log the mobile request
                                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "SubmitPayment", errorMsg, channelRefNo);

                                //dl.AddLoginLocation(dealerName, longitude, latitude, address, mtid);
                                return errorMsg;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (returnString.Equals("DEALER_JUST_DEBITED"))
                            {
                                cd.ReverseTransaction(dealerName, Convert.ToInt32(amt), meterNo, ex.Message, initialAC, "-1" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000000, 9999999).ToString());
                                reverseNotAllowed = true;
                            }

                            return "0|Error - ENSURE to confirm availability of receipt from the Payment History BEFORE retrying";
                        }
                    }
                    else
                    {
                        errorMsg = "0|Amount must be greater than ZER0";
                        cl.LogPaymentError(meterNo, channelRefNo, int.Parse(amt), errorMsg, "PAYMENT", DateTime.Now/*, dCode, DateTime.Now, false*/);
                        //log the mobile request
                        ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "SubmitPayment", errorMsg, channelRefNo);

                        //dl.AddLoginLocation(dealerName, longitude, latitude, address, mtid);
                        return errorMsg;
                    }
                }
                else
                {
                    errorMsg = "0|Wrong CRCode was specified.";
                    //log the mobile request
                    ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "SubmitPayment", errorMsg, channelRefNo);

                    //dl.AddLoginLocation(dealerName, longitude, latitude, address, mtid);
                    return errorMsg;
                }
            }
            else
            {
                errorMsg = "0|Wrong channel ref code";
                cl.LogPaymentError(meterNo, channelRefNo, int.Parse(amt), errorMsg, "PAYMENT", DateTime.Now/*, dCode, DateTime.Now, false*/);

                //log the mobile request
                ClsPayment.LogMobileRequest2(dl.GetDealerCode(dealerName).ToString(), DateTime.Now, mtid, "SubmitPayment", errorMsg, channelRefNo);

                //dl.AddLoginLocation(dealerName, longitude, latitude, address, mtid);
                return errorMsg;
            }

        }
    }
}