using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sasco
{
    public partial class SettingFrm : Form
    {
        public SettingFrm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            enableDoubleBuff(panel1);
            enableDoubleBuff(panel2);
            enableDoubleBuff(bunifuGradientPanel1);
        }
        public static void enableDoubleBuff(System.Windows.Forms.Control cont)
        {
            System.Reflection.PropertyInfo DemoProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            DemoProp.SetValue(cont, true, null);
        }

        String token;
        public static string ID;
        public static string SetValueForToken = "";
        string str = ConfigurationManager.AppSettings["str"];
        private void SettingFrm_Load(object sender, EventArgs e)
        {
            token = Dashboard.SetValueForToken;

            ID = Dashboard.ID;
            loadsetting();
        }

        public void loadsetting()
        {
            Cursor = Cursors.WaitCursor;
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(str);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    RequestParameter.getalllabelsClass cls = new RequestParameter.getalllabelsClass { get = 1 };
                    var response = client.PostAsJsonAsync("api/FolderSettings/GetFolderPaths", cls).Result;
                    var a = response.Content.ReadAsStringAsync();
                    string json = a.Result;
                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

                    if (dt.Rows.Count > 0)
                    {
                        network.Text = dt.Rows[0]["networkFolderPath"].ToString();
                        archive.Text = dt.Rows[0]["archiveFolderPath"].ToString();
                        activateBtn.Text = "Update";
                    }
                    //labelgrid.Columns["isDefault"].Visible = false;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            Cursor = Cursors.Arrow;
        }

        private void bunifuLabel3_Click(object sender, EventArgs e)
        {

        }

        private void bunifuPictureBox2_Click(object sender, EventArgs e)
        {
            SetValueForToken = token;

            Dashboard dash = new Dashboard();
            this.Hide();
            dash.ShowDialog();
          
            this.Close();
        }

        private void activateBtn_Click(object sender, EventArgs e)
        {
            if (activateBtn.Text == "Save")
            {
                string base64String = "";
                
                Cursor = Cursors.WaitCursor;
                using (var client = new HttpClient())
                {
                    try
                    {
                        client.BaseAddress = new Uri(str);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        RequestParameter.savesettingClass cls = new RequestParameter.savesettingClass { add = 1, networkFolderPath = network.Text, archiveFolderPath = archive.Text};
                        var response = client.PostAsJsonAsync("api/FolderSettings/InsertFolderPaths", cls).Result;

                        var a = response.Content.ReadAsStringAsync();
                        string json = a.Result;
                        RequestParameter.savesettingClass Reply = JsonConvert.DeserializeObject<RequestParameter.savesettingClass>(json);
                        // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);
                        //MessageBox.Show(Reply.message);


                        bunifuSnackbar1.Show(this, Reply.message.ToString(),
                                  Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 1000, "",
                                  Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                  Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                    Cursor = Cursors.Arrow;
                }
            }

            if (activateBtn.Text == "Update")
            {
                
                Cursor = Cursors.WaitCursor;
                using (var client = new HttpClient())
                {
                    try
                    {
                        client.BaseAddress = new Uri(str);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        RequestParameter.savesettingClass cls = new RequestParameter.savesettingClass { update = 1, networkFolderPath = network.Text, archiveFolderPath = archive.Text };
                        var response = client.PostAsJsonAsync("api/FolderSettings/UpdateFolderPaths", cls).Result;

                        var a = response.Content.ReadAsStringAsync();
                        string json = a.Result;
                        RequestParameter.savesettingClass Reply = JsonConvert.DeserializeObject<RequestParameter.savesettingClass>(json);
                        // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);
                        //MessageBox.Show(Reply.message);


                        bunifuSnackbar1.Show(this, Reply.message.ToString(),
                                  Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 1000, "",
                                  Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                  Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                    Cursor = Cursors.Arrow;
                }
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void bunifuButton21_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            network.Text = folderBrowserDialog1.SelectedPath;
           
        }

        private void bunifuButton22_Click(object sender, EventArgs e)
        {
            folderBrowserDialog2.ShowDialog();
            archive.Text = folderBrowserDialog2.SelectedPath;
        }
    }
}
