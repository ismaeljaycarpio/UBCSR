using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace UBCSR
{
    public class Javascript
    {
        public static void ShowModal(Control control, Page pageObject, string modalId)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#" + modalId + "').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(control, pageObject.GetType(), Guid.NewGuid().ToString(), sb.ToString(), false);
        }

        public static void HideModal(Control control, Page pageObject, string modalId)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#" + modalId + "').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(control, pageObject.GetType(), Guid.NewGuid().ToString(), sb.ToString(), false);
        }
    }
}