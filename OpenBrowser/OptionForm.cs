using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace OpenBrowser
{
	partial class SettingsForm : Form
	{
		public PropertyGrid propertyGrid = new PropertyGrid();
		public Button saveButton = new Button();
		public Button cancelButton = new Button();

		private ApplicationSettings app_settings = new ApplicationSettings();

		public SettingsForm()
		{
			this.AutoScaleDimensions = new SizeF(6F, 12F);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Name = "OptionForm";
			this.Text = "設定";
			this.StartPosition = FormStartPosition.CenterParent;
			this.Icon = Program.icon;
			this.Size = new Size(624, 321);
			this.ClientSize = this.Size;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.ResumeLayout(false);

			saveButton.Name = "OK_Button";
			saveButton.Text = "保存";
			saveButton.TabStop = true;
			saveButton.Location = new Point(this.ClientSize.Width - saveButton.Width - 12, this.ClientSize.Height - saveButton.Height - 8);
			saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			saveButton.Click += new EventHandler(Save_Click);
			saveButton.DialogResult = DialogResult.OK;

			cancelButton.Name = "Cancel_Button";
			cancelButton.Text = "キャンセル";
			cancelButton.Location = new Point(saveButton.Width - cancelButton.Width + 458, this.ClientSize.Height - cancelButton.Height - 8);
			cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			cancelButton.Click += new EventHandler(Cancel_Click);
			cancelButton.DialogResult = DialogResult.Cancel;

			propertyGrid.Name = "Settings";
			propertyGrid.SelectedObject = app_settings;
			propertyGrid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			propertyGrid.Size = new Size(600, this.ClientSize.Height - saveButton.Height - 28);
			propertyGrid.Location = new Point(12, 12);

			propertyGrid.Parent = this;
			saveButton.Parent = this;
			cancelButton.Parent = this;
		}

		public void Save_Click(Object sender, EventArgs e)
		{
			Uri home_uri = new Uri(ApplicationSettings.settings_home_uri.ToString().Trim());
			MainForm.home_url = home_uri;

			XmlWriter xmlwr = XmlWriter.Create(@"OpenBrowser.settings.ini");

			xmlwr.WriteStartElement("text");

			xmlwr.WriteStartElement("homepage");
			xmlwr.WriteElementString("name", "uri");
			xmlwr.WriteElementString("url", home_uri.ToString());
			xmlwr.WriteEndElement();

			xmlwr.WriteEndElement();

			xmlwr.Close();

			this.Close();
		}

		public void Cancel_Click(Object sender, EventArgs e)
		{
			this.Close();
		}

	}

	public class ApplicationSettings
	{
		public static Uri settings_home_uri = MainForm.home_url;
		public static bool homebutton_bool = true;

		[Category("ホームページ")]
		[Description("ホームページのURLを設定します。")]
		public Uri URL
		{
			get { return settings_home_uri; }
			set { settings_home_uri = value; }
		}
	}
}