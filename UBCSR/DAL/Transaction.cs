using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web.Security;

namespace UBCSR.DAL
{
    public class Transaction
    {
        public string strSql = "";
        public static string CONN_STRING = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString();
        SqlConnection conn;
        SqlCommand comm;
        SqlDataAdapter adp;
        DataTable dTable;

        #region Inventory
        public void addInventory(string categoryName)
        {
            strSql = "INSERT INTO ItemCategory VALUES(@CategoryName)";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@CategoryName", categoryName);
                conn.Open();
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
        }
        #endregion
    }
}