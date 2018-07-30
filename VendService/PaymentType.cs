using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pawakadApp
{
    public class PaymentType
    {
        public string GetPaymentType(string number)
        {
            if (number.Length == 11)
            {
                return "PREPAID";
            }
            else if (number.Length == 12 || number.Length == 10)
            {
                return "POSTPAID";
            }

            return "PAYMENT_TYPE_UNKNOWN";
        }
    }
}