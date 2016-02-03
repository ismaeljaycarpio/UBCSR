using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;


namespace UBCSR.DAL
{
    public class FileMaintenance
    {
        public string strSql = "";
        public static string CONN_STRING = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString();
        SqlConnection conn;
        SqlCommand comm;
        SqlDataAdapter adp;
        DataTable dTable;

        #region Category
        public void addCategory(string categoryName)
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

        public void editCategory(string categoryName)
        {
            strSql = "UPDATE ItemCategory SET CategoryName = @CategoryName";
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

        public void deleteCategory(string categoryId)
        {
            strSql = "DELETE ItemCategory WHERE Id = @Id";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@Id", categoryId);
                conn.Open();
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
        }

        public DataTable searchCategory(string categorySearch)
        {
            strSql = "SELECT * FROM ItemCategory WHERE CategoryName LIKE '%'@CategoryName'%'";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@CategoryName", categorySearch);
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

        #region Brand
        public void addBrand(string brandName)
        {
            strSql = "INSERT INTO ItemBrand VALUES(@BrandName)";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@BrandName", brandName);
                conn.Open();
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
        }

        public void editBrand(string brandName)
        {
            strSql = "UPDATE ItemBrand SET BrandName = @BrandName";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@BrandName", brandName);
                conn.Open();
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
        }

        public void deleteBrand(string brandId)
        {
            strSql = "DELETE ItemBrand WHERE Id = @Id";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@Id", brandId);
                conn.Open();
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
        }

        public DataTable searchBrand(string brandSearch)
        {
            strSql = "SELECT * FROM ItemBrand WHERE BrandName LIKE '%'@BrandName'%'";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@BrandName", brandSearch);
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

        #region Item
        public void addItem(string itemName)
        {
            strSql = "INSERT INTO Item VALUES(@ItemName)";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@ItemName", itemName);
                conn.Open();
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
        }

        public void editItem(string itemName)
        {
            strSql = "UPDATE Item SET ItemName = @ItemName";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@ItemName", itemName);
                conn.Open();
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
        }

        public void deleteItem(string itemId)
        {
            strSql = "DELETE Item WHERE Id = @Id";
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

        public DataTable searchItem(string itemSearch)
        {
            strSql = "SELECT * FROM Item WHERE ItemName LIKE '%'@ItemName'%'";
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

        //category
        public DataTable searchItemByCat(string itemSearch, string itemCat)
        {
            strSql = "SELECT * FROM Item WHERE ItemName LIKE '%'@ItemName'%' AND ItemCat = @ItemCat";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@ItemName", itemSearch);
                comm.Parameters.AddWithValue("@ItemCat", itemCat);
                dTable = new DataTable();
                adp = new SqlDataAdapter(comm);

                conn.Open();
                adp.Fill(dTable);
                comm.Dispose();
                conn.Close();

                return dTable;
            }
        }

        //brand
        public DataTable searchItemByBrand(string itemSearch, string itemBrand)
        {
            strSql = "SELECT * FROM Item WHERE ItemName LIKE '%'@ItemName'%' AND ItemBrand = @ItemBrand";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@ItemName", itemSearch);
                comm.Parameters.AddWithValue("@ItemBrand", itemBrand);
                dTable = new DataTable();
                adp = new SqlDataAdapter(comm);

                conn.Open();
                adp.Fill(dTable);
                comm.Dispose();
                conn.Close();

                return dTable;
            }
        }

        //category and brand
        public DataTable searchItemByBrand(string itemSearch, string itemCat, string itemBrand)
        {
            strSql = "SELECT * FROM Item WHERE ItemName LIKE '%'@ItemName'%' AND ItemBrand = @ItemBrand AND ItemCat = @ItemCat";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@ItemName", itemSearch);
                comm.Parameters.AddWithValue("@ItemBrand", itemBrand);
                comm.Parameters.AddWithValue("@ItemCat", itemCat);
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