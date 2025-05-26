using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WinFormsApp.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace WinFormsApp.Forms
{
    public partial class ChatbotControl : UserControl
    {
        private TextBox txtMessage = null!;
        private RichTextBox rtbChat = null!;
        private Button btnSend = null!;
        private User currentUser;        public ChatbotControl(User user)
        {
            currentUser = user;
            InitializeComponent();
        }private void InitializeComponent()
        {            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(246, 248, 250); // Light background for modern look

            // Modern light gradient background
            this.Paint += (s, e) => {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    this.ClientRectangle, 
                    Color.FromArgb(255, 255, 255),   // Pure white
                    Color.FromArgb(246, 248, 250),   // Very light gray
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
                
                // Add subtle pattern overlay for depth
                using (var patternBrush = new SolidBrush(Color.FromArgb(5, 0, 0, 0)))
                {
                    for (int i = 0; i < this.Width; i += 40)
                    {
                        for (int j = 0; j < this.Height; j += 40)
                        {
                            e.Graphics.FillEllipse(patternBrush, i, j, 2, 2);
                        }
                    }
                }
            };            // Modern lightweight header with clean design
            Panel headerPanel = new Panel();
            headerPanel.Location = new Point(20, 20);
            headerPanel.Size = new Size(this.Width - 40, 70); // Slightly reduced height for cleaner look
            headerPanel.BackColor = Color.White;
            headerPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            
            // Add modern shadow and rounded corners to header
            headerPanel.Paint += (s, e) => {
                var rect = headerPanel.ClientRectangle;
                
                // Enable high quality rendering
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Create rounded rectangle path
                int radius = 12;
                GraphicsPath path = new GraphicsPath();
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(rect.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(rect.Width - radius, rect.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, rect.Height - radius, radius, radius, 90, 90);
                path.CloseAllFigures();
                
                // Fill with clean white background
                using (var fillBrush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(fillBrush, path);
                }
                
                // Add subtle border
                using (var borderPen = new Pen(Color.FromArgb(30, 144, 218, 244), 1))
                {
                    e.Graphics.DrawPath(borderPen, path);
                }
                
                // Add very subtle shadow effect
                Rectangle shadowRect = new Rectangle(2, rect.Height - 2, rect.Width - 4, 4);
                using (var shadowBrush = new LinearGradientBrush(
                    shadowRect, 
                    Color.FromArgb(20, 0, 0, 0), 
                    Color.Transparent,
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(shadowBrush, shadowRect);
                }
            };            // Modern clean title label
            Label lblTitle = new Label();
            lblTitle.Text = "IHEC AI Assistant";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(13, 110, 253); // Modern blue color
            lblTitle.Location = new Point(60, 10);
            lblTitle.Size = new Size(300, 30);
            lblTitle.BackColor = Color.Transparent;
            
            // Add bot icon image
            PictureBox pictureBoxBot = new PictureBox();
            pictureBoxBot.Size = new Size(36, 36);
            pictureBoxBot.Location = new Point(20, 12);
            pictureBoxBot.BackColor = Color.Transparent;
            // Using emoji as a placeholder for the icon
            // In a real app, you'd use an actual image file with pictureBoxBot.Image = Image.FromFile("path/to/icon.png");
            Label lblBotIcon = new Label();
            lblBotIcon.Text = "🤖";
            lblBotIcon.Font = new Font("Segoe UI Emoji", 20, FontStyle.Regular);
            lblBotIcon.Size = new Size(36, 36);
            lblBotIcon.Location = new Point(0, 0);
            lblBotIcon.TextAlign = ContentAlignment.MiddleCenter;
            pictureBoxBot.Controls.Add(lblBotIcon);
            
            // Cleaner subtitle
            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Your intelligent library companion - Ask me anything!";
            lblSubtitle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblSubtitle.ForeColor = Color.FromArgb(108, 117, 125); // Modern gray
            lblSubtitle.Location = new Point(60, 40);
            lblSubtitle.Size = new Size(500, 20);
            lblSubtitle.BackColor = Color.Transparent;

            // Add modern status indicator
            Label lblStatus = new Label();
            lblStatus.Text = "🟢 Online";
            lblStatus.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblStatus.ForeColor = Color.FromArgb(25, 135, 84); // Modern green
            lblStatus.Location = new Point(headerPanel.Width - 100, 15);
            lblStatus.Size = new Size(80, 20);
            lblStatus.BackColor = Color.Transparent;
            lblStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;            // Add AI model info with clean styling
            Label lblModel = new Label();
            lblModel.Text = "Powered by Gemini AI";
            lblModel.Font = new Font("Segoe UI", 8, FontStyle.Italic);
            lblModel.ForeColor = Color.FromArgb(108, 117, 125); // Modern gray
            lblModel.Location = new Point(headerPanel.Width - 150, 40);
            lblModel.Size = new Size(130, 15);
            lblModel.BackColor = Color.Transparent;
            lblModel.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            headerPanel.Controls.AddRange(new Control[] { pictureBoxBot, lblTitle, lblSubtitle, lblStatus, lblModel });            // Modern light-themed chat display area
            rtbChat = new RichTextBox();
            rtbChat.Location = new Point(20, 105); // Adjusted for updated header height
            rtbChat.Size = new Size(this.Width - 40, 20); // Extremely minimal chat display height
            rtbChat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            rtbChat.ReadOnly = true;
            rtbChat.BackColor = Color.White; // Clean white background
            rtbChat.ForeColor = Color.FromArgb(33, 37, 41); // Dark text for readability
            rtbChat.BorderStyle = BorderStyle.None;
            rtbChat.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            rtbChat.Padding = new Padding(20);
            rtbChat.Visible = true;
            rtbChat.TabStop = false;
            rtbChat.ScrollBars = RichTextBoxScrollBars.Vertical; // Enable vertical scrolling
            rtbChat.AutoWordSelection = false; // Better text selection
            rtbChat.WordWrap = true; // Enable word wrapping for better readability
            rtbChat.AutoSize = false; // Prevent automatic sizing to enable proper scrolling
            
            // Enable text selection and links
            rtbChat.DetectUrls = true;
            rtbChat.HideSelection = false;
            
            // Add modern rounded corners and subtle shadow for chat display
            rtbChat.Paint += (s, e) => {
                var rect = rtbChat.ClientRectangle;
                
                // Enable high quality rendering
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Create rounded rectangle path for the chat area
                int radius = 12;
                GraphicsPath path = new GraphicsPath();
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(rect.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(rect.Width - radius, rect.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, rect.Height - radius, radius, radius, 90, 90);
                path.CloseAllFigures();
                
                // Fill with clean white background
                using (var fillBrush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(fillBrush, path);
                }
                
                // Add subtle border
                using (var borderPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                {
                    e.Graphics.DrawPath(borderPen, path);
                }
                
                // Very subtle shadow effect
                Rectangle shadowRect = new Rectangle(2, rect.Height - 3, rect.Width - 4, 5);
                using (var shadowBrush = new LinearGradientBrush(
                    shadowRect, 
                    Color.FromArgb(15, 0, 0, 0), 
                    Color.Transparent,
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(shadowBrush, shadowRect);
                }
            };            // Modern light-themed input container with fixed position at bottom and always visible
            Panel inputPanel = new Panel();
            inputPanel.Location = new Point(20, 135); // Move up to match extremely reduced chat area
            inputPanel.Size = new Size(this.Width - 40, 60); // Keep input area comfortable
            inputPanel.BackColor = Color.White;
            inputPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            inputPanel.Visible = true;
            inputPanel.BringToFront();
            
            // Add modern rounded styling with subtle shadow for input panel
            inputPanel.Paint += (s, e) => {
                var rect = inputPanel.ClientRectangle;
                
                // Enable high quality rendering
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Create rounded rectangle path
                int radius = 12;
                GraphicsPath path = new GraphicsPath();
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(rect.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(rect.Width - radius, rect.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, rect.Height - radius, radius, radius, 90, 90);
                path.CloseAllFigures();
                
                // Fill with clean white background
                using (var fillBrush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(fillBrush, path);
                }
                
                // Add subtle border
                using (var borderPen = new Pen(Color.FromArgb(40, 0, 0, 0), 1))
                {
                    e.Graphics.DrawPath(borderPen, path);
                }
                
                // Very subtle shadow effect
                Rectangle shadowRect = new Rectangle(2, 0, rect.Width - 4, 4);
                using (var shadowBrush = new LinearGradientBrush(
                    shadowRect, 
                    Color.FromArgb(20, 0, 0, 0), 
                    Color.Transparent,
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(shadowBrush, shadowRect);
                }
                
                // Add placeholder text when empty
                if (!txtMessage.Focused && string.IsNullOrEmpty(txtMessage.Text))
                {
                    using (Font font = new Font("Segoe UI", 10, FontStyle.Italic))
                    using (Brush textBrush = new SolidBrush(Color.FromArgb(120, 108, 117, 125)))
                    {
                        e.Graphics.DrawString("Type your message here...", font, textBrush, 20, 19);
                    }
                }            };

            // Modern light-themed text input with clean design
            txtMessage = new TextBox();
            txtMessage.Location = new Point(15, 10);
            txtMessage.Size = new Size(inputPanel.Width - 80, 40); // Large and comfortable for typing
            txtMessage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMessage.Font = new Font("Segoe UI", 11); // Slightly smaller font for cleaner look
            txtMessage.BorderStyle = BorderStyle.None;
            txtMessage.BackColor = Color.White; // Clean white background
            txtMessage.ForeColor = Color.FromArgb(33, 37, 41); // Dark text for readability
            txtMessage.PlaceholderText = "Type your message..."; // Simpler placeholder
            txtMessage.Multiline = true;
            txtMessage.ScrollBars = ScrollBars.Vertical;
            txtMessage.Enabled = true;
            txtMessage.ReadOnly = false;
            txtMessage.TabStop = true;
            txtMessage.TabIndex = 1;
            txtMessage.Visible = true;
            txtMessage.AcceptsReturn = true; // Allow Enter key in multiline
            txtMessage.AcceptsTab = false; // Don't capture Tab, let it navigate
            txtMessage.BringToFront();
            txtMessage.KeyPress += TxtMessage_KeyPress;
            txtMessage.KeyDown += TxtMessage_KeyDown;
            
            // Clean, minimal styling for text input
            txtMessage.Paint += (s, e) => {
                // Enable high quality rendering
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                if (txtMessage.Focused)
                {
                    using (var focusPen = new Pen(Color.FromArgb(13, 110, 253), 1)) // Modern blue focus border
                    {
                        // Rounded corners for the focus indicator
                        int radius = 6;
                        GraphicsPath path = new GraphicsPath();
                        path.AddArc(0, 0, radius, radius, 180, 90);
                        path.AddArc(txtMessage.Width - radius, 0, radius, radius, 270, 90);
                        path.AddArc(txtMessage.Width - radius, txtMessage.Height - radius, radius, radius, 0, 90);
                        path.AddArc(0, txtMessage.Height - radius, radius, radius, 90, 90);
                        path.CloseAllFigures();
                        
                        e.Graphics.DrawPath(focusPen, path);
                    }
                }
            };
              // Set initial focus and cursor position
            txtMessage.GotFocus += (s, e) => { 
                txtMessage.SelectionStart = txtMessage.Text.Length; 
                txtMessage.Invalidate(); // Trigger repaint for focus border
                System.Diagnostics.Debug.WriteLine("TextBox got focus");
            };
            txtMessage.LostFocus += (s, e) => {
                txtMessage.Invalidate(); // Trigger repaint to remove focus border
                System.Diagnostics.Debug.WriteLine("TextBox lost focus");
            };
            
            // Add debug for mouse click
            txtMessage.Click += (s, e) => {
                System.Diagnostics.Debug.WriteLine("TextBox clicked");
                txtMessage.Focus();
            };            // Modern clean send button with hover effects
            btnSend = new Button();
            btnSend.Location = new Point(inputPanel.Width - 60, 10);
            btnSend.Size = new Size(42, 40); // Match input height
            btnSend.Text = "➤"; // Simple arrow for clean look
            btnSend.BackColor = Color.FromArgb(13, 110, 253); // Modern blue
            btnSend.ForeColor = Color.White;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Font = new Font("Segoe UI Symbol", 14, FontStyle.Bold);
            btnSend.Cursor = Cursors.Hand;
            btnSend.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSend.Click += BtnSend_Click;
            btnSend.Visible = true;
            btnSend.BringToFront();
            btnSend.TabIndex = 2;
            btnSend.TabStop = true;
            
            // Add modern styling with rounded corners
            btnSend.Paint += (s, e) => {
                var rect = btnSend.ClientRectangle;
                
                // Enable high quality rendering
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Create circular/rounded path for button
                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(0, 0, rect.Width, rect.Height);
                
                // Fill with clean blue background
                using (var brush = new SolidBrush(btnSend.BackColor))
                {
                    e.Graphics.FillPath(brush, path);
                }
                
                // Add subtle gradient for depth
                using (var gradientBrush = new LinearGradientBrush(
                    rect, 
                    Color.FromArgb(40, 255, 255, 255),
                    Color.Transparent, 
                    LinearGradientMode.ForwardDiagonal))
                {
                    e.Graphics.FillPath(gradientBrush, path);
                }
                
                // Draw the icon manually for better control
                using (var textBrush = new SolidBrush(btnSend.ForeColor))
                {
                    var stringFormat = new StringFormat()
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(btnSend.Text, btnSend.Font, textBrush, rect, stringFormat);
                }
            };
            
            // Enhanced hover effects with smooth color transitions
            btnSend.MouseEnter += (s, e) => {
                btnSend.BackColor = Color.FromArgb(10, 88, 202); // Slightly darker blue
                btnSend.Invalidate(); // Trigger repaint
            };            btnSend.MouseLeave += (s, e) => {
                btnSend.BackColor = Color.FromArgb(13, 110, 253); // Original blue
                btnSend.Invalidate(); // Trigger repaint
            };// Add controls in the right order to ensure proper z-ordering
            inputPanel.Controls.Add(txtMessage);
            inputPanel.Controls.Add(btnSend);            // Add a toggle button for hiding/showing the chat display
            Button btnToggleChat = new Button();
            btnToggleChat.Location = new Point(this.Width - 80, 95); // Position above the chat display
            btnToggleChat.Size = new Size(24, 24);
            btnToggleChat.Text = "▲"; // Up arrow to indicate "hide"
            btnToggleChat.BackColor = Color.Transparent;
            btnToggleChat.FlatStyle = FlatStyle.Flat;
            btnToggleChat.FlatAppearance.BorderSize = 0;
            btnToggleChat.Font = new Font("Segoe UI Symbol", 8, FontStyle.Regular);
            btnToggleChat.Cursor = Cursors.Hand;
            btnToggleChat.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnToggleChat.Click += (s, e) => {
                if (rtbChat.Visible)
                {
                    rtbChat.Visible = false;
                    btnToggleChat.Text = "▼"; // Down arrow to indicate "show"
                    inputPanel.Location = new Point(20, 105); // Move input panel up when chat is hidden
                }
                else
                {
                    rtbChat.Visible = true;
                    btnToggleChat.Text = "▲"; // Up arrow to indicate "hide"
                    inputPanel.Location = new Point(20, 135); // Move input panel down when chat is shown
                }
            };

            // Add controls to the form in reverse z-order (bottom to top)
            this.Controls.Add(headerPanel);
            this.Controls.Add(btnToggleChat);
            this.Controls.Add(rtbChat);
            this.Controls.Add(inputPanel);// Set initial focus to the text input when the control is created
            this.Load += (s, e) => {
                VerifyTextInputSetup(); // Debug method
                if (txtMessage != null && !txtMessage.IsDisposed)
                {
                    txtMessage.Focus();
                    txtMessage.Select();
                }
            };
              // Ensure the control is visible and properly loaded
            this.HandleCreated += (s, e) => {
                Application.DoEvents();
                if (txtMessage != null && !txtMessage.IsDisposed)
                {
                    txtMessage.Focus();
                }
            };// Additional focus management for when the chatbot tab is activated
            this.VisibleChanged += (s, e) => {
                if (this.Visible && txtMessage != null && !txtMessage.IsDisposed)
                {
                    this.BeginInvoke(new Action(() => {
                        if (txtMessage != null && !txtMessage.IsDisposed)
                        {
                            txtMessage.Focus();
                            txtMessage.SelectionStart = txtMessage.Text.Length;
                            // Ensure scrolling to the latest messages when tab becomes visible
                            EnsureScrollToLatestMessage();
                        }
                    }));
                }
            };
            
            // Handle resize events to ensure proper scrolling after layout changes
            this.Resize += (s, e) => {
                EnsureScrollToLatestMessage();
            };            // Welcome message with clean light theme formatting
            AddMessage("AI Assistant", $"Welcome {currentUser.FirstName}! I'm your intelligent library companion.\n\nI can help you with:\n\n• Book recommendations & discovery\n• Real-time availability checking\n• Reservation assistance & guidance\n• Personalized study suggestions\n• Library services & policies\n• Academic research support\n\nJust type your question and let's explore the world of knowledge together!");
        }private void TxtMessage_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // Don't handle Enter in KeyPress anymore, let KeyDown handle it
        }        private void TxtMessage_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Send message on Enter (without Shift)
                if (!e.Shift)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    SendMessage();
                }
                // Allow new line with Shift+Enter (do nothing special)
            }
            else if (e.KeyCode == Keys.Tab)
            {
                // Handle tab key to navigate between controls
                e.Handled = true;
                e.SuppressKeyPress = true;
                
                if (e.Shift)
                {
                    // Shift+Tab to move backward
                    SelectNextControl(ActiveControl, false, true, true, true);
                }
                else
                {
                    // Tab to move forward
                    SelectNextControl(ActiveControl, true, true, true, true);
                }
            }
        }

        private void BtnSend_Click(object? sender, EventArgs e)
        {
            SendMessage();
        }        private async void SendMessage()
        {
            if (txtMessage == null || txtMessage.IsDisposed) return;
            
            string message = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            AddMessage("You", message);
            txtMessage.Clear();
            if (!txtMessage.IsDisposed)
            {
                txtMessage.Focus(); // Keep focus on text input after sending
            }// Show a modern light-themed typing indicator with animations
            rtbChat.SelectionAlignment = HorizontalAlignment.Left;
            rtbChat.SelectionColor = Color.FromArgb(108, 117, 125); // Modern gray
            rtbChat.SelectionFont = new Font("Segoe UI", 11, FontStyle.Italic);
            rtbChat.SelectionBackColor = Color.FromArgb(233, 236, 239); // Light gray background
            rtbChat.AppendText("  IHEC AI Assistant is typing");              // Animate typing dots with enhanced scrolling - light theme style
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(300);
                rtbChat.AppendText(".");
                EnsureScrollToLatestMessage(); // Use our helper method for consistent behavior
            }
            rtbChat.SelectionBackColor = Color.Transparent;
            rtbChat.AppendText("  \n\n");
            EnsureScrollToLatestMessage(); // Ensure we're still scrolled to the bottom
            
            // Try Gemini API first, fallback to local responses
            string response;
            try {
                response = await GetGeminiResponseAsync(message);
                if (response.Contains("Please configure a valid Google Gemini API key"))
                {
                    response = ProcessMessageLocally(message);
                }
                  // Remove the "typing" indicator (last few lines)
                string currentText = rtbChat.Text;
                int typingIndex = currentText.LastIndexOf("IHEC AI Assistant is typing");
                if (typingIndex >= 0)
                {
                    rtbChat.Select(typingIndex, currentText.Length - typingIndex);
                    rtbChat.SelectedText = "";
                }
                
                AddMessage("AI Assistant", response);
            }
            catch (Exception ex)
            {                // Remove the "typing" indicator
                string currentText = rtbChat.Text;
                int typingIndex = currentText.LastIndexOf("IHEC AI Assistant is typing");
                if (typingIndex >= 0)
                {
                    rtbChat.Select(typingIndex, currentText.Length - typingIndex);
                    rtbChat.SelectedText = "";
                }
                
                AddMessage("AI Assistant", "Sorry, I encountered an error while processing your request. Please try again or rephrase your question.\n\nError details: " + ex.Message);
            }
        }private async Task<string> GetGeminiResponseAsync(string userMessage)
        {
            // TODO: Replace with your valid Google Gemini API key
            string apiKey = "YOUR_VALID_GEMINI_API_KEY_HERE";
            
            if (apiKey == "YOUR_VALID_GEMINI_API_KEY_HERE")
            {
                return "Please configure a valid Google Gemini API key. Go to https://makersuite.google.com/app/apikey to get your API key, then update the code.";
            }
            
            string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";
            
            // Create a context-aware prompt for the library assistant
            string prompt = $"You are a helpful library assistant for IHEC University library. Answer questions about books, library services, reservations, and study recommendations. Keep responses concise and helpful. User question: {userMessage}";

            using (var client = new HttpClient())
            {                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 1000
                    }
                };

                var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");                try
                {
                    var response = await client.PostAsync(apiUrl, content);
                    var responseString = await response.Content.ReadAsStringAsync();
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        return $"API Error ({response.StatusCode}): {responseString}";
                    }
                    
                    using var doc = System.Text.Json.JsonDocument.Parse(responseString);
                    
                    var candidates = doc.RootElement.GetProperty("candidates");
                    if (candidates.GetArrayLength() > 0)
                    {
                        var content_part = candidates[0].GetProperty("content").GetProperty("parts");
                        if (content_part.GetArrayLength() > 0)
                        {
                            var text = content_part[0].GetProperty("text").GetString();
                            return text ?? "(No response from Gemini)";
                        }
                    }
                    return "(No response from Gemini)";
                }
                catch (Exception ex)
                {
                    return $"Error contacting Gemini API: {ex.Message}";
                }
            }
        }        private void AddMessage(string sender, string message)
        {
            // Add timestamp with modern light styling
            string timestamp = DateTime.Now.ToString("HH:mm");
            
            // Modern message bubble styling for light theme
            if (sender.Contains("You") || sender == "You")
            {
                // User message - right aligned with blue bubble
                rtbChat.SelectionAlignment = HorizontalAlignment.Right;
                rtbChat.SelectionColor = Color.FromArgb(108, 117, 125); // Modern gray
                rtbChat.SelectionFont = new Font("Segoe UI", 9, FontStyle.Regular);
                rtbChat.AppendText($"You • {timestamp}\n");
                
                // Create rounded message bubble using custom drawing
                int startPos = rtbChat.TextLength;
                rtbChat.SelectionColor = Color.White; // White text
                rtbChat.SelectionFont = new Font("Segoe UI", 11, FontStyle.Regular);
                rtbChat.SelectionBackColor = Color.FromArgb(13, 110, 253); // Modern blue background
                rtbChat.AppendText($"  {message}  ");
                int endPos = rtbChat.TextLength;
                
                // Reset background and add padding
                rtbChat.SelectionBackColor = Color.Transparent;
                rtbChat.AppendText("\n\n");
                
                // Store the message positions for potential custom drawing if needed
                // (This could be used later with custom drawing if RichTextBox limitations are hit)
            }
            else
            {
                // AI Assistant message - left aligned with light gray bubble
                rtbChat.SelectionAlignment = HorizontalAlignment.Left;
                rtbChat.SelectionColor = Color.FromArgb(13, 110, 253); // Blue for bot name
                rtbChat.SelectionFont = new Font("Segoe UI", 9, FontStyle.Bold);
                rtbChat.AppendText($"IHEC AI Assistant • {timestamp}\n");
                
                // Create rounded message bubble
                int startPos = rtbChat.TextLength;
                rtbChat.SelectionColor = Color.FromArgb(33, 37, 41); // Dark text
                rtbChat.SelectionFont = new Font("Segoe UI", 11, FontStyle.Regular);
                rtbChat.SelectionBackColor = Color.FromArgb(233, 236, 239); // Light gray background
                rtbChat.AppendText($"  {message}  ");
                int endPos = rtbChat.TextLength;
                
                // Reset background and add padding
                rtbChat.SelectionBackColor = Color.Transparent;
                rtbChat.AppendText("\n\n");
            }
            
            // Add subtle separator with cleaner styling
            rtbChat.SelectionAlignment = HorizontalAlignment.Center;
            rtbChat.SelectionColor = Color.FromArgb(222, 226, 230); // Very light gray separator
            rtbChat.SelectionFont = new Font("Segoe UI", 6, FontStyle.Regular);
            rtbChat.AppendText("· · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · ·\n\n");// Reset alignment and ensure scrolling to the latest message
            rtbChat.SelectionAlignment = HorizontalAlignment.Left;
            
            // Call helper method to ensure proper scrolling
            EnsureScrollToLatestMessage();
        }

        private string ProcessMessageLocally(string message)
        {
            message = message.ToLower();

            // Simple keyword-based responses
            if (message.Contains("recommend") || message.Contains("suggestion"))
            {
                return "I can recommend books based on your study field. Please check the Library tab for available books in your area of study.";
            }
            else if (message.Contains("available") || message.Contains("availability"))
            {
                return "Please specify which book you're looking for, and I'll check its availability.";
            }
            else if (message.Contains("reserve") || message.Contains("reservation"))
            {
                return "To reserve a book, go to the Library tab, select the book you want, and click 'Reserve Selected'.";
            }
            else if (message.Contains("profile") || message.Contains("account"))
            {
                return "You can view and edit your profile information in the Profile tab.";
            }
            else if (message.Contains("help"))
            {
                return "I can help you with:\n- Book recommendations\n- Checking availability\n- Reservation instructions\n- Navigation help\n\nWhat would you like to know?";
            }
            else if (message.Contains("hello") || message.Contains("hi"))
            {
                return $"Hello! How can I assist you with the library today?";
            }
            else
            {
                return "I'm here to help with library-related questions. Try asking about book recommendations, availability, or reservations!\n\nNote: For enhanced AI responses, please configure a valid Google Gemini API key.";
            }
        }        // Method to verify text input is properly sized and positioned
        private void VerifyTextInputSetup()
        {
            if (txtMessage != null && !txtMessage.IsDisposed)
            {
                System.Diagnostics.Debug.WriteLine($"TextBox Location: {txtMessage.Location}");
                System.Diagnostics.Debug.WriteLine($"TextBox Size: {txtMessage.Size}");
                System.Diagnostics.Debug.WriteLine($"TextBox Enabled: {txtMessage.Enabled}");
                System.Diagnostics.Debug.WriteLine($"TextBox Visible: {txtMessage.Visible}");
                System.Diagnostics.Debug.WriteLine($"TextBox ReadOnly: {txtMessage.ReadOnly}");
                System.Diagnostics.Debug.WriteLine($"TextBox Parent: {txtMessage.Parent?.Name}");
            }
        }
          // Method to ensure proper scrolling in the chat display
        private void EnsureScrollToLatestMessage()
        {
            if (rtbChat != null && !rtbChat.IsDisposed)
            {
                // Move caret to the end of text to ensure scrolling to latest message
                rtbChat.SelectionStart = rtbChat.TextLength;
                rtbChat.ScrollToCaret();
                
                // Force UI update to ensure scrolling happens immediately
                Application.DoEvents();
            }
        }
    }
}