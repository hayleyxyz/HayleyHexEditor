namespace HayleyHexEditor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new())
            {
                dialog.Filter = "All Files|*.*";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    hexControl.Stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read);
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // For testing purposes, open a file when the form loads.
            hexControl.Stream = new FileStream("C:\\Windows\\System32\\notepad.exe", FileMode.Open, FileAccess.Read);
        }
    }
}
