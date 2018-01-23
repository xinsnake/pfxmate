using System;
using System.Deployment.Application;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Documents;

namespace PfxMate.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        X509Certificate2Collection collection;
        X509Certificate2 cert;

        public MainWindow()
        {
            InitWindow();
        }

        public MainWindow(string _certPath, string _certPass)
        {
            InitWindow();
            LoadCertificate(_certPath, _certPass);
        }
        private void InitWindow()
        {
            InitializeComponent();

            string version;
            try
            {
                version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch (InvalidDeploymentException)
            {
                version = " DEBUG";
            }

            Title = "PfxMate - v" + version;
            LabelCertLoaded.Content = "No certificate is loaded.";
        }

        private void LoadCertificate(string certPath)
        {
            LoadCertificate(certPath, "");
        }

        private void LoadCertificate(string certPath, string certPass)
        {
            while (true)
            {
                if (certPass == "")
                {
                    var pwdWindow = new PasswordWindow(certPath);
                    var pwdWindowResult = pwdWindow.ShowDialog();

                    if (pwdWindowResult != true)
                    {
                        return;
                    }

                    certPass = pwdWindow.Password;
                }

                try
                {
                    collection = new X509Certificate2Collection();
                    collection.Import(certPath, certPass, (X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable));

                    LabelCertLoaded.Content = "Cert loaded: " + certPath;
                    SetCertFromCollection();

                    break;
                }
                catch (Exception ex)
                {
                    certPass = "";
                    MessageBox.Show("Error when loading the cert: " + certPath + "\n" + ex.Message, "Invalid Password");
                }
            }
        }

        private void SetCertFromCollection()
        {
            if (collection.Count == 1)
            {
                MessageBox.Show(
                    "Certification chain NOT detected, this certificate may be self-signed!",
                    "Cert Chain Not Detected");
                cert = collection[0];
            }
            else
            {
                foreach (var c in collection)
                {
                    var subject = c.Subject;

                    var found = false;

                    foreach (var c2 in collection)
                    {
                        if (c2.Issuer == subject)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found == false)
                    {
                        cert = c;
                        break;
                    }
                }
            }
            ShowCert();
        }

        private void ShowCert()
        {
            CopyAsBase64Btn.IsEnabled = true;
            CopyTumbprintLowerBtn.IsEnabled = true;
            CopyTumbprintUpperBtn.IsEnabled = true;
            ExportAsPfxBtn.IsEnabled = true;

            var message = "Subject:\n" + cert.Subject + "\n\n";
            message += "Issuer:\n" + cert.Issuer + "\n\n";
            message += "Thumbprint:\n" + cert.Thumbprint + "\n\n";
            message += "Serial Number:\n" + cert.SerialNumber;
            RichTextBoxCertInfo.Document.Blocks.Clear();
            RichTextBoxCertInfo.Document.Blocks.Add(new Paragraph(new Run(message)));
        }

        private void Button_Click_CopyLower(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(cert.Thumbprint.ToLower());
            MessageBox.Show("Thumbprint copied (lowercase).", "Success");
        }

        private void Button_Click_CopyUpper(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(cert.Thumbprint.ToUpper());
            MessageBox.Show("Thumbprint copied (UPPERCASE).", "Success");
        }
        private void Button_Click_CopyBase64(object sender, RoutedEventArgs e)
        {
            var certBase64 = Convert.ToBase64String(collection.Export(X509ContentType.Pkcs12, String.Empty));
            Clipboard.SetText(certBase64);
            MessageBox.Show("Base64 version of the certificate copied.", "Success");
        }

        private void Button_Click_ExportPfx(object sender, RoutedEventArgs e)
        {

            var passwordDialog = new PasswordWindow();
            var passwordResult = passwordDialog.ShowDialog();

            if (passwordResult != true)
            {
                return;
            }

            var certPass = passwordDialog.Password;

            var fileDialog = new System.Windows.Forms.SaveFileDialog()
            {
                Filter = @"PFX Files (*.pfx)|*.pfx"
            };

            var certPath = string.Empty;

            var fileResult = fileDialog.ShowDialog();
            switch (fileResult)
            {
                case System.Windows.Forms.DialogResult.OK:
                    certPath = fileDialog.FileName;
                    break;
                default:
                    break;
            }

            var certificate = collection.Export(X509ContentType.Pkcs12, certPass);

            if (certificate == null)
            {
                MessageBox.Show("Error when exporting the cert: certificate is empty.", "Unalbe to process certificate");
                return;
            }

            try
            {
                File.WriteAllBytes(certPath, certificate);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when exporting the cert: " + ex.Message, "Unalbe to process certificate");
            }
        }
        
        private void MenuItem_LoadPfxCert_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = @"PFX Files (*.pfx)|*.pfx",
                Multiselect = false
            };

            var result = fileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    var certPath = fileDialog.FileName;
                    LoadCertificate(certPath);
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    break;
            }
        }

        private void MenuItem_LoadBase64Cert_Click(object sender, RoutedEventArgs e)
        {
            var loadBase64CertWindow = new LoadBase64CertWindow();
            var loadBase64CertWindowResult = loadBase64CertWindow.ShowDialog();

            if (loadBase64CertWindowResult != true)
            {
                return;
            }

            try
            {
                collection = new X509Certificate2Collection();
                collection.Import(
                    loadBase64CertWindow.CertBase64,
                    loadBase64CertWindow.CertBase64Pwd,
                    (X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable));

                LabelCertLoaded.Content = "Cert loaded from Base64 string";
                SetCertFromCollection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when loading the cert: " + ex.Message, "Unalbe to process certificate");
            }
        }

        private void MenuItem_Close_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }
        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("PfxMate - Xinyun Zhou", "About");
        }
    }
}
