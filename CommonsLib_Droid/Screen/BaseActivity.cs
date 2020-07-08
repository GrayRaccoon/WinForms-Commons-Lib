using System;
using System.Threading.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Widget;
using CommonsLib_APP.Screen;
using CommonsLib_IOC.Config;
using Serilog;

namespace CommonsLib_Droid.Screen
{
    /// <summary>
    /// Base Android Screen with common screen utilities.
    /// </summary>
    public class BaseActivity: AppCompatActivity, IAppScreen<BaseActivity>
    {
        private ILogger _logger;

        public ILogger Logger
        {
            get => _logger;
            set => _logger = value.ForContext(GetType());
        }
        
        private Toast _toast;

        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public event Action ScreenCreate;
        
        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public event Action ScreenResume;
        
        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public event Action ScreenPause;
        
        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public event Action ScreenDestroy;

        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public BaseActivity Ctx => this;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            IoCManager.Resolver.InjectProperties(this);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            RunOnUiThread(() => ScreenCreate?.Invoke());
        }

        protected override void OnResume()
        {
            base.OnResume();
            RunOnUiThread(() => ScreenResume?.Invoke());
            LoadData();
        }

        protected override void OnPause()
        {
            base.OnPause();
            RunOnUiThread(() => ScreenPause?.Invoke());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RunOnUiThread(() => ScreenDestroy?.Invoke());
        }

        public override void OnRequestPermissionsResult(
            int requestCode, string[] permissions, 
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public void SetScreenTitle(string title)
        {
            if (SupportActionBar == null) return;
            SupportActionBar.Title = title;
        }

        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public void ShowToast(string msg)
        {
            if (_toast == null)
                _toast = Toast.MakeText(Ctx, msg, ToastLength.Long);
            else
                _toast.SetText(msg);
            _toast.Show();
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
    }
}