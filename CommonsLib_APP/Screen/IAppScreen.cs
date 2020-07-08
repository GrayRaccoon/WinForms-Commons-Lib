using System;
using System.Threading.Tasks;

namespace CommonsLib_APP.Screen
{
    /// <summary>
    /// Base Application Screen
    /// </summary>
    /// <typeparam name="TScreenCtx">Application Context Type</typeparam>
    public interface IAppScreen<out TScreenCtx>
    {

        /// <summary>
        /// Screen Create State Event
        /// </summary>
        event Action ScreenCreate;
        
        /// <summary>
        /// Screen Resume State Event
        /// </summary>
        event Action ScreenResume;
        
        /// <summary>
        /// Screen Pause State Event
        /// </summary>
        event Action ScreenPause;
        
        /// <summary>
        /// Screen Destroy State Event
        /// </summary>
        event Action ScreenDestroy;

        /// <summary>
        /// Current App Screen Context.
        /// </summary>
        TScreenCtx Ctx { get; }

        /// <summary>
        /// Sets Current Screen Title.
        /// </summary>
        /// <param name="title"></param>
        void SetScreenTitle(string title);

        /// <summary>
        /// Displays quick toast message.
        /// </summary>
        /// <param name="msg">message to display</param>
        void ShowToast(string msg);

        /// <summary>
        /// Runs a Task in the app main thread. 
        /// </summary>
        /// <param name="uiTask">Task to be run</param>
        void RunOnUiThread(Action uiTask);
        
        /// <summary>
        /// Runs a Task in the app main thread.
        /// </summary>
        /// <param name="uiTask">Task to be run.</param>
        /// <typeparam name="T">Task return type.</typeparam>
        /// <returns>Task result</returns>
        Task<T> RunOnUiThread<T>(Func<T> uiTask);

        /// <summary>
        /// Main method where to reload screen data.
        /// </summary>
        void LoadData();

        /// <summary>
        /// Finishes current screen, removing it from application stack.
        /// </summary>
        void Finish();

    }

}