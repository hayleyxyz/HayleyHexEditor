using System;
using System.IO;
using System.Drawing.Text;

namespace HayleyHexEditor
{
    internal class FontManager
    {
        public PrivateFontCollection PrivateFontCollection { get; private set; }

        private static FontManager instance = new FontManager();

        private FontManager() : this(new PrivateFontCollection())
        { }

        private FontManager(PrivateFontCollection privateFontCollection)
        { PrivateFontCollection = privateFontCollection; }

        public static FontManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FontManager();
                }

                return instance;
            }
        }

        public void LoadLocalFont(string fontPath)
        {
            var fileInfo = new FileInfo(fontPath);

            if (fileInfo.Exists)
            {
                PrivateFontCollection.AddFontFile(fontPath);
                return;
            }

            var directory = new DirectoryInfo(fontPath);

            foreach (FileInfo file in directory.EnumerateFiles("*.ttf", SearchOption.AllDirectories))
            {
                PrivateFontCollection.AddFontFile(file.FullName);
            }
        }

        internal Font GetFont(string familyName, int emSize, FontStyle style = FontStyle.Regular)
        {
            var family = PrivateFontCollection.Families
                .First(f => f.Name == familyName);

            return new Font(family, emSize, style);
        }
    }
}
