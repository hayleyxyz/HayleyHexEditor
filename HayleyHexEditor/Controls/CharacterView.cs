using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace HayleyHexEditor.Controls
{
    public partial class CharacterView : UserControl
    {
        private byte[] viewBuffer;
        
        public byte[] ViewBuffer
        {
            get => viewBuffer;
            set
            {
                viewBuffer = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The number of rows that are displayed in the control.
        /// </summary>
        private int rowCount = 25;

        /// <summary>
        /// The number of columns that are displayed in the control.
        /// </summary>
        private int columnCount = 16;

        private int numBytesInDisplay => rowCount * columnCount;

        

        /// <summary>
        /// The size of a character in the current font.
        /// </summary>
        private SizeF charSize;

        public CharacterView()
        {
            InitializeComponent();
            this.Padding = new Padding(0);
            this.Margin = new Padding(0);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            this.charSize = e.Graphics.MeasureString("0", this.Font);

            // Calculate with for hex view
            this.Width = (int) ((charSize.Width * (columnCount * 2)) - charSize.Width);
            
            if (viewBuffer == null)
            {
                return;
            }

            DrawHex(e.Graphics);
        }

        private void DrawHex(Graphics graphics)
        {
            for (var row = 0; row < rowCount; row++)
            {
                Color backColor = SystemColors.ControlLightLight;
                Brush foreground = new SolidBrush(ForeColor);

                var stringToDraw = new StringBuilder(numBytesInDisplay * 3 + 1);

                for (int col = 0; col < columnCount; col++)
                {
                    stringToDraw.Append(viewBuffer[(row * columnCount) + col].ToString("X2", CultureInfo.InvariantCulture));
                    stringToDraw.Append(' ');
                }


                try
                {
                    graphics.DrawString(stringToDraw.ToString(), this.Font, foreground, x: 0, y: 0 + row * charSize.Height);
                }

                finally
                {
                    if (foreground != null)
                    {
                        foreground.Dispose();
                    }
                }
            }
        }
    }
}
