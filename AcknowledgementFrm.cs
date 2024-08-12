using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using Newtonsoft.Json;

namespace Sasco
{
    public partial class AcknowledgementFrm : Form
    {
        public AcknowledgementFrm()
        {
            InitializeComponent();
        }
        public class LoginClass
        {
            public int id { get; set; }
            public string loginName { get; set; }
            public string password { get; set; }
        }
        string str = ConfigurationManager.AppSettings["str"];
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string enteredPassword = "abc";
                string correctPassword = "your_password_here";
                Cursor = Cursors.WaitCursor;
                if (passwordBox.Text.ToString().Trim() != "")
                {
                    using (var client = new HttpClient())
                    {
                        try
                        {


                            client.BaseAddress = new Uri(str);
                            RequestParameter.LoginClass lgn = new RequestParameter.LoginClass { loginName = StoreLabels.loginName, Password = passwordBox.Text.ToString() };
                            var response = client.PostAsJsonAsync("api/User/Login", lgn).Result;
                            var a = response.Content.ReadAsStringAsync();
                            string json = a.Result;
                            RequestParameter.LoginClass account = JsonConvert.DeserializeObject<RequestParameter.LoginClass>(json);
                            if (account.status.ToString().Trim() == "401")
                            {
                                MessageBox.Show("Incorrect password. Please try again.");

                            }
                            else
                            {

                                this.DialogResult = DialogResult.OK;
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
                        }

                    }
                }
                else
                {
                    MessageBox.Show("Password cannot be empty");
                }
            }
            else
            {
                // User clicked No or closed the dialog, handle accordingly
            }
          
        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you dont want to print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      

            if (result == DialogResult.Yes)
            {
                string enteredPassword = "abc";
                string correctPassword = "your_password_here";
                Cursor = Cursors.WaitCursor;
                if (passwordBox.Text.ToString().Trim() != "")
                {
                    using (var client = new HttpClient())
                    {
                        try
                        {


                            client.BaseAddress = new Uri(str);
                            RequestParameter.LoginClass lgn = new RequestParameter.LoginClass { loginName = StoreLabels.loginName, Password = passwordBox.Text.ToString() };
                            var response = client.PostAsJsonAsync("api/User/Login", lgn).Result;
                            var a = response.Content.ReadAsStringAsync();
                            string json = a.Result;
                            RequestParameter.LoginClass account = JsonConvert.DeserializeObject<RequestParameter.LoginClass>(json);
                            if (account.status.ToString().Trim() == "401")
                            {
                                MessageBox.Show("Incorrect password. Please try again.");

                            }
                            else
                            {

                                this.DialogResult = DialogResult.Ignore;
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
                        }

                    }
                }
                else
                {
                    MessageBox.Show("Password cannot be empty");
                }
            }
            else
            {
                // User clicked No or closed the dialog, handle accordingly
            }
        }
    }
}

        //        if (enteredPassword == correctPassword)
        //    {
        //        string a = StoreLabels.userIDD;
        //        this.DialogResult = DialogResult.OK;
        //    }
        //    else
        //    {
        //        MessageBox.Show("Incorrect password. Please try again.");
        //        passwordBox.Text = string.Empty;
        //    }
        //}

        //private void AcknowledgementFrm_Load(object sender, EventArgs e)
        //{

        //}
  //  }

