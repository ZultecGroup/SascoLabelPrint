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
    public partial class StoresMultipleFrm : Form
    {
        public StoresMultipleFrm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            enableDoubleBuff(panel1);
            enableDoubleBuff(panel2);
            enableDoubleBuff(bunifuGradientPanel1);
            DataTable storedt = new DataTable();
            token = LoginFrm.SetValueForToken;
            userID = LoginFrm.userId;

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
                    storedt = JsonConvert.DeserializeObject<DataTable>(json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            for (int i = 0; i < storedt.Rows.Count; i++)
            {



                Bunifu.Framework.UI.BunifuTileButton bt = new Bunifu.Framework.UI.BunifuTileButton();
                bt.Name = storedt.Rows[i]["storeCode"].ToString();
                bt.LabelText = storedt.Rows[i]["storeCode"].ToString()+" - "+storedt.Rows[i]["storeName"].ToString();
                bt.BackColor = Color.DodgerBlue;
                bt.color = Color.DodgerBlue;
                bt.colorActive = Color.Black;
                bt.ForeColor = Color.White;
                bt.Width = 170;
                bt.Height = 163;
                bt.LabelPosition = 55;
               
                bt.Click += (sender, e) =>
                {
                    SetValueForToken = token;
                    ID = userID;
                    code = bt.Name.ToString();
                    
                    StoreLabels dash = new StoreLabels();
                    this.Hide();
                    dash.ShowDialog();
                   
                    this.Close();
                };
                flowLayoutPanel1.Controls.Add(bt);

            }
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

        String userID;
        public static string code;
        public static string storetype;



        private void StoresMultipleFrm_Load(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            Loadcompanydetails();

        }

      
        private void bunifuPictureBox2_Click(object sender, EventArgs e)
        {
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

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
