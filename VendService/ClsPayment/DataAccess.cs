using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace pawakadApp.Cls
{

    public class DataAccess : IDisposable
    {
        SqlCommand m_Command;			// holds the command
        SqlConnection m_Connection;		// holds the connection
        SqlTransaction m_Transaction;   // holds the transaction

        public DataAccess()
            : this(false)
        {
        }

        public DataAccess(bool IsTransaction)
        {
            // setup the connection object
            m_Connection = new SqlConnection();
            //m_Connection.ConnectionString = @"Server=.\SQLExpress;Database=phc_db;User Id=sa;Password=katana;";
            m_Connection.ConnectionString = ConfigurationManager.ConnectionStrings["pwkConn"].ToString();

            try
            {
                // begin the transaction (if required)
                if (IsTransaction == true)
                {
                    // open the connection
                    OpenConnection();

                    // start the transaction
                    m_Transaction = m_Connection.BeginTransaction();
                }

                // reset the state of the object
                Reset();
            }
            finally
            {
                CloseConnection();
            }
        }

        public void Reset()
        {
            // setup the command object
            m_Command = new SqlCommand();
            m_Command.Connection = m_Connection;
            // add the transaction if we need to
            if (m_Transaction != null)
            {
                m_Command.Transaction = m_Transaction;
            }
        }

        public void Dispose()
        {
            // close the database connection
            CloseConnection();

            // don't want to call the GC
            GC.SuppressFinalize(this);
        }

        //Christian: changed access modifier from private to public
        public void OpenConnection()
        {
            if (m_Connection.State == ConnectionState.Closed)
            {
                m_Connection.Open();
            }
        }

        //Christian: changed access modifier from private to public
        public void CloseConnection()
        {
            if (m_Connection.State == ConnectionState.Open)
            {
                m_Connection.Close();
            }
        }

        public void CommitTransaction()
        {
            m_Transaction.Commit();
        }

        public void RollbackTransaction()
        {
            m_Transaction.Rollback();
        }

        public void AddParameter(string strName, SqlDbType objType, ParameterDirection objDirection)
        {
            SqlParameter l_Param = new SqlParameter();
            l_Param.ParameterName = strName;
            l_Param.SqlDbType = objType;
            l_Param.Direction = objDirection;
            m_Command.Parameters.Add(l_Param);
        }

        public void AddParameter(string strName, SqlDbType objType, ParameterDirection objDirection, object objValue)
        {
            AddParameter(strName, objType, objDirection);
            ModifyParameter(strName, objValue);
        }

        public object GetParameter(string strName)
        {
            // does the parameter exist
            if (m_Command.Parameters.IndexOf(strName) != 0)
            {
                return (m_Command.Parameters[strName].Value);
            }
            else
            {
                return (null);
            }
        }

        public void ModifyParameter(string strName, object objValue)
        {
            // we need to play nice with GUIDs
            if (m_Command.Parameters[strName].SqlDbType == SqlDbType.UniqueIdentifier)
            {
                // if a string then need to create a new GUID object
                if (objValue.GetType() == typeof(String))
                {
                    objValue = new System.Guid(objValue.ToString());
                }
            }

            // modify the value of the parameter
            m_Command.Parameters[strName].Value = objValue;
        }

        public void ClearParameters()
        {
            m_Command.Parameters.Clear();
        }
        public void RemoveParameter(string strName)
        {
            // does the parameter exist
            if (m_Command.Parameters.IndexOf(strName) != 0)
            {
                m_Command.Parameters.RemoveAt(m_Command.Parameters.IndexOf(strName));
            }
        }

        public SqlDataAdapter ExecuteAdapter(string strCommand)
        {
            return (ExecuteAdapter(strCommand, CommandType.StoredProcedure));
        }
        public SqlDataAdapter ExecuteAdapter(string strCommand, CommandType objType)
        {
            // set the properties correctly
            m_Command.CommandText = strCommand;
            m_Command.CommandType = objType;
            SqlDataAdapter objAdapter;
            try
            {
                // open the database connection
                OpenConnection();

                // create the data adapater object
                objAdapter = new SqlDataAdapter(m_Command);
            }
            finally
            {
                CloseConnection();
            }

            // return the adapter
            return (objAdapter);
        }

        public int ExecuteNonQuery(string strCommand)
        {
            return (ExecuteNonQuery(strCommand, CommandType.StoredProcedure));
        }
        public int ExecuteNonQuery(string strCommand, CommandType objType)
        {

            // set the properties correctly
            m_Command.CommandText = strCommand;
            m_Command.CommandType = objType;
            int intReturn;
            try
            {
                // open the database connection
                OpenConnection();

                // execute the query and return the correct result
                intReturn = m_Command.ExecuteNonQuery();
            }
            finally
            {
                // close the connection
                CloseConnection();
            }
            // return the result
            return (intReturn);
        }

        public SqlDataReader ExecuteReader(string strCommand)
        {
            return (ExecuteReader(strCommand, CommandType.StoredProcedure));
        }
        public SqlDataReader ExecuteReader(string strCommand, CommandType objType)
        {
            return (ExecuteReader(strCommand, objType, CommandBehavior.CloseConnection));
        }
        public SqlDataReader ExecuteReader(string strCommand, CommandType objType, CommandBehavior objBehaviour)
        {
            // set the properties correctly
            m_Command.CommandText = strCommand;
            m_Command.CommandType = objType;
            SqlDataReader objReader;
            try
            {
                // open the database connection
                OpenConnection();

            }
            finally
            {
                // execute the query and return the correct result
                objReader = m_Command.ExecuteReader(objBehaviour);
            }
            // return the reader
            return (objReader);
                
        }

        public object ExecuteScalar(string strCommand)
        {
            return (ExecuteScalar(strCommand, CommandType.StoredProcedure));
        }
        public object ExecuteScalar(string strCommand, CommandType objType)
        {
            // set the properties correctly
            m_Command.CommandText = strCommand;
            m_Command.CommandType = objType;
            object objReturn;
            try
            {
                // open the database connection
                OpenConnection();

                // execute the query and return the correct result
                objReturn = m_Command.ExecuteScalar();
            }
            finally
            {               
                // close the connection
                CloseConnection();
            }
            // return the result
            return (objReturn);
        }

        //Christian: 8/3/14
        public SqlConnection GetConnection()
        {
            return m_Connection;
        }
    }
}
