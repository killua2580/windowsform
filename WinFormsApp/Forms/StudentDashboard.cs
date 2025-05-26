using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WinFormsApp.Models;
using WinFormsApp.Data;
using Microsoft.Data.SqlClient;

namespace WinFormsApp.Forms
{
    // Add UserStatistics class
    public class UserStatistics
    {
        public int ReservedBooks { get; set; }
        public int ActiveReservations { get; set; }
        public int LikedBooks { get; set; }
        public string? FavoriteCategory { get; set; }
        public int BooksInField { get; set; }
        public int TotalAvailableBooks { get; set; }
    }

    public partial class StudentDashboard : Form
    {
        private User currentUser;
        private TabControl tabControl = null!;
        private ListView lstRecommended = null!;
        private ListView lstBooks = null!;
        private TextBox txtSearch = null!;

        public StudentDashboard(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadRecommendedBooks();
        }        private void InitializeComponent()
        {
            this.Text = $"IHEC Digital Library - Welcome {currentUser.FirstName}";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(248, 249, 250);

            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            tabControl.ItemSize = new Size(120, 35);
            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
              // Custom tab drawing for modern look
            tabControl.DrawItem += (s, e) => {
                if (s is TabControl tc)
                {
                    Rectangle r = e.Bounds;
                    string title = tc.TabPages[e.Index].Text;
                    
                    using (Brush brush = new SolidBrush(e.Index == tc.SelectedIndex ? 
                        Color.FromArgb(70, 130, 180) : Color.FromArgb(200, 200, 200)))
                    {
                        e.Graphics.FillRectangle(brush, r);
                    }
                    
                    using (Brush textBrush = new SolidBrush(e.Index == tc.SelectedIndex ? 
                        Color.White : Color.FromArgb(80, 80, 80)))
                    {
                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;
                        e.Graphics.DrawString(title, tabControl.Font, textBrush, r, sf);
                    }
                }
            };

            // Home Tab with modern icons
            TabPage homeTab = new TabPage("🏠 Home");
            homeTab.BackColor = Color.White;
            CreateHomeTab(homeTab);

            // Library Tab
            TabPage libraryTab = new TabPage("📚 Library");
            libraryTab.BackColor = Color.White;
            CreateLibraryTab(libraryTab);

            // Profile Tab
            TabPage profileTab = new TabPage("👤 Profile");
            profileTab.BackColor = Color.White;
            CreateProfileTab(profileTab);

            // Chatbot Tab
            TabPage chatbotTab = new TabPage("🤖 AI Assistant");
            chatbotTab.BackColor = Color.White;
            CreateChatbotTab(chatbotTab);

            tabControl.TabPages.AddRange(new TabPage[] { homeTab, libraryTab, profileTab, chatbotTab });
            this.Controls.Add(tabControl);
        }        private void CreateHomeTab(TabPage tab)
        {
            // Modern gradient background for the tab
            tab.Paint += (s, e) => {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    tab.ClientRectangle, 
                    Color.FromArgb(245, 247, 250), 
                    Color.FromArgb(195, 207, 226), 
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, tab.ClientRectangle);
                }
            };

            // Welcome section with modern card design
            Panel welcomeCard = new Panel();
            welcomeCard.Location = new Point(20, 20);
            welcomeCard.Size = new Size(900, 80);
            welcomeCard.BackColor = Color.White;
            welcomeCard.BorderStyle = BorderStyle.None;
            
