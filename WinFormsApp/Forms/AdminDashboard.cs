using System;
using System.Drawing;
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
    }

    public partial class AdminDashboard : Form
    {
        private User currentAdmin;
        private Label lblTotalBooksValue = null!;
        private Label lblAvailableBooksValue = null!;
        private Label lblReservedBooksValue = null!;
        private Label lblTotalUsersValue = null!;

        public AdminDashboard(User admin)
        {
            currentAdmin = admin;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Admin Dashboard";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            TabControl tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;

            // Dashboard Tab
            TabPage dashTab = new TabPage("Dashboard");
            CreateDashboardTab(dashTab);

            // Users Tab
            TabPage usersTab = new TabPage("Manage Users");
            CreateUsersTab(usersTab);

            // Books Tab
            TabPage booksTab = new TabPage("Manage Books");
            CreateBooksTab(booksTab);

            tabControl.TabPages.AddRange(new TabPage[] { dashTab, usersTab, booksTab });
            this.Controls.Add(tabControl);
        }

        private void CreateDashboardTab(TabPage tab)
        {
            Label lblTitle = new Label();
            lblTitle.Text = "Library Statistics";
            lblTitle.Font = new Font("Arial", 18, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(300, 30);

            // Statistics panels
            Panel statsPanel = new Panel();
            statsPanel.Location = new Point(20, 70);
            statsPanel.Size = new Size(1000, 200);
            statsPanel.BorderStyle = BorderStyle.FixedSingle;

            // Create stat boxes with label references
            CreateStatBox(statsPanel, "Total Books", "0", 20, 20, Color.Blue, ref lblTotalBooksValue);
            CreateStatBox(statsPanel, "Available Books", "0", 220, 20, Color.Green, ref lblAvailableBooksValue);
            CreateStatBox(statsPanel, "Reserved Books", "0", 420, 20, Color.Orange, ref lblReservedBooksValue);
            CreateStatBox(statsPanel, "Total Users", "0", 620, 20, Color.Purple, ref lblTotalUsersValue);

            // Add refresh button
            Button btnRefreshStats = new Button();
            btnRefreshStats.Text = "Refresh Statistics";
            btnRefreshStats.Location = new Point(20, 290);
            btnRefreshStats.Size = new Size(150, 35);
            btnRefreshStats.BackColor = Color.DodgerBlue;
            btnRefreshStats.ForeColor = Color.White;
            btnRefreshStats.Click += (s, e) => LoadStatistics();

            tab.Controls.AddRange(new Control[] { lblTitle, statsPanel, btnRefreshStats });
            
            // Load initial statistics
            LoadStatistics();
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
            lblTitle.Size = new Size(60, 20);

            TextBox txtTitle = new TextBox();
            txtTitle.Location = new Point(80, 28);
            txtTitle.Size = new Size(300, 25);

            Label lblAuthor = new Label();
            lblAuthor.Text = "Author:";
            lblAuthor.Location = new Point(15, 65);
            lblAuthor.Size = new Size(60, 20);

            TextBox txtAuthor = new TextBox();
            txtAuthor.Location = new Point(80, 63);
            txtAuthor.Size = new Size(300, 25);

            Label lblISBN = new Label();
            lblISBN.Text = "ISBN:";
            lblISBN.Location = new Point(15, 100);
            lblISBN.Size = new Size(60, 20);

            TextBox txtISBN = new TextBox();
            txtISBN.Location = new Point(80, 98);
            txtISBN.Size = new Size(300, 25);

            Label lblCategory = new Label();
            lblCategory.Text = "Category:";
            lblCategory.Location = new Point(15, 135);
            lblCategory.Size = new Size(60, 20);

            TextBox txtCategory = new TextBox();
            txtCategory.Location = new Point(80, 133);
            txtCategory.Size = new Size(300, 25);

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
            lblCover.Size = new Size(60, 20);

            TextBox txtCoverUrl = new TextBox();
            txtCoverUrl.Location = new Point(80, 203);
            txtCoverUrl.Size = new Size(300, 25);
            txtCoverUrl.PlaceholderText = "Enter cover image URL (optional)";

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
            btnDeleteBook.Click += (s, e) => DeleteSelectedBook(lstBooks);            // Add a test button to verify database connection
            Button btnTestDB = new Button();
            btnTestDB.Text = "Test DB";
            btnTestDB.Location = new Point(730, 540);
            btnTestDB.Size = new Size(80, 35);
            btnTestDB.BackColor = Color.Orange;
            btnTestDB.ForeColor = Color.White;
            btnTestDB.Click += (s, e) => TestDatabaseConnection();

            // Add a button to validate database structure
            Button btnValidateDB = new Button();
            btnValidateDB.Text = "Check DB";
            btnValidateDB.Location = new Point(820, 540);
            btnValidateDB.Size = new Size(80, 35);
            btnValidateDB.BackColor = Color.Purple;
            btnValidateDB.ForeColor = Color.White;
            btnValidateDB.Click += (s, e) => ValidateDatabaseStructure();

            tab.Controls.AddRange(new Control[] { grpAddBook, lstBooks, btnRefreshBooks, btnDeleteBook, btnTestDB, btnValidateDB });
            
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
                            int bookCount = 0;
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
                                bookCount++;
                                
                                // Debug: Show the first few book's Tag values
                                if (bookCount <= 3)
                                {
                                    string debugMsg = $"Book {bookCount}: ID={reader["BookID"]}, Title={reader["Title"]}, Tag={item.Tag}";
                                    MessageBox.Show(debugMsg, "Debug - Book Loading", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                            
                            // Show final count
                            MessageBox.Show($"Loaded {bookCount} books total", "Debug - Load Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            // First, verify the method is being called
            MessageBox.Show("DeleteSelectedBook method called!", "Debug - Method Entry", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a book to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedItem = listView.SelectedItems[0];
            
            // Critical debug: Check if Tag contains BookID
            string debugInfo = $"Selection Debug:\n" +
                             $"- Items selected: {listView.SelectedItems.Count}\n" +
                             $"- Selected item Tag: {selectedItem.Tag?.ToString() ?? "NULL"}\n" +
                             $"- First subitem (Title): {selectedItem.SubItems[1].Text}";
            
            MessageBox.Show(debugInfo, "Debug - Selection Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            if (selectedItem.Tag == null || !int.TryParse(selectedItem.Tag.ToString(), out int bookId))
            {
                MessageBox.Show("Invalid book selection. The Tag property is missing or invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            
                            // Critical debug: Show final result
                            string resultInfo = $"Delete Operation Result:\n" +
                                              $"- BookID: {bookId}\n" +
                                              $"- SQL: {deleteBook}\n" +
                                              $"- Rows affected: {rowsAffected}";
                            
                            MessageBox.Show(resultInfo, "Debug - Delete Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
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

        private void TestDatabaseConnection()
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    
                    // Test basic query
                    string testQuery = "SELECT COUNT(*) FROM Books";
                    using (var command = new SqlCommand(testQuery, connection))
                    {
                        var count = command.ExecuteScalar();
                        MessageBox.Show($"Database connection successful!\nBooks in database: {count}", "Database Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection failed!\nError: {ex.Message}", "Database Test", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ValidateDatabaseStructure()
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    
                    // Check if Books table exists
                    string checkTableQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME = 'Books'";
                    
                    using (var command = new SqlCommand(checkTableQuery, connection))
                    {
                        int tableExists = (int)command.ExecuteScalar();
                        MessageBox.Show($"Books table exists: {tableExists > 0}", "Debug Table Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
                    // Check table structure
                    string checkColumnsQuery = @"
                        SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'Books'
                        ORDER BY ORDINAL_POSITION";
                    
                    using (var command = new SqlCommand(checkColumnsQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            string columns = "Books table structure:\n";
                            while (reader.Read())
                            {
                                columns += $"- {reader["COLUMN_NAME"]} ({reader["DATA_TYPE"]}, Nullable: {reader["IS_NULLABLE"]})\n";
                            }
                            MessageBox.Show(columns, "Debug Table Structure", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error validating database structure: {ex.Message}", "Debug Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}