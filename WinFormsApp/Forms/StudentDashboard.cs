using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp.Models;
using WinFormsApp.Data;

namespace WinFormsApp.Forms
{
    public partial class StudentDashboard : Form
    {
        private User currentUser;
        private TabControl tabControl;

        public StudentDashboard(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadRecommendedBooks();
        }

        private void InitializeComponent()
        {
            this.Text = $"Library Dashboard - Welcome {currentUser.FirstName}";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;

            // Home Tab
            TabPage homeTab = new TabPage("Home");
            CreateHomeTab(homeTab);

            // Library Tab
            TabPage libraryTab = new TabPage("Library");
            CreateLibraryTab(libraryTab);

            // Profile Tab
            TabPage profileTab = new TabPage("Profile");
            CreateProfileTab(profileTab);

            // Chatbot Tab
            TabPage chatbotTab = new TabPage("Chatbot");
            CreateChatbotTab(chatbotTab);

            tabControl.TabPages.AddRange(new TabPage[] { homeTab, libraryTab, profileTab, chatbotTab });
            this.Controls.Add(tabControl);
        }

        private void CreateHomeTab(TabPage tab)
        {
            Label lblWelcome = new Label();
            lblWelcome.Text = $"Welcome back, {currentUser.FirstName}!";
            lblWelcome.Font = new Font("Arial", 16, FontStyle.Bold);
            lblWelcome.Location = new Point(20, 20);
            lblWelcome.Size = new Size(400, 30);

            Label lblRecommended = new Label();
            lblRecommended.Text = "Recommended Books for You:";
            lblRecommended.Font = new Font("Arial", 12, FontStyle.Bold);
            lblRecommended.Location = new Point(20, 70);
            lblRecommended.Size = new Size(300, 25);

            ListView lstRecommended = new ListView();
            lstRecommended.Location = new Point(20, 100);
            lstRecommended.Size = new Size(900, 400);
            lstRecommended.View = View.Details;
            lstRecommended.FullRowSelect = true;
            lstRecommended.GridLines = true;
            lstRecommended.Columns.AddRange(new ColumnHeader[] {
                new ColumnHeader() { Text = "Title", Width = 200 },
                new ColumnHeader() { Text = "Author", Width = 150 },
                new ColumnHeader() { Text = "Category", Width = 120 },
                new ColumnHeader() { Text = "Available", Width = 80 },
                new ColumnHeader() { Text = "Field", Width = 100 }
            });

            tab.Controls.AddRange(new Control[] { lblWelcome, lblRecommended, lstRecommended });
        }

        private void CreateLibraryTab(TabPage tab)
        {
            Label lblSearch = new Label();
            lblSearch.Text = "Search Books:";
            lblSearch.Location = new Point(20, 20);
            lblSearch.Size = new Size(100, 20);

            TextBox txtSearch = new TextBox();
            txtSearch.Location = new Point(130, 18);
            txtSearch.Size = new Size(300, 25);

            Button btnSearch = new Button();
            btnSearch.Text = "Search";
            btnSearch.Location = new Point(450, 16);
            btnSearch.Size = new Size(80, 30);

            ListView lstBooks = new ListView();
            lstBooks.Location = new Point(20, 60);
            lstBooks.Size = new Size(900, 500);
            lstBooks.View = View.Details;
            lstBooks.FullRowSelect = true;
            lstBooks.GridLines = true;
            lstBooks.Columns.AddRange(new ColumnHeader[] {
                new ColumnHeader() { Text = "Title", Width = 200 },
                new ColumnHeader() { Text = "Author", Width = 150 },
                new ColumnHeader() { Text = "Category", Width = 120 },
                new ColumnHeader() { Text = "Available", Width = 80 },
                new ColumnHeader() { Text = "Total", Width = 80 }
            });

            Button btnReserve = new Button();
            btnReserve.Text = "Reserve Selected";
            btnReserve.Location = new Point(20, 580);
            btnReserve.Size = new Size(120, 35);
            btnReserve.BackColor = Color.Orange;
            btnReserve.ForeColor = Color.White;

            Button btnLike = new Button();
            btnLike.Text = "Like Selected";
            btnLike.Location = new Point(160, 580);
            btnLike.Size = new Size(120, 35);
            btnLike.BackColor = Color.Red;
            btnLike.ForeColor = Color.White;

            tab.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, btnSearch, lstBooks, btnReserve, btnLike
            });
        }

        private void CreateProfileTab(TabPage tab)
        {
            Label lblProfile = new Label();
            lblProfile.Text = "Profile Information";
            lblProfile.Font = new Font("Arial", 16, FontStyle.Bold);
            lblProfile.Location = new Point(20, 20);
            lblProfile.Size = new Size(200, 30);

            // Display user information
            int yPos = 70;
            CreateProfileField(tab, "Name:", $"{currentUser.FirstName} {currentUser.LastName}", yPos);
            yPos += 40;
            CreateProfileField(tab, "Email:", currentUser.Email, yPos);
            yPos += 40;
            CreateProfileField(tab, "Study Level:", currentUser.StudyLevel, yPos);
            yPos += 40;
            CreateProfileField(tab, "Study Field:", currentUser.StudyField, yPos);
            yPos += 60;

            Label lblStats = new Label();
            lblStats.Text = "Library Statistics";
            lblStats.Font = new Font("Arial", 14, FontStyle.Bold);
            lblStats.Location = new Point(20, yPos);
            lblStats.Size = new Size(200, 25);
            yPos += 40;

            // Add statistics here (books reserved, liked, etc.)
            CreateProfileField(tab, "Books Reserved:", "0", yPos);
            yPos += 40;
            CreateProfileField(tab, "Books Liked:", "0", yPos);

            tab.Controls.Add(lblProfile);
            tab.Controls.Add(lblStats);
        }

        private void CreateProfileField(TabPage tab, string label, string value, int yPos)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Location = new Point(20, yPos);
            lbl.Size = new Size(150, 20);
            lbl.Font = new Font("Arial", 10, FontStyle.Bold);

            Label val = new Label();
            val.Text = value;
            val.Location = new Point(180, yPos);
            val.Size = new Size(300, 20);

            tab.Controls.AddRange(new Control[] { lbl, val });
        }

        private void CreateChatbotTab(TabPage tab)
        {
            ChatbotControl chatbot = new ChatbotControl(currentUser);
            chatbot.Dock = DockStyle.Fill;
            tab.Controls.Add(chatbot);
        }

        private void LoadRecommendedBooks()
        {
            // Implementation for loading recommended books based on user's field
        }
    }
} 