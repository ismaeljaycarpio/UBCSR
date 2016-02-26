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
        public void addToInventory(string itemId, string stocks, DateTime expiration, string serial, string remarks)
        {
            strSql = "INSERT INTO Inventory VALUES(@ItemId,@Stocks,@Expiration,@Serial,@DateAdded,@Remarks)";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@ItemId", itemId);
                comm.Parameters.AddWithValue("@Stocks", stocks);
                comm.Parameters.AddWithValue("@Expiration", expiration);
                comm.Parameters.AddWithValue("@Serial", serial);
                comm.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                comm.Parameters.AddWithValue("@Remarks", remarks);
                conn.Open();
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
        }

        public void editInventory(string itemId, string stocks, 
            DateTime expiration, string serial, string remarks,
            string Id)
        {
            strSql = "UPDATE Inventory SET ItemId = @ItemId, Stocks = @Stocks, Expiration = @Expiration, " +
                "Serial = @Serial, Remarks = @Remarks " +
                "WHERE Id = @Id";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@ItemId", itemId);
                comm.Parameters.AddWithValue("@Stocks", stocks);
                comm.Parameters.AddWithValue("@Expiration", expiration);
                comm.Parameters.AddWithValue("@Serial", serial);
                comm.Parameters.AddWithValue("@Remarks", remarks);
                comm.Parameters.AddWithValue("@Id", Id);
                conn.Open();
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
        }

        public void updateStocks(string stocks, string Id)
        {
            strSql = "UPDATE Inventory SET Stocks = (Stocks + @Stocks) " +
                "WHERE Id = @Id";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@Stocks", stocks);
                comm.Parameters.AddWithValue("@Id", Id);
                conn.Open();
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
        }

        public void deleteInventory(string itemId)
        {
            strSql = "DELETE Inventory WHERE Id = @Id";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@Id", itemId);
                conn.Open();
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
        }

        public DataTable searchInventory(string itemSearch)
        {
            strSql = "SELECT Inventory.Id, Item.ItemName, ItemCategory.CategoryName, ItemBrand.BrandName, " +
                "Inventory.Stocks, Inventory.Expiration, Inventory.Serial, Inventory.Remarks " +
                "FROM " +
                "ItemCategory, ItemBrand, Item, Inventory " +
                "WHERE Item.ItemCategoryId = ItemCategory.Id AND " +
                "Item.ItemBrandId = ItemBrand.Id AND " +
                "Item.Id = Inventory.ItemId AND " +
                "Item.ItemName LIKE '%' + @ItemName + '%'";
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

        public DataTable getInventory(int rowId)
        {
            strSql = "SELECT * FROM Inventory WHERE Id = @Id";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@Id", rowId);
                dTable = new DataTable();
                adp = new SqlDataAdapter(comm);

                conn.Open();
                adp.Fill(dTable);
                comm.Dispose();
                conn.Close();

                return dTable;
            }
        }

        #endregion
    }
}