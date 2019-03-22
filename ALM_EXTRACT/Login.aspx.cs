using System;
using TDAPIOLELib;

namespace ALM_EXTRACT
{
    public partial class Login : System.Web.UI.Page
    {
        public static TDConnection otaConnection;
        public DesignSteps dsgnSteps;
        public TestPlan tstPlan;
        public TestAttachments tstAttachments;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Clear text and visibility
            ClearErrorLabel();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (IsValidInput())
                return;

            ClearErrorLabel();

            if (IsPostBack)
            {
                try
                {
                    otaConnection = new ALM_Core().LoginALM(txtALMUrl.Value, txtUserName.Value, txtPWD.Value, txtDomain.Value, txtProject.Value);

                    Session["myTDConnection"] = otaConnection;

                    if (otaConnection == null)
                        Response.Redirect("~/Login.aspx?", false);
                    else
                        Response.Redirect("~/utility.aspx?", false);
                }
                catch (Exception ex)
                {
                    Utilities.LogException(ex);
                    lblTextError.Visible = true;
                    lblTextError.Value = ex.Message.ToString();
                }
            }
        }

        private bool IsValidInput()
        {
            ClearErrorLabel();

            if (string.IsNullOrEmpty(txtALMUrl.Value) || string.IsNullOrEmpty(txtUserName.Value) ||
                string.IsNullOrEmpty(txtPWD.Value) || string.IsNullOrEmpty(txtProject.Value) ||
                string.IsNullOrEmpty(txtDomain.Value))

            {
                lblTextError.Visible = true;
                lblTextError.Value = "Please enter all the mandatory text field";
                return true;
            }
            else
                return false;
        }

        private void ClearErrorLabel()
        {
            lblTextError.Value = string.Empty;
            lblTextError.Visible = false;
        }
    }
}