using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.IO;

namespace Sasco
{
    public partial class StoreFrm : Form
    {
        public StoreFrm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            enableDoubleBuff(panel1);
            enableDoubleBuff(panel2);
            enableDoubleBuff(bunifuGradientPanel1);
            // Attach event handlers
            bunifuTextBox1.TextChanged += TxtSearch_TextChanged;
            checkedListBox1.DrawItem += CheckedListBox1_DrawItem;
            //checkedListBox1.SelectedIndexChanged += CheckedListBox1_SelectedIndexChanged;save

        }
        public static void enableDoubleBuff(System.Windows.Forms.Control cont)
        {
            System.Reflection.PropertyInfo DemoProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            DemoProp.SetValue(cont, true, null);
        }
        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = bunifuTextBox1.Text.ToLower();

            // Clear previous selections
            checkedListBox1.ClearSelected();
            int breaker = 0;
            // Loop through items and highlight/select matching ones
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
               
                string itemText = checkedListBox1.Items[i].ToString().ToLower();

                if (itemText.ToString() == searchTerm)
                {
                    checkedListBox1.SetSelected(i, true);
                    breaker= 1;
                }

                else if (itemText.Contains(searchTerm))
                {
                    if(breaker<1)
                    { 
                    checkedListBox1.SetSelected(i, true);
                    }
                }
                //else
                //{
                //    checkedListBox1.ClearSelected();
                //}

            }
            if(bunifuTextBox1.Text=="")
            {
                checkedListBox1.ClearSelected();
            }
        }


        private void CheckedListBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= checkedListBox1.Items.Count)
                return;

            string itemText = checkedListBox1.Items[e.Index].ToString();

            e.DrawBackground();

            if (checkedListBox1.GetItemChecked(e.Index))
            {
                // Highlight selected items with a yellow background
                using (SolidBrush highlightBrush = new SolidBrush(Color.Yellow))
                {
                    e.Graphics.FillRectangle(highlightBrush, e.Bounds);
                }

                TextRenderer.DrawText(e.Graphics, itemText, e.Font, e.Bounds, Color.Black);
            }
            else
            {
                TextRenderer.DrawText(e.Graphics, itemText, e.Font, e.Bounds, e.ForeColor);
            }

            e.DrawFocusRectangle();
        }
        String token;
        String ID;
        int IDstore;
        string storesID;
        public static string SetValueForToken = "";
        string str = ConfigurationManager.AppSettings["str"];
        string[] values;
        private void StoreFrm_Load(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            token = Dashboard.SetValueForToken;


            ID = Dashboard.ID;
            Loadcompanydetails();
            checkedListBox1.Items.Add("Printer 1");
            checkedListBox1.Items.Add("Printer 2");
            // Define column headers
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
         
           DTitems= loadAllStores();
           
            Cursor = Cursors.Arrow;
        }

        private DataTable loadAllStores()
        {
            DataTable stores=new DataTable(); ;
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
                    stores = JsonConvert.DeserializeObject<DataTable>(json);
                    if(stores.Rows.Count>0)
                    { 
                    stores.Columns.Remove("isDeleted");
                    stores.Columns.Remove("createdBy");
                    stores.Columns.Remove("createdOn");
                    stores.Columns.Remove("lastUpdatedBy");
                    stores.Columns.Remove("lastUpdatedOn");

                    storegrid.DataSource = stores;
                    storegrid.Columns["storeID"].HeaderText = " Store ID";
                    storegrid.Columns["storeCode"].HeaderText = "Store Code";
                    storegrid.Columns["storeName"].HeaderText = "Store Name";
                    storegrid.Columns["storeType"].HeaderText = "Store Type";
                    storegrid.Columns["printersCount"].HeaderText = "No. of Printer(s)";
                    storegrid.Columns["labelCode"].HeaderText = "Label Code";


                    storegrid.Columns["storeID"].Visible = false;
                      
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                
            }
            return stores;
        }

      
        private void tabPage1_Click_1(object sender, EventArgs e)
        {

        }

     
        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void CreateTab_Click(object sender, EventArgs e)
        {
            saveBtn.Text = "Save";
            storemainlabel.Text = "Store Creation";
            storecodetxt.Enabled = true;
            Cursor = Cursors.WaitCursor;
            checkedListBox1.Items.Clear();
            storeTypeDD.DataSource = null;
            btn_search.Visible = false;
            //foreach (string printname in PrinterSettings.InstalledPrinters)
            //{
            //    checkedListBox1.Items.Add(printname);
            //}

            var installedPrinters = PrinterSettings.InstalledPrinters;
            DataTable dtta = loadAllUsedPrinters();

            for (int i = 0; i < installedPrinters.Count; i++)
            {
                int check = 0;
                string printName = installedPrinters[i];
                checkedListBox1.Items.Add(printName);
                for (int j = 0; j < dtta.Rows.Count; j++)
                {
                   
                    if (printName == dtta.Rows[j][1].ToString())
                    {
                        checkedListBox1.Items.Remove(printName);
                    }
                    
                }
                
            }

            DataTable dtt = loadAllLabels();
            bunifuDropdown1.DataSource = dtt;
            if(dtt.Rows.Count>0)
            { 
            bunifuDropdown1.DisplayMember = "labelCode";
            bunifuDropdown1.ValueMember = "labelCode";
            }
            bunifuDropdown1.Text = "Select Label";

            //bunifuDropdown1.DataSource = dtt.Columns["labelCode"];
            bunifuPages1.PageIndex = 1;
            DataTable dttt = loadAllStoreTypes();
            storeTypeDD.DataSource = dttt;
            if (dttt.Rows.Count > 0)
            {
                storeTypeDD.DisplayMember = "storeType";
                storeTypeDD.ValueMember = "storeTypeID";
            }
            storeTypeDD.Text = "Select Store Type";
            Cursor = Cursors.Arrow;
        }

        private DataTable loadAllUsedPrinters()
        {
            DataTable dtforPrinters = null;
            using (var client = new HttpClient())
            {

                try
                {
                    client.BaseAddress = new Uri(str);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    RequestParameter.getalllabelsClass cls = new RequestParameter.getalllabelsClass { get = 1 };
                    var response = client.PostAsJsonAsync("api/Printers/GetAllPrinters", cls).Result;
                    var a = response.Content.ReadAsStringAsync();
                    string json = a.Result;
                    dtforPrinters = JsonConvert.DeserializeObject<DataTable>(json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

            }
            return dtforPrinters;
        }

        public DataTable loadAllLabels()
        {
            DataTable dtforlabel=null;
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
                    dtforlabel = JsonConvert.DeserializeObject<DataTable>(json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
               
            }
            return dtforlabel;
        }
        public DataTable loadAllStoreTypes()
        {
            DataTable dtforlabel = null;
            using (var client = new HttpClient())
            {

                try
                {
                    client.BaseAddress = new Uri(str);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                   
                    var response = client.GetAsync("api/storeType/GetAllStoreTypes").Result;
                    var a = response.Content.ReadAsStringAsync();
                    string json = a.Result;
                    dtforlabel = JsonConvert.DeserializeObject<DataTable>(json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

            }
            return dtforlabel;
        }
        public DataTable loadAllLabelsagainststore(string code)
        {
            DataTable dtforlabel = null;
            using (var client = new HttpClient())
            {

                try
                {
                    client.BaseAddress = new Uri(str);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    RequestParameter.getallstoreClassgainstStore cls = new RequestParameter.getallstoreClassgainstStore { storeCode = code };
                    var response = client.PostAsJsonAsync("api/Printers/GetAllPrintersAgainstStoreCode", cls).Result;
                    var a = response.Content.ReadAsStringAsync();
                    string json = a.Result;
                    dtforlabel = JsonConvert.DeserializeObject<DataTable>(json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

            }
            return dtforlabel;
        }
        private void StoreList_Click(object sender, EventArgs e)
        {
            bunifuPages1.PageIndex = 0;
            btn_search.Visible = true;
            btn_search.Text = "";
        }


        private void saveBtn_Click(object sender, EventArgs e)
        {
            string storeType;

            storeType = storeTypeDD.SelectedValue.ToString();
            if (storecodetxt.Text != "" || storenametxt.Text != "" || bunifuDropdown1.Text != "Select" || storeTypeDD.Text != "Select Store Type")
            {
                List<RequestParameter.printerss> printerList = new List<RequestParameter.printerss>();
                var texts = this.checkedListBox1.CheckedItems.Cast<object>()
                .Select(x => this.checkedListBox1.GetItemText(x));
                string a = string.Join(",", texts);

                values = a.Split(',');

                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = values[i].Trim();

                    printerList.Add(new RequestParameter.printerss
                    {
                        storeCode = storecodetxt.Text,
                        printerName = values[i]
                    });
                }


                var json2 = JsonConvert.SerializeObject(new
                {
                    printerList
                });
                string labeltext;
                if (bunifuDropdown1.Text == "Select Label")
                {
                    labeltext = "";
                }
                else
                {
                    labeltext = bunifuDropdown1.Text;
                }
              
                
                if (saveBtn.Text == "Save")
                {
                    Cursor = Cursors.WaitCursor;
                    using (var client = new HttpClient())
                    {
                        
                        try
                        {
                            if (checkedListBox1.CheckedItems.Count < 1)
                            {

                                var confirmResult = MessageBox.Show("Are you sure to you want to add without printers?",
                                               "Confirmation!!",
                                               MessageBoxButtons.YesNo);
                                if (confirmResult == DialogResult.Yes)
                                {
                                    client.BaseAddress = new Uri(str);
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                                    RequestParameter.createstoreClass cls = new RequestParameter.createstoreClass { add = 1, storeCode = storecodetxt.Text, storeName = storenametxt.Text, lastUpdatedBy = int.Parse(ID), createdBy = int.Parse(ID), printerList = printerList, labelCode = labeltext,storeType=int.Parse(storeType)  };
                                    var response = client.PostAsJsonAsync("api/Stores/InsertStore", cls).Result;

                                    var aa = response.Content.ReadAsStringAsync();
                                    string json = aa.Result;
                                    RequestParameter.createstoreClass Reply = JsonConvert.DeserializeObject<RequestParameter.createstoreClass>(json);
                                    // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);




                                    // loadAllUncheckedBrands();
                                   


                                    if (Reply.status != "200")
                                    {
                                       
                                        bunifuSnackbar1.Show(this, Reply.message,
                                                                            Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error, 5000, "",
                                                                            Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                                                            Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
                                    
                                    }
                                    else
                                    {
                                        bunifuPages1.PageIndex = 0;
                                        loadAllStores();
                                        bunifuSnackbar1.Show(this, Reply.message,
                                                           Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 5000, "",
                                                           Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                                           Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
                                        storecodetxt.Text = "";
                                        storenametxt.Text = "";
                                        bunifuDropdown1.Text = "Select Label";
                                        storeTypeDD.Text = "Select Store Type";
                                    }
                                }
                            }
                            else
                                {
                                    client.BaseAddress = new Uri(str);
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                                    RequestParameter.createstoreClass cls = new RequestParameter.createstoreClass { add = 1, storeCode = storecodetxt.Text, storeName = storenametxt.Text, lastUpdatedBy = int.Parse(ID), createdBy = int.Parse(ID), printerList = printerList, labelCode = labeltext, storeType=int.Parse(storeType) };
                                    var response = client.PostAsJsonAsync("api/Stores/InsertStore", cls).Result;

                                    var aa = response.Content.ReadAsStringAsync();
                                    string json = aa.Result;
                                    RequestParameter.createstoreClass Reply = JsonConvert.DeserializeObject<RequestParameter.createstoreClass>(json);
                                // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);
                                if (Reply.status != "200")
                                {
                                 
                                    bunifuSnackbar1.Show(this, Reply.message,
                                                                        Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error, 5000, "",
                                                                        Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                                                        Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);

                                }
                                else
                                {
                                    bunifuPages1.PageIndex = 0;
                                    loadAllStores();
                                    bunifuSnackbar1.Show(this, Reply.message,
                                                       Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 5000, "",
                                                       Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                                       Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
                                    storecodetxt.Text = "";
                                    storenametxt.Text = "";
                                    bunifuDropdown1.Text = "Select Label";
                                    storeTypeDD.Text = "Select Store Type";
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


                if (saveBtn.Text == "Update")
                {

                    Cursor = Cursors.WaitCursor;
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            if (checkedListBox1.CheckedItems.Count < 1)
                            {

                                var confirmResult = MessageBox.Show("Are you sure to you want to add without printers?",
                                               "Confirmation!!",
                                               MessageBoxButtons.YesNo);
                                if (confirmResult == DialogResult.Yes)
                                {
                                    client.BaseAddress = new Uri(str);
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                                RequestParameter.createstoreClass cls = new RequestParameter.createstoreClass { update = 1, storeID = IDstore, storeCode = storecodetxt.Text, storeName = storenametxt.Text, lastUpdatedBy = int.Parse(ID), createdBy = int.Parse(ID), printerList = printerList, labelCode = labeltext, storeType = int.Parse(storeType) };
                                var response = client.PostAsJsonAsync("api/Stores/UpdateStore", cls).Result;

                             var aa = response.Content.ReadAsStringAsync();
                             string json = aa.Result;
                             RequestParameter.createstoreClass Reply = JsonConvert.DeserializeObject<RequestParameter.createstoreClass>(json);
                             // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);
                             loadAllStores();
                             bunifuSnackbar1.Show(this, Reply.message,
                                               Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 5000, "",
                                               Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                               Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
                                  storecodetxt.Text = "";
                                  storenametxt.Text = "";
                                    bunifuDropdown1.Text = "Select Label";
                                    storeTypeDD.Text = "Select Store Type";

                                    // loadAllUncheckedBrands();
                                    bunifuPages1.PageIndex = 0;
                                    btn_search.Visible = true;
                                }
                            }
                            else
                            {
                                client.BaseAddress = new Uri(str);
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                                RequestParameter.createstoreClass cls = new RequestParameter.createstoreClass { update = 1, storeID = IDstore, storeCode = storecodetxt.Text, storeName = storenametxt.Text, lastUpdatedBy = int.Parse(ID), createdBy = int.Parse(ID), printerList = printerList, labelCode = labeltext, storeType = int.Parse(storeType) };
                                var response = client.PostAsJsonAsync("api/Stores/UpdateStore", cls).Result;

                                var aa = response.Content.ReadAsStringAsync();
                                string json = aa.Result;
                                RequestParameter.createstoreClass Reply = JsonConvert.DeserializeObject<RequestParameter.createstoreClass>(json);
                                // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);
                                loadAllStores();
                                bunifuSnackbar1.Show(this, Reply.message,
                                                  Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 5000, "",
                                                  Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                                  Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
                                storecodetxt.Text = "";
                                storenametxt.Text = "";
                                bunifuDropdown1.Text = "Select Label";
                                storeTypeDD.Text = "Select Store Type";


                                // loadAllUncheckedBrands();
                                bunifuPages1.PageIndex = 0;
                                btn_search.Visible = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
                        }
                    }
                    Cursor = Cursors.Arrow; // change cursor to normal type
                }
            }
            else
            {
                bunifuSnackbar1.Show(this, "Fields Cannot be empty",
                                              Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 2000, "",
                                              Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                              Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
            }
        }

        private void storegrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btn_search.Visible = false;
            storemainlabel.Text = "Store Updation";

            storecodetxt.Enabled = false;
            Cursor = Cursors.WaitCursor;


            //  brandid = brandgird.Rows[e.RowIndex].Cells["brandID"].Value.ToString();
            //  brandcode = brandgird.Rows[e.RowIndex].Cells["brandCode"].Value.ToString();
            //  brandNameTextBox.Text = brandgird.Rows[e.RowIndex].Cells[1].Value.ToString();
            //instantiate Bunifu Loader using Bunifu.UI.WinForms

            //    loadAllUncheckedBrandsalongwithBrand();
            IDstore = int.Parse(storegrid.Rows[e.RowIndex].Cells["storeID"].Value.ToString());
            storecodetxt.Text = storegrid.Rows[e.RowIndex].Cells["storeCode"].Value.ToString();
            storenametxt.Text = storegrid.Rows[e.RowIndex].Cells["storeName"].Value.ToString();
            //storecodetxt.Text = storegrid.Rows[e.RowIndex].Cells["storeCode"].Value.ToString();
            bunifuDropdown1.DataSource = null;
            DataTable dtt = loadAllLabels();
            bunifuDropdown1.DataSource = dtt;
            if (dtt.Rows.Count > 0) {
                bunifuDropdown1.DisplayMember = "labelCode";
                bunifuDropdown1.ValueMember = "labelCode";
                bunifuDropdown1.SelectedValue = storegrid.Rows[e.RowIndex].Cells["labelCode"].Value.ToString();
            }
            DataTable dttt = loadAllLabelsagainststore(storegrid.Rows[e.RowIndex].Cells["storeCode"].Value.ToString());

            DataTable dtta = loadAllUsedPrinters();
            DataTable dtfornotforstorecode = dtfornotforStorecode(storegrid.Rows[e.RowIndex].Cells["storeCode"].Value.ToString());

            storeTypeDD.DataSource = null;
            checkedListBox1.Items.Clear();
            for (int i = 0; i < dttt.Rows.Count; i++)
            {
                checkedListBox1.Items.Add(dttt.Rows[i][1].ToString());
                checkedListBox1.SetItemChecked(i, true);
            }


            var installedPrinters = PrinterSettings.InstalledPrinters;

            for (int i = 0; i < installedPrinters.Count; i++)
            {
                int check = 0;
                string printName = installedPrinters[i];
                for (int j = 0; j < dttt.Rows.Count; j++)
                {

                    if (printName == dttt.Rows[j][1].ToString())
                    {
                        check = 1;
                    }
                }
                if (check == 0)
                {
                    checkedListBox1.Items.Add(printName);
                }
            }


            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                int check = 0;
                string printName = checkedListBox1.Items[i].ToString();
                // checkedListBox1.Items.Add(printName);


                for (int j = 0; j < dtfornotforstorecode.Rows.Count; j++)
                {

                    if (printName == dtfornotforstorecode.Rows[j][1].ToString())
                    {
                        checkedListBox1.Items.Remove(printName);
                    }

                }

            }

            DataTable dtttt = loadAllStoreTypes();
            storeTypeDD.DataSource = dtttt;
            if (dtttt.Rows.Count > 0)
            {
                storeTypeDD.DisplayMember = "storeType";
                storeTypeDD.ValueMember = "storeTypeID";
            }
            if (storegrid.Rows[e.RowIndex].Cells["storeType"].Value.ToString().ToLower() == "highway")
            {
                storeTypeDD.SelectedValue = 2;
            }
            if (storegrid.Rows[e.RowIndex].Cells["storeType"].Value.ToString().ToLower() == "airport")
            {
                storeTypeDD.SelectedValue = 3;
            }
            if (storegrid.Rows[e.RowIndex].Cells["storeType"].Value.ToString().ToLower() == "icitypg")
            {
                storeTypeDD.SelectedValue = 1;
            }
            saveBtn.Text = "Update";
            bunifuPages1.PageIndex = 1;
            Cursor = Cursors.Arrow; // change cursor to normal type
        }

        private DataTable dtfornotforStorecode(string code )
        {
            DataTable dtforPrinters = null;
            using (var client = new HttpClient())
            {

                try
                {
                    client.BaseAddress = new Uri(str);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    RequestParameter.getallstoreClassgainstStore cls = new RequestParameter.getallstoreClassgainstStore { storeCode = code };
                    var response = client.PostAsJsonAsync("api/Printers/GetAllPrintersNotForStoreCode", cls).Result;
                    var a = response.Content.ReadAsStringAsync();
                    string json = a.Result;
                    dtforPrinters = JsonConvert.DeserializeObject<DataTable>(json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

            }
            return dtforPrinters;
        }

        private void bunifuPictureBox2_Click(object sender, EventArgs e)
        {
            SetValueForToken = token;

            Dashboard dash = new Dashboard();
            this.Hide();
            dash.ShowDialog();
          
            this.Close();
        }

        private void storegrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var confirmResult = MessageBox.Show("Are you sure to delete this item ??",
                                        "Confirm Delete!!",
                                        MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    Cursor = Cursors.WaitCursor;
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            client.BaseAddress = new Uri(str);
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                            storesID = storegrid["storeID", storegrid.CurrentCell.RowIndex].Value.ToString();
                            RequestParameter.deletestoreClass lgn = new RequestParameter.deletestoreClass { storeID = int.Parse(storesID), delete = 1, update = 1, lastUpdatedBy = int.Parse(ID) };
                            var response = client.PostAsJsonAsync("api/Stores/DeleteStore", lgn).Result;

                            var a = response.Content.ReadAsStringAsync();
                            string json = a.Result;
                            RequestParameter.deletelabelClass Reply = JsonConvert.DeserializeObject<RequestParameter.deletelabelClass>(json);
                            // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);
                            //MessageBox.Show(Reply.message);
                            bunifuSnackbar1.Show(this, Reply.message.ToString(),
                                        Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 1000, "",
                                        Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopCenter,
                                        Bunifu.UI.WinForms.BunifuSnackbar.Hosts.FormOwner);
                            loadAllStores();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
                        }
                    }
                    Cursor = Cursors.Arrow;
                }
                else
                {
                    // If 'No', do something here.
                }

            }
        }

        private void storegrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private List<int> selectedIndexes = new List<int>(); // Store the selected item indexes

        private void button1_Click(object sender, EventArgs e)
        {
            
            // Iterate through the items and hide/unhide them based on the search term
            string searchTerm = bunifuTextBox1.Text.ToLower(); // Convert search term to lowercase for case-insensitive search
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                string itemText = checkedListBox1.Items[i].ToString().ToLower();

                // Determine whether to highlight the item based on the search term
                bool shouldHighlight = itemText.Contains(searchTerm);

                // Set the font style to bold for highlighting or regular for normal
                Font font = shouldHighlight ? new Font(checkedListBox1.Font, FontStyle.Bold) : checkedListBox1.Font;
                checkedListBox1.Items[i] = new CustomCheckedListBoxItem(checkedListBox1.Items[i].ToString(), shouldHighlight, font);
            }

            checkedListBox1.EndUpdate(); // Resume updates
        }
        public class CustomCheckedListBoxItem
        {
            public string Text { get; }
            public bool Checked { get; }
            public Font Font { get; }

            public CustomCheckedListBoxItem(string text, bool checkedState, Font font)
            {
                Text = text;
                Checked = checkedState;
                Font = font;
            }

            public override string ToString()
            {
                return Text;
            }
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
        DataTable DTitems;
        private void btn_search_TextChange(object sender, EventArgs e)
        {
            string searchKeyword = btn_search.Text.Trim().ToLower();
            // Your logic to handle the complete scanned input
            // MessageBox.Show("Scanned: " + input);

            DataView dv = DTitems.DefaultView;
                dv.RowFilter = $"storeCode LIKE '%{searchKeyword}%' OR storeName LIKE '%{searchKeyword}%'";

            // Reset the BindingSource to apply the filtered DataView.
            storegrid.DataSource = dv;

            }
        }
}
