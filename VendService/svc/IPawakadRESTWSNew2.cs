using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Web.Services;

namespace PwkRESTWS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPawakadRESTWS" in both code and config file together.
    [ServiceContract]
    public interface IPawakadRESTWSNew2
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "SayWassup/{toWho}")]
        string SayWassup(string toWho);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ValidateCustomerAndEPIN?acctNo={acctNo}&amt={amt}&dealerUserName={dealerUserName}&dealerPassword={dealerPassword}&mtid={mtid}&businessUnit={businessUnit}")]
        string ValidateCustomerAndEPIN(string acctNo, string amt, string dealerUserName, string dealerPassword, string mtid, string businessUnit);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "SubmitPayment?acctNo={acctNo}&amt={amt}&dealerUserName={dealerUserName}&custName={custName}&custAddress={custAddress}&crCode={crCode}&fixedCharge={fixedCharge}&conlogTID={conlogTID}&dealerPassword={dealerPassword}&mtid={mtid}&collectionType={collectionType}&accountType={accountType}&orgName={orgName}&orgNo={orgNo}&invoiceNo={invoiceNo}&businessUnit={businessUnit}&channelRefNo={channelRefNo}&phoneNo={phoneNo}")]
        string SubmitPayment(string acctNo, string amt, string dealerUserName, string custName, string custAddress, string crCode, string fixedCharge, string conlogTID, string dealerPassword, string mtid, string channelRefNo, string phoneNo, string collectionType, string accountType, string orgName, string orgNo, string invoiceNo, string businessUnit);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Login?dealerUserName={dealerUserName}&dealerPassword={dealerPassword}&mtid={mtid}")]
        string Login(string dealerUserName, string dealerPassword, string mtid);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "GetPaymentLog?acctNo={acctNo}&dealerUserName={dealerUserName}&dealerPassword={dealerPassword}&mtid={mtid}")]
        string GetPaymentLog(string acctNo, string dealerUserName, string dealerPassword, string mtid);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "GetReceipt?channelRefNo={channelRefNo}&dealerUserName={dealerUserName}&dealerPassword={dealerPassword}&mtid={mtid}")]
        string GetReceipt(string channelRefNo, string dealerUserName, string dealerPassword, string mtid);

        //[OperationContract]
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "SubmitProduct?productName={productName}&productPrice={productPrice}&customerName={customerName}&customerEmail={customerEmail}")]
        //string SubmitProduct(string productName, string productPrice, string customerName, string customerEmail);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Logout?dealerUserName={dealerUserName}")]
        string Logout(string dealerUserName);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "GetFaq?dealerUserName={dealerUserName}&dealerPassword={dealerPassword}&mtid={mtid}")]
        string GetFaq(string dealerUserName, string dealerPassword, string mtid);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "GetAnsweredFaq?qid={id}&dealerUserName={dealerUserName}&dealerPassword={dealerPassword}&mtid={mtid}")]
        string GetAnsweredFaq(string id, string dealerUserName, string dealerPassword, string mtid);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "GetMsgs?dealerUserName={dealerUserName}&dealerPassword={dealerPassword}&mtid={mtid}")]
        string GetMsgs(string dealerUserName, string dealerPassword, string mtid);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "GetMsg?qid={id}&dealerUserName={dealerUserName}&dealerPassword={dealerPassword}&mtid={mtid}")]
        string GetMsg(string id, string dealerUserName, string dealerPassword, string mtid);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ResetPassword?dealerUserName={dealerUserName}&dealerEmail={dealerEmail}&mtid={mtid}")]
        string ResetPassword(string dealerUserName, string dealerEmail, string mtid);
    }
}
