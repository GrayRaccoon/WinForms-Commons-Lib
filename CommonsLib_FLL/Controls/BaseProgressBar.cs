using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CommonsLib_FLL.Controls
{
    /// <summary>
    /// Base class to create ProgressBar User component implementations.
    /// </summary>
    public abstract class BaseProgressBar : UserControl
    {

        // Private attributes
        private Bitmap _bmp;
        private PictureBox _pictureBox;

        private Color _bgColor;
        private Color _activeColor;


        /// <summary>
        /// Defines the required minimal update in percentage for the PB animations.
        /// </summary>
        [Category("Appearance")]
        [Description("Smooth Increment For Progress Bar.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SmoothIncrement { get; set; } = 1;


        /// <summary>
        /// Defines the Progress bar Inactive Sections Color.
        /// BgColor is an alias for BackColor.
        /// </summary>
        [Category("Appearance")]
        [Description("Inactive Color.")]
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BgColor
        {
            get => _bgColor;
            set
            {
                this.BackColor = value;
                this.PerformLayout();
            }
        }

        /// <summary>
        /// BackColor is an alias for BgColor.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        public override Color BackColor { get => base.BackColor; set => base.BackColor = _bgColor = value; }

        /// <summary>
        /// Defines the Progress bar Active Sections Color.
        /// </summary>
        [Category("Appearance")]
        [Description("Active Color.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ActiveColor
        {
            get => _activeColor;
            set
            {
                _activeColor = value;
                this.PerformLayout();
            }
        }

        /// <summary>
        /// Defines the amount of pixels by each percentage unit.
        /// </summary>
        public double PbUnit => this.Width / 100.0;

        /// <summary>
        /// Defines the Graphics Object where to Draw Active Sections.
        /// </summary>
        protected Graphics PbGraphics { get; private set; }


        /// <summary>
        /// Internal Constructor.
        /// </summary>
        internal BaseProgressBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize the base control elements.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            BgColor = Color.PowderBlue;
            ActiveColor = Color.LightSeaGreen;

            _bmp = new Bitmap(this.Width, this.Height);
            _pictureBox = new PictureBox {Location = new Point(0, 0)};
            this.Controls.Add(_pictureBox);

            this.ResumeLayout(false);
            this.PerformLayout();
        }


        /// <summary>
        /// Event where we detect any redraw operations for this component.
        /// Here we setup the base tasks, the implementation only needs to write
        /// the active sections.
        /// </summary>
        /// <param name="lEvent">Event Arguments.</param>
        protected override void OnLayout(LayoutEventArgs lEvent)
        {
            base.OnLayout(lEvent);

            _pictureBox.Size = new Size(this.Width, this.Height);

            if (_bmp == null || _bmp.Width != this.Width || _bmp.Height != this.Height)
            {
                _bmp?.Dispose();
                _bmp = new Bitmap(this.Width, this.Height);
            }

            PbGraphics = Graphics.FromImage(_bmp);
            PbGraphics.Clear(_bgColor);

            // call to implementation active areas drawing.
            OnProgressBarDraw();

            _pictureBox.Image = _bmp;
        }


        /// <summary>
        /// Utility function to draw active areas.
        /// </summary>
        /// <param name="from">Percentage from</param>
        /// <param name="to">Percentage to</param>
        protected void DrawPercentage(int from, int to)
        {
            var width = to - from;
            PbGraphics.FillRectangle(
                new SolidBrush(this.ActiveColor),
                new Rectangle(
                    (int)(from * this.PbUnit),
                    0,
                    (int)(width * this.PbUnit),
                    this.Height)
                );
        }


        /// <summary>
        /// Implementation will override this method
        /// in order to write some active areas.
        /// </summary>
        protected abstract void OnProgressBarDraw();


        /// <summary>
        /// Releases custom components.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this._bmp.Dispose();
            this._bmp = null;
            this._pictureBox.Dispose();
            base.Dispose(disposing);
        }
    }
}