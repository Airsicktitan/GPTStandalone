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
        private CheckBox _checkBoxGpt;
        private CheckBox _checkBoxSense;
        private Button _sendButton;

        public Form1()
        {
            InitializeComponent();
            this.Text = @"GPT - Syniti Sense";
            this.BackColor = Color.Black;

            // Set the form size to ensure both panels fit comfortably
            this.Size = new Size(1660, 680);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Set the form to be resizable
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // Create a TableLayoutPanel to hold the controls and the SplitContainer
            var tableLayoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                AutoSize = true,
            };
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Fixed height for control panel
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Remaining space for SplitContainer
            Controls.Add(tableLayoutPanel);

            // Create a panel for input controls
            var controlPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 50
            };
            tableLayoutPanel.Controls.Add(controlPanel, 0, 0);

            // Initialize text input box, checkboxes, and button for sending text
            InitializeControls(controlPanel);

            // Use a SplitContainer to handle resizing for WebView2 panels
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                IsSplitterFixed = false,
                FixedPanel = FixedPanel.None
            };
            tableLayoutPanel.Controls.Add(splitContainer, 0, 1);

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
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public sealed override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        [AllowNull]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }


        private void InitializeControls(Panel controlPanel)
        {
            // TextBox for input
            _inputTextBox = new TextBox
            {
                PlaceholderText = @"Ask GPT, Syniti Sense, or both!",
                Location = new Point(10, 10),
                Width = 300
            };
            controlPanel.Controls.Add(_inputTextBox);

            // CheckBox for sending to GPT (WebView1)
            _checkBoxGpt = new CheckBox
            {
                Location = new Point(320, 10),
                Text = @"Send to GPT",
                ForeColor = Color.White
            };
            controlPanel.Controls.Add(_checkBoxGpt);

            // CheckBox for sending to Sense (WebView2)
            _checkBoxSense = new CheckBox
            {
                Location = new Point(440, 10),
                Text = @"Send to Sense",
                ForeColor = Color.White
            };
            controlPanel.Controls.Add(_checkBoxSense);

            // Button for sending the text
            _sendButton = new Button
            {
                Location = new Point(570, 10),
                Text = @"Send",
                ForeColor = Color.Black,
                BackColor = Color.LightGray
            };
            _sendButton.Click += SendButton_Click!;
            controlPanel.Controls.Add(_sendButton);
        }

        private async void SendButton_Click(object sender, EventArgs e)
        {
            try
            {
                var textToSend = EscapeForJavaScript(_inputTextBox.Text);

                if (string.IsNullOrWhiteSpace(textToSend)) return;

                // For ChatGPT (targeting the input area and clicking the submit button)
                if (_checkBoxGpt.Checked)
                {
                    await _webView1.ExecuteScriptAsync(
                        $$"""
                        (function() {
                            // Clear any previous interval to avoid lingering checks
                            clearInterval(window.gptCheckExist);

                            // Start a new interval to check for the textarea element
                            window.gptCheckExist = setInterval(function() {
                                let promptTextarea = document.getElementById('prompt-textarea');
                                if (promptTextarea) {
                                    clearInterval(window.gptCheckExist);

                                    // Set the value for the textarea using innerHTML (as per requirement)
                                    promptTextarea.innerHTML = '{{textToSend}}';

                                    // Dispatch input events to ensure the UI reacts correctly
                                    promptTextarea.dispatchEvent(new Event('input', { bubbles: true }));
                                    promptTextarea.dispatchEvent(new Event('change', { bubbles: true }));

                                    // Delay before finding and clicking the submit button
                                    setTimeout(function() {
                                        // Find the submit button using the data-testid attribute
                                        let submitButton = document.querySelector('button[data-testid="send-button"]');
                                        if (submitButton) {
                                            // Try clicking the button even if it appears disabled
                                            if (submitButton.disabled) return

                                            submitButton.click();
                                        } else {
                                            console.log('GPT Submit button not found');
                                        }
                                    }, 500); // 500ms delay to allow the button state to update
                                }
                            }, 100); // Checks every 100ms until the element is found
                        })();
                        """);
                }

                // For Syniti Sense (targeting the textarea with id "chat-input" and clicking the submit button)
                if (_checkBoxSense.Checked) {
                    await _webView2.ExecuteScriptAsync(
                    $$"""
                    (function() {
                        // Clear any previous interval to avoid lingering checks
                        clearInterval(window.senseCheckExist);

                        // Start a new interval to check for the chat input element
                        window.senseCheckExist = setInterval(function() {
                            let chatInputDiv = document.querySelector('div.MuiInputBase-root.MuiInput-root.MuiInputBase-colorPrimary.MuiInputBase-fullWidth.MuiInputBase-formControl.MuiInputBase-multiline.MuiInputBase-adornedStart.MuiInputBase-adornedEnd.css-11iqwpy'); // Get the main container div
                            let chatInput = document.getElementById('chat-input');
                            let formControl = chatInput.closest('.MuiFormControl-root'); // Get the associated form control

                            if (chatInput && chatInputDiv && formControl) {
                                clearInterval(window.senseCheckExist);

                                // Add the necessary class to the div container
                                chatInputDiv.classList.add('Mui-focused');

                                // Set focus on the form control to ensure the input is detected properly
                                formControl.focus();

                                // Set the value for the textarea AFTER the focus actions
                                chatInput.textContent = "{{textToSend}}";

                                console.log('Input is:', chatInput.value);

                                chatInput.setAttribute("value", "{{textToSend}}");

                                // Trigger events to simulate real user input interactions
                                chatInput.dispatchEvent(new Event('input', { bubbles: true }));
                                chatInput.dispatchEvent(new Event('change', { bubbles: true }));

                                // Enable the submit button once a value is present by modifying disabled property and triggering input event
                                let submitButton = document.querySelector('svg[data-testid="TelegramIcon"]');

                                // Check if the SVG element exists, then click its parent button
                                if (submitButton) {
                                    let button = submitButton.closest('button');
                                    if (button) {
                                        button.click();
                                        chatInput.textContent = "";
                                    } else {
                                        console.log("Parent button not found");
                                    }
                                } else {
                                    console.log("SVG element not found");
                                }
                            }
                        }, 1000); // Checks every 100ms until the element is found
                    })();
                    """
                    );
                    Console.WriteLine($@"Sent and submitted to Syniti Sense: {textToSend}");
                }

                // Clear the input box after sending
                _inputTextBox.Clear();
            }
            catch (Exception exception)
            {
                MessageBox.Show($@"An error occurred while sending text: {exception.Message}");
            }
        }

        private static string EscapeForJavaScript(string input)
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
                MessageBox.Show($@"An error occurred while initializing web views: {ex.Message}");
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
