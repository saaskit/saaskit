using System;
using System.Web;

namespace SaasKit.Demos.AspNet
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentTenant = Request.GetOwinContext().GetTenantInstance();
            lblTenant.Text = "Current Tenant: " + currentTenant;
        }
    }
}