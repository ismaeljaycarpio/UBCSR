using System;
using System.Collections;
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
        public const string CHECKED_ITEMS = "SelectedCustomersIndex"; 
        CSRContextDataContext db = new CSRContextDataContext();
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                bindGridview();
                bindDropdown();
                ddlSubject.Items.Insert(0, new ListItem("Select Subject", "0"));
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Page.Validate("vgPrimaryAdd");
            if(Page.IsValid)
            {
                Reservation r = new Reservation();
                r.UserId = Guid.Parse(Membership.GetUser().ProviderUserKey.ToString());
                r.SubjectId = Convert.ToInt32(ddlSubject.SelectedValue);
                r.ExperimentNo = txtExpNo.Text;
                r.DateRequested = DateTime.Now;
                r.DateFrom = Convert.ToDateTime(txtDateNeeded.Text);
                r.DateTo = Convert.ToDateTime(txtDateNeededTo.Text);
                r.LabRoom = txtLabRoom.Text;
                r.ApprovalStatus = "Pending";
                r.IsReleased = false;
                r.IsReturned = false;


                db.Reservations.InsertOnSubmit(r);
                db.SubmitChanges();

                int reserveId = r.Id;

                gvInv.AllowPaging = false;
                gvInv.DataBind();

                //reserve selected items
                foreach (GridViewRow row in gvInv.Rows)
                {
                    string quantity = "";
                    string quantityByGroup = "";
                    int invId = Convert.ToInt32(((Label)row.FindControl("lblRowId")).Text);

                    if (
                        (quantity = ((TextBox)row.FindControl("txtQuantityToBorrow")).Text) != String.Empty &&
                        (quantityByGroup = ((TextBox)row.FindControl("txtQuantityToBorrowByGroup")).Text) != String.Empty
                       )
                    {
                        ReservationItem ri = new ReservationItem();
                        ri.InventoryId = invId;
                        ri.Quantity = Convert.ToInt32(quantity);
                        ri.ReservationId = reserveId;
                        ri.QuantityByGroup = Convert.ToInt32(quantityByGroup);

                        db.ReservationItems.InsertOnSubmit(ri);
                        db.SubmitChanges();
                    }
                }

                gvInv.AllowPaging = true;
                gvInv.DataBind();

                Response.Redirect("~/reserve/default.aspx");
            }
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
                    where inv.Stocks > 0
                    select new
                    {
                        Id = inv.Id,
                        Name = i.ItemName,
                        Stocks = inv.Stocks
                    };

            gvInv.DataSource = q.ToList();
            gvInv.DataBind();
        }

        protected void bindDropdown()
        {
            var q = from s in db.SubjectLINQs
                    join sec in db.Sections
                    on s.SectionId equals sec.Id
                    select new
                    {
                        Name = s.Name + "  [" + sec.Section1 + "]",
                        Id = s.Id
                    };

            ddlSubject.DataSource = q;
            ddlSubject.DataTextField = "Name";
            ddlSubject.DataValueField = "Id";
            ddlSubject.DataBind();
        }

        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if(Convert.ToDateTime(txtDateNeeded.Text) >= Convert.ToDateTime(txtDateNeededTo.Text))
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void gvInv_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void gvInv_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvInv.PageIndex = e.NewPageIndex;
            bindGridview();
        }

        private void RememberOldValues()
        {
            ArrayList categoryIDList = new ArrayList();
            int index = -1;
            foreach (GridViewRow row in gvInv.Rows)
            {
                index = (int)gvInv.DataKeys[row.RowIndex].Value;
                TextBox txtQuantityToBorrow = ((TextBox)row.FindControl("txtQuantityToBorrow")) as TextBox;
                TextBox txtQuantityToBorrowByGroup = ((TextBox)row.FindControl("txtQuantityToBorrowByGroup")) as TextBox;

                // Check in the Session
                if (Session[CHECKED_ITEMS] != null)
                    categoryIDList = (ArrayList)Session[CHECKED_ITEMS];
                if (txtQuantityToBorrow.Text != String.Empty &&
                    txtQuantityToBorrowByGroup.Text != String.Empty)
                {
                    if (!categoryIDList.Contains(index))
                        categoryIDList.Add(index);
                }
                else
                    categoryIDList.Remove(index);
            }
            if (categoryIDList != null && categoryIDList.Count > 0)
                Session[CHECKED_ITEMS] = categoryIDList;
        }

        private void RePopulateValues()
        {
            ArrayList categoryIDList = (ArrayList)Session[CHECKED_ITEMS];
            if (categoryIDList != null && categoryIDList.Count > 0)
            {
                foreach (GridViewRow row in gvInv.Rows)
                {
                    int index = (int)gvInv.DataKeys[row.RowIndex].Value;
                    if (categoryIDList.Contains(index))
                    {
                        CheckBox myCheckBox = (CheckBox)row.FindControl("CheckBox1");
                        myCheckBox.Checked = true;
                    }
                }
            }
        }
    }
}