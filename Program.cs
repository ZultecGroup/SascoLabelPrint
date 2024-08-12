using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sasco
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Registration registration = new Registration();

            //if (!registration.IsSoftwareLicenseValid)
            //{
            //    registration.ShowDialog();

            //    if (!registration.IsRegistered)
            //        return;
            //}

            ApplicationSettings();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginFrm());
        }

        private static void ApplicationSettings()
        {
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;

            // Creating a Global culture specific to our application.
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo(appSettings["Region"] ?? "en-US");

            // Creating the DateTime Information specific to our application.
            System.Globalization.DateTimeFormatInfo dateTimeInfo = new System.Globalization.DateTimeFormatInfo();

            // Defining various date and time formats.
            dateTimeInfo.DateSeparator = appSettings["DateSeparator"] ?? "/";
            dateTimeInfo.LongDatePattern = appSettings["LongDatePattern"] ?? "MM/dd/yyyy hh:mm:ss tt";
            dateTimeInfo.ShortDatePattern = appSettings["ShortDatePattern"] ?? "MM/dd/yyyy";
            dateTimeInfo.LongTimePattern = appSettings["LongTimePattern"] ?? "hh:mm:ss tt";
            dateTimeInfo.ShortTimePattern = appSettings["ShortTimePattern"] ?? " hh:mm tt";

            // Setting application with date time format.
            cultureInfo.DateTimeFormat = dateTimeInfo;

            // Assigning our custom Culture to the application.
            Application.CurrentCulture = cultureInfo;
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
    }
}
