using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp.Models;
using WinFormsApp.Data;

namespace WinFormsApp.Forms
{
    public partial class LoginForm : Form
    {
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Button btnSkip;
        private Label lblTitle;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "College Library Management - Login";
            this.Size = new Size(900, 700); // Unified, larger size
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Semi-transparent panel for controls
            Panel panel = new Panel();
            panel.BackColor = Color.FromArgb(220, 255, 255, 255); // Slightly less transparent
            panel.Size = new Size(420, 500);
            panel.Location = new Point((this.Width - panel.Width) / 2, (this.Height - panel.Height) / 2);
            panel.Anchor = AnchorStyles.None;
            panel.BorderStyle = BorderStyle.None;

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
            lblTitle = new Label();
            lblTitle.Text = "College Library System";
            lblTitle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(40, 40, 80);
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Location = new Point(30, 20);
            lblTitle.Size = new Size(320, 40);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Email
            Label lblEmail = new Label();
            lblEmail.Text = "Email:";
            lblEmail.Location = new Point(30, 80);
            lblEmail.Size = new Size(100, 22);
            lblEmail.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblEmail.ForeColor = Color.FromArgb(40, 40, 80);
            lblEmail.BackColor = Color.Transparent;

            txtEmail = new TextBox();
            txtEmail.Location = new Point(30, 105);
            txtEmail.Size = new Size(320, 30);
            txtEmail.Font = new Font("Segoe UI", 12);
            txtEmail.BorderStyle = BorderStyle.FixedSingle;

            // Password
            Label lblPassword = new Label();
            lblPassword.Text = "Password:";
            lblPassword.Location = new Point(30, 145);
            lblPassword.Size = new Size(100, 22);
            lblPassword.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblPassword.ForeColor = Color.FromArgb(40, 40, 80);
            lblPassword.BackColor = Color.Transparent;

            txtPassword = new TextBox();
            txtPassword.Location = new Point(30, 170);
            txtPassword.Size = new Size(320, 30);
            txtPassword.Font = new Font("Segoe UI", 12);
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;

            // Login Button
            btnLogin = new Button();
            btnLogin.Text = "Login";
            btnLogin.Location = new Point(30, 220);
            btnLogin.Size = new Size(140, 40);
            btnLogin.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnLogin.BackColor = Color.FromArgb(0, 120, 215);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click;

            // Register Button
            btnRegister = new Button();
            btnRegister.Text = "Register";
            btnRegister.Location = new Point(210, 220);
            btnRegister.Size = new Size(140, 40);
            btnRegister.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnRegister.BackColor = Color.FromArgb(0, 180, 120);
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Cursor = Cursors.Hand;
            btnRegister.Click += BtnRegister_Click;

            // Skip Button
            btnSkip = new Button();
            btnSkip.Text = "Skip";
            btnSkip.Location = new Point(130, 280);
            btnSkip.Size = new Size(140, 40);
            btnSkip.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnSkip.BackColor = Color.Gray;
            btnSkip.ForeColor = Color.White;
            btnSkip.FlatStyle = FlatStyle.Flat;
            btnSkip.FlatAppearance.BorderSize = 0;
            btnSkip.Cursor = Cursors.Hand;
            btnSkip.Click += BtnSkip_Click;

            panel.Controls.AddRange(new Control[] {
                lblTitle, lblEmail, txtEmail, lblPassword, txtPassword, btnLogin, btnRegister, btnSkip
            });
            this.Controls.Add(panel);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            User user = AuthenticateUser(email, password);
            if (user != null)
            {
                if (user.IsBlocked)
                {
                    MessageBox.Show("Your account has been blocked. Please contact administration.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                this.Hide();
                if (user.IsAdmin)
                {
                    AdminDashboard adminForm = new AdminDashboard(user);
                    adminForm.ShowDialog();
                }
                else
                {
                    StudentDashboard studentForm = new StudentDashboard(user);
                    studentForm.ShowDialog();
                }
                this.Show();
                txtPassword.Clear();
            }
            else
            {
                MessageBox.Show("Invalid email or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            RegistrationForm regForm = new RegistrationForm();
            regForm.ShowDialog();
        }

        private void BtnSkip_Click(object sender, EventArgs e)
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
                IsAdmin = false
            };
            this.Hide();
            StudentDashboard studentForm = new StudentDashboard(guestUser);
            studentForm.ShowDialog();
            this.Show();
        }

        private User AuthenticateUser(string email, string password)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Users WHERE Email = @email AND Password = @password";
                using (var command = new System.Data.SQLite.SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@password", DatabaseHelper.HashPassword(password));

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Email = reader["Email"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                StudyLevel = reader["StudyLevel"].ToString(),
                                StudyField = reader["StudyField"].ToString(),
                                IsBlocked = Convert.ToBoolean(reader["IsBlocked"]),
                                IsAdmin = Convert.ToBoolean(reader["IsAdmin"])
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
} 