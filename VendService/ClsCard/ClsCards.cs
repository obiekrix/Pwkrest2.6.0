using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;


namespace pawakadApp.ClsCard
{
    public class ClsCards
    {
        internal SqlDataReader GetCArdPaymentLog(string cardPin)
        {

            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@cardPin", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, cardPin);
            SqlDataReader dr = da.ExecuteReader("UP_getPaymentMode", CommandType.StoredProcedure);
            return dr;
        }



        internal SqlDataReader GetCardDetailsTable(long cardSerial)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@cardSerial", System.Data.SqlDbType.BigInt, System.Data.ParameterDirection.Input, cardSerial);
            SqlDataReader dr = da.ExecuteReader("UP_GetCardDetailsTable", CommandType.StoredProcedure);
            return dr;
        }

        internal SqlDataReader DealerPerf1()
        {
            Cls.DataAccess da = new Cls.DataAccess();
            SqlDataReader dr = da.ExecuteReader("UP_DealerPerformance1", CommandType.StoredProcedure);
            return dr;
        }


        internal SqlDataReader GetCardStatus(long cardSerial, string userName)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@cardSerial", System.Data.SqlDbType.BigInt, System.Data.ParameterDirection.Input, cardSerial);
            da.AddParameter("@userName", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, userName);
            SqlDataReader dr = da.ExecuteReader("UPR_CardStatus", CommandType.StoredProcedure);
            return dr;
        }

        internal SqlDataReader GetAllocations(string allocatedby, string startDate, string endDate /*Christian: included these params*/, int startAt, int rows)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@allocatedby", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, allocatedby);
            da.AddParameter("@startDate", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, endDate);

            //Christian 11/2/14: included these params
            da.AddParameter("@startAt", System.Data.SqlDbType.Int, System.Data.ParameterDirection.Input, startAt);
            da.AddParameter("@rows", System.Data.SqlDbType.Int, System.Data.ParameterDirection.Input, rows);

            SqlDataReader dr = da.ExecuteReader("UPR_GetAllocation", CommandType.StoredProcedure);
            return dr;
        }

        //Christian 12/2/14:
        internal int GetAllocationsCount(string allocatedby, string startDate, string endDate)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@allocatedby", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, allocatedby);
            da.AddParameter("@startDate", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, endDate);


            int count = (int)da.ExecuteScalar("UPR_GetAllocationCount", CommandType.StoredProcedure);
            return count;
        }

        internal SqlDataReader GetDealerStatus(string dealerUserName)
        {

            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@dealerUserName", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dealerUserName);
            SqlDataReader dr = da.ExecuteReader("UPR_GetDealerStatus", CommandType.StoredProcedure);
            return dr;
        }

        internal SqlDataReader GetCardStatusByRange(long startSerial, long endSerial)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@startSerial", System.Data.SqlDbType.BigInt, System.Data.ParameterDirection.Input, startSerial);
            da.AddParameter("@endSerial", System.Data.SqlDbType.BigInt, System.Data.ParameterDirection.Input, endSerial);
            SqlDataReader dr = da.ExecuteReader("UP_CardValueByUsedUnUsed", CommandType.StoredProcedure);
            return dr;
        }

        internal SqlDataReader GetCardStatusByRangeDenom(int cardDenom, long startSerial, long endSerial, string dealer)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@cardDenom", System.Data.SqlDbType.BigInt, System.Data.ParameterDirection.Input, cardDenom);
            da.AddParameter("@startSerial", System.Data.SqlDbType.BigInt, System.Data.ParameterDirection.Input, startSerial);
            da.AddParameter("@endSerial", System.Data.SqlDbType.BigInt, System.Data.ParameterDirection.Input, endSerial);
            da.AddParameter("@sessionDealer", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dealer);
            SqlDataReader dr = da.ExecuteReader("UPR_CardStatusByRangeCarDenom", CommandType.StoredProcedure);
            return dr;
        }

        //Christian:8//3/14
        public SqlDataReader RunThisQuery(string query)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.OpenConnection();
            SqlCommand cmd = new SqlCommand(query, da.GetConnection());
            //da.AddParameter("@startDate", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, startDate);
            //da.AddParameter("@endDate", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, endDate);

            SqlDataReader dr = cmd.ExecuteReader();

            return dr;
        }

        //Christian:8//3/14
        public string RunThisQuery2(string query)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.OpenConnection();
            SqlCommand cmd = new SqlCommand(query, da.GetConnection());

            string dr = (string)cmd.ExecuteScalar();

            return dr;
        }

        public void RunNonQuery(string query)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.OpenConnection();
            SqlCommand cmd = new SqlCommand(query, da.GetConnection());

            cmd.ExecuteNonQuery();
        }

        //Christian: 13/05/14
        public SqlDataReader RunQueryForBreadCrumb(string query)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.OpenConnection();
            SqlCommand cmd = new SqlCommand(query, da.GetConnection());

            SqlDataReader dr = cmd.ExecuteReader();

            return dr;
        }

        //Christian:13/3/14
        public SqlDataReader GetSubDealerTotalAllocationWorth(string allocatedby, string subDealer, string allocationType, string startDate, string endDate)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@allocatedby", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, allocatedby);
            da.AddParameter("@subDealer", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, subDealer);
            da.AddParameter("@allocationType", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, allocationType);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);

            SqlDataReader dr = da.ExecuteReader("UPR_GetSubDealerTotalAllocationWorth", CommandType.StoredProcedure);

            return dr;
        }

        //Christian:13/3/14
        public SqlDataReader GetAllocationsFromParent(string allocatedby, string startDate, string endDate)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@allocatedby", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, allocatedby);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);

            SqlDataReader dr = da.ExecuteReader("UPR_GetAllocationsFromParent", CommandType.StoredProcedure);

            return dr;
        }

        //Christian:15/3/14
        public dynamic GetDealerTotalAllocationWorthToSubDealers(string allocatedby, string subDealer, string startDate, string endDate, string allocationType)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@allocatedby", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, allocatedby);
            da.AddParameter("@subDealer", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, subDealer);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);
            da.AddParameter("@allocationType", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, allocationType);

            dynamic total = da.ExecuteScalar("UPR_GetDealerTotalAllocationWorthToSubDealers", CommandType.StoredProcedure);

            return total;
        }

        //Christian:15/3/14
        public SqlDataReader GetDealerBreakDown(string allocatedby, string startDate, string endDate)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@allocatedby", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, allocatedby);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);

            SqlDataReader dr = da.ExecuteReader("UPR_GetDealerBreakDown", CommandType.StoredProcedure);

            return dr;
        }

        //Christian:15/3/14
        public dynamic GetDealerTotalAllocationWorth(string allocatedby, string startDate, string endDate)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@allocatedby", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, allocatedby);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);

            dynamic total = da.ExecuteScalar("UPR_GetDealerTotalAllocationWorth", CommandType.StoredProcedure);

            return total;
        }

        public SqlDataReader GetSubDealersTotalTransaction(string dCode, string tier, string startDate, string endDate)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@dealerCode", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dCode);
            da.AddParameter("@dealerTier", System.Data.SqlDbType.Int, System.Data.ParameterDirection.Input, int.Parse(tier));
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);

            SqlDataReader dr = da.ExecuteReader("UPR_GetSubDealersTotalTransaction", CommandType.StoredProcedure);

            return dr;
        }

        public SqlDataReader GetSubDealersTotalTransactionNew(string dCode, string tier, string startDate, string endDate)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@dealerCode", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dCode);
            da.AddParameter("@dealerTier", System.Data.SqlDbType.Int, System.Data.ParameterDirection.Input, int.Parse(tier));
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);

            SqlDataReader dr = da.ExecuteReader("UPR_GetSubDealersTotalTransactionNew", CommandType.StoredProcedure);

            return dr;
        }

        public dynamic GetDealerTotalTransactionWorth(/*string dCode, string tier*/ string dealer, string startDate, string endDate)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            //da.AddParameter("@dealerCode", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dCode);
            //da.AddParameter("@dealerTier", System.Data.SqlDbType.Int, System.Data.ParameterDirection.Input, int.Parse(tier));
            da.AddParameter("@dealer", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dealer);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);

            dynamic amt = da.ExecuteScalar("UPR_GetDealerTotalTransactionWorth", CommandType.StoredProcedure);

            return amt;
        }

        public dynamic GetDealerTotalTransactionWorthNew(/*string dCode, string tier*/ string dealer, string startDate, string endDate)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            //da.AddParameter("@dealerCode", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dCode);
            //da.AddParameter("@dealerTier", System.Data.SqlDbType.Int, System.Data.ParameterDirection.Input, int.Parse(tier));
            da.AddParameter("@dealer", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dealer);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);

            dynamic amt = da.ExecuteScalar("UPR_GetDealerTotalTransactionWorthNew", CommandType.StoredProcedure);

            return amt;
        }

        public SqlDataReader GetDealerBreakDown2(/*string dCode, string tier*/ string dealer, string startDate, string endDate)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            //da.AddParameter("@dealerCode", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dCode);
            //da.AddParameter("@dealerTier", System.Data.SqlDbType.Int, System.Data.ParameterDirection.Input, int.Parse(tier));
            da.AddParameter("@dealer", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dealer);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);

            SqlDataReader dr = da.ExecuteReader("UPR_GetDealerBreakDown2", CommandType.StoredProcedure);

            return dr;
        }

        public SqlDataReader GetDealerBreakDown2New(/*string dCode, string tier*/ string dealer, string startDate, string endDate)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            //da.AddParameter("@dealerCode", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dCode);
            //da.AddParameter("@dealerTier", System.Data.SqlDbType.Int, System.Data.ParameterDirection.Input, int.Parse(tier));
            da.AddParameter("@dealer", System.Data.SqlDbType.VarChar, System.Data.ParameterDirection.Input, dealer);
            da.AddParameter("@startDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, startDate);
            da.AddParameter("@endDate", System.Data.SqlDbType.DateTime, System.Data.ParameterDirection.Input, endDate);

            SqlDataReader dr = da.ExecuteReader("UPR_GetDealerBreakDown2New", CommandType.StoredProcedure);

            return dr;
        }

    }
}