            // Add shadow effect
            welcomeCard.Paint += (s, e) => {
                // Draw shadow
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(30, 0, 0, 0)), 
                    3, 3, welcomeCard.Width, welcomeCard.Height);
                // Draw card background
                e.Graphics.FillRectangle(Brushes.White, 0, 0, welcomeCard.Width - 3, welcomeCard.Height - 3);
            };

            Label lblWelcome = new Label();
            lblWelcome.Text = $"🎉 Welcome back, {currentUser.FirstName}!";
            lblWelcome.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblWelcome.ForeColor = Color.FromArgb(70, 130, 180);
            lblWelcome.Location = new Point(30, 15);
            lblWelcome.Size = new Size(500, 30);
            lblWelcome.BackColor = Color.Transparent;

            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Discover your next favorite book from our curated recommendations";
            lblSubtitle.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            lblSubtitle.ForeColor = Color.FromArgb(120, 120, 120);
            lblSubtitle.Location = new Point(30, 45);
            lblSubtitle.Size = new Size(500, 20);
            lblSubtitle.BackColor = Color.Transparent;

            // Add Logout Button to welcome card
            Button btnLogout = new Button();
            btnLogout.Text = "🚪 Logout";
            btnLogout.Location = new Point(750, 25);
            btnLogout.Size = new Size(120, 30);
            btnLogout.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnLogout.BackColor = Color.FromArgb(220, 53, 69);
            btnLogout.ForeColor = Color.White;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Click += (s, e) => {
                var result = MessageBox.Show("Are you sure you want to logout?", "Logout Confirmation", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    this.Close();
                }
            };
            
            btnLogout.MouseEnter += (s, e) => btnLogout.BackColor = Color.FromArgb(225, 83, 97);
            btnLogout.MouseLeave += (s, e) => btnLogout.BackColor = Color.FromArgb(220, 53, 69);

            welcomeCard.Controls.AddRange(new Control[] { lblWelcome, lblSubtitle, btnLogout });

            // Recommendations section header
            Label lblRecommended = new Label();
            lblRecommended.Text = "📚 Personalized Recommendations";
            lblRecommended.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblRecommended.ForeColor = Color.FromArgb(52, 73, 94);
            lblRecommended.Location = new Point(20, 120);
            lblRecommended.Size = new Size(400, 25);
            lblRecommended.BackColor = Color.Transparent;

            // Enhanced ListView with modern styling
            lstRecommended = new ListView();
            lstRecommended.Location = new Point(20, 155);
            lstRecommended.Size = new Size(900, 350);
            lstRecommended.View = View.Details;
            lstRecommended.FullRowSelect = true;
            lstRecommended.GridLines = true;
            lstRecommended.BorderStyle = BorderStyle.None;
            lstRecommended.BackColor = Color.White;
            lstRecommended.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            
            // Add modern column headers with emojis
            lstRecommended.Columns.AddRange(new ColumnHeader[] {
                new ColumnHeader() { Text = "📖 Title", Width = 220 },
                new ColumnHeader() { Text = "✍️ Author", Width = 170 },
                new ColumnHeader() { Text = "📂 Category", Width = 140 },
                new ColumnHeader() { Text = "📊 Available", Width = 100 },
                new ColumnHeader() { Text = "🎓 Field", Width = 120 }
            });

            // Modern action buttons with enhanced styling
            Button btnRefreshHome = new Button();
            btnRefreshHome.Text = "🔄 Refresh Recommendations";
            btnRefreshHome.Location = new Point(20, 520);
            btnRefreshHome.Size = new Size(200, 40);
            btnRefreshHome.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnRefreshHome.BackColor = Color.FromArgb(70, 130, 180);
            btnRefreshHome.ForeColor = Color.White;
            btnRefreshHome.FlatStyle = FlatStyle.Flat;
            btnRefreshHome.FlatAppearance.BorderSize = 0;
            btnRefreshHome.Cursor = Cursors.Hand;
            btnRefreshHome.Click += (s, e) => LoadRecommendedBooks();
            
            // Add hover effects
            btnRefreshHome.MouseEnter += (s, e) => btnRefreshHome.BackColor = Color.FromArgb(100, 149, 237);
            btnRefreshHome.MouseLeave += (s, e) => btnRefreshHome.BackColor = Color.FromArgb(70, 130, 180);

            Button btnViewAllBooks = new Button();
            btnViewAllBooks.Text = "📚 Browse All Books";
            btnViewAllBooks.Location = new Point(240, 520);
            btnViewAllBooks.Size = new Size(180, 40);
            btnViewAllBooks.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnViewAllBooks.BackColor = Color.FromArgb(34, 139, 34);
            btnViewAllBooks.ForeColor = Color.White;
            btnViewAllBooks.FlatStyle = FlatStyle.Flat;
            btnViewAllBooks.FlatAppearance.BorderSize = 0;
            btnViewAllBooks.Cursor = Cursors.Hand;
            btnViewAllBooks.Click += (s, e) => {
                // Switch to Library tab
                if (tabControl.TabPages.Count > 1)
                    tabControl.SelectedIndex = 1;
            };
            
            // Add hover effects
            btnViewAllBooks.MouseEnter += (s, e) => btnViewAllBooks.BackColor = Color.FromArgb(60, 179, 113);
            btnViewAllBooks.MouseLeave += (s, e) => btnViewAllBooks.BackColor = Color.FromArgb(34, 139, 34);

            // Add Exit Application button
            Button btnExitApp = new Button();
            btnExitApp.Text = "❌ Exit Application";
            btnExitApp.Location = new Point(740, 520);
            btnExitApp.Size = new Size(180, 40);
            btnExitApp.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnExitApp.BackColor = Color.FromArgb(108, 117, 125);
            btnExitApp.ForeColor = Color.White;
            btnExitApp.FlatStyle = FlatStyle.Flat;
            btnExitApp.FlatAppearance.BorderSize = 0;
            btnExitApp.Cursor = Cursors.Hand;
            btnExitApp.Click += (s, e) => {
                var result = MessageBox.Show("Are you sure you want to exit the application?", "Exit Application", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            };
            
            btnExitApp.MouseEnter += (s, e) => btnExitApp.BackColor = Color.FromArgb(128, 137, 145);
            btnExitApp.MouseLeave += (s, e) => btnExitApp.BackColor = Color.FromArgb(108, 117, 125);

            tab.Controls.AddRange(new Control[] { welcomeCard, lblRecommended, lstRecommended, btnRefreshHome, btnViewAllBooks, btnExitApp });
        }        private void CreateLibraryTab(TabPage tab)
        {
            // Modern gradient background for the tab
            tab.Paint += (s, e) => {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    tab.ClientRectangle, 
                    Color.FromArgb(245, 247, 250), 
                    Color.FromArgb(195, 207, 226), 
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, tab.ClientRectangle);
                }
            };

            // Search section with modern card design
            Panel searchCard = new Panel();
            searchCard.Location = new Point(20, 20);
            searchCard.Size = new Size(900, 70);
            searchCard.BackColor = Color.White;
            searchCard.BorderStyle = BorderStyle.None;
            
            // Add shadow effect
            searchCard.Paint += (s, e) => {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(30, 0, 0, 0)), 
                    3, 3, searchCard.Width, searchCard.Height);
                e.Graphics.FillRectangle(Brushes.White, 0, 0, searchCard.Width - 3, searchCard.Height - 3);
            };

            Label lblSearch = new Label();
            lblSearch.Text = "🔍 Search Library Collection:";
            lblSearch.Location = new Point(20, 15);
            lblSearch.Size = new Size(200, 20);
            lblSearch.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblSearch.ForeColor = Color.FromArgb(52, 73, 94);
            lblSearch.BackColor = Color.Transparent;            txtSearch = new TextBox();
            txtSearch.Location = new Point(20, 40);
            txtSearch.Size = new Size(350, 25);
            txtSearch.Font = new Font("Segoe UI", 11);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.PlaceholderText = "Search by title, author, category, or field...";
            txtSearch.Enabled = true;
            txtSearch.ReadOnly = false;
            txtSearch.TabIndex = 1;

            Button btnSearch = new Button();
            btnSearch.Text = "🔍 Search";
            btnSearch.Location = new Point(380, 38);
            btnSearch.Size = new Size(100, 30);
            btnSearch.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSearch.BackColor = Color.FromArgb(34, 139, 34);
            btnSearch.ForeColor = Color.White;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Cursor = Cursors.Hand;
            btnSearch.Click += (s, e) => SearchBooks(txtSearch.Text);
            
            btnSearch.MouseEnter += (s, e) => btnSearch.BackColor = Color.FromArgb(60, 179, 113);
            btnSearch.MouseLeave += (s, e) => btnSearch.BackColor = Color.FromArgb(34, 139, 34);

            Button btnShowAll = new Button();
            btnShowAll.Text = "📚 Show All";
            btnShowAll.Location = new Point(490, 38);
            btnShowAll.Size = new Size(100, 30);
            btnShowAll.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnShowAll.BackColor = Color.FromArgb(70, 130, 180);
            btnShowAll.ForeColor = Color.White;
            btnShowAll.FlatStyle = FlatStyle.Flat;
            btnShowAll.FlatAppearance.BorderSize = 0;
            btnShowAll.Cursor = Cursors.Hand;
            btnShowAll.Click += (s, e) => LoadAllBooks();
            
            btnShowAll.MouseEnter += (s, e) => btnShowAll.BackColor = Color.FromArgb(100, 149, 237);
            btnShowAll.MouseLeave += (s, e) => btnShowAll.BackColor = Color.FromArgb(70, 130, 180);

            searchCard.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnSearch, btnShowAll });

            // Enhanced Books ListView with modern styling
            lstBooks = new ListView();
            lstBooks.Location = new Point(20, 110);
            lstBooks.Size = new Size(900, 380);
            lstBooks.View = View.Details;
            lstBooks.FullRowSelect = true;
            lstBooks.GridLines = true;
            lstBooks.BorderStyle = BorderStyle.None;
            lstBooks.BackColor = Color.White;
            lstBooks.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            
            // Add modern column headers with emojis
            lstBooks.Columns.AddRange(new ColumnHeader[] {
                new ColumnHeader() { Text = "🆔 ID", Width = 60 },
                new ColumnHeader() { Text = "📖 Title", Width = 220 },
                new ColumnHeader() { Text = "✍️ Author", Width = 170 },
                new ColumnHeader() { Text = "📂 Category", Width = 120 },
                new ColumnHeader() { Text = "🎓 Field", Width = 120 },
                new ColumnHeader() { Text = "📊 Available", Width = 90 },
                new ColumnHeader() { Text = "📚 Total", Width = 80 }
            });

            // Modern action buttons with enhanced styling
            Panel actionPanel = new Panel();
            actionPanel.Location = new Point(20, 500);
            actionPanel.Size = new Size(900, 50);
            actionPanel.BackColor = Color.Transparent;

            Button btnReserve = new Button();
            btnReserve.Text = "📋 Reserve Selected";
            btnReserve.Location = new Point(0, 10);
            btnReserve.Size = new Size(160, 40);
            btnReserve.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnReserve.BackColor = Color.FromArgb(255, 140, 0);
            btnReserve.ForeColor = Color.White;
            btnReserve.FlatStyle = FlatStyle.Flat;
            btnReserve.FlatAppearance.BorderSize = 0;
            btnReserve.Cursor = Cursors.Hand;
            btnReserve.Click += (s, e) => ReserveSelectedBook();
            
            btnReserve.MouseEnter += (s, e) => btnReserve.BackColor = Color.FromArgb(255, 165, 0);
            btnReserve.MouseLeave += (s, e) => btnReserve.BackColor = Color.FromArgb(255, 140, 0);

            Button btnLike = new Button();
            btnLike.Text = "❤️ Like Selected";
            btnLike.Location = new Point(180, 10);
            btnLike.Size = new Size(150, 40);
            btnLike.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnLike.BackColor = Color.FromArgb(220, 53, 69);
            btnLike.ForeColor = Color.White;
            btnLike.FlatStyle = FlatStyle.Flat;
            btnLike.FlatAppearance.BorderSize = 0;
            btnLike.Cursor = Cursors.Hand;
            btnLike.Click += (s, e) => LikeSelectedBook();
            
            btnLike.MouseEnter += (s, e) => btnLike.BackColor = Color.FromArgb(225, 83, 97);
            btnLike.MouseLeave += (s, e) => btnLike.BackColor = Color.FromArgb(220, 53, 69);

            Button btnRefresh = new Button();
            btnRefresh.Text = "🔄 Refresh";
            btnRefresh.Location = new Point(350, 10);
            btnRefresh.Size = new Size(120, 40);
            btnRefresh.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnRefresh.BackColor = Color.FromArgb(70, 130, 180);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += (s, e) => LoadAllBooks();
            
            btnRefresh.MouseEnter += (s, e) => btnRefresh.BackColor = Color.FromArgb(100, 149, 237);
            btnRefresh.MouseLeave += (s, e) => btnRefresh.BackColor = Color.FromArgb(70, 130, 180);

            actionPanel.Controls.AddRange(new Control[] { btnReserve, btnLike, btnRefresh });

            tab.Controls.AddRange(new Control[] { searchCard, lstBooks, actionPanel });

            // Load all books initially
            LoadAllBooks();
        }        private void CreateProfileTab(TabPage tab)
        {
            // Modern gradient background for the tab
            tab.Paint += (s, e) => {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    tab.ClientRectangle, 
                    Color.FromArgb(245, 247, 250), 
                    Color.FromArgb(195, 207, 226), 
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, tab.ClientRectangle);
                }
            };

            // Profile Information Card
            Panel profileCard = new Panel();
            profileCard.Location = new Point(20, 20);
            profileCard.Size = new Size(450, 300);
            profileCard.BackColor = Color.White;
            profileCard.BorderStyle = BorderStyle.None;
            
            // Add shadow effect to profile card
            profileCard.Paint += (s, e) => {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(30, 0, 0, 0)), 
                    3, 3, profileCard.Width, profileCard.Height);
                e.Graphics.FillRectangle(Brushes.White, 0, 0, profileCard.Width - 3, profileCard.Height - 3);
            };

            Label lblProfile = new Label();
            lblProfile.Text = "👤 Profile Information";
            lblProfile.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblProfile.ForeColor = Color.FromArgb(70, 130, 180);
            lblProfile.Location = new Point(20, 20);
            lblProfile.Size = new Size(250, 30);
            lblProfile.BackColor = Color.Transparent;

            // Display user information with modern styling
            int yPos = 70;
            CreateModernProfileField(profileCard, "👤 Name:", $"{currentUser.FirstName} {currentUser.LastName}", yPos);
            yPos += 35;
            CreateModernProfileField(profileCard, "📧 Email:", currentUser.Email, yPos);
            yPos += 35;
            CreateModernProfileField(profileCard, "🎓 Study Level:", currentUser.StudyLevel, yPos);
            yPos += 35;
            CreateModernProfileField(profileCard, "📚 Study Field:", currentUser.StudyField, yPos);
            yPos += 35;
            CreateModernProfileField(profileCard, "📅 Member Since:", currentUser.CreatedDate.ToString("MMM dd, yyyy"), yPos);

            // Statistics Card
            Panel statsCard = new Panel();
            statsCard.Location = new Point(490, 20);
            statsCard.Size = new Size(430, 300);
            statsCard.BackColor = Color.White;
            statsCard.BorderStyle = BorderStyle.None;
            
            // Add shadow effect to stats card
            statsCard.Paint += (s, e) => {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(30, 0, 0, 0)), 
                    3, 3, statsCard.Width, statsCard.Height);
                e.Graphics.FillRectangle(Brushes.White, 0, 0, statsCard.Width - 3, statsCard.Height - 3);
            };

            Label lblStats = new Label();
            lblStats.Text = "📊 Library Statistics";
            lblStats.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblStats.ForeColor = Color.FromArgb(70, 130, 180);
            lblStats.Location = new Point(20, 20);
            lblStats.Size = new Size(250, 30);
            lblStats.BackColor = Color.Transparent;

            // Get actual statistics from database
            var stats = GetUserStatistics();
            
            yPos = 70;
            CreateModernStatsField(statsCard, "📋 Books Reserved:", stats.ReservedBooks.ToString(), yPos, Color.FromArgb(255, 140, 0));
            yPos += 35;
            CreateModernStatsField(statsCard, "🟢 Active Reservations:", stats.ActiveReservations.ToString(), yPos, Color.FromArgb(34, 139, 34));
            yPos += 35;
            CreateModernStatsField(statsCard, "❤️ Books Liked:", stats.LikedBooks.ToString(), yPos, Color.FromArgb(220, 53, 69));
            yPos += 35;
            CreateModernStatsField(statsCard, "⭐ Favorite Category:", stats.FavoriteCategory ?? "None", yPos, Color.FromArgb(102, 51, 153));
            yPos += 35;
            CreateModernStatsField(statsCard, "🎓 Books in Your Field:", stats.BooksInField.ToString(), yPos, Color.FromArgb(70, 130, 180));

            // Action buttons with modern styling
            Button btnRefreshStats = new Button();
            btnRefreshStats.Text = "🔄 Refresh Statistics";
            btnRefreshStats.Location = new Point(20, 340);
            btnRefreshStats.Size = new Size(200, 45);
            btnRefreshStats.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnRefreshStats.BackColor = Color.FromArgb(70, 130, 180);
            btnRefreshStats.ForeColor = Color.White;
            btnRefreshStats.FlatStyle = FlatStyle.Flat;
            btnRefreshStats.FlatAppearance.BorderSize = 0;
            btnRefreshStats.Cursor = Cursors.Hand;
            btnRefreshStats.Click += (s, e) => RefreshProfileTab(tab);
            
            btnRefreshStats.MouseEnter += (s, e) => btnRefreshStats.BackColor = Color.FromArgb(100, 149, 237);
            btnRefreshStats.MouseLeave += (s, e) => btnRefreshStats.BackColor = Color.FromArgb(70, 130, 180);

            Button btnViewReservations = new Button();
            btnViewReservations.Text = "📋 View My Reservations";
            btnViewReservations.Location = new Point(240, 340);
            btnViewReservations.Size = new Size(200, 45);
            btnViewReservations.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnViewReservations.BackColor = Color.FromArgb(255, 140, 0);
            btnViewReservations.ForeColor = Color.White;
            btnViewReservations.FlatStyle = FlatStyle.Flat;
            btnViewReservations.FlatAppearance.BorderSize = 0;
            btnViewReservations.Cursor = Cursors.Hand;
            btnViewReservations.Click += (s, e) => {
                MessageBox.Show($"You have {stats.ActiveReservations} active reservations.\nView them in the Library section!", 
                    "Reservations Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            
            btnViewReservations.MouseEnter += (s, e) => btnViewReservations.BackColor = Color.FromArgb(255, 165, 0);
            btnViewReservations.MouseLeave += (s, e) => btnViewReservations.BackColor = Color.FromArgb(255, 140, 0);

            tab.Controls.AddRange(new Control[] { profileCard, statsCard, btnRefreshStats, btnViewReservations });
        }

        private void CreateModernProfileField(Panel parent, string label, string? value, int yPos)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Location = new Point(20, yPos);
            lbl.Size = new Size(150, 25);
            lbl.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(52, 73, 94);
            lbl.BackColor = Color.Transparent;

            Label val = new Label();
            val.Text = value ?? "N/A";
            val.Location = new Point(180, yPos);
            val.Size = new Size(250, 25);
            val.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            val.ForeColor = Color.FromArgb(73, 80, 87);
            val.BackColor = Color.Transparent;

            parent.Controls.AddRange(new Control[] { lbl, val });
        }

        private void CreateModernStatsField(Panel parent, string label, string value, int yPos, Color accentColor)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Location = new Point(20, yPos);
            lbl.Size = new Size(200, 25);
            lbl.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(52, 73, 94);
            lbl.BackColor = Color.Transparent;

            Label val = new Label();
            val.Text = value;
            val.Location = new Point(230, yPos);
            val.Size = new Size(180, 25);
            val.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            val.ForeColor = accentColor;
            val.BackColor = Color.Transparent;
            val.TextAlign = ContentAlignment.MiddleLeft;

            parent.Controls.AddRange(new Control[] { lbl, val });
        }

        private void CreateProfileField(TabPage tab, string label, string? value, int yPos)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Location = new Point(20, yPos);
            lbl.Size = new Size(150, 20);
            lbl.Font = new Font("Arial", 10, FontStyle.Bold);

            Label val = new Label();
            val.Text = value ?? "N/A";
            val.Location = new Point(180, yPos);
            val.Size = new Size(300, 20);

            tab.Controls.AddRange(new Control[] { lbl, val });
        }

        private void CreateChatbotTab(TabPage tab)
        {
            ChatbotControl chatbot = new ChatbotControl(currentUser);
            chatbot.Dock = DockStyle.Fill;
            tab.Controls.Add(chatbot);
        }        private void LoadRecommendedBooks()
        {
            if (lstRecommended == null) return;

            lstRecommended.Items.Clear();
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    
                    // Enhanced recommendation query that prioritizes liked books
                    string query = @"
                        WITH RecommendedBooks AS (
                            -- First priority: Books liked by the user that are available
                            SELECT DISTINCT b.BookID, b.Title, b.Author, b.Category, b.StudyField, 
                                   b.AvailableCount, b.TotalCount, 1 as Priority, 'Liked by you' as ReasonText
                            FROM Books b
                            INNER JOIN Likes l ON b.BookID = l.BookID
                            WHERE l.UserID = @userId AND b.AvailableCount > 0
                            
                            UNION ALL
                            
                            -- Second priority: Books from same category as liked books
                            SELECT DISTINCT b.BookID, b.Title, b.Author, b.Category, b.StudyField, 
                                   b.AvailableCount, b.TotalCount, 2 as Priority, 'Similar to liked books' as ReasonText
                            FROM Books b
                            WHERE b.AvailableCount > 0 
                            AND b.Category IN (
                                SELECT DISTINCT b2.Category 
                                FROM Books b2 
                                INNER JOIN Likes l ON b2.BookID = l.BookID 
                                WHERE l.UserID = @userId
                            )
                            AND b.BookID NOT IN (
                                SELECT l2.BookID FROM Likes l2 WHERE l2.UserID = @userId
                            )
                            
                            UNION ALL
                            
                            -- Third priority: Books from user's study field
                            SELECT DISTINCT b.BookID, b.Title, b.Author, b.Category, b.StudyField, 
                                   b.AvailableCount, b.TotalCount, 3 as Priority, 'Matches your field' as ReasonText
                            FROM Books b
                            WHERE b.AvailableCount > 0 
                            AND (b.StudyField = @field OR b.StudyField = 'General')
                            AND b.BookID NOT IN (
                                SELECT l.BookID FROM Likes l WHERE l.UserID = @userId
                            )
                            AND b.Category NOT IN (
                                SELECT DISTINCT b2.Category 
                                FROM Books b2 
                                INNER JOIN Likes l ON b2.BookID = l.BookID 
                                WHERE l.UserID = @userId
                            )
                        )
                        SELECT TOP 10 BookID, Title, Author, Category, StudyField, AvailableCount, TotalCount, ReasonText
                        FROM RecommendedBooks
                        ORDER BY Priority, AvailableCount DESC";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", currentUser.UserID);
                        command.Parameters.AddWithValue("@field", currentUser.StudyField ?? "General");
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string reasonText = reader["ReasonText"]?.ToString() ?? "";
                                string title = reader["Title"]?.ToString() ?? "";
                                
                                // Add visual indicator for different recommendation types
                                if (reasonText == "Liked by you")
                                    title = "❤️ " + title;
                                else if (reasonText == "Similar to liked books")
                                    title = "📚 " + title;
                                
                                ListViewItem item = new ListViewItem(title);
                                item.SubItems.Add(reader["Author"]?.ToString() ?? "");
                                item.SubItems.Add(reader["Category"]?.ToString() ?? "");
                                item.SubItems.Add(reader["AvailableCount"].ToString());
                                item.SubItems.Add(reader["StudyField"]?.ToString() ?? "");
                                item.Tag = reader["BookID"];
                                item.ToolTipText = reasonText; // Show recommendation reason in tooltip
                                lstRecommended.Items.Add(item);
                            }
                        }
                    }
                }

                if (lstRecommended.Items.Count == 0)
                {
                    ListViewItem noBooks = new ListViewItem("No books available");
                    noBooks.SubItems.Add("");
                    noBooks.SubItems.Add("");
                    noBooks.SubItems.Add("");
                    noBooks.SubItems.Add("");
                    lstRecommended.Items.Add(noBooks);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recommended books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAllBooks()
        {
            if (lstBooks == null) return;

            lstBooks.Items.Clear();
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT BookID, Title, Author, Category, StudyField, AvailableCount, TotalCount FROM Books ORDER BY Title";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListViewItem item = new ListViewItem(reader["BookID"].ToString());
                                item.SubItems.Add(reader["Title"]?.ToString() ?? "");
                                item.SubItems.Add(reader["Author"]?.ToString() ?? "");
                                item.SubItems.Add(reader["Category"]?.ToString() ?? "");
                                item.SubItems.Add(reader["StudyField"]?.ToString() ?? "");
                                item.SubItems.Add(reader["AvailableCount"].ToString());
                                item.SubItems.Add(reader["TotalCount"].ToString());
                                item.Tag = reader["BookID"];
                                lstBooks.Items.Add(item);
                            }
                        }
                    }
                }

                if (lstBooks.Items.Count == 0)
                {
                    ListViewItem noBooks = new ListViewItem("No books found");
                    noBooks.SubItems.Add("");
                    noBooks.SubItems.Add("");
                    noBooks.SubItems.Add("");
                    noBooks.SubItems.Add("");
                    noBooks.SubItems.Add("");
                    noBooks.SubItems.Add("");
                    lstBooks.Items.Add(noBooks);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchBooks(string searchTerm)
        {
            if (lstBooks == null || string.IsNullOrWhiteSpace(searchTerm)) 
            {
                LoadAllBooks();
                return;
            }

            lstBooks.Items.Clear();
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"SELECT BookID, Title, Author, Category, StudyField, AvailableCount, TotalCount 
                                   FROM Books 
                                   WHERE Title LIKE @search OR Author LIKE @search OR Category LIKE @search OR StudyField LIKE @search
                                   ORDER BY Title";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@search", $"%{searchTerm}%");
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListViewItem item = new ListViewItem(reader["BookID"].ToString());
                                item.SubItems.Add(reader["Title"]?.ToString() ?? "");
                                item.SubItems.Add(reader["Author"]?.ToString() ?? "");
                                item.SubItems.Add(reader["Category"]?.ToString() ?? "");
                                item.SubItems.Add(reader["StudyField"]?.ToString() ?? "");
                                item.SubItems.Add(reader["AvailableCount"].ToString());
                                item.SubItems.Add(reader["TotalCount"].ToString());
                                item.Tag = reader["BookID"];
                                lstBooks.Items.Add(item);
                            }
                        }
                    }
                }

                if (lstBooks.Items.Count == 0)
                {
                    ListViewItem noBooks = new ListViewItem($"No books found for '{searchTerm}'");
                    noBooks.SubItems.Add("");
                    noBooks.SubItems.Add("");
                    noBooks.SubItems.Add("");
                    noBooks.SubItems.Add("");
                    noBooks.SubItems.Add("");
                    noBooks.SubItems.Add("");
                    lstBooks.Items.Add(noBooks);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReserveSelectedBook()
        {
            if (lstBooks?.SelectedItems.Count > 0)
            {
                var selectedItem = lstBooks.SelectedItems[0];
                if (selectedItem.Tag != null && int.TryParse(selectedItem.Tag.ToString(), out int bookId))
                {
                    try
                    {
                        using (var connection = DatabaseHelper.GetConnection())
                        {
                            connection.Open();
                            
                            // Check if book is available
                            string checkQuery = "SELECT AvailableCount FROM Books WHERE BookID = @bookId";
                            using (var checkCommand = new SqlCommand(checkQuery, connection))
                            {
                                checkCommand.Parameters.AddWithValue("@bookId", bookId);
                                var availableCount = checkCommand.ExecuteScalar();
                                
                                if (availableCount != null && (int)availableCount > 0)
                                {
                                    // Add reservation
                                    string reserveQuery = @"INSERT INTO Reservations (UserID, BookID, ReservationDate, Status) 
                                                          VALUES (@userId, @bookId, @date, @status)";
                                    using (var reserveCommand = new SqlCommand(reserveQuery, connection))
                                    {
                                        reserveCommand.Parameters.AddWithValue("@userId", currentUser.UserID);
                                        reserveCommand.Parameters.AddWithValue("@bookId", bookId);
                                        reserveCommand.Parameters.AddWithValue("@date", DateTime.Now);
                                        reserveCommand.Parameters.AddWithValue("@status", "Active");
                                        reserveCommand.ExecuteNonQuery();
                                    }
                                    
                                    // Update available count
                                    string updateQuery = "UPDATE Books SET AvailableCount = AvailableCount - 1 WHERE BookID = @bookId";
                                    using (var updateCommand = new SqlCommand(updateQuery, connection))
                                    {
                                        updateCommand.Parameters.AddWithValue("@bookId", bookId);
                                        updateCommand.ExecuteNonQuery();
                                    }
                                    
                                    MessageBox.Show("Book reserved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadAllBooks(); // Refresh the list
                                    LoadRecommendedBooks(); // Refresh recommendations
                                }
                                else
                                {
                                    MessageBox.Show("This book is not available for reservation.", "Not Available", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reserving book: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a book to reserve.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LikeSelectedBook()
        {
            if (lstBooks?.SelectedItems.Count > 0)
            {
                var selectedItem = lstBooks.SelectedItems[0];
                if (selectedItem.Tag != null && int.TryParse(selectedItem.Tag.ToString(), out int bookId))
                {
                    try
                    {
                        using (var connection = DatabaseHelper.GetConnection())
                        {
                            connection.Open();
                            
                            // Check if already liked
                            string checkQuery = "SELECT COUNT(*) FROM Likes WHERE UserID = @userId AND BookID = @bookId";
                            using (var checkCommand = new SqlCommand(checkQuery, connection))
                            {
                                checkCommand.Parameters.AddWithValue("@userId", currentUser.UserID);
                                checkCommand.Parameters.AddWithValue("@bookId", bookId);
                                var count = (int)checkCommand.ExecuteScalar();
                                
                                if (count == 0)
                                {
                                    // Add like
                                    string likeQuery = @"INSERT INTO Likes (UserID, BookID, LikeDate) 
                                                       VALUES (@userId, @bookId, @date)";
                                    using (var likeCommand = new SqlCommand(likeQuery, connection))
                                    {
                                        likeCommand.Parameters.AddWithValue("@userId", currentUser.UserID);
                                        likeCommand.Parameters.AddWithValue("@bookId", bookId);
                                        likeCommand.Parameters.AddWithValue("@date", DateTime.Now);
                                        likeCommand.ExecuteNonQuery();
                                    }
                                    
                                    MessageBox.Show("Book liked successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadRecommendedBooks(); // Refresh recommendations to include newly liked book
                                }
                                else
                                {
                                    MessageBox.Show("You have already liked this book.", "Already Liked", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error liking book: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a book to like.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private UserStatistics GetUserStatistics()
        {
            UserStatistics stats = new UserStatistics();
            
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            (SELECT COUNT(*) FROM Reservations WHERE UserID = @userId) AS ReservedBooks,
                            (SELECT COUNT(*) FROM Reservations WHERE UserID = @userId AND Status = 'Active') AS ActiveReservations,
                            (SELECT COUNT(*) FROM Likes WHERE UserID = @userId) AS LikedBooks,
                            (SELECT TOP 1 Category FROM Books b
                             INNER JOIN Likes l ON b.BookID = l.BookID
                             WHERE l.UserID = @userId
                             GROUP BY Category
                             ORDER BY COUNT(*) DESC) AS FavoriteCategory,
                            (SELECT COUNT(*) FROM Books WHERE StudyField = @field AND AvailableCount > 0) AS BooksInField,
                            (SELECT COUNT(*) FROM Books WHERE AvailableCount > 0) AS TotalAvailableBooks
                        ";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", currentUser.UserID);
                        command.Parameters.AddWithValue("@field", currentUser.StudyField ?? "General");
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                stats.ReservedBooks = reader.GetInt32(0);
                                stats.ActiveReservations = reader.GetInt32(1);
                                stats.LikedBooks = reader.GetInt32(2);
                                stats.FavoriteCategory = reader.IsDBNull(3) ? null : reader.GetString(3);
                                stats.BooksInField = reader.GetInt32(4);
                                stats.TotalAvailableBooks = reader.GetInt32(5);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving statistics: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            return stats;
        }

        private void RefreshProfileTab(TabPage tab)
        {
            // Clear existing controls in the tab
            tab.Controls.Clear();
            
            // Recreate the profile and statistics sections
            CreateProfileTab(tab);
            
            // Optionally, show a message or perform additional actions
            MessageBox.Show("Profile statistics refreshed.", "Refreshed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}