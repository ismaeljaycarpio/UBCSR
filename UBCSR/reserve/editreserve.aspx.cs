using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UBCSR.reserve
{
    public partial class editreserve : System.Web.UI.Page
    {
        CSRContextDataContext db = new CSRContextDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                if(Request.QueryString["resId"] == null)
                {
                    Response.Redirect("~/reserve/default.aspx");
                }
                else
                {
                    int resId = Convert.ToInt32(Request.QueryString["resId"].ToString());
                    var q = (from r in db.Reservations
                             where r.Id == resId
                             select r).ToList();

                    if(q.Count < 1)
                    {
                        //no record found
                        Response.Redirect("~/reserve/default.aspx");
                    }
                    else
                    {
                        var r = q.FirstOrDefault();

                        txtSubject.Text = r.Subject;
                        txtExpNo.Text = r.ExperimentNo;
                        txtDateNeeded.Text = r.DateFrom.ToString();
                        txtDateNeededTo.Text = r.DateTo.ToString();
                        txtLabRoom.Text = r.LabRoom;
                        hfResId.Value = r.Id.ToString();

                        bindGridview();

                        //approval to CSR Head
                        if(!User.IsInRole("CSR Head") && !User.IsInRole("Admin"))
                        {
                            btnApprove.Visible = false;
                            btnDisapprove.Visible = false;
                        }

                        //chk if student
                        if(User.IsInRole("Student"))
                        {
                            disableFields();
                            btnSave.Text = "Ok";
                            btnBorrow.Visible = true;
                        }
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Page.Validate();
            
            if(btnSave.Text != "Ok")
            {
                var q = (from r in db.Reservations
                         where r.Id == Convert.ToInt32(hfResId.Value)
                         select r).FirstOrDefault();

                q.Subject = txtSubject.Text;
                q.ExperimentNo = txtExpNo.Text;
                q.DateFrom = Convert.ToDateTime(txtDateNeeded.Text);
                q.DateTo = Convert.ToDateTime(txtDateNeededTo.Text);
                q.LabRoom = txtLabRoom.Text;

                db.SubmitChanges();

                int resId = q.Id;

                foreach (GridViewRow row in gvInv.Rows)
                {
                    //chk if checkbox is checked
                    if (((CheckBox)row.FindControl("chkRow")).Checked == true)
                    {
                        string quantity = "";
                        int riId = Convert.ToInt32(((Label)row.FindControl("lblRowId")).Text);

                        if ((quantity = ((TextBox)row.FindControl("txtQuantityToBorrow")).Text) != String.Empty)
                        {
                            var r = (from ri in db.ReservationItems
                                     where ri.Id == riId
                                     select ri).FirstOrDefault();

                            r.Quantity = Convert.ToInt32(quantity);

                            db.SubmitChanges();
                        }
                    }
                }
            }
            else
            {
                Response.Redirect("~/reserve/default.aspx");
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

        protected void bindGridview()
        {
            var q = from i in db.Items
                    join inv in db.InventoryLINQs
                    on i.Id equals inv.ItemId
                    join ri in db.ReservationItems
                    on inv.Id equals ri.InventoryId
                    join r in db.Reservations
                    on ri.ReservationId equals r.Id
                    where r.Id == Convert.ToInt32(hfResId.Value)
                    select new
                    {
                        Id = ri.Id,
                        Name = i.ItemName,
                        Stocks = inv.Stocks,
                        Quantity = ri.Quantity
                    };

            gvInv.DataSource = q.ToList();
            gvInv.DataBind();
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            int resId = Convert.ToInt32(Request.QueryString["resId"].ToString());
            var q = (from r in db.Reservations
                     where r.Id == resId
                     select r).FirstOrDefault();

            q.ApprovalStatus = "Approved";

            db.SubmitChanges();

            Response.Redirect("~/reserve/default.aspx");
        }

        protected void btnDisapprove_Click(object sender, EventArgs e)
        {
            int resId = Convert.ToInt32(Request.QueryString["resId"].ToString());
            var q = (from r in db.Reservations
                     where r.Id == resId
                     select r).FirstOrDefault();

            q.ApprovalStatus = "Disapproved";

            db.SubmitChanges();

            Response.Redirect("~/reserve/default.aspx");
        }

        protected void disableFields()
        {
            txtDateNeeded.Enabled = false;
            txtDateNeededTo.Enabled = false;
            txtExpNo.Enabled = false;
            txtLabRoom.Enabled = false;
            txtSubject.Enabled = false;
            gvInv.Enabled = false;
        }

        protected void btnBorrow_Click(object sender, EventArgs e)
        {
            //hook to reservation

        }
    }
}