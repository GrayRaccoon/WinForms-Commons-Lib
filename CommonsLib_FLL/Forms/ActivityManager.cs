using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CommonsLib_FLL.Forms
{
    /// <summary>
    /// Class to handle Application windows management.
    /// </summary>
    public class ActivityManager: ApplicationContext
    {
        /// <summary>
        /// Attribute to track the last available Active Form location.
        /// </summary>
        public Point LastFormLocation { get; set; } = new Point(20, 20);

        /// <summary>
        /// Internal Attribute to track the current Activities Stack
        /// </summary>
        private List<BaseFormScreen> FormStack { get; set; } = new List<BaseFormScreen>();

        /// <summary>
        /// Activity Manager Public Instance.
        /// There will be only one instance of ActivityManager for the whole App.
        /// </summary>
        public static ActivityManager Instance;

        /// <summary>
        /// Private ActivityManager Constructor
        /// </summary>
        private ActivityManager() :base() { }

        /// <summary>
        /// Factory method where to create ActivityManager instance,
        /// this method must be called only once
        /// </summary>
        /// <param name="entryPoint">Application Entry point FormScreen, this might be the main screen or a splash screen.</param>
        /// <returns></returns>
        public static ActivityManager CreateActivityManagerInstance(BaseFormScreen entryPoint)
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("Already Active Activity Manager");
            }
            Instance = new ActivityManager();
            entryPoint.Show();
            return Instance;
        }

        /// <summary>
        /// Internal method to add a newly loaded form into the Forms Stack.
        /// </summary>
        /// <param name="form">Newly loaded form.</param>
        internal void AddToStack(BaseFormScreen form)
        {
            this.FormStack.Add(form);
        }

        /// <summary>
        /// Internal method to remove a form from the Forms Stack.
        /// This method will also display the previous active form screen.
        /// </summary>
        /// <param name="form">Form to be removed from stack.</param>
        internal void RemoveFromStack(BaseFormScreen form)
        {
            this.FormStack.Remove(form);

            // If there are no more active forms, stop the Application.
            if (this.FormStack.Count == 0)
            {
                ExitThread();
                return;
            }

            // If there are active forms, display the previous using last tracked form location.
            var parentForm = this.FormStack.Last();
            parentForm.StartPosition = FormStartPosition.Manual;
            parentForm.Location = this.LastFormLocation;
            parentForm.Show();
        }

    }
}