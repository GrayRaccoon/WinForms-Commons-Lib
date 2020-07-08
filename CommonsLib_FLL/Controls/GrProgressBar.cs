using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonsLib_FLL.Controls
{
    /// <summary>
    /// Basic Progress Bar Implementation.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    [Docking(DockingBehavior.Never)]
    public class GrProgressBar : BaseProgressBar
    {
        // Private attributes
        private int _value = 0;


        /// <summary>
        /// Current Percentage Value.
        /// </summary>
        [Category("Appearance")]
        [Description("Current Value.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Value
        {
            get => _value;
            set
            {
                if (value > 100) _value = 100;
                else if (value < 0) _value = 0;
                else _value = value;
                this.PerformLayout();
            }
        }

        /// <summary>
        /// Public Constructor that calls to base constructor.
        /// </summary>
        public GrProgressBar() : base()
        { }


        /// <summary>
        /// Implementation of base OnProgressBarDraw
        /// It draws a single active area from 0 to the percentage in value.
        /// </summary>
        protected override void OnProgressBarDraw()
        {
            this.DrawPercentage(0, this.Value);
        }


        /// <summary>
        /// Simple method to fill the whole progress bar within specified time.
        /// </summary>
        /// <param name="ms">Total mili seconds to consume.</param>
        /// <returns>Task to await process if required.</returns>
        public Task FillSmoothlyWithin(int ms)
        {
            this.Value = 0;
            return IncreaseSmoothlyWithin(ms, 100);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ms">Total mili seconds to consume.</param>
        /// <param name="pauses">Total of pauses to emulate.</param>
        /// <returns>Task to await process if required.</returns>
        public async Task FillWithPausesWithin(int ms, int pauses = 2)
        {
            this.Value = 0;

            var increments = pauses + 1;

            var ms4Pauses = (int) (ms * 0.22);
            var ms4Increments = ms - ms4Pauses;

            var msPerPause = ms4Pauses / pauses;
            var msPerIncrement = ms4Increments / increments;


            var percentagePerIncrement = 100 / increments;


            for (var i = 0; i < pauses; i++)
            {
                await IncreaseSmoothlyWithin(msPerIncrement, percentagePerIncrement);
                await Task.Delay(msPerPause);
            }

            await IncreaseSmoothlyWithin(msPerIncrement, 100 - this.Value);
        }


        /// <summary>
        /// Base Animator to increase Progress bar value.
        /// </summary>
        /// <param name="ms">Total mili seconds to consume.</param>
        /// <param name="increment">Increment to add into Value.</param>
        /// <returns>Task to await process if required.</returns>
        public Task IncreaseSmoothlyWithin(int ms, int increment)
        {
            return Task.Run(async () =>
            {
                var finalValue = this.Value + increment;
                if (finalValue > 100) finalValue = 100;
                var dx = finalValue - this.Value;

                if (dx <= 0)
                {
                    return;
                }

                var interval = SmoothIncrement * (ms / dx);

                while (true)
                {
                    await Task.Delay(interval);

                    this.Value += SmoothIncrement;
                    if (this.Value >= finalValue)
                    {
                        break;
                    }
                }
            });
        }
    }
}