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
    public class BorrowTransaction
    {

        public string strSql = "";
        public static string CONN_STRING = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString();
        SqlConnection conn;
        SqlCommand comm;
        SqlDataAdapter adp;
        DataTable dTable;

        public DataTable getBorrowed(string itemSearch, string roleName)
        {
            if (roleName.Equals("Student"))
            {
                strSql = "SELECT ";
                using (conn = new SqlConnection(CONN_STRING))
                {
                    comm = new SqlCommand(strSql, conn);
                    comm.Parameters.AddWithValue("@ItemName", itemSearch);
                    dTable = new DataTable();
                    adp = new SqlDataAdapter(comm);

                    conn.Open();
                    adp.Fill(dTable);
                    comm.Dispose();
                    conn.Close();

                    return dTable;
                }
            }
            else if(roleName.Equals("Instructor"))
            {

            }
            else{
                return dTable;
            }
            return dTable;
        }
    }
}