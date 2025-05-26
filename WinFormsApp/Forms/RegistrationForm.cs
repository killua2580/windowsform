using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using WinFormsApp.Models;
using WinFormsApp.Data;
using Microsoft.Data.SqlClient;

namespace WinFormsApp.Forms
{    public partial class RegistrationForm : Form
    {
        private TextBox txtEmail = null!, txtFirstName = null!, txtLastName = null!;
        private ComboBox cmbStudyLevel = null!, cmbStudyField = null!;
        private Button btnRegister = null!, btnSkip = null!;        public RegistrationForm()
        {
            InitializeComponent();
            // Set focus to email textbox when form loads
            this.Load += (s, e) => txtEmail.Focus();
        }private void InitializeComponent()
        {
            this.Text = "IHEC Digital Library - Registration";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(240, 248, 255);

            // Create gradient background
            this.Paint += (s, e) => {
                using (var brush = new LinearGradientBrush(
                    this.ClientRectangle,
                    Color.FromArgb(152, 251, 152),
                    Color.FromArgb(34, 139, 34),
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            // Header panel
            Panel headerPanel = new Panel();
            headerPanel.Size = new Size(this.Width, 80);
            headerPanel.Location = new Point(0, 0);
            headerPanel.BackColor = Color.FromArgb(200, 255, 255, 255);
            headerPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Logo in header
            PictureBox headerLogo = new PictureBox();
            try
            {
                headerLogo.Image = Image.FromFile("Resources/306598131_553981393197532_8221545433072465799_n.jpg");
            }
            catch
            {
                headerLogo.Image = null;
            }
            headerLogo.SizeMode = PictureBoxSizeMode.Zoom;
            headerLogo.Size = new Size(60, 60);
            headerLogo.Location = new Point(20, 10);
            headerLogo.BackColor = Color.Transparent;

            // Header title
            Label headerTitle = new Label();
            headerTitle.Text = "IHEC Digital Library - Student Registration";
            headerTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            headerTitle.ForeColor = Color.FromArgb(25, 25, 112);
            headerTitle.BackColor = Color.Transparent;
            headerTitle.Location = new Point(100, 25);
            headerTitle.Size = new Size(500, 30);

            headerPanel.Controls.AddRange(new Control[] { headerLogo, headerTitle });

            // Main registration panel
            Panel mainPanel = new Panel();
            mainPanel.Size = new Size(600, 550);
            mainPanel.Location = new Point((this.Width - 600) / 2, (this.Height - 550) / 2 + 40);
            mainPanel.BackColor = Color.White;
            mainPanel.Anchor = AnchorStyles.None;
            
            // Add shadow effect
            mainPanel.Paint += (s, e) => {
                var rect = new Rectangle(5, 5, mainPanel.Width - 10, mainPanel.Height - 10);
                using (var shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, rect);
                }
                using (var brush = new SolidBrush(Color.White))
                {
                    var mainRect = new Rectangle(0, 0, mainPanel.Width - 5, mainPanel.Height - 5);
                    e.Graphics.FillRectangle(brush, mainRect);
                }
            };

            int yPos = 30;

            // Welcome section
            Label welcomeLabel = new Label();
            welcomeLabel.Text = "Join Our Digital Library";
            welcomeLabel.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            welcomeLabel.ForeColor = Color.FromArgb(25, 25, 112);
            welcomeLabel.BackColor = Color.Transparent;
            welcomeLabel.Location = new Point(50, yPos);
            welcomeLabel.Size = new Size(500, 35);
            welcomeLabel.TextAlign = ContentAlignment.MiddleCenter;
            yPos += 50;

            // Form fields with modern styling
            // Email
            Label lblEmail = new Label();
            lblEmail.Text = "📧 College Email (@ihec.ucar.tn)";
            lblEmail.Location = new Point(50, yPos);
            lblEmail.Size = new Size(500, 25);
            lblEmail.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblEmail.ForeColor = Color.FromArgb(70, 70, 70);
            lblEmail.BackColor = Color.Transparent;
            yPos += 30;            Panel emailBorder = new Panel();
            emailBorder.Location = new Point(48, yPos - 2);
            emailBorder.Size = new Size(504, 39);
            emailBorder.BackColor = Color.FromArgb(200, 200, 200);
            
            txtEmail = new TextBox();
            txtEmail.Location = new Point(50, yPos);
            txtEmail.Size = new Size(500, 35);
            txtEmail.Font = new Font("Segoe UI", 12);
            txtEmail.BorderStyle = BorderStyle.None;
            txtEmail.BackColor = Color.FromArgb(250, 250, 250);
            txtEmail.PlaceholderText = "Enter your college email ending with @ihec.ucar.tn";
            txtEmail.Enabled = true;
            txtEmail.ReadOnly = false;
            txtEmail.TabIndex = 1;
            yPos += 50;

            // First Name
            Label lblFirstName = new Label();
            lblFirstName.Text = "👤 First Name";
            lblFirstName.Location = new Point(50, yPos);
            lblFirstName.Size = new Size(240, 25);
            lblFirstName.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblFirstName.ForeColor = Color.FromArgb(70, 70, 70);
            lblFirstName.BackColor = Color.Transparent;

            Label lblLastName = new Label();
            lblLastName.Text = "👤 Last Name";
            lblLastName.Location = new Point(310, yPos);
            lblLastName.Size = new Size(240, 25);
            lblLastName.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblLastName.ForeColor = Color.FromArgb(70, 70, 70);
            lblLastName.BackColor = Color.Transparent;
            yPos += 30;            Panel firstNameBorder = new Panel();
            firstNameBorder.Location = new Point(48, yPos - 2);
            firstNameBorder.Size = new Size(244, 39);
            firstNameBorder.BackColor = Color.FromArgb(200, 200, 200);

            Panel lastNameBorder = new Panel();
            lastNameBorder.Location = new Point(308, yPos - 2);
            lastNameBorder.Size = new Size(244, 39);
            lastNameBorder.BackColor = Color.FromArgb(200, 200, 200);
            
            txtFirstName = new TextBox();
            txtFirstName.Location = new Point(50, yPos);
            txtFirstName.Size = new Size(240, 35);
            txtFirstName.Font = new Font("Segoe UI", 12);
            txtFirstName.BorderStyle = BorderStyle.None;
            txtFirstName.BackColor = Color.FromArgb(250, 250, 250);
            txtFirstName.PlaceholderText = "Your first name";
            txtFirstName.Enabled = true;
            txtFirstName.ReadOnly = false;
            txtFirstName.TabIndex = 2;

            txtLastName = new TextBox();
            txtLastName.Location = new Point(310, yPos);
            txtLastName.Size = new Size(240, 35);
            txtLastName.Font = new Font("Segoe UI", 12);
            txtLastName.BorderStyle = BorderStyle.None;
            txtLastName.BackColor = Color.FromArgb(250, 250, 250);
            txtLastName.PlaceholderText = "Your last name";
            txtLastName.Enabled = true;
            txtLastName.ReadOnly = false;
            txtLastName.TabIndex = 3;
            yPos += 50;

            // Study Level and Field
            Label lblStudyLevel = new Label();
            lblStudyLevel.Text = "🎓 Study Level";
            lblStudyLevel.Location = new Point(50, yPos);
            lblStudyLevel.Size = new Size(240, 25);
            lblStudyLevel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblStudyLevel.ForeColor = Color.FromArgb(70, 70, 70);
            lblStudyLevel.BackColor = Color.Transparent;

            Label lblStudyField = new Label();
            lblStudyField.Text = "📚 Study Field";
            lblStudyField.Location = new Point(310, yPos);
            lblStudyField.Size = new Size(240, 25);
            lblStudyField.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblStudyField.ForeColor = Color.FromArgb(70, 70, 70);
            lblStudyField.BackColor = Color.Transparent;
            yPos += 30;

            cmbStudyLevel = new ComboBox();
            cmbStudyLevel.Items.AddRange(new string[] { "1st Year", "2nd Year", "3rd Year", "Master 1", "Master 2", "PhD", "Other" });
            cmbStudyLevel.Location = new Point(50, yPos);
            cmbStudyLevel.Size = new Size(240, 35);
            cmbStudyLevel.Font = new Font("Segoe UI", 12);
            cmbStudyLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStudyLevel.BackColor = Color.FromArgb(250, 250, 250);

            cmbStudyField = new ComboBox();
            cmbStudyField.Items.AddRange(new string[] { "Business Intelligence", "Finance", "Marketing", "Accounting", "Management", "Big Data Analytics", "Economics", "International Business", "Other" });
            cmbStudyField.Location = new Point(310, yPos);
            cmbStudyField.Size = new Size(240, 35);
            cmbStudyField.Font = new Font("Segoe UI", 12);
            cmbStudyField.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStudyField.BackColor = Color.FromArgb(250, 250, 250);
            yPos += 70;

            // Action buttons
            btnRegister = new Button();
            btnRegister.Text = "🎯 CREATE ACCOUNT";
            btnRegister.Location = new Point(125, yPos);
            btnRegister.Size = new Size(350, 50);
            btnRegister.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnRegister.BackColor = Color.FromArgb(34, 139, 34);
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Cursor = Cursors.Hand;
            btnRegister.Click += BtnRegister_Click;

            btnRegister.MouseEnter += (s, e) => btnRegister.BackColor = Color.FromArgb(60, 179, 113);
            btnRegister.MouseLeave += (s, e) => btnRegister.BackColor = Color.FromArgb(34, 139, 34);            btnSkip = new Button();
            btnSkip.Text = "👤 CONTINUE AS GUEST";
            btnSkip.Location = new Point(50, yPos + 60);
            btnSkip.Size = new Size(170, 45);
            btnSkip.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnSkip.BackColor = Color.FromArgb(169, 169, 169);
            btnSkip.ForeColor = Color.White;
            btnSkip.FlatStyle = FlatStyle.Flat;
            btnSkip.FlatAppearance.BorderSize = 0;
            btnSkip.Cursor = Cursors.Hand;
            btnSkip.Click += BtnSkip_Click;

            btnSkip.MouseEnter += (s, e) => btnSkip.BackColor = Color.FromArgb(128, 128, 128);
            btnSkip.MouseLeave += (s, e) => btnSkip.BackColor = Color.FromArgb(169, 169, 169);

            // Back to Login button
            Button btnBackToLogin = new Button();
            btnBackToLogin.Text = "🔙 BACK TO LOGIN";
            btnBackToLogin.Location = new Point(240, yPos + 60);
            btnBackToLogin.Size = new Size(170, 45);
            btnBackToLogin.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnBackToLogin.BackColor = Color.FromArgb(70, 130, 180);
            btnBackToLogin.ForeColor = Color.White;
            btnBackToLogin.FlatStyle = FlatStyle.Flat;
            btnBackToLogin.FlatAppearance.BorderSize = 0;
            btnBackToLogin.Cursor = Cursors.Hand;
            btnBackToLogin.Click += BtnBackToLogin_Click;

            btnBackToLogin.MouseEnter += (s, e) => btnBackToLogin.BackColor = Color.FromArgb(100, 149, 237);
            btnBackToLogin.MouseLeave += (s, e) => btnBackToLogin.BackColor = Color.FromArgb(70, 130, 180);

            // Exit Application button
            Button btnExit = new Button();
            btnExit.Text = "🚪 EXIT APPLICATION";
            btnExit.Location = new Point(430, yPos + 60);
            btnExit.Size = new Size(170, 45);
            btnExit.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnExit.BackColor = Color.FromArgb(220, 53, 69);
            btnExit.ForeColor = Color.White;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Cursor = Cursors.Hand;
            btnExit.Click += BtnExit_Click;

            btnExit.MouseEnter += (s, e) => btnExit.BackColor = Color.FromArgb(255, 69, 58);
            btnExit.MouseLeave += (s, e) => btnExit.BackColor = Color.FromArgb(220, 53, 69);

            // Benefits section
            Label benefitsLabel = new Label();
            benefitsLabel.Text = "✨ Benefits: Personalized Recommendations • Reservation System • Digital Access • Study Analytics";
            benefitsLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            benefitsLabel.ForeColor = Color.FromArgb(100, 100, 100);
            benefitsLabel.BackColor = Color.Transparent;
            benefitsLabel.Location = new Point(50, yPos + 120);
            benefitsLabel.Size = new Size(500, 40);
            benefitsLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Close button
            Button closeButton = new Button();
            closeButton.Text = "✕";
            closeButton.Location = new Point(this.Width - 40, 10);
            closeButton.Size = new Size(30, 30);
            closeButton.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            closeButton.BackColor = Color.FromArgb(220, 53, 69);
            closeButton.ForeColor = Color.White;
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Cursor = Cursors.Hand;
            closeButton.Click += (s, e) => this.Close();
            closeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;            // Add controls in the correct order to ensure proper z-order
            // First add non-interactive controls
            mainPanel.Controls.AddRange(new Control[] {
                welcomeLabel, lblEmail,
                lblFirstName, lblLastName,
                lblStudyLevel, lblStudyField,
                btnRegister, btnSkip, btnBackToLogin, btnExit, benefitsLabel
            });
            
            // Add border panels
            mainPanel.Controls.Add(emailBorder);
            mainPanel.Controls.Add(firstNameBorder);
            mainPanel.Controls.Add(lastNameBorder);
            
            // Add interactive controls last so they appear on top
            mainPanel.Controls.Add(txtEmail);
            mainPanel.Controls.Add(txtFirstName);
            mainPanel.Controls.Add(txtLastName);
            mainPanel.Controls.Add(cmbStudyLevel);
            mainPanel.Controls.Add(cmbStudyField);
            
            // Ensure text boxes are brought to front
            txtEmail.BringToFront();
            txtFirstName.BringToFront();
            txtLastName.BringToFront();

            this.Controls.AddRange(new Control[] { headerPanel, mainPanel, closeButton });
        }

        private void BtnRegister_Click(object? sender, EventArgs e)        {
            try
            {
                // Enhanced validation - all fields are mandatory
                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Email is required. Please enter your college email.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }

                if (!txtEmail.Text.EndsWith("@ihec.ucar.tn"))
                {
                    MessageBox.Show("Please enter a valid college email ending with @ihec.ucar.tn", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtFirstName.Text))
                {
                    MessageBox.Show("First name is required. Please enter your first name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFirstName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtLastName.Text))
                {
                    MessageBox.Show("Last name is required. Please enter your last name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLastName.Focus();
                    return;
                }

                if (cmbStudyLevel.SelectedIndex == -1)
                {
                    MessageBox.Show("Study level is required. Please select your study level.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbStudyLevel.Focus();
                    return;
                }

                if (cmbStudyField.SelectedIndex == -1)
                {
                    MessageBox.Show("Study field is required. Please select your study field.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbStudyField.Focus();
                    return;
                }

                // Check if email already exists
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    
                    string checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = @email";
                    using (var checkCommand = new SqlCommand(checkEmailQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        int emailCount = (int)checkCommand.ExecuteScalar();
                        
                        if (emailCount > 0)
                        {
                            MessageBox.Show("This email is already registered. Please use a different email.", "Email Already Exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtEmail.Focus();
                            return;
                        }
                    }

                    // Register user (without profile picture)
                    string query = @"INSERT INTO Users (Email, FirstName, LastName, StudyLevel, StudyField, Password, CreatedDate, IsBlocked, IsAdmin)
                                    VALUES (@email, @firstName, @lastName, @studyLevel, @studyField, @password, @createdDate, @isBlocked, @isAdmin)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        command.Parameters.AddWithValue("@firstName", txtFirstName.Text.Trim());
                        command.Parameters.AddWithValue("@lastName", txtLastName.Text.Trim());
                        command.Parameters.AddWithValue("@studyLevel", cmbStudyLevel.SelectedItem?.ToString() ?? "");
                        command.Parameters.AddWithValue("@studyField", cmbStudyField.SelectedItem?.ToString() ?? "");
                        command.Parameters.AddWithValue("@password", DatabaseHelper.HashPassword("defaultPassword123"));
                        command.Parameters.AddWithValue("@createdDate", DateTime.Now);
                        command.Parameters.AddWithValue("@isBlocked", false);
                        command.Parameters.AddWithValue("@isAdmin", false);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Registration successful! You can now login with password: defaultPassword123", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error during registration:\n{sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration failed:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        private void BtnSkip_Click(object? sender, EventArgs e)
        {
            // Create a guest user
            var guestUser = new User
            {
                UserID = -1,
                Email = "guest@ihec.ucar.tn",
                FirstName = "Guest",
                LastName = "User",
                StudyLevel = "N/A",
                StudyField = "General",
                IsBlocked = false,
                IsAdmin = false,
                Password = "" // Required property
            };
            this.Hide();
            StudentDashboard studentForm = new StudentDashboard(guestUser);
            studentForm.ShowDialog();
            this.Show();
        }

        private void BtnBackToLogin_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnExit_Click(object? sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to exit the application?", 
                "Exit Application", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}