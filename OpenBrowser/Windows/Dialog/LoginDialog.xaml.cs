using System.Windows;
using System.Windows.Media;

namespace OpenBrowser.Windows.Dialog
{
    /// <summary>
    /// LoginDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class LoginDialog : Window
    {
        public string Username { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;

        public LoginDialog() => InitializeComponent();

        private void LoginDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(usernameTextBox.Text)) usernameTextBox.Focus();
        }

        private void LoginDialog_Closed(object sender, EventArgs e)
        {
            this.Loaded -= LoginDialog_Loaded;
            loginButton.Click -= LoginButton_Click;
            cancelButton.Click -= CancelButton_Click;
        }

        private async void LoginButton_Click(object? sender, RoutedEventArgs e)
        {
            bool isUsernameEmpty = string.IsNullOrWhiteSpace(usernameTextBox.Text);
            bool isPasswordEmpty = string.IsNullOrWhiteSpace(passwordBox.Password);

            if (isUsernameEmpty || isPasswordEmpty)
            {
                ShowErrorMessage(isUsernameEmpty, isPasswordEmpty);
                await Task.Delay(2000);
                ResetErrorMessage();
            }
            else
            {
                Username = usernameTextBox.Text;
                Password = passwordBox.Password;

                DialogResult = true;
            }
        }

        private void ShowErrorMessage(bool isUsernameEmpty, bool isPasswordEmpty)
        {
            var errorMessage = string.Empty;

            if (isUsernameEmpty && isPasswordEmpty)
                errorMessage += "ユーザー名とパスワード";
            else
            {
                if (isUsernameEmpty) errorMessage += "ユーザー名";
                if (isPasswordEmpty) errorMessage += "パスワード";
            }

            errorMessage += "を入力してください";

            descriptionLabel.Text = errorMessage;
            descriptionLabel.Foreground = Brushes.Red;
        }

        private void ResetErrorMessage()
        {
            descriptionLabel.Text = "ログイン情報を入力してください";
            descriptionLabel.Foreground = Brushes.Black;
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
