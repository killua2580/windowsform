using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp.Models;
using WinFormsApp.Data;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WinFormsApp.Forms
{
    public partial class ChatbotForm : Form
    {
        private TextBox txtMessage;
        private RichTextBox rtbChat;
        private Button btnSend;
        private User currentUser;

        public ChatbotForm(User user)
        {
            currentUser = user;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Library Assistant Chatbot";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

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

        private void TxtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SendMessage();
            }
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private async void SendMessage()
        {
            string message = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            AddMessage("You", message);
            txtMessage.Clear();

            // Call Gemini API for response
            string response = await GetGeminiResponseAsync(message);
            AddMessage("Bot", response);
        }

        private async Task<string> GetGeminiResponseAsync(string userMessage)
        {
            // NEVER hardcode API keys in production!
            string apiKey = "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";
            string apiUrl = "https://openrouter.ai/api/v1/chat/completions";
            string prompt = $"You are a helpful library assistant. Answer questions about book availability and library services. User: {userMessage}";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.DefaultRequestHeaders.Add("HTTP-Referer", "https://your-app-domain.com");
                client.DefaultRequestHeaders.Add("X-Title", "IHEC Library Chatbot");

                var requestBody = new
                {
                    model = "google/gemini-pro",
                    messages = new[] {
                        new { role = "user", content = prompt }
                    }
                };
                var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(apiUrl, content);
                    response.EnsureSuccessStatusCode();
                    var responseString = await response.Content.ReadAsStringAsync();
                    using var doc = System.Text.Json.JsonDocument.Parse(responseString);
                    var choices = doc.RootElement.GetProperty("choices");
                    if (choices.GetArrayLength() > 0)
                    {
                        var msg = choices[0].GetProperty("message").GetProperty("content").GetString();
                        return msg ?? "(No response from Gemini)";
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

        private string ProcessMessage(string message)
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
                return "I'm here to help with library-related questions. Try asking about book recommendations, availability, or reservations!";
            }
        }

        private string GetRecommendations()
        {
            List<string> recommendations = new List<string>();
            
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT Title, Author FROM Books WHERE StudyField = @field OR StudyField = 'General' LIMIT 3";
                using (var command = new System.Data.SQLite.SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@field", currentUser.StudyField);
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
        }
    }
} 