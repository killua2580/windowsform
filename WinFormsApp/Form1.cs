namespace WinFormsApp;

public partial class Form1 : Form
{
    private PictureBox pictureBox1; // Corrected type from 'object' to 'PictureBox'

    public Form1()
    {
        InitializeComponent(); // Ensure this is called first to initialize components
        pictureBox1 = new PictureBox(); // Initialize the PictureBox instance
        pictureBox1.Image = Image.FromFile("chemin/vers/image.jpg"); // Set the image
        this.Controls.Add(pictureBox1); // Add PictureBox to the form's controls
    }
}
