using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace WinFormsApp4
{
    public partial class Form1 : Form
    {
        private readonly WebView2 _webView1;
        private readonly WebView2 _webView2;
        private TextBox _inputTextBox;
        private CheckBox _checkBoxGPT;
        private CheckBox _checkBoxSense;
        private Button _sendButton;

        public Form1()
        {
            InitializeComponent();
            this.Text = "GPT - Syniti Sense";
            this.BackColor = Color.Black;

            // Set the form size to ensure both panels fit comfortably
            this.Size = new Size(1660, 680);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Set the form to be resizable
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // Use a SplitContainer to handle resizing
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                IsSplitterFixed = false,
                FixedPanel = FixedPanel.None
            };
            Controls.Add(splitContainer);

            // Initialize and add the first rounded panel to the left side of the split container
            var panel1 = new RoundedPanel
            {
                BorderRadius = 30,
                Dock = DockStyle.Fill
            };
            splitContainer.Panel1.Controls.Add(panel1);

            // Initialize the first WebView2
            _webView1 = new WebView2
            {
                Dock = DockStyle.Fill
            };
            panel1.Controls.Add(_webView1);

            // Initialize and add the second rounded panel to the right side of the split container
            var panel2 = new RoundedPanel
            {
                BorderRadius = 30,
                Dock = DockStyle.Fill
            };
            splitContainer.Panel2.Controls.Add(panel2);

            // Initialize the second WebView2
            _webView2 = new WebView2
            {
                Dock = DockStyle.Fill
            };
            panel2.Controls.Add(_webView2);

            // Set SplitterDistance to initially split the panels evenly after load
            this.Load += async (s, e) =>
            {
                splitContainer.SplitterDistance = this.ClientSize.Width / 2;
                await InitializeWebViewsAsync(); // Await the async method here after form load
            };

            // Use ResizeEnd to keep the panels evenly split after resizing ends
            this.ResizeEnd += (s, e) => splitContainer.SplitterDistance = this.ClientSize.Width / 2;

            // Initialize text input box, checkboxes, and button for sending text
            InitializeControls();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public sealed override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        [AllowNull]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void InitializeControls()
        {
            // TextBox for input
            _inputTextBox = new TextBox
            {
                Location = new Point(10, 10),
                Width = 300
            };
            Controls.Add(_inputTextBox);

            // CheckBox for sending to GPT (WebView1)
            _checkBoxGPT = new CheckBox
            {
                Location = new Point(320, 10),
                Text = "Send to GPT",
                ForeColor = Color.White
            };
            Controls.Add(_checkBoxGPT);

            // CheckBox for sending to Sense (WebView2)
            _checkBoxSense = new CheckBox
            {
                Location = new Point(420, 10),
                Text = "Send to Sense",
                ForeColor = Color.White
            };
            Controls.Add(_checkBoxSense);

            // Button for sending the text
            _sendButton = new Button
            {
                Location = new Point(520, 10),
                Text = "Send",
                ForeColor = Color.Black,
                BackColor = Color.LightGray
            };
            _sendButton.Click += SendButton_Click;
            Controls.Add(_sendButton);
        }

        private async void SendButton_Click(object sender, EventArgs e)
        {
            string textToSend = _inputTextBox.Text;

            if (_checkBoxGPT.Checked && !string.IsNullOrWhiteSpace(textToSend))
            {
                await _webView1.ExecuteScriptAsync($"document.body.innerText += '{EscapeForJavaScript(textToSend)}';");
            }

            if (_checkBoxSense.Checked && !string.IsNullOrWhiteSpace(textToSend))
            {
                await _webView2.ExecuteScriptAsync($"document.body.innerText += '{EscapeForJavaScript(textToSend)}';");
            }
        }

        private string EscapeForJavaScript(string input)
        {
            // Escape the input to prevent JavaScript injection
            return input.Replace("'", "\\'").Replace("\"", "\\\"");
        }

        private async Task InitializeWebViewsAsync()
        {
            try
            {
                // Set a user data folder to persist session cookies and other data
                var userDataFolderPath1 =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "WinFormsApp4", "WebView2Data1");
                var userDataFolderPath2 =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "WinFormsApp4", "WebView2Data2");

                // Initialize and load the first URL with persistent data folder for GPT
                var envOptions1 = new CoreWebView2EnvironmentOptions();
                var webViewEnvironment1 =
                    await CoreWebView2Environment.CreateAsync(null, userDataFolderPath1, envOptions1);
                await _webView1.EnsureCoreWebView2Async(webViewEnvironment1);
                _webView1.Source = new Uri("http://chat.openai.com");

                // Initialize and load the second URL with persistent data folder for Google
                var envOptions2 = new CoreWebView2EnvironmentOptions();
                var webViewEnvironment2 =
                    await CoreWebView2Environment.CreateAsync(null, userDataFolderPath2, envOptions2);
                await _webView2.EnsureCoreWebView2Async(webViewEnvironment2);
                _webView2.Source = new Uri("http://sense.syniti.com");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while initializing web views: {ex.Message}");
            }
        }
    }

    // Custom Panel class to create rounded corners
    public class RoundedPanel : Panel
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int BorderRadius { get; set; } = 20;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Create the path for rounded rectangle
            var path = GetRoundedRectanglePath(ClientRectangle, BorderRadius);
            Region = new Region(path);

            // Optionally draw a border (adjust pen width and color as needed)
            using var pen = new Pen(Color.Black, 2);
            graphics.DrawPath(pen, path);
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            var diameter = radius * 2;
            var size = new Size(diameter, diameter);
            var arc = new Rectangle(rect.Location, size);

            // Top left arc
            path.AddArc(arc, 180, 90);

            // Top right arc
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}
