using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sasco
{
    public partial class StoreLabels : Form
    {
        private BackgroundWorker worker;
        int countfora;
        public StoreLabels()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            enableDoubleBuff(panel1);
            enableDoubleBuff(panel2);
            enableDoubleBuff(bunifuGradientPanel1);
            // Initialize BackgroundWorker
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;

            // Set up event handlers
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        String token;
        public static string ID;
        public static string role;
        public static string loginName;
        public static string SetValueForToken = "";
        string str = ConfigurationManager.AppSettings["str"];
        int def;
        string storesID;
        String userID;
        string code; //store code
       
        string storeType; //store type
        string labelcode;
        string labeldesign;
        string labeldesigncopy;
        public static void enableDoubleBuff(System.Windows.Forms.Control cont)
        {
            System.Reflection.PropertyInfo DemoProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            DemoProp.SetValue(cont, true, null);
        }
        DataTable allprinter;
        DataTable DTitems;
        Timer scannerTimer = new Timer();
        string scannedInput = "";
        private Timer timer;
        private void StoreLabels_Load(object sender, EventArgs e)
        {
            if (role != "")
            {

            }
            scannerTimer.Interval = 2000; // Adjust the delay as needed (in milliseconds)
            scannerTimer.Tick += new EventHandler(scannerTimer_Tick);
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;

            token = Dashboard.SetValueForToken;
            code = StoresMultipleFrm.code;
            storeType = StoresMultipleFrm.storetype;
            ID = Dashboard.ID;
            role = LoginFrm.role;
            loginName = LoginFrm.loginName;
            if (role != "")
            {
                token = LoginFrm.SetValueForToken;
                code = LoginFrm.storeCode;
                DayOfWeek wk = DateTime.Today.DayOfWeek;
                timer = new Timer();
                timer.Interval = 10 * 60 * 1000; // 10 minutes in milliseconds
                timer.Tick += Timer_Tick;
                Timer_Tick(null, EventArgs.Empty);

                timer.Start();
            }
            else
            {
                allprinter = loadconcernprinters();
                //  Loadcompanydetails();
                DTitems = loadItems();
                loadItems();
                loadlabeldesign();
                bindingSource = new BindingSource();
                bindingSource.DataSource = itemgridview.DataSource;
                filteredBindingSource = new BindingSource(bindingSource, "");
                itemgridview.DataSource = filteredBindingSource;
            }
        
          


        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            allprinter = loadconcernprinters();
            //  Loadcompanydetails();
            DTitems = loadItems();
            loadItems();
            loadlabeldesign();

            bindingSource = new BindingSource();
            bindingSource.DataSource = itemgridview.DataSource;
            filteredBindingSource = new BindingSource(bindingSource, "");

            // Set filteredBindingSource as the DataGridView's data source
            itemgridview.DataSource = filteredBindingSource;
            string appFolderPath = AppDomain.CurrentDomain.BaseDirectory;

            // Specify the file name and path
            string filePath = Path.Combine(appFolderPath, "dd.txt");
            try
            {
                // Check if the file exists, and create it if it doesn't
                if (!File.Exists(filePath))
                {
                    string currentDate = DateTime.Now.ToString("MM-dd-yyyy");
                    // Create the text file
                    File.WriteAllText(filePath, currentDate);

                }
                else
                {
                    string dateText = File.ReadAllText(filePath);
                    string currentDate = DateTime.Now.ToString("MM-dd-yyyy");

                    //if (currentDate != dateText)
                    //{
                    DataTable roww = new DataTable();
                    //roww = loadrowcountlabel(DateTime.Now.ToString("MM-dd-yyyy"));
                    roww = loadrowcountlabel(dateText);
                    if (roww.Rows.Count > 0)
                    {
                        if (roww.Rows[0][0].ToString() != "0")
                        {
                            MessageBox.Show("You have " + roww.Rows.Count.ToString() + " new / modified labels to print, acknowledge and print");
                            acknowledgement(filePath, currentDate, roww);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        void scannerTimer_Tick(object sender, EventArgs e)
        {
            scannerTimer.Stop(); // Stop the timer

            // Process the scanned input after the delay
            HandleScannedInput(scannedInput);
            scannedInput = ""; // Reset scannedInput for the next scan
        }


        public void acknowledgement(string filep, string datee,DataTable dt_item)
        {
            string a = filep;
            string b = datee;
            using (AcknowledgementFrm passwordForm = new AcknowledgementFrm())
            {
                DialogResult dialogResult = passwordForm.ShowDialog();

                if (dialogResult == DialogResult.OK)
                {
                    File.WriteAllText(filep, datee);
                    string appFolderPath = AppDomain.CurrentDomain.BaseDirectory;
                    string filePatharc = Path.Combine(appFolderPath, "log.txt");
                    // Check if the file exists, and create it if it doesn't
                    if (!File.Exists(filePatharc))
                    {

                        // Create the text file
                        try
                        {
                            using (StreamWriter writer = new StreamWriter(filePatharc, true))
                            {
                                writer.WriteLine("User " + loginName + " acknowledged to print labels on " + datee);
                            }
                            Print.Enabled = false;
                            stopPrint.Enabled = true;
                            lblPrinting.Visible = true;
                            worker.RunWorkerAsync(dt_item);

                            //  Print_Click_acknowledgement(dt_item);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }

                    }
                    else
                    {
                        try
                        {
                            using (StreamWriter writer = new StreamWriter(filePatharc, true))
                            {
                                writer.WriteLine("User " + loginName + " acknowledged to print labels on " + datee);
                            }
                            Print.Enabled = false;
                            stopPrint.Enabled = true;
                            lblPrinting.Visible = true;
                            worker.RunWorkerAsync(dt_item);
                            
                            //MessageBox.Show("loop over");
                            //Print_Click_acknowledgement(dt_item);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                    }
                    // The user entered the correct password.
                    // You can proceed with your desired actions here.
                }
                else if(dialogResult == DialogResult.Ignore)
                {
                    File.WriteAllText(filep, datee);
                    string appFolderPath = AppDomain.CurrentDomain.BaseDirectory;
                    string filePatharc = Path.Combine(appFolderPath, "log.txt");
                    // Check if the file exists, and create it if it doesn't
                    if (!File.Exists(filePatharc))
                    {

                        // Create the text file
                        try
                        {
                            using (StreamWriter writer = new StreamWriter(filePatharc, true))
                            {
                                writer.WriteLine("User " + loginName + " acknowledged to print labels on " + datee);
                            }

                          //  Print_Click_acknowledgement(dt_item);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }

                    }
                    else
                    {
                        try
                        {
                            using (StreamWriter writer = new StreamWriter(filePatharc, true))
                            {
                                writer.WriteLine("User " + loginName + " acknowledged to print labels on " + datee);
                            }

                            //Print_Click_acknowledgement(dt_item);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                    }
                    // The user entered the correct password.
                    // You can proceed with your desired actions here.
                }
                else
                {
                    acknowledgement(a, b, dt_item);
                }
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            BackgroundWorker worker = sender as BackgroundWorker;

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            DataTable data = e.Argument as DataTable;
            Print_Click_acknowledgement(data, worker, e);

        }

        // BackgroundWorker RunWorkerCompleted event handler (handle completion)
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Print.Enabled = true;
            stopPrint.Enabled = false;
            lblPrinting.Visible = false;
           
            if (e.Cancelled)
            {
                // Handle cancellation
                MessageBox.Show("Print(s) cancelled.");
            }
            else if (e.Error != null)
            {
                // Handle errors
                MessageBox.Show("An error occurred: " + e.Error.Message);
            }
            else
            {
                if (countfora > 0)
                {
                    bunifuSnackbar1.Show(this,
                              "All Labels printed",
                              Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success,
                              1500, "DISMISS");
                }
                else
                {
                    bunifuSnackbar1.Show(this,
                    "No item selected",
                    Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Warning,
                    1500, "DISMISS");
                }
                // Handle successful completion
                //MessageBox.Show("Operation completed successfully.");
            }
           
        }
        private void Print_Click_acknowledgement(DataTable dt_item,BackgroundWorker worker, DoWorkEventArgs e)
        {
             countfora = 0;

            for (int k = 0;k < dt_item.Rows.Count;k++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }


                for (int i = 0; i < allprinter.Rows.Count; i++)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    countfora = 1;
                        labeldesigncopy = labeldesign;
                        labeldesigncopy = labeldesigncopy.Replace("#Barcode", dt_item.Rows[k]["barcode"].ToString());
                        labeldesigncopy = labeldesigncopy.Replace("#ItemCode", dt_item.Rows[k]["itemNo"].ToString());
                        labeldesigncopy = labeldesigncopy.Replace("آرديسك#", dt_item.Rows[k]["arName"].ToString());
                        labeldesigncopy = labeldesigncopy.Replace("#EngName", dt_item.Rows[k]["engName"].ToString());
                        labeldesigncopy = labeldesigncopy.Replace("#SuppCode", dt_item.Rows[k]["suppCode"].ToString());
                        labeldesigncopy = labeldesigncopy.Replace("#StoreCode", dt_item.Rows[k]["storeCode"].ToString());
                    // Get the current date
                    DateTime currentDate = DateTime.Now;

                        // Format the date as MMddyy
                        string formattedDate = currentDate.ToString("MMddyy");
                        labeldesigncopy = labeldesigncopy.Replace("#CurrDate", formattedDate);
                        decimal f = decimal.Parse(dt_item.Rows[k]["price"].ToString());
                        string aa = f.ToString("0.00");
                        string[] arr = aa.Split('.');
                        aa = arr[0];
                        string aa2 = "." + arr[1];
                        labeldesigncopy = labeldesigncopy.Replace("#Pri1", aa);
                        labeldesigncopy = labeldesigncopy.Replace("#Pri2", aa2);

                        //byte[] bytes = Encoding.Default.GetBytes(labeldesigncopy);
                        //labeldesigncopy = Encoding.UTF8.GetString(bytes);

                        //var document = new PrintDocument();
                        //document.PrintPage += document_PrintPage;
                        //document.Print();

                        // List the print server's queues


                        PrintDocument printDocument = new PrintDocument();
                        //printDocument.PrinterSettings.PrinterName = "ZDesigner ZD410-203dpi ZPL"; // Set the default Zebra printer
                        printDocument.PrinterSettings.PrinterName = allprinter.Rows[i]["printerName"].ToString(); // Set the default Zebra printer

                        string zplCommand = labeldesigncopy;

                    //System.Threading.Thread.Sleep(5000);
                    RawPrinterHelper.SendStringToPrinter(printDocument.PrinterSettings.PrinterName, zplCommand);
                    printDocument.Print();


                }
            }
            
        }

        public DataTable loadrowcountlabel(string datee)
        {

            DataTable dtforlabel = null;
            using (var client = new HttpClient())
            {

                try
                {
                
                    client.BaseAddress = new Uri(str);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    RequestParameter.getrowcount cls = new RequestParameter.getrowcount { date = datee, storeTypeID = storeType
                    };
                 
                    var response = client.PostAsJsonAsync("api/Items/GetItemsCountAgainstDate", cls).Result;
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
        public DataTable loadconcernprinters()
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

        private void loadlabeldesign()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(str);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    RequestParameter.getalllabelsagainstscodeClass cls = new RequestParameter.getalllabelsagainstscodeClass { storeCode = code };
                    var response = client.PostAsJsonAsync("api/Labels/GetLabelInfoAgainstStoreCode", cls).Result;
                    var a = response.Content.ReadAsStringAsync();
                    string json = a.Result;
                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                    if (dt.Rows.Count < 1)
                    {
                        defaultlabeltxt.Visible = true;
                        itemgridview.ReadOnly = true;
                    }
                    else
                    {


                        labeldesign = dt.Rows[0]["labelText"].ToString();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private DataTable loadItems()
        {

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(str);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    RequestParameter.getalllabelsagainstscodeClass cls = new RequestParameter.getalllabelsagainstscodeClass { storeCode = code };
                    var response = client.PostAsJsonAsync("api/Stores/GetStoreTypeIDAgainstStoreCode", cls).Result;
                    var a = response.Content.ReadAsStringAsync();
                    string json = a.Result;
                    DataTable dta = JsonConvert.DeserializeObject<DataTable>(json);
                    if (dta.Rows.Count < 1)
                    {
                        defaultlabeltxt.Visible = true;
                        itemgridview.ReadOnly = true;
                    }
                    else
                    {


                        storeType = dta.Rows[0]["storeTypeID"].ToString();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }


            DataTable dt = new DataTable();
            Cursor = Cursors.WaitCursor;
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(str);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    RequestParameter.getalllabelsagainstscodeClass cls = new RequestParameter.getalllabelsagainstscodeClass { storeCode = storeType };
                    var response = client.PostAsJsonAsync("api/Items/GetAllItemsAgainstStoreCode", cls).Result;
                    var a = response.Content.ReadAsStringAsync();
                    string json = a.Result;
                    dt = JsonConvert.DeserializeObject<DataTable>(json);

                    //dt.Columns.Remove("createdBy");
                    //dt.Columns.Remove("lastUpdatedBy");
                    if (dt.Rows[0]["id"] == "")
                    {
                        dt.Rows.Clear();
                    }
                    itemgridview.DataSource = dt;
                    itemgridview.Columns["Price"].DefaultCellStyle.Format = "0.00";
                    // itemgridview.Columns["lastModifyDate"].DefaultCellStyle.Format = "d";
                    itemgridview.DefaultCellStyle.Format = "N2";
                    itemgridview.Columns["itemNo"].HeaderText = "Item Code";
                    itemgridview.Columns["engName"].HeaderText = "Eng Item Desc";
                    itemgridview.Columns["arName"].HeaderText = "Ar Item Desc";
                    itemgridview.Columns["barcode"].HeaderText = "Barcode";
                    itemgridview.Columns["unit"].HeaderText = "Unit";
                    itemgridview.Columns["storeCode"].HeaderText = "Store Code";
                    itemgridview.Columns["price"].HeaderText = "Price";
                    itemgridview.Columns["suppCode"].HeaderText = "Supplier Code";
                    itemgridview.Columns["lastModifyDate"].HeaderText = "Modify Date";


                    itemgridview.Columns["id"].Visible = false;
                    itemgridview.Columns["storeCode"].Visible = false;

                    itemgridview.Columns["itemNo"].ReadOnly = true;
                    itemgridview.Columns["engName"].ReadOnly = true;
                    itemgridview.Columns["arName"].ReadOnly = true;
                    itemgridview.Columns["barcode"].ReadOnly = true;
                    itemgridview.Columns["unit"].ReadOnly = true;
                    itemgridview.Columns["storeCode"].ReadOnly = true;
                    itemgridview.Columns["price"].ReadOnly = true;
                    itemgridview.Columns["suppCode"].ReadOnly = true;
                    itemgridview.Columns["lastModifyDate"].ReadOnly = true;

                    //labelgrid.Columns["isDefault"].Visible = false;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            Cursor = Cursors.Arrow;
            return dt;

        }

        private void bunifuGradientPanel1_Click(object sender, EventArgs e)
        {
            Dashboard str = new Dashboard();
            this.Hide();
            str.ShowDialog();
            this.Close();
        }

        private void Print_Click(object sender, EventArgs e)
        {
            int count = 0;
            int columnIndex = itemgridview.CurrentCell.ColumnIndex;
            string columnName = itemgridview.Columns[1].Name;
            string message = string.Empty;
            string str = string.Empty;
            string strname = string.Empty;
            foreach (DataGridViewRow row in itemgridview.Rows)
            {


                bool isSelected = Convert.ToBoolean(row.Cells[0].Value);
                for (int i = 0; i < allprinter.Rows.Count; i++)
                {
                    if (isSelected)
                    {
                        count = 1;
                        labeldesigncopy = labeldesign;
                        labeldesigncopy = labeldesigncopy.Replace("#Barcode", row.Cells["barcode"].Value.ToString());
                        labeldesigncopy = labeldesigncopy.Replace("#ItemCode", row.Cells["itemNo"].Value.ToString());
                        labeldesigncopy = labeldesigncopy.Replace("آرديسك#", row.Cells["arName"].Value.ToString());
                        labeldesigncopy = labeldesigncopy.Replace("#EngName", row.Cells["engName"].Value.ToString());
                        labeldesigncopy = labeldesigncopy.Replace("#SuppCode", row.Cells["suppCode"].Value.ToString());
                        labeldesigncopy = labeldesigncopy.Replace("#StoreCode", row.Cells["storeCode"].Value.ToString());
                        // Get the current date
                        DateTime currentDate = DateTime.Now;

                        // Format the date as MMddyy
                        string formattedDate = currentDate.ToString("MMddyy");
                        labeldesigncopy = labeldesigncopy.Replace("#CurrDate", formattedDate);
                        decimal f = decimal.Parse(row.Cells["price"].Value.ToString());
                        string aa = f.ToString("0.00");
                        string[] arr = aa.Split('.');
                        aa = arr[0];
                        string aa2 = "." + arr[1];
                        labeldesigncopy = labeldesigncopy.Replace("#Pri1", aa);
                        labeldesigncopy = labeldesigncopy.Replace("#Pri2", aa2);

                        //byte[] bytes = Encoding.Default.GetBytes(labeldesigncopy);
                        //labeldesigncopy = Encoding.UTF8.GetString(bytes);

                        //var document = new PrintDocument();
                        //document.PrintPage += document_PrintPage;
                        //document.Print();

                        // List the print server's queues


                        PrintDocument printDocument = new PrintDocument();
                        //printDocument.PrinterSettings.PrinterName = "ZDesigner ZD410-203dpi ZPL"; // Set the default Zebra printer
                        printDocument.PrinterSettings.PrinterName = allprinter.Rows[i]["printerName"].ToString(); // Set the default Zebra printer

                        string zplCommand = labeldesigncopy;
                        RawPrinterHelper.SendStringToPrinter(printDocument.PrinterSettings.PrinterName, zplCommand);
                        printDocument.Print();
                    }
                }
            }
            if (count > 0)
            {
                bunifuSnackbar1.Show(this,
                          "All Labels printed",
                          Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success,
                          1500, "DISMISS");
            }
            else
            {
                bunifuSnackbar1.Show(this,
                "No item selected",
                Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Warning,
                1500, "DISMISS");
            }
        }
        void document_PrintPage(object sender, PrintPageEventArgs e)
        {
            // string zplCommand = labeldesigncopy;

            // Send the ZPL command to the printer
            //   RawPrinterHelper.SendStringToPrinter(printDocument.PrinterSettings.PrinterName, zplCommand);
            //var graphics = e.Graphics;
            //var normalFont = new Font("Calibri", 14);

            //var pageBounds = e.MarginBounds;
            //var drawingPoint = new PointF(pageBounds.Left, (pageBounds.Top + normalFont.Height));

            //graphics.DrawString(labeldesigncopy, normalFont, Brushes.Black, drawingPoint);

            //drawingPoint.Y += normalFont.Height;


            //e.HasMorePages = false; // No pages after this page.
        }

        private void bunifuPictureBox2_Click(object sender, EventArgs e)
        {
            if (role == "User")
            {

                LoginFrm dashed = new LoginFrm();
                this.Hide();
                dashed.ShowDialog();

                this.Close();
            }
            else
            {


                SetValueForToken = token;

                StoresMultipleFrm dash = new StoresMultipleFrm();
                this.Hide();
                dash.ShowDialog();

                this.Close();
            }
        }
        private BindingSource bindingSource;
        private BindingSource filteredBindingSource;
        private void LoadSelectedRows()
        {
            // Load selected rows from storage (e.g., database or a file)
            // For this example, we'll select the first and third rows
            selectedRowIndices = new List<int> { 0, 2 };

            // Restore selection in the DataGridView
            foreach (int index in selectedRowIndices)
            {
                itemgridview.Rows[index].Cells["Column1"].Value = true;
            }
        }

        private void PreserveSelection()
        {
            selectedRowIndices.Clear();

            // Store the indices of selected rows
            for (int i = 0; i < itemgridview.Rows.Count; i++)
            {
                DataGridViewCheckBoxCell cell = itemgridview.Rows[i].Cells["Column1"] as DataGridViewCheckBoxCell;

                // Check if the cell exists and its value is boolean before casting
                if (cell != null && cell.Value is bool)
                {
                    if ((bool)cell.Value)
                    {
                        selectedRowIndices.Add(i);
                    }
                }
            }
        }
        private void RestoreSelection()
        {
            // Reapply the selection based on the stored indices
            foreach (int index in selectedRowIndices)
            {
                itemgridview.Rows[index].Cells["Column1"].Value = true;
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
        //    Dictionary<string, bool> columnVisibilityState = new Dictionary<string, bool>();
        int c = 0;
        private void bunifuTextBox1_TextChange(object sender, EventArgs e)
        {
            if (directprint.Checked == true)
            {
                scannedInput += bunifuTextBox1.Text; // Concatenate scanned characters
                bunifuTextBox1.SelectAll();
                scannerTimer.Stop();
                scannerTimer.Start(); // Restart the timer
            }
            else
            {

                HandleScannedInput("nothing");
            }
        }
        void HandleScannedInput(string input)
        {
            // Your logic to handle the complete scanned input
            // MessageBox.Show("Scanned: " + input);
            if (directprint.Checked == true)
            {
                bunifuTextBox1.Text = input;
            }
            string searchKeyword = bunifuTextBox1.Text.Trim().ToLower();
            // Store the selected columns in the custom control.
            StoreSelectedColumns();

            // Filter the data based on the search keyword.
            DataView dv = DTitems.DefaultView;
            dv.RowFilter = $"itemNo LIKE '%{searchKeyword}%' OR suppCode LIKE '%{searchKeyword}%'OR engName LIKE '%{searchKeyword}%' OR arName LIKE '%{searchKeyword}%' OR unit LIKE '%{searchKeyword}%' OR lastModifyDate LIKE '%{searchKeyword}%' OR price LIKE '%{searchKeyword}%' OR barcode LIKE '%{searchKeyword}%'";

            // Reset the BindingSource to apply the filtered DataView.
            bindingSource.DataSource = dv;

            // Restore the selected columns in the custom control.
            RestoreSelectedColumns();

            if (directprint.Checked == true)
            {
                if (bunifuTextBox1.Text != "")
                {
                    if (itemgridview.Rows.Count > 0 && itemgridview.Rows.Count < 2)
                    {
                        int count = 0;
                        int columnIndex = itemgridview.CurrentCell.ColumnIndex;
                        string columnName = itemgridview.Columns[1].Name;
                        string message = string.Empty;
                        string str = string.Empty;
                        string strname = string.Empty;

                        foreach (DataGridViewRow row in itemgridview.Rows)
                        {

                            // bool isSelected = Convert.ToBoolean(row.Cells[0].Value);
                            for (int i = 0; i < allprinter.Rows.Count; i++)
                            {

                                c = 1;
                                count = 1;
                                labeldesigncopy = labeldesign;
                                labeldesigncopy = labeldesigncopy.Replace("#Barcode", row.Cells["barcode"].Value.ToString());
                                labeldesigncopy = labeldesigncopy.Replace("#ItemCode", row.Cells["itemNo"].Value.ToString());
                                labeldesigncopy = labeldesigncopy.Replace("آرديسك#", row.Cells["arName"].Value.ToString());
                                labeldesigncopy = labeldesigncopy.Replace("#EngName", row.Cells["engName"].Value.ToString());
                                labeldesigncopy = labeldesigncopy.Replace("#SuppCode", row.Cells["suppCode"].Value.ToString());
                                labeldesigncopy = labeldesigncopy.Replace("#StoreCode", row.Cells["storeCode"].Value.ToString());
                                // Get the current date
                                DateTime currentDate = DateTime.Now;

                                // Format the date as MMddyy
                                string formattedDate = currentDate.ToString("MMddyy");
                                labeldesigncopy = labeldesigncopy.Replace("#CurrDate", formattedDate);
                                decimal f = decimal.Parse(row.Cells["price"].Value.ToString());
                                string aa = f.ToString("0.00");
                                string[] arr = aa.Split('.');
                                aa = arr[0];
                                string aa2 = "." + arr[1];
                                labeldesigncopy = labeldesigncopy.Replace("#Pri1", aa);
                                labeldesigncopy = labeldesigncopy.Replace("#Pri2", aa2);

                                //byte[] bytes = Encoding.Default.GetBytes(labeldesigncopy);
                                //labeldesigncopy = Encoding.UTF8.GetString(bytes);

                                //var document = new PrintDocument();
                                //document.PrintPage += document_PrintPage;
                                //document.Print();

                                // List the print server's queues


                                PrintDocument printDocument = new PrintDocument();
                                //printDocument.PrinterSettings.PrinterName = "ZDesigner ZD410-203dpi ZPL"; // Set the default Zebra printer
                                printDocument.PrinterSettings.PrinterName = allprinter.Rows[i]["printerName"].ToString(); // Set the default Zebra printer

                                string zplCommand = labeldesigncopy;
                                RawPrinterHelper.SendStringToPrinter(printDocument.PrinterSettings.PrinterName, zplCommand);
                                printDocument.Print();

                                if (count > 0)
                                {
                                    bunifuSnackbar1.Show(this,
                                              "All Labels printed",
                                              Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success,
                                              1500, "DISMISS");
                                    bunifuTextBox1.SelectAll();
                                }
                                else
                                {
                                    bunifuSnackbar1.Show(this,
                                    "No item selected",
                                    Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Warning,
                                    1500, "DISMISS");
                                    bunifuTextBox1.SelectAll();
                                }

                            }
                        }
                    }

                    else
                    {
                        bunifuSnackbar1.Show(this,
                                      "No item found",
                                      Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Warning,
                                      2500, "DISMISS");
                        //bunifuTextBox1.Text = "";
                        bunifuTextBox1.SelectAll();
                    }
                }
            }

            return;
        }
        private List<int> selectedRowIndices = new List<int>();
        private List<String> barcodeList = new List<String>();

        public void StoreSelectedColumns()
        {
            //  barcodeList.Clear();

            // Store the indices of selected rows
            for (int i = 0; i < itemgridview.Rows.Count; i++)
            {
                DataGridViewCheckBoxCell cell = itemgridview.Rows[i].Cells["Column1"] as DataGridViewCheckBoxCell;

                // Check if the cell exists and its value is boolean before casting
                if (cell != null && cell.Value is bool)
                {
                    if ((bool)cell.Value)
                    {
                        selectedRowIndices.Add(i);
                        barcodeList.Add(itemgridview.Rows[i].Cells["Barcode"].Value.ToString());
                    }
                }
            }
        }

        public void RestoreSelectedColumns()
        {
            // Reapply the selection based on the stored indices
            foreach (string barcode in barcodeList)
            {
                for (int i = 0; i < itemgridview.Rows.Count; i++)
                {
                    if (barcode == itemgridview.Rows[i].Cells["Barcode"].Value.ToString())

                    {
                        DataGridViewCheckBoxCell cell = itemgridview.Rows[i].Cells["Column1"] as DataGridViewCheckBoxCell;
                        cell.Value = true;
                        itemgridview.Rows[i].Selected = true;
                    }
                }
            }
        }
        private bool allRowsSelected = false; // Track the selection state

        private void itemgridview_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex == 0)
            {
                if (allRowsSelected)
                {
                    // Deselect all rows when the header cell is clicked again.
                    for (int i = 0; i < itemgridview.Rows.Count; i++)
                    {
                        itemgridview.Rows[i].Selected = false;
                        DataGridViewCheckBoxCell cell = itemgridview.Rows[i].Cells["Column1"] as DataGridViewCheckBoxCell;
                        cell.Value = false;
                    }

                    allRowsSelected = false;
                }
                else
                {
                    // Select all rows when the header cell is clicked.
                    for (int i = 0; i < itemgridview.Rows.Count; i++)
                    {
                        itemgridview.Rows[i].Selected = true;
                        DataGridViewCheckBoxCell cell = itemgridview.Rows[i].Cells["Column1"] as DataGridViewCheckBoxCell;
                        cell.Value = true;
                    }

                    allRowsSelected = true;
                }
            }

        }

        private void bunifuTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void bunifuLabel1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuCheckBox1_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuCheckBox.CheckedChangedEventArgs e)
        {

        }

        int selecting=0;

    

        private void selectAll(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if (selecting == 0)
                {
                    foreach (DataGridViewRow row in itemgridview.Rows)
                    {

                        DataGridViewCheckBoxCell chkCell = row.Cells[0] as DataGridViewCheckBoxCell;

                        if (chkCell != null)
                        {
                            // Set the checkbox value to true

                            chkCell.Value = true;
                        }
                    }
                    selecting = 1;
                }
                else
                {
                    foreach (DataGridViewRow row in itemgridview.Rows)
                    {

                        DataGridViewCheckBoxCell chkCell = row.Cells[0] as DataGridViewCheckBoxCell;

                        if (chkCell != null)
                        {
                            // Set the checkbox value to true

                            chkCell.Value = false;
                        }
                    }
                    selecting = 0;
                }
            }
        }

        private void stopPrint_Click(object sender, EventArgs e)
        {
            // Cancel the background worker if it is running
            if (worker.IsBusy)
            {
                worker.CancelAsync();
            }
        }
    }
}
