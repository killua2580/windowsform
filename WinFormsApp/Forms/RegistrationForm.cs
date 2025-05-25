using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WinFormsApp.Models;
using WinFormsApp.Data;
using Microsoft.Data.SqlClient;

namespace WinFormsApp.Forms
{
    public partial class RegistrationForm : Form
    {
        private TextBox txtEmail = null!, txtFirstName = null!, txtLastName = null!, txtProfilePictureUrl = null!;
        private ComboBox cmbStudyLevel = null!, cmbStudyField = null!;
        private PictureBox picProfile = null!;
        private Button btnRegister = null!, btnLoadImage = null!, btnSkip = null!;

        public RegistrationForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Student Registration";
            this.Size = new Size(900, 700); // Unified, larger size
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Semi-transparent panel for controls
            Panel panel = new Panel();
            panel.BackColor = Color.FromArgb(220, 255, 255, 255); // Slightly less transparent
            panel.Size = new Size(420, 600);
            panel.Location = new Point((this.Width - panel.Width) / 2, (this.Height - panel.Height) / 2);
            panel.Anchor = AnchorStyles.None;
            panel.BorderStyle = BorderStyle.None;

            int yPos = 20;

            // Logo at the top-left
            PictureBox picLogo = new PictureBox();
            try
            {
                picLogo.Image = Image.FromFile("Resources/306598131_553981393197532_8221545433072465799_n.jpg");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load logo image: {ex.Message}", "Logo Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                picLogo.Image = null;
            }
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.Size = new Size(100, 100);
            picLogo.Location = new Point(30, 20);
            picLogo.BackColor = Color.Transparent;
            this.Controls.Add(picLogo);

            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "Student Registration";
            lblTitle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(40, 40, 80);
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Location = new Point(40, yPos);
            lblTitle.Size = new Size(340, 40);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            yPos += 60;

            // Email
            Label lblEmail = new Label();
            lblEmail.Text = "College Email (@ihec.ucar.tn):";
            lblEmail.Location = new Point(40, yPos);
            lblEmail.Size = new Size(300, 22);
            lblEmail.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblEmail.ForeColor = Color.FromArgb(40, 40, 80);
            lblEmail.BackColor = Color.Transparent;
            yPos += 25;

            txtEmail = new TextBox();
            txtEmail.Location = new Point(40, yPos);
            txtEmail.Size = new Size(340, 30);
            txtEmail.Font = new Font("Segoe UI", 12);
            txtEmail.BorderStyle = BorderStyle.FixedSingle;
            yPos += 40;

            // First Name
            Label lblFirstName = new Label();
            lblFirstName.Text = "First Name:";
            lblFirstName.Location = new Point(40, yPos);
            lblFirstName.Size = new Size(120, 22);
            lblFirstName.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblFirstName.ForeColor = Color.FromArgb(40, 40, 80);
            lblFirstName.BackColor = Color.Transparent;
            yPos += 25;

            txtFirstName = new TextBox();
            txtFirstName.Location = new Point(40, yPos);
            txtFirstName.Size = new Size(340, 30);
            txtFirstName.Font = new Font("Segoe UI", 12);
            txtFirstName.BorderStyle = BorderStyle.FixedSingle;
            yPos += 40;

            // Last Name
            Label lblLastName = new Label();
            lblLastName.Text = "Last Name:";
            lblLastName.Location = new Point(40, yPos);
            lblLastName.Size = new Size(120, 22);
            lblLastName.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblLastName.ForeColor = Color.FromArgb(40, 40, 80);
            lblLastName.BackColor = Color.Transparent;
            yPos += 25;

            txtLastName = new TextBox();
            txtLastName.Location = new Point(40, yPos);
            txtLastName.Size = new Size(340, 30);
            txtLastName.Font = new Font("Segoe UI", 12);
            txtLastName.BorderStyle = BorderStyle.FixedSingle;
            yPos += 40;

            // Study Level
            Label lblStudyLevel = new Label();
            lblStudyLevel.Text = "Study Level:";
            lblStudyLevel.Location = new Point(40, yPos);
            lblStudyLevel.Size = new Size(120, 22);
            lblStudyLevel.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblStudyLevel.ForeColor = Color.FromArgb(40, 40, 80);
            lblStudyLevel.BackColor = Color.Transparent;
            yPos += 25;

            cmbStudyLevel = new ComboBox();
            cmbStudyLevel.Items.AddRange(new string[] { "1", "2", "3", "M1", "M2", "Other" });
            cmbStudyLevel.Location = new Point(40, yPos);
            cmbStudyLevel.Size = new Size(340, 30);
            cmbStudyLevel.Font = new Font("Segoe UI", 12);
            cmbStudyLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            yPos += 40;

            // Study Field
            Label lblStudyField = new Label();
            lblStudyField.Text = "Study Field:";
            lblStudyField.Location = new Point(40, yPos);
            lblStudyField.Size = new Size(120, 22);
            lblStudyField.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblStudyField.ForeColor = Color.FromArgb(40, 40, 80);
            lblStudyField.BackColor = Color.Transparent;
            yPos += 25;

            cmbStudyField = new ComboBox();
            cmbStudyField.Items.AddRange(new string[] { "BI", "Finance", "Marketing", "Accounting", "Management", "Big Data", "Other" });
            cmbStudyField.Location = new Point(40, yPos);
            cmbStudyField.Size = new Size(340, 30);
            cmbStudyField.Font = new Font("Segoe UI", 12);
            cmbStudyField.DropDownStyle = ComboBoxStyle.DropDownList;
            yPos += 40;

