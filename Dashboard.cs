using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using ExcelApp = Microsoft.Office.Interop.Excel;
using System.Windows.Threading;

namespace Sasco
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            enableDoubleBuff(panel1);
            enableDoubleBuff(panel2);
            enableDoubleBuff(bunifuGradientPanel1);
          
         
            enableDoubleBuff(Setting);

        }
        public static string LabelText { get; set; }
        private FileWatcherUtility fileWatcherUtilityy;
        String token;
        String userID;
        public static string SetValueForToken = "";
        public static string ID = "";
        string str = ConfigurationManager.AppSettings["str"];
        public static void enableDoubleBuff(System.Windows.Forms.Control cont)
        {
            System.Reflection.PropertyInfo DemoProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            DemoProp.SetValue(cont, true, null);
        }
        private void Dashboard_Load(object sender, EventArgs e)
        {
            //     this.Size = new Size(
            //Screen.PrimaryScreen.WorkingArea.Width, // Set width to screen width
            //Screen.PrimaryScreen.WorkingArea.Height - 40 // Adjust for taskbar (e.g., 40 pixels)
            //);
            this.Text = "Shelf Label Printing";
            //this.FormBorderStyle = FormBorderStyle.Sizable;
            //this.WindowState = FormWindowState.Maximized;
            //this.ControlBox = true; // Enable the control box (minimize and close buttons)
            //this.MinimizeBox = true; // Enable the minimize button
            //this.MaximizeBox = false; // Disable the maximize button
            //this.FormBorderStyle=FormBorderStyle.None;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            token = LoginFrm.SetValueForToken;
            userID = LoginFrm.userId;
            Loadcompanydetails();
            DataTable dt = Loadsettings();
            if(dt.Rows.Count>0)
            {
                Cursor = Cursors.WaitCursor;
                fileWatcherUtilityy = new FileWatcherUtility();
                fileWatcherUtilityy.InitializeWatcher(dt.Rows[0]["networkFolderPath"].ToString(), "*.xlsx", token);
               
                Cursor = Cursors.Arrow;
                
            }

        }
       
        public DataTable Loadsettings()
        {
            DataTable dt = new DataTable();
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
                    dt = JsonConvert.DeserializeObject<DataTable>(json);

                  

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            return dt;
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
        private void bunifuTileButton6_Click(object sender, EventArgs e)
        {
            fileWatcherUtilityy.StopWatching();
            SetValueForToken = token;
            ID = userID;
            SettingFrm dash = new SettingFrm();
            this.Hide();
            dash.ShowDialog();
            this.Close();
        }

        //private void bunifuTileButton5_Click(object sender, EventArgs e)
        //{
        //    this.Hide();
        //    DeviceInit dash = new DeviceInit();
        //    this.Close();
        //    dash.ShowDialog();
        //}

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        //private void bunifuTileButton5_Click_1(object sender, EventArgs e)
        //{
            
        //    DeviceInit dash = new DeviceInit();
        //    this.Hide();
        //    dash.ShowDialog();
        //    this.Close();
        
        //}

        private void bunifuTileButton1_Click(object sender, EventArgs e)
        {
            fileWatcherUtilityy.StopWatching();
            SetValueForToken = token;
            ID = userID;
            UserFrm us = new UserFrm();

            this.Hide();
            us.ShowDialog();
            
            this.Close();
        }

        private void Store_Click(object sender, EventArgs e)
        {
            fileWatcherUtilityy.StopWatching();
            SetValueForToken = token;
            ID = userID;
            StoreFrm dash = new StoreFrm();
            this.Hide();
            dash.ShowDialog();
            
            this.Close();
        }

        //private void Brand_Click(object sender, EventArgs e)
        //{
        //    BrandFrm dash = new BrandFrm();
        //    dash.ShowDialog();
        //    this.Close();
        //}

        private void CompanyInfo_Click(object sender, EventArgs e)
        {
            fileWatcherUtilityy.StopWatching();
            SetValueForToken = token;
            ID = userID;
            CompanyInformationFrm dash = new CompanyInformationFrm();
            this.Hide();
            dash.ShowDialog();
          
            this.Close();
        }

        private void LabelClick_Click(object sender, EventArgs e)
        {
            fileWatcherUtilityy.StopWatching();
            SetValueForToken = token;
            ID = userID;
            LabelFrm dash = new LabelFrm();
            this.Hide();
            dash.ShowDialog();
            this.Close();
        }

        private void bunifuTileButton2_Click(object sender, EventArgs e)
        {
            fileWatcherUtilityy.StopWatching();
            SetValueForToken = token;
            ID = userID;
            StoresMultipleFrm dash = new StoresMultipleFrm();
            this.Hide();
            dash.ShowDialog();
            this.Close();

        }

        private void bunifuTileButton3_Click(object sender, EventArgs e)
        {
            fileWatcherUtilityy.StopWatching();
            LoginFrm dash = new LoginFrm();
            this.Hide();
            dash.ShowDialog();
            this.Close();
        }
    }
}
