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

        private AddressView addressView;
        private CharacterView hexView;
        private CharacterView characterView;

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

            this.ColumnCount = 4;
            this.RowCount = 1;
            this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, scrollBar.Width));


            addressView = new AddressView();
            addressView.Dock = DockStyle.Left;
            addressView.Text = "Address";
            addressView.Font = this.Font;
            addressView.Font = FontManager.Instance.GetFont("Source Code Pro", 10);
            
            this.Controls.Add(addressView, 0, 0);

            hexView = new CharacterView();
            hexView.Dock = DockStyle.Left;
            hexView.Text = "Hex";
            hexView.Font = this.Font; 
            hexView.Font = FontManager.Instance.GetFont("Source Code Pro", 10);

            this.Controls.Add(hexView, 1, 0);

            characterView = new CharacterView();
            characterView.Dock = DockStyle.Left;
            characterView.Text = "Character";
            characterView.Font = this.Font;
            characterView.Font = FontManager.Instance.GetFont("Source Code Pro", 10);

            this.Controls.Add(characterView, 2, 0);

            this.Invalidated += (sender, args) =>
            {
                FillBuffer();
                
                this.addressView.Invalidate();
                this.hexView.Invalidate();
                this.characterView.Invalidate();
            };

            this.Controls.Add(scrollBar, 3, 0);
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

            this.addressView.Address = currentPosition;
            this.hexView.ViewBuffer = internalBuffer;
            this.characterView.ViewBuffer = internalBuffer;
        }


        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            FillBuffer();

            base.OnPaint(e);

            if (stream == null) {
                // TODO
                return;
            }

            scrollBar.Show();
        }
    }
}
