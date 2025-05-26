using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WinFormsApp.Models;
using WinFormsApp.Data;
using Microsoft.Data.SqlClient;

namespace WinFormsApp.Forms
{
    // AdminStatistics class
    public class AdminStatistics
    {
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int ReservedBooks { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int BlockedUsers { get; set; }
        public int TotalReservations { get; set; }
        public int TotalLikes { get; set; }
    }    public partial class AdminDashboard : Form
    {
        private User currentAdmin;
        private TabControl tabControl = null!;
        private Label lblTotalBooksValue = null!;
        private Label lblAvailableBooksValue = null!;
        private Label lblReservedBooksValue = null!;
        private Label lblTotalUsersValue = null!;

        public AdminDashboard(User admin)
        {
            currentAdmin = admin;
            InitializeComponent();
        }        private void InitializeComponent()
        {
            this.Text = $"IHEC Digital Library - Admin Panel - {currentAdmin.FirstName}";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(248, 249, 250);

            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            tabControl.ItemSize = new Size(150, 40);
            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            
            // Custom tab drawing for modern admin look
            tabControl.DrawItem += (s, e) => {
                if (s is TabControl tc)
                {
                    Rectangle r = e.Bounds;
                    string title = tc.TabPages[e.Index].Text;
                    
                    using (Brush brush = new SolidBrush(e.Index == tc.SelectedIndex ? 
                        Color.FromArgb(220, 53, 69) : Color.FromArgb(200, 200, 200)))
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

            // Dashboard Tab with admin icon
            TabPage dashTab = new TabPage("📊 Dashboard");
            dashTab.BackColor = Color.White;
            CreateDashboardTab(dashTab);

            // Users Tab
            TabPage usersTab = new TabPage("👥 Manage Users");
            usersTab.BackColor = Color.White;
            CreateUsersTab(usersTab);

            // Books Tab
            TabPage booksTab = new TabPage("📚 Manage Books");
            booksTab.BackColor = Color.White;
            CreateBooksTab(booksTab);

            tabControl.TabPages.AddRange(new TabPage[] { dashTab, usersTab, booksTab });
            this.Controls.Add(tabControl);
        }        private void CreateDashboardTab(TabPage tab)
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

            // Header section
            Panel headerPanel = new Panel();
            headerPanel.Location = new Point(20, 20);
            headerPanel.Size = new Size(900, 80);
            headerPanel.BackColor = Color.White;
            
            // Add shadow effect
            headerPanel.Paint += (s, e) => {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(30, 0, 0, 0)), 
                    3, 3, headerPanel.Width, headerPanel.Height);
                e.Graphics.FillRectangle(Brushes.White, 0, 0, headerPanel.Width - 3, headerPanel.Height - 3);
            };

            Label lblTitle = new Label();
            lblTitle.Text = "📊 Library Management Dashboard";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(220, 53, 69);
            lblTitle.Location = new Point(30, 15);
            lblTitle.Size = new Size(500, 35);
            lblTitle.BackColor = Color.Transparent;

            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Real-time statistics and system overview";
            lblSubtitle.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblSubtitle.ForeColor = Color.FromArgb(120, 120, 120);
            lblSubtitle.Location = new Point(30, 50);
            lblSubtitle.Size = new Size(400, 20);
            lblSubtitle.BackColor = Color.Transparent;

            // Add Logout Button to header
            Button btnLogout = new Button();
            btnLogout.Text = "🚪 Logout";
            btnLogout.Location = new Point(750, 25);
            btnLogout.Size = new Size(120, 35);
            btnLogout.Font = new Font("Segoe UI", 11, FontStyle.Bold);
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

            headerPanel.Controls.AddRange(new Control[] { lblTitle, lblSubtitle, btnLogout });

            // Modern statistics cards container
            Panel statsContainer = new Panel();
            statsContainer.Location = new Point(20, 120);
            statsContainer.Size = new Size(900, 200);
            statsContainer.BackColor = Color.Transparent;

            // Create modern stat cards
            CreateModernStatCard(statsContainer, "📚 Total Books", "0", 0, 0, Color.FromArgb(70, 130, 180), ref lblTotalBooksValue);
            CreateModernStatCard(statsContainer, "✅ Available Books", "0", 220, 0, Color.FromArgb(34, 139, 34), ref lblAvailableBooksValue);
            CreateModernStatCard(statsContainer, "📋 Reserved Books", "0", 440, 0, Color.FromArgb(255, 140, 0), ref lblReservedBooksValue);
            CreateModernStatCard(statsContainer, "👥 Total Users", "0", 660, 0, Color.FromArgb(102, 51, 153), ref lblTotalUsersValue);

            // Action buttons with modern styling
            Button btnRefreshStats = new Button();
            btnRefreshStats.Text = "🔄 Refresh Statistics";
            btnRefreshStats.Location = new Point(20, 340);
            btnRefreshStats.Size = new Size(200, 50);
            btnRefreshStats.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnRefreshStats.BackColor = Color.FromArgb(70, 130, 180);
            btnRefreshStats.ForeColor = Color.White;
            btnRefreshStats.FlatStyle = FlatStyle.Flat;
            btnRefreshStats.FlatAppearance.BorderSize = 0;
            btnRefreshStats.Cursor = Cursors.Hand;
            btnRefreshStats.Click += (s, e) => LoadStatistics();
            
            btnRefreshStats.MouseEnter += (s, e) => btnRefreshStats.BackColor = Color.FromArgb(100, 149, 237);
            btnRefreshStats.MouseLeave += (s, e) => btnRefreshStats.BackColor = Color.FromArgb(70, 130, 180);

            Button btnManageUsers = new Button();
            btnManageUsers.Text = "👥 Manage Users";
            btnManageUsers.Location = new Point(240, 340);
            btnManageUsers.Size = new Size(180, 50);
            btnManageUsers.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnManageUsers.BackColor = Color.FromArgb(220, 53, 69);
            btnManageUsers.ForeColor = Color.White;
            btnManageUsers.FlatStyle = FlatStyle.Flat;
            btnManageUsers.FlatAppearance.BorderSize = 0;
            btnManageUsers.Cursor = Cursors.Hand;
            btnManageUsers.Click += (s, e) => {
                // Switch to Users tab
                if (tabControl != null && tabControl.TabPages.Count > 1)
                    tabControl.SelectedIndex = 1;
            };
            
            btnManageUsers.MouseEnter += (s, e) => btnManageUsers.BackColor = Color.FromArgb(225, 83, 97);
            btnManageUsers.MouseLeave += (s, e) => btnManageUsers.BackColor = Color.FromArgb(220, 53, 69);

            Button btnManageBooks = new Button();
            btnManageBooks.Text = "📚 Manage Books";
            btnManageBooks.Location = new Point(440, 340);
            btnManageBooks.Size = new Size(180, 50);
            btnManageBooks.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnManageBooks.BackColor = Color.FromArgb(34, 139, 34);
            btnManageBooks.ForeColor = Color.White;
            btnManageBooks.FlatStyle = FlatStyle.Flat;
            btnManageBooks.FlatAppearance.BorderSize = 0;
            btnManageBooks.Cursor = Cursors.Hand;
            btnManageBooks.Click += (s, e) => {
                // Switch to Books tab
                if (tabControl != null && tabControl.TabPages.Count > 2)
                    tabControl.SelectedIndex = 2;
            };
            
            btnManageBooks.MouseEnter += (s, e) => btnManageBooks.BackColor = Color.FromArgb(60, 179, 113);
            btnManageBooks.MouseLeave += (s, e) => btnManageBooks.BackColor = Color.FromArgb(34, 139, 34);

            // Add Exit Application button
            Button btnExitApp = new Button();
            btnExitApp.Text = "❌ Exit Application";
            btnExitApp.Location = new Point(640, 340);
            btnExitApp.Size = new Size(180, 50);
            btnExitApp.Font = new Font("Segoe UI", 12, FontStyle.Bold);
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

            tab.Controls.AddRange(new Control[] { headerPanel, statsContainer, btnRefreshStats, btnManageUsers, btnManageBooks, btnExitApp });
            
            // Load initial statistics
            LoadStatistics();
        }

