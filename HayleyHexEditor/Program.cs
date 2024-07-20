namespace HayleyHexEditor
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            FontManager.Instance.LoadLocalFont("Fonts\\Source_Code_Pro\\static\\SourceCodePro-Regular.ttf");

            Application.Run(new MainForm());
        }
    }
}