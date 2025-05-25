using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp.Models;
using WinFormsApp.Data;
using Microsoft.Data.SqlClient;

namespace WinFormsApp.Forms
{
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

            lstRecommended = new ListView();
            lstRecommended.Location = new Point(20, 100);
            lstRecommended.Size = new Size(820, 400);
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

            Button btnRefreshHome = new Button();
            btnRefreshHome.Text = "Refresh";
            btnRefreshHome.Location = new Point(20, 520);
            btnRefreshHome.Size = new Size(100, 30);
            btnRefreshHome.BackColor = Color.DodgerBlue;
            btnRefreshHome.ForeColor = Color.White;
            btnRefreshHome.Click += (s, e) => LoadRecommendedBooks();

            tab.Controls.AddRange(new Control[] { lblWelcome, lblRecommended, lstRecommended, btnRefreshHome });
        }

        private void CreateLibraryTab(TabPage tab)
        {
            Label lblSearch = new Label();
            lblSearch.Text = "Search Books:";
            lblSearch.Location = new Point(20, 20);
            lblSearch.Size = new Size(100, 20);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(130, 18);
            txtSearch.Size = new Size(300, 25);

            Button btnSearch = new Button();
            btnSearch.Text = "Search";
            btnSearch.Location = new Point(450, 16);
            btnSearch.Size = new Size(80, 30);
            btnSearch.BackColor = Color.Green;
            btnSearch.ForeColor = Color.White;
            btnSearch.Click += (s, e) => SearchBooks(txtSearch.Text);

            Button btnShowAll = new Button();
            btnShowAll.Text = "Show All";
            btnShowAll.Location = new Point(540, 16);
            btnShowAll.Size = new Size(80, 30);
            btnShowAll.BackColor = Color.Blue;
            btnShowAll.ForeColor = Color.White;
            btnShowAll.Click += (s, e) => LoadAllBooks();

            lstBooks = new ListView();
            lstBooks.Location = new Point(20, 60);
            lstBooks.Size = new Size(820, 450);
            lstBooks.View = View.Details;
            lstBooks.FullRowSelect = true;
            lstBooks.GridLines = true;
            lstBooks.Columns.AddRange(new ColumnHeader[] {
                new ColumnHeader() { Text = "ID", Width = 50 },
                new ColumnHeader() { Text = "Title", Width = 200 },
                new ColumnHeader() { Text = "Author", Width = 150 },
                new ColumnHeader() { Text = "Category", Width = 100 },
                new ColumnHeader() { Text = "Field", Width = 100 },
                new ColumnHeader() { Text = "Available", Width = 80 },
                new ColumnHeader() { Text = "Total", Width = 80 }
            });

            Button btnReserve = new Button();
            btnReserve.Text = "Reserve Selected";
            btnReserve.Location = new Point(20, 530);
            btnReserve.Size = new Size(120, 35);
            btnReserve.BackColor = Color.Orange;
            btnReserve.ForeColor = Color.White;
            btnReserve.Click += (s, e) => ReserveSelectedBook();

            Button btnLike = new Button();
            btnLike.Text = "Like Selected";
            btnLike.Location = new Point(160, 530);
            btnLike.Size = new Size(120, 35);
            btnLike.BackColor = Color.Red;
            btnLike.ForeColor = Color.White;
            btnLike.Click += (s, e) => LikeSelectedBook();

            Button btnRefresh = new Button();
            btnRefresh.Text = "Refresh";
            btnRefresh.Location = new Point(300, 530);
            btnRefresh.Size = new Size(100, 35);
            btnRefresh.BackColor = Color.DodgerBlue;
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Click += (s, e) => LoadAllBooks();

            tab.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, btnSearch, btnShowAll, lstBooks, btnReserve, btnLike, btnRefresh
            });

            // Load all books initially
            LoadAllBooks();
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
        }

        private void LoadRecommendedBooks()
        {
            if (lstRecommended == null) return;

            lstRecommended.Items.Clear();
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"SELECT TOP 10 BookID, Title, Author, Category, StudyField, AvailableCount, TotalCount 
                                   FROM Books 
                                   WHERE AvailableCount > 0 AND (StudyField = @field OR StudyField = 'General')
                                   ORDER BY AvailableCount DESC";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@field", currentUser.StudyField ?? "General");
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListViewItem item = new ListViewItem(reader["Title"]?.ToString() ?? "");
                                item.SubItems.Add(reader["Author"]?.ToString() ?? "");
                                item.SubItems.Add(reader["Category"]?.ToString() ?? "");
                                item.SubItems.Add(reader["AvailableCount"].ToString());
                                item.SubItems.Add(reader["StudyField"]?.ToString() ?? "");
                                item.Tag = reader["BookID"];
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
    }
}