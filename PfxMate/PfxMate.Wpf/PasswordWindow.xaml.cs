using System.Windows;
using System.Windows.Input;

namespace PfxMate.Wpf
{
    /// <summary>
    /// Interaction logic for PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        public string Password { get; set; }

        public PasswordWindow(string certPath)
        {
            InitializeComponent();

            LabelCert.Content = "Password for " + certPath;
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            Password = PasswordBoxPassword.Password;
            DialogResult = true;
            Close();
        }

        private void PasswordBoxPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || PasswordBoxPassword.Password.Length == 0) return;

            ButtonSubmit_Click(sender, e);
        }
    }
}
