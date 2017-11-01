using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PfxMate
{
    /// <summary>
    /// Interaction logic for LoadBase64Cert.xaml
    /// </summary>
    public partial class LoadBase64CertWindow : Window
    {
        public byte[] CertBase64 { get; set; }
        public string CertBase64Pwd { get; set; }

        public LoadBase64CertWindow()
        {
            InitializeComponent();
        }

        private void Base64BtnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CertBase64 = Convert.FromBase64String(Base64TextBox.Text.Trim());
                CertBase64Pwd = Base64PwdTextBox.Password;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when loading the cert: " + ex.Message, "Unalbe to process certificate");
                Base64TextBox.Text = "";
            }
        }

        private void Base64TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || Base64TextBox.Text.Length == 0) return;

            Base64BtnOk_Click(sender, e);
        }
    }
}
