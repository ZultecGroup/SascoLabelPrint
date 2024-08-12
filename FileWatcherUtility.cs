using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Data;
using Newtonsoft.Json;
using System.Net.Http;
using System.Configuration;
using System.Net.Http.Headers;
using Microsoft.Office.Interop.Excel;
namespace Sasco
{
    
    public class FileWatcherUtility
    {
        string msgg2 = "";
        private LoaderFrm load = new LoaderFrm();
        public static FileSystemWatcher FileWatcher { get; private set; }
        private FileSystemWatcher fileWatcher;
        public void ChangeLabelText(string newText)
        {
            Dashboard.LabelText = newText; // Set the label text directly
        }
        public void StopWatching()
        {

            FileWatcher.EnableRaisingEvents = false;
            FileWatcher.Dispose();
           
        }
        public  void InitializeWatcher(string path, string filter, string token)
        {
            //Dashboard parentObj = new Dashboard();
          

            if (Directory.Exists(path))
            {
                

                FileWatcher = new FileSystemWatcher
            {
                Path = path,
                Filter = filter,
                NotifyFilter = NotifyFilters.LastWrite
            };
           


                // Attach event handlers here
                FileWatcher.Changed += new FileSystemEventHandler(OnFileChanged);
                // Add more event handlers as needed

                FileWatcher.EnableRaisingEvents = true;
            }
            else
            {
                MessageBox.Show("Folder path not found, please check in Admin Settings Folder Path");
            }
        }
    
        private  void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Form uiThreadForm = System.Windows.Forms.Application.OpenForms[0];
            uiThreadForm.Invoke((MethodInvoker)delegate {
                load.Show();
            });

            //MessageBox.Show("File is about to be imported,click ok to process and wait for the import message");
            FileWatcher.EnableRaisingEvents = false;
            string filePath = e.FullPath;

         string textt=   ReadExcelFile(e.FullPath);
            FileWatcher.EnableRaisingEvents = true;
            // Handle the file change event here
            uiThreadForm.Invoke((MethodInvoker)delegate {
                load.Hide();
            });
            MessageBox.Show(textt);

            // Perform actions like reading, updating, or processing the file
        }


