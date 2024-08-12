using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace Sasco
{
    public partial class UserFrm : Form
    {
        DataTable DTitems;
        public UserFrm()
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
        String ID;
        string str = ConfigurationManager.AppSettings["str"];
        int useraccess;
        int userid;
        string useridd;
        int userRoleId;
        private void StoreList_Click(object sender, EventArgs e)
        {
            bunifuPages1.PageIndex = 0;
          
            btn_search.Visible = true;
        }

        private void CreateTab_Click(object sender, EventArgs e)
        {
            loginNameTxt.Enabled = true;

            bunifuPages1.PageIndex = 1;
                saveBtn.Text = "Save";
                loginNameTxt.Text = "";
                userNameTxt.Text = "";
                PasswordTxt.Text = "";
                EmailTxt.Text = "";
              loadallstores();
                storeDropdown.Text = "Select";
          
            btn_search.Visible = false;
          
        }
        private void loadallstores()
        {
            
            storeDropdown.Items.Clear();
            Cursor = Cursors.WaitCursor;
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(str);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    RequestParameter.getallstoresClass lgn = new RequestParameter.getallstoresClass { get = 1 };
                    var response = client.PostAsJsonAsync("api/Stores/GetAllStores", lgn).Result;
                    var a = response.Content.ReadAsStringAsync();
                    string json = a.Result;
                    DataTable stores = JsonConvert.DeserializeObject<DataTable>(json);
                    if(stores.Rows.Count>0)
                    { 
                    stores.Columns.Remove("isDeleted");
                    stores.Columns.Remove("createdBy");
                    stores.Columns.Remove("createdOn");
                    stores.Columns.Remove("lastUpdatedBy");
                    stores.Columns.Remove("lastUpdatedOn");
                    for (int i = 0; i < stores.Rows.Count; i++)

                    {
                        string theValue = stores.Rows[i].ItemArray[1].ToString();
                        storeDropdown.Items.Add(theValue);

                    }
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            Cursor = Cursors.Arrow; // change cursor to normal type
            
        }
        private void UserFrm_Load(object sender, EventArgs e)
        {

            token = Dashboard.SetValueForToken;

            ID = Dashboard.ID;
            DTitems = loadAllUsers();
            Loadcompanydetails();
        }

        private DataTable loadAllUsers()
        {
            DataTable usertbl = new DataTable();
            using (var client = new HttpClient())
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    client.BaseAddress = new Uri(str);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    RequestParameter.getallstoresClass lgn = new RequestParameter.getallstoresClass { get = 1 };
                    var response = client.PostAsJsonAsync("api/User/GetAllUsers", lgn).Result;
                    var a = response.Content.ReadAsStringAsync();
                    string json = a.Result;
                     usertbl = JsonConvert.DeserializeObject<DataTable>(json);


                    // account.Columns.Remove("password");
                    usertbl.Columns.Remove("rowNo");

                    usertbl.Columns.Remove("roleID");
                    usergrid.DataSource = usertbl;

                    usergrid.Columns["password"].Visible = false;
                    usergrid.Columns["userAccess"].Visible = false;

                    usergrid.Columns["ID"].Visible = false;
                    usergrid.Columns["loginName"].HeaderText = "Login Name";
                    usergrid.Columns["userName"].HeaderText = "Username";
                    usergrid.Columns["storeCode"].HeaderText = "Store Code";
                    usergrid.Columns["role"].HeaderText = "Role";
                    usergrid.Columns["emailAddress"].HeaderText = "Email";
                    Cursor = Cursors.Arrow;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            return usertbl;
        }

        private void UserFrm_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Escape)
            {

                e.SuppressKeyPress = true;
                Dashboard dash = new Dashboard();
                this.Close();
                dash.ShowDialog();
            }
        }

        private void bunifuLabel12_Click(object sender, EventArgs e)
        {

        }

        private void bunifuRadioButton3_CheckedChanged2(object sender, Bunifu.UI.WinForms.BunifuRadioButton.CheckedChangedEventArgs e)
        {

        }

        private void bunifuLabel9_Click(object sender, EventArgs e)
        {

        }

        private void bunifuLabel10_Click(object sender, EventArgs e)
        {

        }

        private void bunifuRadioButton2_CheckedChanged2(object sender, Bunifu.UI.WinForms.BunifuRadioButton.CheckedChangedEventArgs e)
        {

        }

        private void bunifuRadioButton1_CheckedChanged2(object sender, Bunifu.UI.WinForms.BunifuRadioButton.CheckedChangedEventArgs e)
        {

        }

        private void bunifuLabel6_Click(object sender, EventArgs e)
        {

        }

        private void bunifuLabel5_Click(object sender, EventArgs e)
        {

        }

        private void saveBtn_Click(object sender, EventArgs e)
        {

        }

        private void bunifuPictureBox2_Click(object sender, EventArgs e)
        {
            Dashboard str = new Dashboard();
            this.Hide();
            str.ShowDialog();
          
            this.Close();
        }

        private void usergrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            loginNameTxt.Enabled = false;
            btn_search.Visible = false;
            Cursor = Cursors.WaitCursor;
            bunifuPages1.PageIndex = 1;
            loadallstores();
            loginNameTxt.Text = usergrid.Rows[e.RowIndex].Cells["loginName"].Value.ToString();
            userNameTxt.Text = usergrid.Rows[e.RowIndex].Cells["userName"].Value.ToString();
            //PasswordTxt.Text = usergrid.Rows[e.RowIndex].Cells[3].Value.ToString();
            //role.Text = usergrid.Rows[e.RowIndex].Cells[3].Value.ToString();
            EmailTxt.Text = usergrid.Rows[e.RowIndex].Cells["emailAddress"].Value.ToString();
            userid = int.Parse(usergrid.Rows[e.RowIndex].Cells["id"].Value.ToString());
            storeDropdown.Text = usergrid.Rows[e.RowIndex].Cells["storeCode"].Value.ToString();
            roleCombo.Text = usergrid.Rows[e.RowIndex].Cells["Role"].Value.ToString();
            if (usergrid.Rows[e.RowIndex].Cells["userAccess"].Value.ToString() == "1")
            {
                bunifuRadioButton1.Checked = true;
            }
            if (usergrid.Rows[e.RowIndex].Cells["userAccess"].Value.ToString() == "2")
            {
                bunifuRadioButton2.Checked = true;
            }
            if (usergrid.Rows[e.RowIndex].Cells["userAccess"].Value.ToString() == "3")
            {
                bunifuRadioButton3.Checked = true;
            }
            saveBtn.Text = "Update";
            Cursor = Cursors.Arrow;
        }

        private void saveBtn_Click_1(object sender, EventArgs e)
        {
            if (loginNameTxt.Text != "" && userNameTxt.Text != "" && PasswordTxt.Text != "" && storeDropdown.SelectedText.ToString() != "Select" && EmailTxt.Text != "")
            {
                string pattern = @"^(?=.*\d)(?=.*[!@#$%^&*])(?=.*[a-zA-Z]).{8,}$";

                // Check if the entered password matches the pattern
                bool isPasswordValid = Regex.IsMatch(PasswordTxt.Text, pattern);
               

                    if (PasswordTxt.Text == conpassword.Text)
                    {
                    if (isPasswordValid)
                    {
                        checkUserAccess();
                        checkUserRole();
                        if (saveBtn.Text == "Save")
                        {
                           
                            Cursor = Cursors.WaitCursor;
                            using (var client = new HttpClient())
                            {
                                try
                                {
                                    string storetext;
                                    if (storeDropdown.Text=="Select")
                                    {
                                        storetext = "";
                                    }
                                    else
                                    {
                                        storetext = storeDropdown.Text;
                                    }
                                    client.BaseAddress = new Uri(str);
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                                    RequestParameter.createuserClass cls = new RequestParameter.createuserClass { add = 1, loginName = loginNameTxt.Text, userName = userNameTxt.Text, password = PasswordTxt.Text, storeCode = storetext, emailAddress = EmailTxt.Text, roleID = userRoleId, userAccess = useraccess, id = userid };
                                    var response = client.PostAsJsonAsync("api/User/InsertUser", cls).Result;

                                    var a = response.Content.ReadAsStringAsync();
                                    string json = a.Result;
                                    RequestParameter.createstoresClass Reply = JsonConvert.DeserializeObject<RequestParameter.createstoresClass>(json);
                                    // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);
                                    if(Reply.status!="200")
                                    {
                                        bunifuSnackbar1.Show(this, Reply.message,
                                                                            Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error, 5000, "",
                                                                            Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                                                            Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
                                    }
                                    else { 
                                    bunifuSnackbar1.Show(this, Reply.message,
                                                                             Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 5000, "",
                                                                             Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                                                             Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
                                    //MessageBox.Show(Reply.message);
                                    loginNameTxt.Text = "";
                                    loginNameTxt.Text = "";
                                    PasswordTxt.Text = "";
                                    conpassword.Text = "";
                                    storeDropdown.Text = "";
                                    EmailTxt.Text = "";
                                    loadAllUsers();
                                    bunifuPages1.PageIndex = 0;
                                        btn_search.Visible = true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message.ToString());
                                }
                                Cursor = Cursors.Arrow; // change cursor to normal type
                            }

                        }

                        if (saveBtn.Text == "Update")
                        {
                            Cursor = Cursors.WaitCursor;
                            using (var client = new HttpClient())
                            {
                                try
                                {
                                    string storetext;
                                    if (storeDropdown.Text == "Select")
                                    {
                                        storetext = "";
                                    }
                                    else
                                    {
                                        storetext = storeDropdown.Text;
                                    }
                                    client.BaseAddress = new Uri(str);
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                                    RequestParameter.createuserClass cls = new RequestParameter.createuserClass { update = 1, loginName = loginNameTxt.Text, userName = userNameTxt.Text, password = PasswordTxt.Text, storeCode = storetext, emailAddress = EmailTxt.Text, roleID = userRoleId, userAccess = useraccess, id = userid };
                                    var response = client.PostAsJsonAsync("api/User/UpdateUser", cls).Result;

                                    var a = response.Content.ReadAsStringAsync();
                                    string json = a.Result;
                                    RequestParameter.createstoresClass Reply = JsonConvert.DeserializeObject<RequestParameter.createstoresClass>(json);
                                    // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);
                                    bunifuSnackbar1.Show(this, Reply.message,
                                                                             Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 1000, "",
                                                                             Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                                                             Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
                                    //MessageBox.Show(Reply.message);
                                    loginNameTxt.Text = "";
                                    loginNameTxt.Text = "";
                                    PasswordTxt.Text = "";
                                    conpassword.Text = "";
                                    storeDropdown.Text = "";
                                    EmailTxt.Text = "";
                                    conpassword.Text = "";
                                    loadAllUsers();
                                    bunifuPages1.PageIndex = 0;
                                    btn_search.Visible = true;
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message.ToString());
                                }
                                Cursor = Cursors.Arrow; // change cursor to normal type
                            }

                        }
                    }

                    else
                    {
                        bunifuSnackbar1.Show(this, "Password must be minimum 8 charactor long and Contains at least one special character, Contains at least one letter Upper/Lower, Contains at least one digit ",
                                    Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error, 5000, "",
                                    Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                    Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
                    }
                }
                else
                {
                    bunifuSnackbar1.Show(this, "Password not match",
                                    Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error, 5000, "",
                                    Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                    Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
                }



            }
            else
            {
                bunifuSnackbar1.Show(this, "Fields cannot be empty",
                Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error, 5000, "",
                Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
            }
        }

        private void checkUserRole()
        {
            if (roleCombo.Text == "Admin")
            {
                userRoleId = 1;
            }
            if (roleCombo.Text == "User")
            {
                userRoleId = 2;
            }
        }

        private void checkUserAccess()
        {
            if (bunifuRadioButton1.Checked == true)
            {
                useraccess = 1;

            }
            if (bunifuRadioButton2.Checked == true)
            {
                useraccess = 2;
            }
            if (bunifuRadioButton3.Checked == true)
            {
                useraccess = 3;
            }

        }

        private void usergrid_KeyDown(object sender, KeyEventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (e.KeyCode == Keys.Delete)
            {
                var confirmResult = MessageBox.Show("Are you sure to delete this user ??",
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
                          //  storesID = labelgrid[0, labelgrid.CurrentCell.RowIndex].Value.ToString();
                            useridd = (usergrid["ID", usergrid.CurrentCell.RowIndex].Value.ToString());
                            RequestParameter.deleteuserClass lgn = new RequestParameter.deleteuserClass { id = int.Parse(useridd), delete = 1 , update=1 };
                            var response = client.PostAsJsonAsync("api/User/DeleteUser", lgn).Result;

                            var a = response.Content.ReadAsStringAsync();
                            string json = a.Result;
                            RequestParameter.deleteuserClass Reply = JsonConvert.DeserializeObject<RequestParameter.deleteuserClass>(json);
                            // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);
                            //MessageBox.Show(Reply.message);
                            bunifuSnackbar1.Show(this, Reply.message,
                                         Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 1000, "",
                                         Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                         Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
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
                loadAllUsers();
                Cursor = Cursors.Arrow; // change cursor to normal type
            }
        }

        private void storegrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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

        private void btn_search_TextChange(object sender, EventArgs e)
        {
            string searchKeyword = btn_search.Text.Trim().ToLower();
            // Your logic to handle the complete scanned input
            // MessageBox.Show("Scanned: " + input);

            DataView dv = DTitems.DefaultView;
            dv.RowFilter = $"loginName LIKE '%{searchKeyword}%' OR userName LIKE '%{searchKeyword}%' OR storeCode LIKE '%{searchKeyword}%' OR role LIKE '%{searchKeyword}%'";

            // Reset the BindingSource to apply the filtered DataView.
            usergrid.DataSource = dv;
        }
    }
}
