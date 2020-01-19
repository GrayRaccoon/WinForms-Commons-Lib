using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonsLib_DAL.Data;
using Serilog.Core;

namespace CommonsLib_FLL.Forms
{
    /// <summary>
    /// Use this base class to get some base form functions and 
    /// to have a common class to integrate with the ActivityManager Stack.
    /// </summary>
    public class BaseFormScreen : Form
    {
        protected Logger Logger => LocalLogger.MainLogger;

        /// <summary>
        /// Finish Form life cycle by trying to: hide, close and then dispose current form object.
        /// </summary>
        protected void Finish()
        {
            this.Hide();
            this.Close();
            this.Dispose();
        }

        /// <summary>
        /// Start new Form and hidde caller form, but keep it in window stack.
        /// </summary>
        /// <param name="form">New Form to start.</param>
        /// <param name="useSamePosition">Whether or not to set same caller form location into new the form.</param>
        protected void StartForm(BaseFormScreen form, bool useSamePosition = true)
        {
            if (useSamePosition)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = this.Location;
            }
            form.Show();
            this.Hide();
            form.Activate();
        }

        /// <summary>
        /// When a form is loaded, let's add it to the ActivityManager Form Stack.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Logger.Information($"Started screen: {this.GetType().Name}");
            ActivityManager.Instance?.AddToStack(this);
        }

        /// <summary>
        /// When a form is closing, let's remove from the ActivityManager Form Stack.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Logger.Information($"Closing Screen: {this.GetType().Name}");
            ActivityManager.Instance?.RemoveFromStack(this);
        }

        /// <summary>
        /// Every time an active form location is updated, Let's track the location in the activity manager.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            if (ActivityManager.Instance != null)
            {
                ActivityManager.Instance.LastFormLocation = this.Location;
            }
        }

        /// <summary>
        /// Activate loading cursor, call this method before starting a long blocking process.
        /// </summary>
        protected void ActivateLoadingCursor()
        {
            this.Cursor = Cursor.Current = Cursors.WaitCursor;
        }

        /// <summary>
        /// Deactivate custom cursor, call this method after finishing a process that changes the cursor.
        /// </summary>
        protected void DeactivateCustomCursor()
        {
            this.Cursor = Cursor.Current = Cursors.Default;
        }


        /// <summary>
        /// Runs a sent Action in the UI Thread.
        /// </summary>
        /// <param name="uiTask">Task to run on UI Thread.</param>
        public void RunOnUiThread(Action uiTask)
        {
            Invoke((MethodInvoker)delegate { uiTask(); });
        }

        /// <summary>
        /// Runs a sent Action in the UI Thread and waits for its completion.
        /// </summary>
        /// <param name="uiTask">Task to run on UI Thread</param>
        /// <typeparam name="T">Type of return</typeparam>
        /// <returns>Generated Task to await.</returns>
        public Task<T> RunOnUiThread<T>(UiTaskActionWithReturn<T> uiTask)
        {
            var t = new TaskCompletionSource<T>();
            this.RunOnUiThread(() =>
            {
                t.TrySetResult(
                    uiTask()
                );
            });
            return t.Task;
        }
        
    }
    
    /// <summary>
    /// Delegate to use while calling RunOnUiThread.
    /// </summary>
    /// <typeparam name="T">Expected return type.</typeparam>
    public delegate T UiTaskActionWithReturn<out T>();

}