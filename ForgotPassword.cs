using Newtonsoft.Json;
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
namespace Sasco
{
    public partial class ForgotPassword : Form
    {
        public ForgotPassword()
        {
            InitializeComponent();
        }
        string str = ConfigurationManager.AppSettings["str"];
        private void activateBtn_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(str);
                    RequestParameter.generatetokenforemail lgn = new RequestParameter.generatetokenforemail { loginName = loginNameEmail.Text.ToString() };
                    var response = client.PostAsJsonAsync("api/User/GeneratePasswordToken", lgn).Result;

                    var a = response.Content.ReadAsStringAsync();
                    string json = a.Result;
                    RequestParameter.generatetokenforemail Reply = JsonConvert.DeserializeObject<RequestParameter.generatetokenforemail>(json);
                    // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);
                    MessageBox.Show(Reply.message);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            bunifuPages1.PageIndex = 1;
            Cursor = Cursors.Arrow;
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (password.Text == conpassword.Text)
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        client.BaseAddress = new Uri(str);
                        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        RequestParameter.forgotpassword cls = new RequestParameter.forgotpassword { loginName = loginname.Text, newPassword = password.Text, token = tokencode.Text };
                        var response = client.PostAsJsonAsync("api/User/ForgetPassword", cls).Result;

                        var a = response.Content.ReadAsStringAsync();
                        string json = a.Result;
                        RequestParameter.forgotpassword Reply = JsonConvert.DeserializeObject<RequestParameter.forgotpassword>(json);
                        // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);
                        MessageBox.Show(Reply.message);
                        LoginFrm login = new LoginFrm();
                        this.Hide();

                        login.ShowDialog();

                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        bunifuSnackbar1.Show(this, ex.Message.ToString(),
               Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error);
                        //MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
            else
            {
                bunifuSnackbar1.Show(this, "Password Not match",
                Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error, 1000, "",
                Bunifu.UI.WinForms.BunifuSnackbar.Positions.BottomCenter,
                Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
            }
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            LoginFrm login = new LoginFrm();
            this.Hide();

            login.ShowDialog();

            this.Close();
        }

        private void bunifuButton21_Click(object sender, EventArgs e)
        {
            LoginFrm login = new LoginFrm();
            this.Hide();

            login.ShowDialog();

            this.Close();
        }
    }
}
