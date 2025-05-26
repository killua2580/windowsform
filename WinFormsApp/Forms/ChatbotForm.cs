using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp.Models;
using WinFormsApp.Data;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace WinFormsApp.Forms
{
    public partial class ChatbotForm : Form
    {
        private TextBox txtMessage = null!;
        private RichTextBox rtbChat = null!;
        private Button btnSend = null!;
        private User currentUser;        public ChatbotForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            // Set focus to message textbox when form loads
            this.Load += (s, e) => txtMessage.Focus();
        }

        private void InitializeComponent()
        {
            this.Text = "Library Assistant Chatbot";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(247, 247, 247);

            // Chat display with modern styling
            rtbChat = new RichTextBox();
            rtbChat.Location = new Point(20, 20);
            rtbChat.Size = new Size(840, 220); // Reduced height for compact display
            rtbChat.ReadOnly = true;
            rtbChat.BackColor = Color.White;
            rtbChat.Font = new Font("Segoe UI", 11);
            rtbChat.BorderStyle = BorderStyle.None;
            rtbChat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            
            // Add rounded border effect for chat display
            rtbChat.Paint += (s, e) => {
                using (var pen = new Pen(Color.FromArgb(200, 200, 200), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, rtbChat.Width - 1, rtbChat.Height - 1);
                }
            };

            // Claude-like input container
            Panel inputContainer = new Panel();
            inputContainer.Location = new Point(20, 250); // Move up to match reduced chat area
            inputContainer.Size = new Size(760, 40); // Reduced height for compact input
            inputContainer.BackColor = Color.White;
            inputContainer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            
            // Add rounded border and shadow effect
            inputContainer.Paint += (s, e) => {
                var rect = inputContainer.ClientRectangle;
                
                // Draw shadow
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, new Rectangle(2, 2, rect.Width, rect.Height));
                }
                
                // Draw main container
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillRectangle(brush, new Rectangle(0, 0, rect.Width - 2, rect.Height - 2));
                }
                
                // Draw border
                using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 3, rect.Height - 3);
                }
            };
            
            // Multi-line text input like Claude
            txtMessage = new TextBox();
            txtMessage.Location = new Point(15, 5);
            txtMessage.Size = new Size(640, 28); // Reduced height
            txtMessage.Font = new Font("Segoe UI", 12);
            txtMessage.BorderStyle = BorderStyle.None;
            txtMessage.BackColor = Color.White;
            txtMessage.PlaceholderText = "Message Library Assistant...";
            txtMessage.Multiline = true;
            txtMessage.ScrollBars = ScrollBars.Vertical;
            txtMessage.Enabled = true;
            txtMessage.ReadOnly = false;
            txtMessage.TabIndex = 1;
            txtMessage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;            txtMessage.KeyPress += TxtMessage_KeyPress;
            txtMessage.KeyDown += TxtMessage_KeyDown;
            
            // Add textbox to container
            inputContainer.Controls.Add(txtMessage);

            // Modern send button with icon
            btnSend = new Button();
            btnSend.Location = new Point(670, 5);
            btnSend.Size = new Size(32, 28); // Reduced size
            btnSend.BackColor = Color.FromArgb(16, 163, 127);
            btnSend.ForeColor = Color.White;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnSend.Cursor = Cursors.Hand;
            btnSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSend.Click += BtnSend_Click;
            
            // Add hover effects for send button
            btnSend.MouseEnter += (s, e) => btnSend.BackColor = Color.FromArgb(13, 148, 115);
            btnSend.MouseLeave += (s, e) => btnSend.BackColor = Color.FromArgb(16, 163, 127);
            
            inputContainer.Controls.Add(btnSend);

            // Close Chatbot button
            Button btnClose = new Button();
            btnClose.Text = "✕ Close";
            btnClose.Location = new Point(790, 20);
            btnClose.Size = new Size(70, 30);
            btnClose.BackColor = Color.FromArgb(220, 53, 69);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.Click += BtnClose_Click;

            btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(255, 69, 58);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.FromArgb(220, 53, 69);

            // Clear Chat button
            Button btnClear = new Button();
            btnClear.Text = "🗑️ Clear";
            btnClear.Location = new Point(790, 60);
            btnClear.Size = new Size(70, 30);
            btnClear.BackColor = Color.FromArgb(169, 169, 169);
            btnClear.ForeColor = Color.White;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.Cursor = Cursors.Hand;
            btnClear.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClear.Click += BtnClear_Click;

            btnClear.MouseEnter += (s, e) => btnClear.BackColor = Color.FromArgb(128, 128, 128);
            btnClear.MouseLeave += (s, e) => btnClear.BackColor = Color.FromArgb(169, 169, 169);

            this.Controls.AddRange(new Control[] { rtbChat, inputContainer, btnClose, btnClear });

            // Welcome message
            AddMessage("Bot", "Hello! I'm your library assistant. Ask me about books, availability, or recommendations!");
        }        private void TxtMessage_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SendMessage();
            }
        }

        private void TxtMessage_KeyDown(object? sender, KeyEventArgs e)
        {
            // Allow Shift+Enter for new lines, Enter alone to send
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SendMessage();
            }
        }private void BtnSend_Click(object? sender, EventArgs e)
        {
            SendMessage();
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnClear_Click(object? sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to clear the chat history?", 
                "Clear Chat", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                rtbChat.Clear();
                AddMessage("Bot", "Hello! I'm your library assistant. Ask me about books, availability, or recommendations!");
            }
        }private async void SendMessage()
        {
            string message = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            AddMessage("You", message);
            txtMessage.Clear();

            // Try Gemini API first, fallback to local responses
            string response = await GetGeminiResponseAsync(message);
            if (response.Contains("Please configure a valid Google Gemini API key"))
            {
                response = ProcessMessageLocally(message);
            }
            AddMessage("Bot", response);
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
            rtbChat.SelectionColor = sender == "You" ? Color.Blue : Color.Green;
            rtbChat.SelectionFont = new Font(rtbChat.Font, FontStyle.Bold);
            rtbChat.AppendText($"{sender}: ");
            
            rtbChat.SelectionColor = Color.Black;
            rtbChat.SelectionFont = new Font(rtbChat.Font, FontStyle.Regular);
            rtbChat.AppendText($"{message}\n\n");
            
            rtbChat.ScrollToCaret();
        }

        private string ProcessMessageLocally(string message)
        {
            message = message.ToLower();

            // Simple keyword-based responses
            if (message.Contains("recommend") || message.Contains("suggestion"))
            {
                return GetRecommendations();
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
                return $"Hello {currentUser.FirstName}! How can I assist you with the library today?";
            }
            else
            {
                return "I'm here to help with library-related questions. Try asking about book recommendations, availability, or reservations!\n\nNote: For enhanced AI responses, please configure a valid Google Gemini API key.";
            }
        }

        private string GetRecommendations()
        {
            List<string> recommendations = new List<string>();
            
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT TOP 3 Title, Author FROM Books WHERE StudyField = @field OR StudyField = 'General'";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@field", currentUser.StudyField ?? "General");
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                recommendations.Add($"• {reader["Title"]} by {reader["Author"]}");
                            }
                        }
                    }
                }

                if (recommendations.Count > 0)
                {
                    return $"Based on your field ({currentUser.StudyField}), I recommend:\n\n" + string.Join("\n", recommendations);
                }
                else
                {
                    return "I don't have specific recommendations for your field right now, but check the Library tab for all available books!";
                }
            }            catch (Exception)
            {
                return "Sorry, I couldn't fetch recommendations at the moment. Please try again later.";
            }
        }
    }
}