            // Profile Picture
            Label lblProfile = new Label();
            lblProfile.Text = "Profile Picture URL:";
            lblProfile.Location = new Point(40, yPos);
            lblProfile.Size = new Size(150, 22);
            lblProfile.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblProfile.ForeColor = Color.FromArgb(40, 40, 80);
            lblProfile.BackColor = Color.Transparent;
            yPos += 25;

            txtProfilePictureUrl = new TextBox();
            txtProfilePictureUrl.Location = new Point(40, yPos);
            txtProfilePictureUrl.Size = new Size(260, 30);
            txtProfilePictureUrl.Font = new Font("Segoe UI", 10);
            txtProfilePictureUrl.BorderStyle = BorderStyle.FixedSingle;
            txtProfilePictureUrl.PlaceholderText = "Enter image URL (optional)";

            btnLoadImage = new Button();
            btnLoadImage.Text = "Preview";
            btnLoadImage.Location = new Point(310, yPos);
            btnLoadImage.Size = new Size(70, 30);
            btnLoadImage.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnLoadImage.BackColor = Color.FromArgb(0, 120, 215);
            btnLoadImage.ForeColor = Color.White;
            btnLoadImage.FlatStyle = FlatStyle.Flat;
            btnLoadImage.FlatAppearance.BorderSize = 0;
            btnLoadImage.Cursor = Cursors.Hand;
            btnLoadImage.Click += BtnLoadImage_Click;
            yPos += 40;

            picProfile = new PictureBox();
            picProfile.Location = new Point(40, yPos);
            picProfile.Size = new Size(100, 100);
            picProfile.BorderStyle = BorderStyle.FixedSingle;
            picProfile.SizeMode = PictureBoxSizeMode.StretchImage;
            picProfile.BackColor = Color.LightGray;
            yPos += 120;

            // Register Button
            btnRegister = new Button();
            btnRegister.Text = "Register";
            btnRegister.Location = new Point(110, yPos);
            btnRegister.Size = new Size(200, 45);
            btnRegister.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnRegister.BackColor = Color.FromArgb(0, 180, 120);
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Cursor = Cursors.Hand;
            btnRegister.Click += BtnRegister_Click;

            // Skip Button
            btnSkip = new Button();
            btnSkip.Text = "Skip";
            btnSkip.Location = new Point(110, yPos + 60);
            btnSkip.Size = new Size(200, 45);
            btnSkip.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnSkip.BackColor = Color.Gray;
            btnSkip.ForeColor = Color.White;
            btnSkip.FlatStyle = FlatStyle.Flat;
            btnSkip.FlatAppearance.BorderSize = 0;
            btnSkip.Cursor = Cursors.Hand;
            btnSkip.Click += BtnSkip_Click;

            panel.Controls.AddRange(new Control[] {
                lblTitle, lblEmail, txtEmail, lblFirstName, txtFirstName,
                lblLastName, txtLastName, lblStudyLevel, cmbStudyLevel,
                lblStudyField, cmbStudyField, lblProfile, txtProfilePictureUrl,
                btnLoadImage, picProfile, btnRegister, btnSkip
            });
            this.Controls.Add(panel);
        }

        private async void BtnLoadImage_Click(object? sender, EventArgs e)
        {
            string imageUrl = txtProfilePictureUrl.Text.Trim();
            if (string.IsNullOrEmpty(imageUrl))
            {
                picProfile.Image = null;
                return;
            }

            try
            {
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                    using (var ms = new System.IO.MemoryStream(imageBytes))
                    {
                        picProfile.Image = Image.FromStream(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load image from URL: {ex.Message}", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                picProfile.Image = null;
            }
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(txtEmail.Text) || !txtEmail.Text.EndsWith("@ihec.ucar.tn"))
                {
                    MessageBox.Show("Please enter a valid college email ending with @ihec.ucar.tn", "Validation Error");
                    return;
                }

                if (string.IsNullOrEmpty(txtFirstName.Text) || string.IsNullOrEmpty(txtLastName.Text))
                {
                    MessageBox.Show("Please enter both first and last name.", "Validation Error");
                    return;
                }

                if (cmbStudyLevel.SelectedIndex == -1 || cmbStudyField.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select both study level and field.", "Validation Error");
                    return;
                }

                // Check if email already exists
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    
                    string checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = @email";
                    using (var checkCommand = new SqlCommand(checkEmailQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@email", txtEmail.Text);
                        int emailCount = (int)checkCommand.ExecuteScalar();
                        
                        if (emailCount > 0)
                        {
                            MessageBox.Show("This email is already registered. Please use a different email.", "Email Already Exists");
                            return;
                        }
                    }

                    // Register user
                    string query = @"INSERT INTO Users (Email, FirstName, LastName, StudyLevel, StudyField, ProfilePicture, Password, CreatedDate, IsBlocked, IsAdmin)
                                    VALUES (@email, @firstName, @lastName, @studyLevel, @studyField, @profilePicture, @password, @createdDate, @isBlocked, @isAdmin)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@email", txtEmail.Text);
                        command.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                        command.Parameters.AddWithValue("@lastName", txtLastName.Text);
                        command.Parameters.AddWithValue("@studyLevel", cmbStudyLevel.SelectedItem?.ToString() ?? "");
                        command.Parameters.AddWithValue("@studyField", cmbStudyField.SelectedItem?.ToString() ?? "");
                        command.Parameters.AddWithValue("@profilePicture", string.IsNullOrWhiteSpace(txtProfilePictureUrl.Text) ? (object)DBNull.Value : txtProfilePictureUrl.Text.Trim());
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
        }

        private void BtnSkip_Click(object? sender, EventArgs e)
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
    }
}