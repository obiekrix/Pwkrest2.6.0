using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;


namespace pawakadApp.ClsAdmin
{
    public class ClsDealerReports
    {
        internal SqlDataReader GetAmountByDealerCardNumber()
        {
            Cls.DataAccess da = new Cls.DataAccess();
            SqlDataReader dr = da.ExecuteReader("[UP_AmountByDealerCardNumber]", CommandType.StoredProcedure);
            return dr;
        }

    }
}