        private void CreateModernStatCard(Panel parent, string title, string value, int x, int y, Color accentColor, ref Label labelValueRef)
        {
            Panel card = new Panel();
            card.Location = new Point(x, y);
            card.Size = new Size(200, 120);
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.None;
            
            // Add shadow effect to card
            card.Paint += (s, e) => {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(30, 0, 0, 0)), 
                    3, 3, card.Width, card.Height);
                e.Graphics.FillRectangle(Brushes.White, 0, 0, card.Width - 3, card.Height - 3);
                
                // Add accent color bar at top
                e.Graphics.FillRectangle(new SolidBrush(accentColor), 0, 0, card.Width - 3, 5);
            };

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(52, 73, 94);
            lblTitle.Location = new Point(15, 20);
            lblTitle.Size = new Size(170, 25);
            lblTitle.BackColor = Color.Transparent;

            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblValue.ForeColor = accentColor;
            lblValue.Location = new Point(15, 50);
            lblValue.Size = new Size(170, 40);
            lblValue.BackColor = Color.Transparent;
            lblValue.TextAlign = ContentAlignment.MiddleLeft;

            Label lblDescription = new Label();
            lblDescription.Text = "Real-time count";
            lblDescription.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblDescription.ForeColor = Color.FromArgb(120, 120, 120);
            lblDescription.Location = new Point(15, 95);
            lblDescription.Size = new Size(170, 15);
            lblDescription.BackColor = Color.Transparent;

