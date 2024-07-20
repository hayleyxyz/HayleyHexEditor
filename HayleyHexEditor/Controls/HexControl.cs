using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace HayleyHexEditor.Controls
{
    public partial class HexControl : TableLayoutPanel
    {
        private Stream stream;

        public Stream? Stream
        {
            get => stream;
            set
            {
                if (stream != value)
                {
                    stream = value;
                    
                    if (streamChangedEvent != null)
                    {
                        ((EventHandler) streamChangedEvent)?.Invoke(this, EventArgs.Empty);
                    }

                    scrollBar.Maximum = (int) stream.Length / ScrollIncrement;

                    this.Invalidate();
                }
            }
        }

        private static readonly object streamChangedEvent;

        [Description("Raised when the System.IO.Stream used by the control changes.")]
        public event EventHandler? StreamChanged
        {
            add => Events.AddHandler(streamChangedEvent, value);
            remove => Events.RemoveHandler(streamChangedEvent, value);
        }


        private VScrollBar scrollBar;
        private TextBox edit;

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
        /// The internal buffer is used to store the data that is displayed in the control.
        /// </summary>
        private byte[] internalBuffer;

        /// <summary>
        /// Number of characters to use for the address column.
        /// </summary>
        private int addressWidth = 8;

        private int ScrollIncrement => columnCount;

        private SizeF CharacterDrawSize
        {
            get
            {
                using (var graphics = CreateGraphics())
                {
                    return graphics.MeasureString("W", this.Font);
                }
            }
        }

        private SizeF SpaceDrawSize
        {
            get
            {
                using (var graphics = CreateGraphics())
                {
                    return graphics.MeasureString(" ", this.Font);
                }
            }
        }

        private SizeF HexRowDrawSize
        {
            get
            {
                return ((this.CharacterDrawSize * columnCount) * 2) + this.SpaceDrawSize * columnCount;
            }
        }

        private SizeF AddressDrawSize
        {
            get
            {
                using (var graphics = CreateGraphics())
                {
                    return graphics.MeasureString("0".PadLeft(addressWidth), this.Font);
                }
            }
        }

        public HexControl()
        {
            InitializeComponent();

            this.Padding = Padding.Empty;
            this.Margin = Padding.Empty;

            scrollBar = new VScrollBar();
            //VScrollBar.ValueChanged += new EventHandler(this.ScrollChanged);
            scrollBar.TabStop = true;
            scrollBar.TabIndex = 0;
            scrollBar.Dock = DockStyle.Right;
            scrollBar.Visible = false;

            edit = new TextBox();
            edit.AutoSize = false;
            edit.BorderStyle = BorderStyle.None;
            edit.Multiline = true;
            edit.ReadOnly = true;
            edit.ScrollBars = ScrollBars.Both;
            edit.AcceptsTab = true;
            edit.AcceptsReturn = true;
            edit.Dock = DockStyle.Fill;
            edit.Margin = Padding.Empty;
            edit.WordWrap = false;
            edit.Visible = false;

            Controls.Add(scrollBar, 0, 0);
            Controls.Add(edit, 0, 0);

            scrollBar.ValueChanged += (sender, e) =>
            {
                if (stream != null)
                {
                    stream.Seek(scrollBar.Value * ScrollIncrement, SeekOrigin.Begin);
                }

                this.Invalidate();
            };
        }

        private void FillBuffer()
        {
            if (stream == null)
            {
                return;
            }

            if (internalBuffer == null || internalBuffer.Length < numBytesInDisplay)
            {
                // Round up to the next kb
                int size = (numBytesInDisplay + 1023) & ~1023;
                internalBuffer = new byte[size];
            }

            // Peek read the data from the stream without advancing the position
            var currentPosition = stream.Position;

            // Ensure we don't read past the end of the stream
            int bytesToRead = (int) Math.Min(internalBuffer.Length, stream.Length - currentPosition);

            stream.Read(internalBuffer, 0, bytesToRead);
            stream.Seek(-bytesToRead, SeekOrigin.Current);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (stream == null) {
                // TODO
                return;
            }

            // TODO: This should be done in a separate method
            scrollBar.Maximum = (int) stream.Length / ScrollIncrement;

            scrollBar.Show();


            // TODO: This should be done in a separate method
            FillBuffer();

            DrawAddress(e.Graphics);
            DrawHex(e.Graphics);
            DrawCharacters(e.Graphics);
        }

        private void DrawCharacters(Graphics graphics)
        {
            for (var row = 0; row < rowCount; row++)
            {
                var stringToDraw = new StringBuilder(columnCount);

                for (int col = 0; col < columnCount; col++)
                {
                    var b = internalBuffer[(row * columnCount) + col];
                    stringToDraw.Append(b < 32 ? '.' : (char) b);
                }

                graphics.DrawString(
                    s: stringToDraw.ToString(),
                    font: this.Font,
                    brush: new SolidBrush(ForeColor),
                    x: AddressDrawSize.Width + HexRowDrawSize.Width,
                    y: 0 + row * AddressDrawSize.Height,
                    format: StringFormat.GenericDefault
                );
            }
        }

        private void DrawAddress(Graphics graphics)
        {
            for (var row = 0; row < rowCount; row++)
            {
                var address = (stream.Position + row * columnCount).ToString("X" + this.addressWidth, CultureInfo.InvariantCulture);
                graphics.DrawString(address, this.Font, new SolidBrush(ForeColor), x: 0, y: AddressDrawSize.Height * row);
            }
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
                    stringToDraw.Append(internalBuffer[(row * columnCount) + col].ToString("X2", CultureInfo.InvariantCulture));
                    stringToDraw.Append(' ');
                }

                try
                {
                    graphics.DrawString(stringToDraw.ToString(), this.Font, foreground, x: AddressDrawSize.Width, y: 0 + row * AddressDrawSize.Height, StringFormat.GenericDefault);
                }

                finally
                {
                    if (foreground != null)
                    {
                        foreground.Dispose();
                    }
                }
            }

            // DEBUG
            graphics.DrawRectangle(Pens.Black, 0, 0, AddressDrawSize.Width, AddressDrawSize.Height);
        }
    }
}
