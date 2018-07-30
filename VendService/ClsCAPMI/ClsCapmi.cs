using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace pawakadApp.ClsCAPMI
{
    public class ClsCapmi
    {
        public int SaveCAPMI(string TransRef, string TypeOfMeter, string LandLordOrTenant, string SurName, string FirstName, string PhoneNumber, string AltPhoneNumber, string Email, string LandLordName, string TenantName, string Area, string Address, bool Attest)
        {
            Cls.DataAccess da = new Cls.DataAccess();
            da.AddParameter("@TransRef", SqlDbType.VarChar, ParameterDirection.Input, TransRef);
            da.AddParameter("@TypeOfMeter", SqlDbType.VarChar, ParameterDirection.Input, TypeOfMeter);
            da.AddParameter("@LandLordOrTenant", SqlDbType.VarChar, ParameterDirection.Input, LandLordOrTenant);
            da.AddParameter("@SurName", SqlDbType.VarChar, ParameterDirection.Input, SurName);
            da.AddParameter("@FirstName", SqlDbType.VarChar, ParameterDirection.Input, FirstName);
            da.AddParameter("@PhoneNumber", SqlDbType.VarChar, ParameterDirection.Input, PhoneNumber);
            da.AddParameter("@AltPhoneNumber", SqlDbType.VarChar, ParameterDirection.Input, AltPhoneNumber);
            da.AddParameter("@Email", SqlDbType.VarChar, ParameterDirection.Input, Email);
            da.AddParameter("@LandLordName", SqlDbType.VarChar, ParameterDirection.Input, LandLordName);
            da.AddParameter("@TenantName", SqlDbType.VarChar, ParameterDirection.Input, TenantName);
            da.AddParameter("@Area", SqlDbType.VarChar, ParameterDirection.Input, Area);
            da.AddParameter("@Address", SqlDbType.VarChar, ParameterDirection.Input, Address);
            da.AddParameter("@Attest", SqlDbType.VarChar, ParameterDirection.Input, Attest);
            int result = da.ExecuteNonQuery("[UP_CAPMIInfoInsert]", CommandType.StoredProcedure);
            return result;
        }

    }
}