using pawakadApp.ClsAdmin;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace pawakadApp
{
    public class Misc
    {
        static string username = System.Configuration.ConfigurationManager.AppSettings["clientId"];
        static string password = System.Configuration.ConfigurationManager.AppSettings["clientKey"];

        private static string clientId = "clientId=" + username + "&clientKey=" + password;

        public static string GetClientId
        {
            get
            {
                return clientId;
            }
        }

        public static string SpaceTokens(StringBuilder token)
        {
            if (!token.ToString().Contains("&"))
            {
                if (token.ToString().IndexOf(" ") > 4)
                {
                    token.Insert(token.ToString().IndexOf(" ") - 4, " ");

                    SpaceTokens(token);
                }

                return token.ToString();
            }
            else
            {
                token.Replace("&", "");

                if (token.ToString().IndexOf(" ") > 4)
                {
                    token.Insert(token.ToString().IndexOf(" ") - 4, " ");

                    SpaceTokens(token);
                }

                token.Insert(25, "\b\n");
                token.Insert(52, "\b\n");

                return token.ToString();
            }
        }

        public static bool AlreadyTransacted(string mrUID, string dCode)
        {
            ClsDealer cd = new ClsDealer();

            int reqUIDCount = cd.GetRequestUniqueIDCount(mrUID, dCode);

            return reqUIDCount > 0;
        }

        public static bool IsDirectDownline(String parentDealer, String subDealer)
        {
            ClsDealer cls = new ClsDealer();

            SqlDataReader dr = cls.GetSubDealers(parentDealer);

            while (dr.Read())
            {
                if (dr["username"].ToString().Equals(subDealer))
                {
                    return true;
                }
            }

            return false;
        }

        public static string DecodeCT(string ctCode)
        {
            string ct = "";

            if (ctCode.Equals("NRG", StringComparison.OrdinalIgnoreCase))
            {
                ct = "ENERGY";
            }
            else if(ctCode.Equals("PEN", StringComparison.OrdinalIgnoreCase))
            {
                ct = "PENALTY";
            }
            else if(ctCode.Equals("RCN", StringComparison.OrdinalIgnoreCase))
            {
                ct = "RECONNECTION";
            }
            else if(ctCode.Equals("LOR", StringComparison.OrdinalIgnoreCase))
            {
                ct = "LOSS OF REVENUE";
            }

            return ct;
        }
    }
}