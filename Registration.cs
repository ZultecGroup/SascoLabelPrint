using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms;
using ZulSoft.License;
namespace Sasco
{
    public partial class Registration : Form
    {
        UcRegistration registration;

        public Registration()
        {
            registration = new UcRegistration();

            registration.SelectedEdition = "C";
            registration.ClientCharacter = "L";
            registration.ServerCharacter = "L";

            registration.SupportEmail = "support@zulsoft.com";
            registration.SupportPhone = "+92 42 3530 1417";
            registration.SupportFax = "+92 42 3530 1039";
            registration.SupportPhoneExt = "";

            InitializeComponent();

            registration.Parent = this;
        }

        public bool IsSoftwareLicenseValid
        {
            get
            {
                return registration.IsLicenseValid();
            }
        }

        public bool IsRegistered
        {
            get
            {
                return registration.IsRegistered;
            }
        }

        private void Registration_Load(object sender, EventArgs e)
        {
            registration.Dock = DockStyle.Fill;
            registration.Load += registration_Load;
        }

        void registration_Load(object sender, EventArgs e)
        {

        }
    }
}

