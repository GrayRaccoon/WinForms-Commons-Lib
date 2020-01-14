using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace CommonsLib_FLL.Controls
{
    /// <summary>
    /// Indeterminate Progress Bar Implementation.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    [Docking(DockingBehavior.Never)]
    public class GrIndeterminateProgressBar : BaseProgressBar
    {
        public static readonly int DefaultUpdateTime = 27;
        public static readonly int[] InitialPoints = { 0, 33, 66 };
        public static readonly int BarLineLength = 25;

        // Private attributes
        private int[] _startPoints = InitialPoints;

        private BackgroundWorker _bgTask;


        /// <summary>
        /// Time to wait after each update.
        /// </summary>
        [Category("Appearance")]
        [Description("Smooth Increment For Progress Bar.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        protected int UpdateEachMs { get; set; } = DefaultUpdateTime;

        /// <summary>
        /// Whether or not Process is running.
        /// </summary>
        public bool IsRunning { get; private set; } = false;

        /// <summary>
        /// Get all lines to draw for all the start points.
        /// </summary>
        internal List<Line> AllLines
        {
            get
            {
                var allLines = new List<Line>();
                if (!this.IsRunning)
                {
                    allLines.Add(new Line() { StartPoint = 0, EndPoint = 100 });
                    return allLines;
                }
                this._startPoints.ToList().ForEach(startPoint =>
                {
                    allLines.AddRange(
                        GetLinesToDrawFor(startPoint)
                        );
                });
                return allLines;
            }
        }


        /// <summary>
        /// Public Constructor that calls to base constructor.
        /// </summary>
        public GrIndeterminateProgressBar() : base() { }


        /// <summary>
        /// Start Indeterminate Progress Bar Animation.
        /// </summary>
        public void DoStart()
        {
            if (IsRunning)
            {
                return;
            }
            IsRunning = true;
            _bgTask = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _bgTask.DoWork += BackgroundWork;
            _bgTask.ProgressChanged += delegate
            {
                this.PerformLayout();
            };
            _bgTask.RunWorkerCompleted += delegate
            {
                DoEnd();
            };
            _bgTask.RunWorkerAsync(_bgTask);

        }

        /// <summary>
        /// Stop Indeterminate Progress Bar Animation.
        /// </summary>
        public void DoEnd()
        {
            IsRunning = false;
            _bgTask?.CancelAsync();
            _bgTask = null;
        }

        /// <summary>
        /// Represents the Background work that will run in this component.
        /// </summary>
        /// <param name="sender">Process invoker.</param>
        /// <param name="e">Event Arguments</param>
        private void BackgroundWork(object sender, DoWorkEventArgs e)
        {
            var localBgTask = (BackgroundWorker)e.Argument;
            while (true)
            {
                Thread.Sleep(UpdateEachMs);

                if (localBgTask.CancellationPending)
                {
                    e.Cancel = true;
                    this.PerformLayout();
                    break;
                }

                IncreaseStartPoints();
                localBgTask.ReportProgress(0);
            }
        }


        /// <summary>
        /// Implementation of base OnProgressBarDraw
        /// It draws multiple active areas depending on the current state.
        /// </summary>
        protected override void OnProgressBarDraw()
        {
            this.AllLines.ForEach(line =>
            {
                this.DrawPercentage(line.StartPoint, line.EndPoint);
            });
        }


        /// <summary>
        /// Get required lines to Draw certain StartPoint.
        /// </summary>
        /// <param name="startPoint">Point to draw</param>
        /// <returns>Lines to Draw</returns>
        private List<Line> GetLinesToDrawFor(int startPoint)
        {
            var lines2draw = new List<Line>();

            var endPoint = startPoint + BarLineLength;

            if (endPoint <= 100)
            {
                lines2draw.Add(new Line() {
                    StartPoint = startPoint,
                    EndPoint = endPoint
                });
                return lines2draw;
            }

            endPoint -= 100;

            lines2draw.Add(new Line()
            {
                StartPoint = startPoint,
                EndPoint = 100
            });
            lines2draw.Add(new Line()
            {
                StartPoint = 0,
                EndPoint = endPoint
            });

            return lines2draw;
        }


        /// <summary>
        /// Increase and update all start points.
        /// </summary>
        private void IncreaseStartPoints()
        {
            for (var i=0; i < _startPoints.Length ;i++)
            {
                _startPoints[i] = IncreaseStartPoint(_startPoints[i]);
            }
        }

        /// <summary>
        /// Calculate new value for a point.
        /// </summary>
        /// <param name="point">Point to increase.</param>
        /// <returns>Resulted Increased Point.</returns>
        private int IncreaseStartPoint(int point)
        {
            point += SmoothIncrement * 2;
            if (point >= 100)
            {
                point -= 100;
            }
            return point;
        }



        /// <summary>
        /// Make sure Animation is stopped before disposing.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            DoEnd();
            base.Dispose(disposing);
        }

    }

    /// <summary>
    /// Data Object that represents an active section in the progress bar.
    /// </summary>
    internal class Line
    {
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }

        public override string ToString()
        {
            return $"{StartPoint},{EndPoint}";
        }
    }
}