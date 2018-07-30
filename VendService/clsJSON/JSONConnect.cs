using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Specialized;
using System.Configuration;


namespace pawakadApp.Cls
{
    public class JSONConnect
    {
        //string principal = ConfigurationManager.AppSettings["pwkPrincipal"].ToString();
        //string credentials = ConfigurationManager.AppSettings["pwkCredentials"].ToString();
        //string strGetCustomerUrl = ConfigurationManager.AppSettings["strGetCustomerUrl"].ToString();
        //string strSubmitPaymentUrl = ConfigurationManager.AppSettings["strSubmitPaymentUrl"].ToString();
        //string strGetPaymentDetailsUrl = ConfigurationManager.AppSettings["strGetPaymentDetailsUrl"].ToString();

        //public dynamic GetCustomerDetailsByAccountNumber(string customerAccountNumber)
        //{
        //   // ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;

        //    WebClient webClient = new WebClient();

        //    NameValueCollection formData = new NameValueCollection();
        //    formData["pawakad.principal"] = principal;
        //    formData["pawakad.credentials"] = credentials;
        //    formData["customerAccountNumber"] = customerAccountNumber;

        //    byte[] responseBytes = webClient.UploadValues(strGetCustomerUrl, "POST", formData);
        //    string jsonResult = Encoding.UTF8.GetString(responseBytes);
        //    //dynamic dynJson = JsonConvert.DeserializeObject(jsonResult);
        //    webClient.Dispose();

        //    return dynJson;
        //}

        //public dynamic GetPaymentDetailsByChannelRefNo(string channelReferenceNumber)
        //{
        //    WebClient webClient = new WebClient();

        //    NameValueCollection formData = new NameValueCollection();
        //    formData["pawakad.principal"] = principal;
        //    formData["pawakad.credentials"] = credentials;
        //    formData["channelReferenceNumber"] = channelReferenceNumber;

        //    byte[] responseBytes = webClient.UploadValues(strGetPaymentDetailsUrl, "POST", formData);
        //    string jsonResult = Encoding.UTF8.GetString(responseBytes);
        //    dynamic dynJson = JsonConvert.DeserializeObject(jsonResult);
        //    webClient.Dispose();

        //    return dynJson;
        //}

        //public dynamic SubmitPayment(string customerAccountNumber, string channelReferenceNumber, string paymentAmount)
        //{
        //    WebClient webClient = new WebClient();

        //    NameValueCollection formData = new NameValueCollection();
        //    formData["pawakad.principal"] = principal;
        //    formData["pawakad.credentials"] = credentials;
        //    formData["customerAccountNumber"] = customerAccountNumber;
        //    formData["channelReferenceNumber"] = channelReferenceNumber;
        //    formData["paymentAmount"] = paymentAmount;

        //    byte[] responseBytes = webClient.UploadValues(strSubmitPaymentUrl, "POST", formData);
        //    string jsonResult = Encoding.UTF8.GetString(responseBytes);
        //    dynamic dynJson = JsonConvert.DeserializeObject(jsonResult);
        //    webClient.Dispose();

        //    return dynJson;
        //}

        //public string GetChannelReferenceNumber()
        //{
        //    Random rnd = new Random();
        //    string strRnd = rnd.Next(100000000, 999999999).ToString();
        //    // SCR stands for scratch card
        //    string channelReferenceNumber = DateTime.Now.ToString("SCRyyMMddHHmmssfffff") + strRnd;
        //    return channelReferenceNumber;
        //}


    }
}