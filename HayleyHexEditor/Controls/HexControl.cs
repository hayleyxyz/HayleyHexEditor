using System;
using System.ComponentModel;
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
        /// The size of a character in the current font.
        /// </summary>
        private SizeF charSize;

        private SizeF charPadding = new SizeF(2, 2);

        public HexControl()
        {
            InitializeComponent();

            scrollBar = new VScrollBar();
            //VScrollBar.ValueChanged += new EventHandler(this.ScrollChanged);
            scrollBar.TabStop = true;
            scrollBar.TabIndex = 0;
            scrollBar.Dock = DockStyle.Right;
            scrollBar.Visible = true;

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

            this.ColumnCount = 3;
            this.RowCount = 1;
            this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));


            var addressView = new CharacterView();
            addressView.Dock = DockStyle.Left;
            addressView.Text = "Address";
            addressView.Font = FontManager.Instance.GetFont("Source Code Pro", 10);
            
            this.Controls.Add(addressView, 0, 0);

            var hexView = new CharacterView();
            hexView.Dock = DockStyle.Left;
            hexView.Text = "Hex";
            hexView.Font = FontManager.Instance.GetFont("Source Code Pro", 10);

            this.Controls.Add(hexView, 0, 0);

            var characterView = new CharacterView();
            characterView.Dock = DockStyle.Left;
            characterView.Text = "Character";
            characterView.Font = FontManager.Instance.GetFont("Source Code Pro", 10);

            this.Controls.Add(characterView, 0, 0);

            //Controls.Add(scrollBar, 0, 0);
            //Controls.Add(edit, 0, 0);




            //this.Font = FontManager.Instance.GetFont("Source Code Pro", 10);

            //this.LocationChanged

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

            scrollBar.Show();
            return;
            DrawHex(e.Graphics);
            
        }

        private void DrawHex(Graphics graphics)
        {
            this.charSize = graphics.MeasureString("0", this.Font);

            for (var row = 0; row < rowCount; row++)
            {
                Color backColor = SystemColors.ControlLightLight;
                Brush foreground = new SolidBrush(ForeColor);

                FillBuffer();

                var stringToDraw = new StringBuilder(numBytesInDisplay * 3 + 1);

                for (int col = 0; col < columnCount; col++)
                {
                    stringToDraw.Append(internalBuffer[(row * columnCount) + col].ToString("X2", CultureInfo.InvariantCulture));
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
