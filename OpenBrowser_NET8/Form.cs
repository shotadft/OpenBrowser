using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Xml.Linq;

namespace OpenBrowser
{
    public partial class MainForm : Form
    {
        public static WebView2 webView = new WebView2();
        private TextBox addressBar = new TextBox();
        private TextBox titileBar = new TextBox();
        private ToolStrip toolStrip = new ToolStrip();
        private ToolStripButton[] stripButton = new ToolStripButton[7];
        private System.Windows.Forms.Timer mainTickEvent = new System.Windows.Forms.Timer();

        public static Uri hmURL = new Uri("https://www.google.com/");
        public const string SettingFilename = @"OpenBrowser.settings.xml";

        public MainForm()
        {
            this.Resize += new EventHandler(this.MainForm_Resize);
            this.Load += new EventHandler(MainForm_Load);
            this.KeyPress += new KeyPressEventHandler(MainForm_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(webView)).BeginInit();

            webView.AllowExternalDrop = true;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = Color.White;
            webView.Name = "webView";
            webView.Source = new Uri("https://www.google.com/", UriKind.Absolute);
            webView.Size = this.ClientSize;
            webView.Dock = DockStyle.Fill;
            webView.TabIndex = 0;
            webView.ZoomFactor = 1D;

            addressBar.Name = "addressTextbox";
            addressBar.Text = null;
            addressBar.Location = new Point(0, 25);
            addressBar.Dock = DockStyle.Top;

            titileBar.Name = "titleBar";
            titileBar.Text = null;
            titileBar.Location = new Point(0, 25);
            titileBar.Dock = DockStyle.Top;
            titileBar.ReadOnly = true;

            this.AutoScaleDimensions = new SizeF(6F, 12F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Name = "MainForm";
            this.Text = Program.app_name;
            ((System.ComponentModel.ISupportInitialize)(webView)).EndInit();
            this.Icon = Program.icon;
            this.Size = new Size(1024, 576);
            this.ClientSize = this.Size;
            this.BackColor = Color.White;

            this.Controls.Add(webView);
            this.Controls.Add(toolStrip);
            this.Controls.Add(addressBar);
            this.Controls.Add(titileBar);

            addressBar.KeyDown += new KeyEventHandler(AddressBar_KeyDown);
            addressBar.KeyPress += new KeyPressEventHandler(MainForm_KeyPress);
            this.KeyDown += new KeyEventHandler(MainForm_KeyDown);

            toolStrip.Name = "toolStrip";
            toolStrip.Dock = DockStyle.Top;

            for (int i = 0; i < stripButton.Length; i++)
            {
                stripButton[i] = new ToolStripButton();
            }

            stripButton[0].Name = "Go";
            stripButton[1].Name = "Back";
            stripButton[2].Name = "Forward";
            stripButton[3].Name = "Reload";
            stripButton[4].Name = "Home";
            stripButton[5].Name = "Exit";
            stripButton[6].Name = "Option";

            stripButton[0].Text = "Go";
            stripButton[1].Text = "<<";
            stripButton[2].Text = ">>";
            stripButton[3].Text = "Reload";
            stripButton[4].Text = "Home";
            stripButton[5].Text = "Exit";
            stripButton[6].Text = "：";

            stripButton[0].ToolTipText = "指定URLに移動";
            stripButton[1].ToolTipText = "前に戻る";
            stripButton[2].ToolTipText = "次に進む";
            stripButton[3].ToolTipText = "更新";
            stripButton[4].ToolTipText = "ホームページに移動";
            stripButton[5].ToolTipText = "終了";
            stripButton[6].ToolTipText = "設定";

            for (int i = 0; i < stripButton.Length; i++)
            {
                toolStrip.Items.Add(stripButton[i]);
            }

            addressBar.Parent = this;
            titileBar.Parent = this;
            toolStrip.Parent = this;
            webView.Parent = this;

            for (int i = 0; i < stripButton.Length; i++)
            {
                stripButton[i].Click += new EventHandler(button_Click);
            }

            mainTickEvent.Interval = 100;
            mainTickEvent.Start();

            mainTickEvent.Tick += new EventHandler(MainTickEvent_Tick);
        }

        private void MainForm_KeyPress(Object? sender, KeyPressEventArgs? e)
        {
            if (e != null && (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Escape))
            {
                e.Handled = true;
            }
        }

        public void MainForm_KeyDown(Object? sender, KeyEventArgs? e)
        {
            if (e != null && e.KeyCode == Keys.F5)
            {
                webView.Reload();
            }
        }

        public void MainForm_Load(Object? sender, EventArgs? e)
        {
            try
            {
                if (e != null)
                {
                    CheckWebView2Runtime();
                    InitializeAsync();

                    if (File.Exists(SettingFilename))
                    {
                        XDocument xdoc = XDocument.Load(SettingFilename);
                        XElement? text = xdoc.Element("text");

                        if (text != null)
                        {
                            System.Collections.IEnumerable home_xml = text.Elements("homepage");
                            foreach (XElement xmle in home_xml)
                            {
                                 Uri hmURL_xml = new Uri(xmle.Element("url").Value);
                                 hmURL = hmURL_xml;
                            }
                        }
                    }
                }
            } 
            catch (Exception ex)
            {
                MessageBox.Show($"Form Error: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async void CheckWebView2Runtime()
        {
            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}"))
            {
                if (key == null || key.GetValue("pv") == null)
                {
                    string localAppFolder = Path.GetTempPath() + @"%localAppData%";
                    try
                    {
                        await webView.EnsureCoreWebView2Async();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("WebView2ランタイムが見つかりません。\n以下リンクからインストールしてください。\nエラー: " + ex.Message.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                    this.Close();
                }
            }
        }

        private void MainForm_Resize(Object? sender, EventArgs? e)
        {
            webView.Size = (this.ClientSize - new Size(webView.Location));
        }

        public async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.NewWindowRequested += NewWindowRequested;
            webView.NavigationStarting += new EventHandler<CoreWebView2NavigationStartingEventArgs>(MainForm_NavigationStarting);
            webView.NavigationCompleted += new EventHandler<CoreWebView2NavigationCompletedEventArgs>(MainForm_NavigationCompleted);
        }

        public void MainForm_NavigationStarting(Object? sender, CoreWebView2NavigationStartingEventArgs? args)
        {
            if (addressBar.Modified == true)
            {
                addressBar.Modified = false;
            }

            if (stripButton[3].Name == "Reload")
            {
                stripButton[3].Text = "Cancel";
                stripButton[3].ToolTipText = "読み込みの中止";
                stripButton[3].Name = "ReloadCancel";
            }
        }

        public void MainForm_NavigationCompleted(Object? sender, CoreWebView2NavigationCompletedEventArgs? args)
        {
            if (stripButton[3].Name == "ReloadCancel")
            {
                stripButton[3].Text = "Reload";
                stripButton[3].ToolTipText = "更新";
                stripButton[3].Name = "Reload";
            }
        }


        public void NewWindowRequested(Object? sender, CoreWebView2NewWindowRequestedEventArgs? e)
        {
            if (e != null)
            {
                e.Handled = true;
                webView.CoreWebView2.Navigate(e.Uri.ToString());
            }
        }

        public void AddressBar_KeyDown(Object? sender, KeyEventArgs? e)
        {
            if (e != null && e.KeyCode == Keys.Enter)
            {
                searchFunc();
            }
        }

        public void MainTickEvent_Tick(Object? sender, EventArgs? e)
        {
            if (webView.CoreWebView2 != null)
            {
                if (webView.CoreWebView2.Source != null && addressBar.Modified == false)
                {
                    addressBar.Text = webView.CoreWebView2.Source.ToString();
                }
                if (webView.CoreWebView2.DocumentTitle != null)
                {
                    titileBar.Text = webView.CoreWebView2.DocumentTitle.ToString();
                }

                if (webView.CanGoBack)
                {
                    stripButton[1].Enabled = true;
                }
                else if (!webView.CanGoBack)
                {
                    stripButton[1].Enabled = false;
                }
                else if (webView.CanGoForward)
                {
                    stripButton[2].Enabled = true;
                }
                else if (!webView.CanGoForward)
                {
                    stripButton[2].Enabled = false;
                }
            }
        }

        public void button_Click(Object? sender, EventArgs? e)
        {
            if (sender == stripButton[0])
            {
                searchFunc();
            }
            else if (sender == stripButton[1])
            {
                webView.GoBack();
            }
            else if (sender == stripButton[2])
            {
                webView.GoForward();
            }
            else if (sender == stripButton[3])
            {
                if (stripButton[3].Name == "Reload")
                {
                    webView.Reload();
                }
                else if (stripButton[3].Name == "ReloadCancel")
                {
                    webView.Stop();
                }
            }
            else if (sender == stripButton[4])
            {
                webView.CoreWebView2.Navigate(hmURL.ToString());
            }
            else if (sender == stripButton[5])
            {
                Application.Exit();
            }
            else if (sender == stripButton[6])
            {
                SettingsForm settingsForm = new SettingsForm();
                settingsForm.ShowDialog();
            }
        }

        private void searchFunc()
        {
            try
            {
                webView.CoreWebView2.Navigate(addressBar.Text);
                if (addressBar.Modified == true)
                {
                    addressBar.Modified = false;
                }
            }
            catch
            {
                MessageBox.Show("URLを入力してください。");
            }
        }
    }
}