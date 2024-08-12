using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sasco
{
    public partial class LabelFrm : Form
    {
        public LabelFrm()
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
        int def;
        string storesID;
        private void bunifuTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void StoreList_Click(object sender, EventArgs e)
        {
            bunifuPages1.PageIndex = 0;
        }

        private void CreateTab_Click(object sender, EventArgs e)
        {
            labelmain.Text = "Label Creation";
            labelCodeTxt.Enabled = true;
            saveBtn.Text = "Save";
            bunifuPages1.PageIndex = 1;
        }

        private void LabelFrm_Load(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            token = Dashboard.SetValueForToken;

            ID = Dashboard.ID;
            Loadcompanydetails();
           
            loadAllLabels();
            Cursor = Cursors.Arrow;
        }
        private void loadAllLabels()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(str);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    RequestParameter.getalllabelsClass cls = new RequestParameter.getalllabelsClass { get = 1 };
                    var response = client.PostAsJsonAsync("api/Labels/GetAllLabels", cls).Result;
                    var a = response.Content.ReadAsStringAsync();
                    string json = a.Result;
                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                    if (dt.Rows.Count > 0) { 
                    dt.Columns.Remove("createdBy");
                    dt.Columns.Remove("lastUpdatedBy");

                    labelgrid.DataSource = dt;
                    labelgrid.Columns["labelID"].HeaderText = " Label ID";
                    labelgrid.Columns["labelCode"].HeaderText = "Label Code";
                    labelgrid.Columns["labelName"].HeaderText = "Label Name";
                    labelgrid.Columns["isDefault"].HeaderText = "Default";


                    labelgrid.Columns["labelText"].Visible = false;
                    labelgrid.Columns["labelID"].Visible = false;
                    }//labelgrid.Columns["isDefault"].Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
        private void bunifuCards1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (labelCodeTxt.Text != "" || labelNameTxt.Text != "" || labelTextTxt.Text != "")
            {
                checkdefault();
                if (saveBtn.Text == "Save")
                {
                    Cursor = Cursors.WaitCursor;
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            client.BaseAddress = new Uri(str);
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            RequestParameter.createlabelClass cls = new RequestParameter.createlabelClass { add = 1, labelCode = labelCodeTxt.Text, labelName = labelNameTxt.Text, labelText = labelTextTxt.Text, lastUpdatedBy = int.Parse(ID), createdBy = int.Parse(ID), isDefault = def };
                            var response = client.PostAsJsonAsync("api/Labels/InsertLabel", cls).Result;

                            var a = response.Content.ReadAsStringAsync();
                            string json = a.Result;
                            RequestParameter.createlabelClass Reply = JsonConvert.DeserializeObject<RequestParameter.createlabelClass>(json);
                            // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);
                            //MessageBox.Show(Reply.message);
                           
                            loadAllLabels();


                            bunifuCheckBox1.Checked = false;
                            bunifuSnackbar1.Show(this, Reply.message.ToString(),
                                      Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 5000, "",
                                      Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                      Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
                            bunifuPages1.PageIndex = 0;
                            bunifuCheckBox1.Checked = false;
                            labelCodeTxt.Text = "";
                            labelNameTxt.Text = "";
                            labelTextTxt.Text = "";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
                        }
                    }
                    Cursor = Cursors.Arrow;
                }

                if (saveBtn.Text == "Update")
                {
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            Cursor = Cursors.WaitCursor;
                            client.BaseAddress = new Uri(str);
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            RequestParameter.createlabelClass cls = new RequestParameter.createlabelClass { update = 1, labelCode = labelCodeTxt.Text, labelName = labelNameTxt.Text, labelText = labelTextTxt.Text, lastUpdatedBy = int.Parse(ID), labelID = int.Parse(storesID), isDefault = def };
                            var response = client.PostAsJsonAsync("api/Labels/UpdateLabel", cls).Result;

                            var a = response.Content.ReadAsStringAsync();
                            string json = a.Result;
                            RequestParameter.createlabelClass Reply = JsonConvert.DeserializeObject<RequestParameter.createlabelClass>(json);
                            // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);
                            //MessageBox.Show(Reply.message);
                            loadAllLabels();
                            bunifuPages1.PageIndex = 0;

                            Cursor = Cursors.Arrow;
                            bunifuSnackbar1.Show(this, Reply.message.ToString(),
                                       Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 5000, "",
                                       Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                       Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);

                            labelCodeTxt.Text = "";
                            labelNameTxt.Text = "";
                            labelTextTxt.Text = "";

                            bunifuCheckBox1.Checked = false;
                            bunifuCheckBox1.Checked = false;

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
                        }
                    }
                    
                }

            }
            else
            {
                bunifuSnackbar1.Show(this, "Label Code / Label Name / Label Text cant be empty",
                                         Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 5000, "",
                                         Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                         Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
            }
        }

        private void checkdefault()
        {
            if (bunifuCheckBox1.Checked == true)
            {
                def = 1;
            }
            else
            {
                def = 0;
            }
        }

        private void labelgrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            labelmain.Text = "Label Update";
            labelCodeTxt.Enabled = false;
            bunifuPages1.PageIndex = 1;
            storesID = labelgrid.Rows[e.RowIndex].Cells[0].Value.ToString();
            labelCodeTxt.Text = labelgrid.Rows[e.RowIndex].Cells["labelCode"].Value.ToString();
            labelNameTxt.Text = labelgrid.Rows[e.RowIndex].Cells["labelName"].Value.ToString();
            labelTextTxt.Text = labelgrid.Rows[e.RowIndex].Cells["labelText"].Value.ToString();
            int defaultch = int.Parse(labelgrid.Rows[e.RowIndex].Cells["isDefault"].Value.ToString());


            if (defaultch == 1)
            {
                bunifuCheckBox1.Checked = true;
            }
            else
            {
                bunifuCheckBox1.Checked = false;
            }
            saveBtn.Text = "Update";
        }

        private void labelgrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var confirmResult = MessageBox.Show("Are you sure to delete this item ??",
                                        "Confirm Delete!!",
                                        MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            client.BaseAddress = new Uri(str);
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                            storesID = labelgrid[0, labelgrid.CurrentCell.RowIndex].Value.ToString();
                            RequestParameter.deletelabelClass lgn = new RequestParameter.deletelabelClass { labelID = int.Parse(storesID), delete = 1, update = 1, lastUpdatedBy = int.Parse(ID) };
                            var response = client.PostAsJsonAsync("api/Labels/DeleteLabel", lgn).Result;

                            var a = response.Content.ReadAsStringAsync();
                            string json = a.Result;
                            RequestParameter.deletelabelClass Reply = JsonConvert.DeserializeObject<RequestParameter.deletelabelClass>(json);
                            // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);
                            //MessageBox.Show(Reply.message);
                            bunifuSnackbar1.Show(this, Reply.message.ToString(),
                                        Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 5000, "",
                                        Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                        Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
                            loadAllLabels();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
                        }
                    }
                }
                else
                {
                    // If 'No', do something here.
                }

            }
        }

        private void bunifuPictureBox2_Click(object sender, EventArgs e)
        {
            SetValueForToken = token;

            Dashboard dash = new Dashboard();
            this.Hide();
            dash.ShowDialog();
           
            this.Close();
        }
        private void Loadcompanydetails()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(str);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    RequestParameter.getalllabelsClass cls = new RequestParameter.getalllabelsClass { get = 1 };
                    var response = client.PostAsJsonAsync("api/Company/GetCompanyDetails", cls).Result;
                    var a = response.Content.ReadAsStringAsync();
                    string json = a.Result;
                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

                    if (dt.Rows.Count > 0)
                    {

                        if (dt.Rows[0]["imageBase64"].ToString() != "")
                        {
                            Image image = Base64ToImage(dt.Rows[0]["imageBase64"].ToString());
                            bunifuPictureBox1.Image = image;
                        }
                    }

                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
        private Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                Image image = Image.FromStream(ms);
                return image;
            }
        }
    }
}
