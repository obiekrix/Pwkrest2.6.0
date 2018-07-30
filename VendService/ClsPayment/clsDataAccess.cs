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

    public class clsDataAccess : IDisposable
    {
        SqlCommand m_Command;			// holds the command
        SqlConnection m_Connection;		// holds the connection
        SqlTransaction m_Transaction;   // holds the transaction

        public SqlConnection Connection
        {
            get
            {
                return m_Connection;
            }
        }

        public clsDataAccess()
            : this(false)
        {
        }

        public clsDataAccess(bool IsTransaction)
        {
            // setup the connection object
            m_Connection = new SqlConnection();
            m_Connection.ConnectionString = ConfigurationManager.ConnectionStrings["pwkConn"].ToString();

            // open the connection
            OpenConnection();

            try
            {
                // begin the transaction (if required)
                if (IsTransaction == true)
                {
                    // start the transaction
                    m_Transaction = m_Connection.BeginTransaction();
                } 
            }
            catch
            {
                CloseConnection();
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

        //Christian: 8/3/14
        public SqlConnection GetConnection()
        {
            return m_Connection;
        }
    }
}
