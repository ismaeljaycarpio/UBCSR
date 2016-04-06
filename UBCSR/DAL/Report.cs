using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web.Security;
using System.Data.SqlTypes;


namespace UBCSR.DAL
{
    public class Report
    {
        public string strSql = "";
        public static string CONN_STRING = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString();
        SqlConnection conn;
        SqlCommand comm;
        SqlDataAdapter adp;
        DataTable dTable;

        public DataTable searchReservation()
        {
            strSql = "SELECT Reservation.Id, Account.LastName + ' , ' + Account.FirstName + ' ' + Account.MiddleName AS [FullName], " +
                "Reservation.Subject, Reservation.ExperimentNo, Reservation.DateRequested, " +
                "Reservation.DateFrom, Reservation.DateTo, Reservation.LabRoom, Reservation.Status, " +
                "Reservation.ApprovalStatus " +
                "FROM " +
                "Reservation, Account " +
                "WHERE Reservation.UserId = Account.UserId";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                dTable = new DataTable();
                adp = new SqlDataAdapter(comm);

                conn.Open();
                adp.Fill(dTable);
                comm.Dispose();
                conn.Close();

                return dTable;
            }
        }
    }
}