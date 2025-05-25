using System;
using System.Drawing;
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
        private User currentUser;

        public ChatbotControl(User user)
        {
            currentUser = user;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;

            // Chat display
            rtbChat = new RichTextBox();
            rtbChat.Location = new Point(20, 20);
            rtbChat.Size = new Size(540, 350);
            rtbChat.ReadOnly = true;
            rtbChat.BackColor = Color.White;

            // Message input
            txtMessage = new TextBox();
            txtMessage.Location = new Point(20, 390);
            txtMessage.Size = new Size(440, 25);
            txtMessage.KeyPress += TxtMessage_KeyPress;

            // Send button
            btnSend = new Button();
            btnSend.Text = "Send";
            btnSend.Location = new Point(480, 388);
            btnSend.Size = new Size(80, 30);
            btnSend.BackColor = Color.DodgerBlue;
            btnSend.ForeColor = Color.White;
            btnSend.Click += BtnSend_Click;

            this.Controls.AddRange(new Control[] { rtbChat, txtMessage, btnSend });

            // Welcome message
            AddMessage("Bot", "Hello! I'm your library assistant. Ask me about books, availability, or recommendations!");
        }

        private void TxtMessage_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SendMessage();
            }
        }

        private void BtnSend_Click(object? sender, EventArgs e)
        {
            SendMessage();
        }        private async void SendMessage()
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
        }

        private void AddMessage(string sender, string message)
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
        }
    }
}