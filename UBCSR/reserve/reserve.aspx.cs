using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UBCSR.borrow
{
    public partial class reserve : System.Web.UI.Page
    {
        CSRContextDataContext db = new CSRContextDataContext();
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                bindGridview();
            }
        }

        protected void bindGridview()
        {
            var q = from i in db.Items
                    join inv in db.InventoryLINQs
                    on i.Id equals inv.ItemId
                    select new
                    {
                        Id = inv.Id,
                        Name = i.ItemName,
                        Stocks = inv.Stocks
                    };

            gvInv.DataSource = q.ToList();
            gvInv.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Page.Validate();
            Reservation r = new Reservation();
            r.UserId = Guid.Parse(Membership.GetUser().ProviderUserKey.ToString());
            r.Subject = txtSubject.Text;
            r.ExperimentNo = txtExpNo.Text;
            r.DateRequested = DateTime.Now;
            r.DateFrom = Convert.ToDateTime(txtDateNeeded.Text);
            r.DateTo = Convert.ToDateTime(txtDateNeededTo.Text);
            r.LabRoom = txtLabRoom.Text;
            r.ApprovalStatus = "Pending";

            db.Reservations.InsertOnSubmit(r);
            db.SubmitChanges();

            int reserveId = r.Id;
            
            foreach(GridViewRow row in gvInv.Rows)
            {
                //chk if checkbox is checked
                if(((CheckBox)row.FindControl("chkRow")).Checked == true)
                {
                    string quantity = "";
                    int invId = Convert.ToInt32(((Label)row.FindControl("lblRowId")).Text);

                    if((quantity = ((TextBox)row.FindControl("txtQuantityToBorrow")).Text) != String.Empty)
                    {
                        ReservationItem ri = new ReservationItem();
                        ri.InventoryId = invId;
                        ri.Quantity = Convert.ToInt32(quantity);
                        ri.ReservationId = reserveId;

                        db.ReservationItems.InsertOnSubmit(ri);
                        db.SubmitChanges();
                    }
                }
            }

            Response.Redirect("~/reserve/default.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/reserve/default.aspx");
        }

        protected void gvInv_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }
    }
}