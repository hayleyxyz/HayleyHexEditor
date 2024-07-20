using System.Text;

namespace HayleyHexEditor.Controls
{
    public class AddressView : UserControl
    {
        private long _address;

        public long Address
        {
            get => _address;
            set
            {
                _address = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The number of rows that are displayed in the control.
        /// </summary>
        private int rowCount = 25;

        private int addressWidth = 8;

        /// <summary>
        /// The size of a character in the current font.
        /// </summary>
        private SizeF charSize;

        public AddressView()
        {
            this.Padding = new Padding(0);
            this.Margin = new Padding(0);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            this.charSize = e.Graphics.MeasureString("0", this.Font);

            this.Width = (int) (charSize.Width * 6);

            for (var row = 0; row < rowCount; row++)
            {
                Color backColor = SystemColors.ControlLightLight;
                Brush foreground = new SolidBrush(ForeColor);

                var stringToDraw = new StringBuilder(rowCount * addressWidth);

                stringToDraw.Append((Address + row).ToString("X" + addressWidth.ToString()));

                try
                {
                    e.Graphics.DrawString(stringToDraw.ToString(), this.Font, foreground, x: 0, y: 0 + row * charSize.Height);
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
