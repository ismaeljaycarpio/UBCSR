using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace UBCSR
{
    public partial class reset_pass : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MembershipUser usr = Membership.GetUser("test-admin");
            usr.ChangePassword(usr.ResetPassword(), "admins");

            Response.Write("Admin Password Change Successfully!!!");
        }
    }
}