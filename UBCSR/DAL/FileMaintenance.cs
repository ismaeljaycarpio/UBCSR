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
    public class FileMaintenance
    {
        public string strSql = "";
        public static string CONN_STRING = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString();
        SqlConnection conn;
        SqlCommand comm;
        SqlDataAdapter adp;
        DataTable dTable;

        #region CategoryType
        public void addCategoryType(string categoryType)
        {
            strSql = "INSERT INTO ItemCategoryType VALUES(@CategoryType)";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@CategoryType", categoryType);
                conn.Open();
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
        }

        public void editCategoryType(string categoryType, string Id)
        {
            strSql = "UPDATE ItemCategoryType SET CategoryType = @CategoryType WHERE Id = @Id";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@CategoryType", categoryType);
                comm.Parameters.AddWithValue("@Id", Id);
                conn.Open();
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
        }

        public void deleteCategoryType(string categoryId)
        {
            strSql = "DELETE ItemCategoryType WHERE Id = @Id";
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

        public DataTable searchCategoryType(string categoryType)
        {
            strSql = "SELECT * FROM ItemCategoryType WHERE CategoryType LIKE '%' + @CategoryType + '%'";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@CategoryType", categoryType);
                dTable = new DataTable();
                adp = new SqlDataAdapter(comm);

                conn.Open();
                adp.Fill(dTable);
                comm.Dispose();
                conn.Close();

                return dTable;
            }
        }

        public DataTable getCategoryType(int categoryId)
        {
            strSql = "SELECT * FROM ItemCategoryType WHERE Id = @Id";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@Id", categoryId);
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

        #region Category
        public void addCategory(string categoryName, string categoryTypeId)
        {
            strSql = "INSERT INTO ItemCategory VALUES(@CategoryName,@CategoryTypeId)";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@CategoryName", categoryName);
                comm.Parameters.AddWithValue("@CategoryTypeId", categoryTypeId);
                conn.Open();
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
        }

        public void editCategory(string categoryName, string categoryTypeId, string Id)
        {
            strSql = "UPDATE ItemCategory SET CategoryName = @CategoryName, CategoryTypeId = @CategoryTypeId WHERE Id = @Id";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@CategoryName", categoryName);
                comm.Parameters.AddWithValue("@CategoryTypeId", categoryTypeId);
                comm.Parameters.AddWithValue("@Id", Id);
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
            strSql = "SELECT ItemCategory.Id, ItemCategory.CategoryName, ItemCategoryType.CategoryType " +
                "FROM ItemCategory, ItemCategoryType " +
                "WHERE " +
                "ItemCategoryType.Id = ItemCategory.CategoryTypeId AND " +
                "CategoryName LIKE '%' + @CategoryName + '%'";
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

        public DataTable getCategory(int categoryId)
        {
            strSql = "SELECT * FROM ItemCategory WHERE Id = @Id";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@Id", categoryId);
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

        public void editBrand(string brandName, string Id)
        {
            strSql = "UPDATE ItemBrand SET BrandName = @BrandName WHERE Id = @Id";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@BrandName", brandName);
                comm.Parameters.AddWithValue("@Id", Id);
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
            strSql = "SELECT * FROM ItemBrand WHERE BrandName LIKE '%'+ @BrandName + '%'";
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

        public DataTable getBrand(int brandId)
        {
            strSql = "SELECT * FROM ItemBrand WHERE Id = @Id";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@Id", brandId);
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
        public void addItem(string itemCatId, string itemBrandId, string itemName)
        {
            strSql = "INSERT INTO Item VALUES(@ItemCategoryId,@ItemBrandId,@ItemName)";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@ItemCategoryId", itemCatId);
                comm.Parameters.AddWithValue("@ItemBrandId", itemBrandId);
                comm.Parameters.AddWithValue("@ItemName", itemName);
                conn.Open();
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
        }

        public void editItem(string itemCatId, string itemBrandId, string itemName, string Id)
        {
            strSql = "UPDATE Item SET ItemCategoryId = @ItemCategoryId, ItemBrandId = @ItemBrandId, ItemName = @ItemName WHERE Id = @Id";
            using (conn = new SqlConnection(CONN_STRING))
            {
                comm = new SqlCommand(strSql, conn);
                comm.Parameters.AddWithValue("@ItemCategoryId", itemCatId);
                comm.Parameters.AddWithValue("@ItemBrandId", itemBrandId);
                comm.Parameters.AddWithValue("@ItemName", itemName);
                comm.Parameters.AddWithValue("@Id", Id);
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
            strSql = "SELECT Item.Id, ItemCategory.CategoryName, ItemBrand.BrandName, Item.ItemName FROM " +
                "ItemCategory, ItemBrand, Item " +
                "WHERE Item.ItemCategoryId = ItemCategory.Id AND " +
                "Item.ItemBrandId = ItemBrand.Id AND " +
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

        public DataTable getItem(int rowId)
        {
            strSql = "SELECT * FROM Item WHERE Id = @Id";
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

        //category
        public DataTable searchItemByCat(string itemSearch, string itemCat)
        {
            strSql = "SELECT * FROM Item WHERE ItemName LIKE '%' + @ItemName + '%' AND ItemCategoryId = @ItemCat";
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
            strSql = "SELECT * FROM Item WHERE ItemName LIKE '%'@ItemName'%' AND ItemBrandId = @ItemBrandId";
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

        #region Account
        public DataTable searchUser(string searchKeyWord)
        {
            strSql = "SELECT Account.StudentId, Memberships.UserId, Memberships.IsApproved, " +
                "Roles.RoleName, " +
                "(Account.LastName + ', ' + Account.FirstName + ' ' + Account.MiddleName) AS [FullName] " +
                "FROM Memberships " +
                "LEFT JOIN UsersInRoles " +
                "ON Memberships.UserId = UsersInRoles.UserId " +
                "LEFT JOIN Roles " +
                "ON Roles.RoleId = UsersInRoles.RoleId " +
                "LEFT JOIN Account " +
                "ON Memberships.UserId = Account.UserId " +
                "WHERE " +
                "(Account.FirstName LIKE '%' + @searchKeyWord + '%' OR " +
                "Account.MiddleName LIKE '%' + @searchKeyWord + '%' OR " +
                "Account.LastName LIKE '%' + @searchKeyWord + '%') " +
                "ORDER BY Account.Id ASC";

            conn = new SqlConnection();
            conn.ConnectionString = CONN_STRING;
            comm = new SqlCommand(strSql, conn);
            comm.Parameters.AddWithValue("@searchKeyWord", searchKeyWord);
            dTable = new DataTable();
            adp = new SqlDataAdapter(comm);

            conn.Open();
            adp.Fill(dTable);

            comm.Dispose();
            adp.Dispose();
            conn.Close();

            return dTable;
        }

        public DataTable SelectUserAccounts(Guid UserId)
        {
            strSql = "SELECT Account.StudentId, Memberships.UserId, Memberships.IsApproved, " +
                "Roles.RoleName, Roles.RoleId," +
                "Account.LastName, Account.FirstName,Account.MiddleName " +
                "FROM Memberships, UsersInRoles, Roles,Account " +
                "WHERE " +
                "Memberships.UserId = Account.UserId " +
                "AND Memberships.UserId = UsersInRoles.UserId " +
                "AND UsersInRoles.RoleId = Roles.RoleId " +
                "AND Memberships.UserId = @UserId";

            conn = new SqlConnection();
            conn.ConnectionString = CONN_STRING;
            comm = new SqlCommand(strSql, conn);
            comm.Parameters.AddWithValue("@UserId", UserId);
            dTable = new DataTable();
            adp = new SqlDataAdapter(comm);

            conn.Open();
            adp.Fill(dTable);
            conn.Close();

            return dTable;
        }

        public void DeactivateUser(Guid UserId)
        {
            strSql = "UPDATE Memberships SET IsApproved = 'False' WHERE UserId = @UserId";

            conn = new SqlConnection();
            conn.ConnectionString = CONN_STRING;

            using (comm = new SqlCommand(strSql, conn))
            {
                conn.Open();
                comm.Parameters.AddWithValue("@UserId", UserId);

                comm.ExecuteNonQuery();
                conn.Close();
            }
            comm.Dispose();
            conn.Dispose();
        }

        public void ActivateUser(Guid UserId)
        {
            strSql = "UPDATE Memberships SET IsApproved = 'True' WHERE UserId = @UserId";

            conn = new SqlConnection();
            conn.ConnectionString = CONN_STRING;

            using (comm = new SqlCommand(strSql, conn))
            {
                conn.Open();
                comm.Parameters.AddWithValue("@UserId", UserId);

                comm.ExecuteNonQuery();
                conn.Close();
            }
            comm.Dispose();
            conn.Dispose();
        }

        public void ResetPassword(Guid UserId)
        {
            MembershipUser mu = Membership.GetUser(UserId);
            string userName = mu.UserName;

            mu.ChangePassword(mu.ResetPassword(), "pass123");
        }

        public void ChangeRole(Guid UserId, string roleName)
        {
            //get user
            MembershipUser _user = Membership.GetUser(UserId);

            //remove user from all his/her roles
            foreach (string role in Roles.GetRolesForUser(_user.UserName))
            {
                Roles.RemoveUserFromRole(_user.UserName, role);
            }

            //assign user to new role
            if (!Roles.IsUserInRole(_user.UserName, roleName))
            {
                Roles.AddUserToRole(_user.UserName, roleName);
            }
        }

        public void addUser(Guid UserId, string firstName, string middleName, string lastName, string studentId)
        {
            strSql = "INSERT INTO Account VALUES(@UserId,@FirstName,@MiddleName,@LastName,@StudentId)";

            conn = new SqlConnection();
            conn.ConnectionString = CONN_STRING;

            using (comm = new SqlCommand(strSql, conn))
            {
                conn.Open();
                comm.Parameters.AddWithValue("@UserId", UserId);
                comm.Parameters.AddWithValue("@FirstName", firstName);
                comm.Parameters.AddWithValue("@MiddleName", middleName);
                comm.Parameters.AddWithValue("@LastName", lastName);
                comm.Parameters.AddWithValue("@StudentId", studentId);
                comm.ExecuteNonQuery();
                conn.Close();
            }
            comm.Dispose();
            conn.Dispose();
        }

        public DataTable getRole(Guid roleId)
        {
            strSql = "SELECT * FROM Roles WHERE RoleId = @RoleId";

            conn = new SqlConnection();
            conn.ConnectionString = CONN_STRING;
            comm = new SqlCommand(strSql, conn);
            comm.Parameters.AddWithValue("@RoleId", roleId);
            dTable = new DataTable();
            adp = new SqlDataAdapter(comm);

            conn.Open();
            adp.Fill(dTable);
            conn.Close();

            return dTable;
        }

        public DataTable getRoles(string roleName)
        {
            strSql = "SELECT * FROM Roles WHERE RoleName LIKE '%' + @RoleName + '%'";

            conn = new SqlConnection();
            conn.ConnectionString = CONN_STRING;
            comm = new SqlCommand(strSql, conn);
            comm.Parameters.AddWithValue("@RoleName", roleName);
            dTable = new DataTable();
            adp = new SqlDataAdapter(comm);

            conn.Open();
            adp.Fill(dTable);
            conn.Close();

            return dTable;
        }

        public void updateRole(string roleName, Guid RoleId)
        {
            strSql = "UPDATE Roles SET RoleName = @RoleName WHERE RoleId = @RoleId";

            conn = new SqlConnection();
            conn.ConnectionString = CONN_STRING;

            using (comm = new SqlCommand(strSql, conn))
            {
                conn.Open();
                comm.Parameters.AddWithValue("@RoleId", RoleId);
                comm.Parameters.AddWithValue("@RoleName", roleName);
                comm.ExecuteNonQuery();
                conn.Close();
            }
            comm.Dispose();
            conn.Dispose();
        }

        public void changeUsername(Guid UserId, string newUsername)
        {
            strSql = "UPDATE Users SET UserName = @UserName WHERE UserId = @UserId";

            conn = new SqlConnection();
            conn.ConnectionString = CONN_STRING;

            using (comm = new SqlCommand(strSql, conn))
            {
                conn.Open();
                comm.Parameters.AddWithValue("@UserName", newUsername);
                comm.Parameters.AddWithValue("@UserId", UserId);
                comm.ExecuteNonQuery();
                conn.Close();
            }
            comm.Dispose();
            conn.Dispose();
        }

        public void editUser(Guid UserId, string firstName, string middleName, string lastName, string studentId)
        {
            strSql = "UPDATE Account SET " +
                "FirstName = @FirstName, " +
                "MiddleName = @MiddleName, " +
                "LastName = @LastName, " +
                "StudentId = @StudentId " +
                "WHERE UserId = @UserId";

            conn = new SqlConnection();
            conn.ConnectionString = CONN_STRING;

            using (comm = new SqlCommand(strSql, conn))
            {
                conn.Open();
                comm.Parameters.AddWithValue("@FirstName", firstName);
                comm.Parameters.AddWithValue("@MiddleName", middleName);
                comm.Parameters.AddWithValue("@LastName", lastName);
                comm.Parameters.AddWithValue("@StudentId", studentId);
                comm.Parameters.AddWithValue("@UserId", UserId);
                comm.ExecuteNonQuery();
                conn.Close();
            }
            comm.Dispose();
            conn.Dispose();
        }

        public void deleteUser(string userName)
        {
            strSql = "DELETE FROM Account WHERE StudentId = @StudentId";

            conn = new SqlConnection();
            conn.ConnectionString = CONN_STRING;

            using (comm = new SqlCommand(strSql, conn))
            {
                conn.Open();
                comm.Parameters.AddWithValue("@StudentId", userName);
                comm.ExecuteNonQuery();
                conn.Close();
            }
            comm.Dispose();
            conn.Dispose();
        }


        #endregion
    }
}