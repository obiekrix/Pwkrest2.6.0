using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace pawakadApp.WS
{
    public class WSCall
    {
        static string apiEndPoint = System.Configuration.ConfigurationManager.AppSettings["apiEndPoint"];

        public string GetJSON(string endPoint)
        {
            //string wsURL = "http://newtest.pwknigeria.com/pwkapiservice/svc/VendService.svc" + endPoint;// +"&" + Misc.GetClientId;

            //string wsURL = "http://newtest.pwknigeria.com/pwkapiliveservice/svc/VendService.svc" + endPoint + "&" + Misc.GetClientId;

            /////REMEMBER TO COMMENT THE FAKE CONLOG CALLS ////////
            string wsURL = apiEndPoint + endPoint;// + "&" + Misc.GetClientId;// +"&dcode=" + dCode;
            /////REMEMBER TO COMMENT THE FAKE CONLOG CALLS ////////

            //string wsURL = "http://localhost:70/mobifin/mobifin_api.php?pwkId=1234&service=echocheck&message=howNah";// +"&dcode=" + dCode;

            /////REMEMBER TO UNCOMMENT THE FAKE CONLOG CALLS ////////
            //string wsURL = "http://conlogie.test.pawakad.com/svc/VendService.svc" + endPoint;// +"&" + Misc.GetClientId;// +"&dcode=" + dCode;
            /////REMEMBER TO UNCOMMENT THE FAKE CONLOG CALLS ////////

            //string wsURL = "http://localhost:11571/svc/VendService.svc" + endPoint;// +"&dcode=" + dCode;

            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(wsURL);

            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            Stream Answer = WebResp.GetResponseStream();

            StreamReader _Answer = new StreamReader(Answer);

            string returnString = _Answer.ReadToEnd().ToString();

            return returnString.Replace("\"", "").Replace("\\", "");
        }
    }
}