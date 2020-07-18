using System;
using System.Linq;
using System.Threading.Tasks;
using CommonsLib_APP.Screen;
using CommonsLib_IOC.Config;
using Foundation;
using GlobalToast;
using Serilog;
using UIKit;

namespace CommonsLib_iOS.Screen
{
    /// <summary>
    /// Base iOS Screen with common screen utilities.
    /// </summary>
    public class BaseViewController: UIViewController, IAppScreen<BaseViewController>
    {
        private ILogger _logger;

        public ILogger Logger
        {
            get => _logger;
            set => _logger = value.ForContext(GetType());
        }

        private Toast _toast;
        
        private bool _isScreenResume = false;
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        private NSObject _didDisconnectNotificationObserver;
        
        public BaseViewController(IntPtr handle) : base(handle) { }

        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public event Action ScreenCreate;
        
        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public event Action ScreenResume;
        
        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public event Action ScreenPause;
        
        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public event Action ScreenDestroy;
        
        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public BaseViewController Ctx => this;
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            IoCManager.Resolver.InjectProperties(this);

            ScreenCreate += OnCreate;
            ScreenResume += OnResume;
            ScreenPause += OnPause;
            ScreenDestroy += OnDestroy;
            
            AddAppStatusEvents();
            
            RunOnUiThread(() => ScreenCreate?.Invoke());
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            AddAppStatusEvents();
            NotifyScreenMightResume();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NotifyScreenMightPause();
        }

        protected virtual void OnCreate() { }

        protected virtual void OnResume()
        {
            LoadData();
        }
        protected virtual void OnPause() { }
        protected virtual void OnDestroy() { }
        
        private void AddAppStatusEvents()
        {
            if (_didBecomeActiveNotificationObserver == null)
                _didBecomeActiveNotificationObserver = UIApplication.Notifications.ObserveDidBecomeActive(
                        delegate { NotifyScreenMightResume(); });
            
            if (_willResignActiveNotificationObserver == null)
                _willResignActiveNotificationObserver = UIApplication.Notifications.ObserveWillResignActive( 
                    delegate { NotifyScreenMightPause(); });
            
            if (_didDisconnectNotificationObserver == null)
                _didDisconnectNotificationObserver = UIScene.Notifications.ObserveDidDisconnect(delegate
                    {
                        RemoveAppStatusEvents();
                        RunOnUiThread(() => ScreenDestroy?.Invoke());
                    });
        }

        private void RemoveAppStatusEvents()
        {
            if (_didBecomeActiveNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_didBecomeActiveNotificationObserver);

            if (_willResignActiveNotificationObserver == null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_willResignActiveNotificationObserver);
            
            if (_didDisconnectNotificationObserver == null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_didDisconnectNotificationObserver);
        }

        private void NotifyScreenMightResume()
        {
            if (_isScreenResume) return;
            RunOnUiThread(() => ScreenResume?.Invoke());
            _isScreenResume = true;
        }

        private void NotifyScreenMightPause()
        {
            if (!_isScreenResume) return;
            RunOnUiThread(() => ScreenPause?.Invoke());
            _isScreenResume = false;
        }

        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public void SetScreenTitle(string title)
        {
            if (NavigationItem == null)
                return;
            NavigationItem.Title = title;
        }

        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public void ShowToast(string msg)
        {
            if (_toast == null)
                _toast = Toast.MakeToast(msg);
            else
            {
                _toast.Dismiss();
                _toast.SetMessage(msg);
            }
            _toast.Show();
        }

        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public void RunOnUiThread(Action uiTask)
        {
            InvokeOnMainThread(uiTask);
        }

        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public Task<T> RunOnUiThread<T>(Func<T> uiTask)
        {
            var t = new TaskCompletionSource<T>();
            RunOnUiThread(() =>
            {
                var taskResult = uiTask();
                t.TrySetResult(taskResult);
            });
            return t.Task;
        }

        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public virtual void LoadData()
        {
            //
        }

        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public void Finish()
        {
            if (NavigationController == null)
                DismissViewController(true, null);
            else if (Equals(this, NavigationController.VisibleViewController))
                NavigationController.PopViewController(true);
            else if (NavigationController.ViewControllers.Contains(this))
                NavigationController.ViewControllers = NavigationController.ViewControllers
                    .Where(vc => Equals(this, vc))
                    .ToArray();
        }

        /// <summary>
        /// Starts a new screen.
        /// </summary>
        /// <param name="viewController">Screen to start.</param>
        public void StartScreen(UIViewController viewController)
        {
            if (NavigationController != null)
                NavigationController.PushViewController(viewController, true);
            else
                PresentViewController(viewController, true, null);
        }

    }
}