using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwkApi.VendCls
{
    class MsgIDGenerator
    {
        Random random;
        public string generateId()
        { 
            /*
            string dateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string uniqueNumber = random.Next(1000).ToString();
            string msgId = dateTime + uniqueNumber;
            return msgId;
            */
            string dateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string uniqueNumber = random.Next(1000000, 9999999).ToString();
            string msgId = dateTime + uniqueNumber;
            return msgId;

        }
    }
}
