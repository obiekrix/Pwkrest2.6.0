using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pawakadApp.clsJSON
{
    public class GenerateChannelRefNumber
    {

        public string channelReferenceNumber()
        {
            Random rnd = new Random();
            string strRnd = rnd.Next(10000, 99999).ToString();
            string channelReferenceNumber =DateTime.Now.ToString("WyyMMddhhmmssfffff")+strRnd;
            return channelReferenceNumber;
        }

    }
}