        private static string ReadExcelFile(string filePath)
        {
            string msgg = "";
            string str = ConfigurationManager.AppSettings["str"];
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook excelWorkbook = excelApp.Workbooks.Open(filePath);
            //List<string> sheetNames = new List<string>();
            List<RequestParameter.importStoreList> sheetNames = new List<RequestParameter.importStoreList>();

            try
            {
                foreach (Worksheet sheet in excelWorkbook.Sheets)
                {
                    sheetNames.Add(new RequestParameter.importStoreList
                    {
                        storeType = sheet.Name
                    });
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
              //  Console.WriteLine(ex.Message);
            }

            System.Data.DataTable dataTableForStoreNames = new System.Data.DataTable();
            #region Send sheet name to API
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(str);
                    //client.Timeout = TimeSpan.FromMinutes(5);
                    client.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
                    // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    RequestParameter.importStoreTypes cls = new RequestParameter.importStoreTypes { importStoreTypesData = sheetNames };
                    var response = client.PostAsJsonAsync("api/StoreType/ImportStoreTypes", cls).Result;

                    var aa = response.Content.ReadAsStringAsync();
                    string json = aa.Result;
                    dataTableForStoreNames = JsonConvert.DeserializeObject<System.Data.DataTable>(json);

                    // RequestParameter.importClass Reply = JsonConvert.DeserializeObject<RequestParameter.importClass>(json);
                    // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);
                    //    MessageBox.Show(Reply.message);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            #endregion

            //from here we can go for store type
            Microsoft.Office.Interop.Excel.Worksheet excelWorksheet= excelWorkbook.Sheets[1];
            System.Data.DataTable dataTable = new System.Data.DataTable();
            int rows = excelWorksheet.UsedRange.Rows.Count;
            int cols = excelWorksheet.UsedRange.Columns.Count;

            // Add columns to the DataTable
            for (int col = 1; col <= cols; col++)
            {
                var cellValue = excelWorksheet.Cells[1, col].Value;
                string columnName = cellValue != null ? cellValue.ToString() : "Column" + col;
                dataTable.Columns.Add(columnName);
            }
            dataTable.Columns.Add("storeTypeID", typeof(string));
            for (int j = 0; j < sheetNames.Count; j++)
            {

                excelWorksheet = excelWorkbook.Sheets[j + 1]; // Access the first sheet (index 1)

                try
                {
                    rows = excelWorksheet.UsedRange.Rows.Count;
                    cols = excelWorksheet.UsedRange.Columns.Count;

                    string storeType = "";
                    storeType = GetStoreTypeID(dataTableForStoreNames, excelWorksheet.Name);

                    //if (excelWorksheet.Name == "ICITYPG")
                    //{
                    //    storeType = "1";
                    //}
                    //if (excelWorksheet.Name == "HIGHWAY")
                    //{
                    //    storeType = "2";
                    //}
                    //if (excelWorksheet.Name == "Airport")
                    //{
                    //    storeType = "3";
                    //}
                    cols = cols + 1;
                    // Add rows to the DataTable
                    for (int row = 2; row <= rows; row++) // Start from the second row
                    {
                        DataRow dataRow = dataTable.NewRow();
                        for (int col = 1; col <= cols; col++)
                        {
                            if (cols == col)
                            {
                                dataRow[col - 1] = storeType;
                            }
                            else
                            {
                                dataRow[col - 1] = excelWorksheet.Cells[row, col].Value;
                            }
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }
                catch
                {

                }
            }
            excelWorkbook.Close();
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorksheet);
              
            

            List<RequestParameter.importFileList> importData = new List<RequestParameter.importFileList>();

             for (int i = 0; i < dataTable.Rows.Count; i++)
             {
              
                importData.Add(new RequestParameter.importFileList
                {

                    itemNo = dataTable.Rows[i]["Item"].ToString(),
                    //engName = dataTable.Rows[i]["Item Name Eng"].ToString(),
                    engName = dataTable.Rows[i]["ENG name"].ToString(),
                    //arName = dataTable.Rows[i]["Item Name AR"].ToString(),
                    arName = dataTable.Rows[i]["AR Name"].ToString(),
                    barcode = dataTable.Rows[i]["Barcode"].ToString(),
                    unit = dataTable.Rows[i]["Unit"].ToString(),
                    //price = dataTable.Rows[i]["Amount in transaction currency"].ToString(),
                    price = dataTable.Rows[i]["Price"].ToString(),
                    //store = dataTable.Rows[i]["Store"].ToString(),
                    store = "",
                
                    //suppCode = dataTable.Rows[i]["Sup#"].ToString(),
                    suppCode = dataTable.Rows[i]["Supplier"].ToString(),
                    storeTypeID = dataTable.Rows[i]["StoreTypeID"].ToString(),

                });
             }


             var json2 = JsonConvert.SerializeObject(new
             {
                importData
             });

             string archivefolderpath="";
                using (var client = new HttpClient())
                {
                    try
                    {
                        client.BaseAddress = new Uri(str);
                        RequestParameter.getalllabelsClass cls = new RequestParameter.getalllabelsClass { get = 1 };
                        var response = client.PostAsJsonAsync("api/FolderSettings/GetFolderPaths", cls).Result;
                        var a = response.Content.ReadAsStringAsync();
                        string json = a.Result;
                    System.Data.DataTable dt = JsonConvert.DeserializeObject<System.Data.DataTable>(json);

                        if (dt.Rows.Count > 0)
                        {
                        archivefolderpath = dt.Rows[0]["archiveFolderPath"].ToString();
                        }
                        //labelgrid.Columns["isDefault"].Visible = false;

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
               
            

             using (var client = new HttpClient())
             {
                try
                {
                    client.BaseAddress = new Uri(str);
                    //client.Timeout = TimeSpan.FromMinutes(5);
                    client.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
                   // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    RequestParameter.importClass cls = new RequestParameter.importClass { importData = importData };
                    var response = client.PostAsJsonAsync("api/ImportData/ImportData", cls).Result;

                    var aa = response.Content.ReadAsStringAsync();
                    string json = aa.Result;
                    RequestParameter.importClass Reply = JsonConvert.DeserializeObject<RequestParameter.importClass>(json);
                    // LoginClass account = JsonConvert.DeserializeObject<LoginClass>(json);
                    msgg= Reply.message;
                    // importGrid.DataSource = null;
                    string sourceFilePath = filePath;
                    DateTime currentTime = DateTime.Now;
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    string timestamp = currentTime.ToString("yyyyMMddHHmmss");
                    fileName = fileName + timestamp + ".xlsx";
                    string destinationFilePath = archivefolderpath +"\\" + fileName;

                    try
                    {
                        File.Move(sourceFilePath, destinationFilePath);
                       // MessageBox.Show("File moved successfully.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error moving file: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            //  Cursor = Cursors.Arrow; // change cursor to normal type

            return msgg;
        }
        //  return dataTable;
        static string GetStoreTypeID(System.Data.DataTable dataTable, string storeType)
        {
            DataRow[] result = dataTable.Select($"storeType = '{storeType}'");
            if (result.Length > 0)
            {
                return result[0]["storeTypeID"].ToString();
            }
            return "Not Found";
        }

    }
   
}
    