            card.Controls.AddRange(new Control[] { lblTitle, lblValue, lblDescription });
            parent.Controls.Add(card);
            
            // Store reference to value label for updates
            labelValueRef = lblValue;
        }

        private void CreateStatBox(Panel parent, string title, string value, int x, int y, Color color, ref Label labelValueRef)
        {
            Panel box = new Panel();
            box.Location = new Point(x, y);
            box.Size = new Size(180, 120);
            box.BackColor = color;

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Arial", 12, FontStyle.Bold);
            lblTitle.Location = new Point(10, 20);
            lblTitle.Size = new Size(160, 25);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.ForeColor = Color.White;
            lblValue.Font = new Font("Arial", 24, FontStyle.Bold);
            lblValue.Location = new Point(10, 50);
            lblValue.Size = new Size(160, 40);
            lblValue.TextAlign = ContentAlignment.MiddleCenter;

            // Store reference to the value label
            labelValueRef = lblValue;

            box.Controls.AddRange(new Control[] { lblTitle, lblValue });
            parent.Controls.Add(box);
        }

        private void CreateUsersTab(TabPage tab)
        {
            ListView lstUsers = new ListView();
            lstUsers.Location = new Point(20, 20);
            lstUsers.Size = new Size(1000, 500);
            lstUsers.View = View.Details;
            lstUsers.FullRowSelect = true;
            lstUsers.GridLines = true;
            lstUsers.Columns.AddRange(new ColumnHeader[] {
                new ColumnHeader() { Text = "ID", Width = 50 },
                new ColumnHeader() { Text = "Name", Width = 150 },
                new ColumnHeader() { Text = "Email", Width = 200 },
                new ColumnHeader() { Text = "Study Level", Width = 100 },
                new ColumnHeader() { Text = "Study Field", Width = 120 },
                new ColumnHeader() { Text = "Status", Width = 80 },
                new ColumnHeader() { Text = "Created", Width = 100 }
            });

            Button btnBlock = new Button();
            btnBlock.Text = "Block User";
            btnBlock.Location = new Point(20, 540);
            btnBlock.Size = new Size(100, 35);
            btnBlock.BackColor = Color.Red;
            btnBlock.ForeColor = Color.White;
            btnBlock.Click += (s, e) => ToggleUserStatus(lstUsers, true);

            Button btnUnblock = new Button();
            btnUnblock.Text = "Unblock User";
            btnUnblock.Location = new Point(140, 540);
            btnUnblock.Size = new Size(100, 35);
            btnUnblock.BackColor = Color.Green;
            btnUnblock.ForeColor = Color.White;
            btnUnblock.Click += (s, e) => ToggleUserStatus(lstUsers, false);

            Button btnDelete = new Button();
            btnDelete.Text = "Delete User";
            btnDelete.Location = new Point(260, 540);
            btnDelete.Size = new Size(100, 35);
            btnDelete.BackColor = Color.DarkRed;
            btnDelete.ForeColor = Color.White;
            btnDelete.Click += (s, e) => DeleteUser(lstUsers);

            Button btnRefresh = new Button();
            btnRefresh.Text = "Refresh";
            btnRefresh.Location = new Point(380, 540);
            btnRefresh.Size = new Size(100, 35);
            btnRefresh.BackColor = Color.DodgerBlue;
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Click += (s, e) => LoadUsers(lstUsers);

            tab.Controls.AddRange(new Control[] { lstUsers, btnBlock, btnUnblock, btnDelete, btnRefresh });
            
            // Initial load
            LoadUsers(lstUsers);
        }

        private void LoadUsers(ListView listView)
        {
            listView.Items.Clear();
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT UserID, FirstName, LastName, Email, StudyLevel, StudyField, IsBlocked, CreatedDate FROM Users ORDER BY LastName, FirstName";
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListViewItem item = new ListViewItem(reader["UserID"].ToString());
                                item.SubItems.Add($"{reader["FirstName"]} {reader["LastName"]}");
                                item.SubItems.Add(reader["Email"]?.ToString() ?? "");
                                item.SubItems.Add(reader["StudyLevel"]?.ToString() ?? "N/A");
                                item.SubItems.Add(reader["StudyField"]?.ToString() ?? "N/A");
                                item.SubItems.Add(Convert.ToBoolean(reader["IsBlocked"]) ? "Blocked" : "Active");
                                item.SubItems.Add(Convert.ToDateTime(reader["CreatedDate"]).ToString("MM/dd/yyyy"));
                                item.Tag = reader["UserID"];
                                
                                // Color blocked users differently
                                if (Convert.ToBoolean(reader["IsBlocked"]))
                                {
                                    item.BackColor = Color.LightCoral;
                                }
                                
                                listView.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToggleUserStatus(ListView listView, bool block)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a user to modify.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedItem = listView.SelectedItems[0];
            if (selectedItem.Tag != null && int.TryParse(selectedItem.Tag.ToString(), out int userId))
            {
                if (userId == currentAdmin.UserID)
                {
                    MessageBox.Show("You cannot modify your own account status.", "Invalid Operation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    using (var connection = DatabaseHelper.GetConnection())
                    {
                        connection.Open();
                        string query = "UPDATE Users SET IsBlocked = @blocked WHERE UserID = @userId";
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@blocked", block);
                            command.Parameters.AddWithValue("@userId", userId);
                            command.ExecuteNonQuery();
                        }
                    }
                    
                    string action = block ? "blocked" : "unblocked";
                    MessageBox.Show($"User {action} successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers(listView);
                    LoadStatistics(); // Refresh statistics after user status change
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating user status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteUser(ListView listView)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a user to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedItem = listView.SelectedItems[0];
            if (selectedItem.Tag != null && int.TryParse(selectedItem.Tag.ToString(), out int userId))
            {
                if (userId == currentAdmin.UserID)
                {
                    MessageBox.Show("You cannot delete your own account.", "Invalid Operation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this user? This action cannot be undone.", 
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (var connection = DatabaseHelper.GetConnection())
                        {
                            connection.Open();
                            
                            // Delete related records first (to avoid foreign key constraints)
                            string deleteLikes = "DELETE FROM Likes WHERE UserID = @userId";
                            using (var command = new SqlCommand(deleteLikes, connection))
                            {
                                command.Parameters.AddWithValue("@userId", userId);
                                command.ExecuteNonQuery();
                            }
                            
                            string deleteReservations = "DELETE FROM Reservations WHERE UserID = @userId";
                            using (var command = new SqlCommand(deleteReservations, connection))
                            {
                                command.Parameters.AddWithValue("@userId", userId);
                                command.ExecuteNonQuery();
                            }
                            
                            // Delete the user
                            string deleteUser = "DELETE FROM Users WHERE UserID = @userId";
                            using (var command = new SqlCommand(deleteUser, connection))
                            {
                                command.Parameters.AddWithValue("@userId", userId);
                                command.ExecuteNonQuery();
                            }
                        }
                        
                        MessageBox.Show("User deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUsers(listView);
                        LoadStatistics(); // Refresh statistics after user deletion
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void CreateBooksTab(TabPage tab)
        {
            // Add Book Section
            GroupBox grpAddBook = new GroupBox();
            grpAddBook.Text = "Add New Book";
            grpAddBook.Location = new Point(20, 20);
            grpAddBook.Size = new Size(400, 350);

            Label lblTitle = new Label();
            lblTitle.Text = "Title:";
            lblTitle.Location = new Point(15, 30);
            lblTitle.Size = new Size(60, 20);            TextBox txtTitle = new TextBox();
            txtTitle.Location = new Point(80, 28);
            txtTitle.Size = new Size(300, 25);
            txtTitle.Enabled = true;
            txtTitle.ReadOnly = false;
            txtTitle.TabIndex = 1;

            Label lblAuthor = new Label();
            lblAuthor.Text = "Author:";
            lblAuthor.Location = new Point(15, 65);
            lblAuthor.Size = new Size(60, 20);            TextBox txtAuthor = new TextBox();
            txtAuthor.Location = new Point(80, 63);
            txtAuthor.Size = new Size(300, 25);
            txtAuthor.Enabled = true;
            txtAuthor.ReadOnly = false;
            txtAuthor.TabIndex = 2;

            Label lblISBN = new Label();
            lblISBN.Text = "ISBN:";
            lblISBN.Location = new Point(15, 100);
            lblISBN.Size = new Size(60, 20);            TextBox txtISBN = new TextBox();
            txtISBN.Location = new Point(80, 98);
            txtISBN.Size = new Size(300, 25);
            txtISBN.Enabled = true;
            txtISBN.ReadOnly = false;
            txtISBN.TabIndex = 3;

            Label lblCategory = new Label();
            lblCategory.Text = "Category:";
            lblCategory.Location = new Point(15, 135);
            lblCategory.Size = new Size(60, 20);            TextBox txtCategory = new TextBox();
            txtCategory.Location = new Point(80, 133);
            txtCategory.Size = new Size(300, 25);
            txtCategory.Enabled = true;
            txtCategory.ReadOnly = false;
            txtCategory.TabIndex = 4;

            Label lblField = new Label();
            lblField.Text = "Field:";
            lblField.Location = new Point(15, 170);
            lblField.Size = new Size(60, 20);

            ComboBox cmbField = new ComboBox();
            cmbField.Items.AddRange(new string[] { "BI", "Finance", "Marketing", "Accounting", "Management", "Big Data", "General", "Other" });
            cmbField.Location = new Point(80, 168);
            cmbField.Size = new Size(300, 25);
            cmbField.DropDownStyle = ComboBoxStyle.DropDownList;

            Label lblCover = new Label();
            lblCover.Text = "Cover URL:";
            lblCover.Location = new Point(15, 205);
            lblCover.Size = new Size(60, 20);            TextBox txtCoverUrl = new TextBox();
            txtCoverUrl.Location = new Point(80, 203);
            txtCoverUrl.Size = new Size(300, 25);
            txtCoverUrl.PlaceholderText = "Enter cover image URL (optional)";
            txtCoverUrl.Enabled = true;
            txtCoverUrl.ReadOnly = false;
            txtCoverUrl.TabIndex = 5;

            Label lblCount = new Label();
            lblCount.Text = "Count:";
            lblCount.Location = new Point(15, 240);
            lblCount.Size = new Size(60, 20);

            NumericUpDown numCount = new NumericUpDown();
            numCount.Location = new Point(80, 238);
            numCount.Size = new Size(100, 25);
            numCount.Minimum = 1;
            numCount.Maximum = 100;
            numCount.Value = 1;

            Button btnAddBook = new Button();
            btnAddBook.Text = "Add Book";
            btnAddBook.Location = new Point(80, 285);
            btnAddBook.Size = new Size(120, 35);
            btnAddBook.BackColor = Color.Green;
            btnAddBook.ForeColor = Color.White;            // Books List
            ListView lstBooks = new ListView();
            lstBooks.Location = new Point(450, 20);
            lstBooks.Size = new Size(700, 500);
            lstBooks.View = View.Details;
            lstBooks.FullRowSelect = true;
            lstBooks.GridLines = true;
            lstBooks.MultiSelect = false; // Ensure single selection only
            lstBooks.Columns.AddRange(new ColumnHeader[] {
                new ColumnHeader() { Text = "ID", Width = 50 },
                new ColumnHeader() { Text = "Title", Width = 200 },
                new ColumnHeader() { Text = "Author", Width = 150 },
                new ColumnHeader() { Text = "Category", Width = 100 },
                new ColumnHeader() { Text = "Field", Width = 100 },
                new ColumnHeader() { Text = "Available", Width = 70 },
                new ColumnHeader() { Text = "Total", Width = 70 }
            });

            btnAddBook.Click += (s, e) => {
                AddBook(txtTitle.Text, txtAuthor.Text, txtISBN.Text, 
                    txtCategory.Text, cmbField.SelectedItem?.ToString(), txtCoverUrl.Text, (int)numCount.Value, lstBooks);
                
                // Clear the form after successful addition
                txtTitle.Clear();
                txtAuthor.Clear();
                txtISBN.Clear();
                txtCategory.Clear();
                cmbField.SelectedIndex = -1;
                txtCoverUrl.Clear();
                numCount.Value = 1;
            };

            grpAddBook.Controls.AddRange(new Control[] {
                lblTitle, txtTitle, lblAuthor, txtAuthor, lblISBN, txtISBN,
                lblCategory, txtCategory, lblField, cmbField, lblCover, txtCoverUrl, 
                lblCount, numCount, btnAddBook
            });

            Button btnRefreshBooks = new Button();
            btnRefreshBooks.Text = "Refresh Books";
            btnRefreshBooks.Location = new Point(450, 540);
            btnRefreshBooks.Size = new Size(120, 35);
            btnRefreshBooks.BackColor = Color.DodgerBlue;
            btnRefreshBooks.ForeColor = Color.White;
            btnRefreshBooks.Click += (s, e) => LoadBooks(lstBooks);            Button btnDeleteBook = new Button();
            btnDeleteBook.Text = "Delete Selected";
            btnDeleteBook.Location = new Point(590, 540);
            btnDeleteBook.Size = new Size(120, 35);
            btnDeleteBook.BackColor = Color.Red;
            btnDeleteBook.ForeColor = Color.White;
            btnDeleteBook.Click += (s, e) => DeleteSelectedBook(lstBooks);

            tab.Controls.AddRange(new Control[] { grpAddBook, lstBooks, btnRefreshBooks, btnDeleteBook });
            
            // Initial load
            LoadBooks(lstBooks);
        }

        private void AddBook(string title, string author, string isbn, string category, string? field, string coverUrl, int count, ListView lstBooks)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(author))
            {
                MessageBox.Show("Please enter both title and author.", "Validation Error");
                return;
            }

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"INSERT INTO Books (Title, Author, ISBN, Category, StudyField, CoverImage, AvailableCount, TotalCount)
                                    VALUES (@title, @author, @isbn, @category, @field, @cover, @count, @count)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@title", title);
                        command.Parameters.AddWithValue("@author", author);
                        command.Parameters.AddWithValue("@isbn", isbn ?? "");
                        command.Parameters.AddWithValue("@category", category ?? "");
                        command.Parameters.AddWithValue("@field", field ?? "General");
                        command.Parameters.AddWithValue("@cover", string.IsNullOrWhiteSpace(coverUrl) ? (object)DBNull.Value : coverUrl.Trim());
                        command.Parameters.AddWithValue("@count", count);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Book added successfully!", "Success");
                        LoadBooks(lstBooks); // Refresh the book list
                        LoadStatistics(); // Refresh statistics after adding book
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding book: " + ex.Message, "Error");
            }
        }        private void LoadBooks(ListView listView)
        {
            listView.Items.Clear();
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM Books ORDER BY Title";
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
                                item.Tag = reader["BookID"]; // Store BookID in Tag for easy access
                                listView.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStatistics()
        {
            try
            {
                var stats = GetAdminStatistics();
                
                // Update the statistics labels with real data
                if (lblTotalBooksValue != null)
                    lblTotalBooksValue.Text = stats.TotalBooks.ToString();
                    
                if (lblAvailableBooksValue != null)
                    lblAvailableBooksValue.Text = stats.AvailableBooks.ToString();
                    
                if (lblReservedBooksValue != null)
                    lblReservedBooksValue.Text = stats.ReservedBooks.ToString();
                    
                if (lblTotalUsersValue != null)
                    lblTotalUsersValue.Text = stats.TotalUsers.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private AdminStatistics GetAdminStatistics()
        {
            AdminStatistics stats = new AdminStatistics();
            
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            (SELECT COUNT(*) FROM Books) AS TotalBooks,
                            (SELECT ISNULL(SUM(AvailableCount), 0) FROM Books) AS AvailableBooks,
                            (SELECT COUNT(*) FROM Reservations WHERE Status = 'Active') AS ReservedBooks,
                            (SELECT COUNT(*) FROM Users) AS TotalUsers,
                            (SELECT COUNT(*) FROM Users WHERE IsBlocked = 'False') AS ActiveUsers,
                            (SELECT COUNT(*) FROM Users WHERE IsBlocked = 'True') AS BlockedUsers,
                            (SELECT COUNT(*) FROM Reservations) AS TotalReservations,
                            (SELECT COUNT(*) FROM Likes) AS TotalLikes
                        ";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                stats.TotalBooks = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                                stats.AvailableBooks = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                                stats.ReservedBooks = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                                stats.TotalUsers = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                                stats.ActiveUsers = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                                stats.BlockedUsers = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                                stats.TotalReservations = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                                stats.TotalLikes = reader.IsDBNull(7) ? 0 : reader.GetInt32(7);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving admin statistics: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            return stats;
        }        private void DeleteSelectedBook(ListView listView)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a book to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedItem = listView.SelectedItems[0];
            
            if (selectedItem.Tag == null || !int.TryParse(selectedItem.Tag.ToString(), out int bookId))
            {
                MessageBox.Show("Invalid book selection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string bookTitle = selectedItem.SubItems[1].Text;
            var result = MessageBox.Show($"Are you sure you want to delete the book '{bookTitle}'?\n\nThis will also delete all related reservations and likes. This action cannot be undone.", 
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                try
                {
                    using (var connection = DatabaseHelper.GetConnection())
                    {
                        connection.Open();
                        
                        // Delete related records first (to avoid foreign key constraints)
                        string deleteLikes = "DELETE FROM Likes WHERE BookID = @bookId";
                        using (var command = new SqlCommand(deleteLikes, connection))
                        {
                            command.Parameters.AddWithValue("@bookId", bookId);
                            command.ExecuteNonQuery();
                        }
                        
                        string deleteReservations = "DELETE FROM Reservations WHERE BookID = @bookId";
                        using (var command = new SqlCommand(deleteReservations, connection))
                        {
                            command.Parameters.AddWithValue("@bookId", bookId);
                            command.ExecuteNonQuery();
                        }
                        
                        // Delete the book
                        string deleteBook = "DELETE FROM Books WHERE BookID = @bookId";
                        using (var command = new SqlCommand(deleteBook, connection))
                        {
                            command.Parameters.AddWithValue("@bookId", bookId);
                            int rowsAffected = command.ExecuteNonQuery();
                            
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show($"Book '{bookTitle}' deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadBooks(listView); // Refresh the book list
                                LoadStatistics(); // Refresh statistics after deletion
                            }
                            else
                            {
                                MessageBox.Show("Book could not be deleted. It may have already been removed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting book: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}