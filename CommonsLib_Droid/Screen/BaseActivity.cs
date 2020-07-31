using System;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Text;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using CheeseBind;
using CommonsLib_APP.Screen;
using CommonsLib_APP.Settings;
using CommonsLib_DAL.Extensions;
using CommonsLib_DAL.Utils;
using CommonsLib_Droid.Attributes;
using CommonsLib_Droid.Extensions;
using CommonsLib_IOC.Config;
using Serilog;

namespace CommonsLib_Droid.Screen
{
    /// <summary>
    /// Base Android Screen with common screen utilities.
    /// </summary>
    public abstract class BaseActivity: AppCompatActivity, IAppScreen<BaseActivity>
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
        
        /// <summary>
        /// Current screen layout resource id.
        /// </summary>
        protected abstract int ScreenResourceId { get; }
        
        /// <summary>
        /// Optional toolbar view id.
        /// </summary>
        protected virtual int ScreenToolbarId => 0;

        /// <summary>
        /// Current Screen toolbar
        /// </summary>
        protected Android.Support.V7.Widget.Toolbar ScreenToolbar { get; set; }

        private string ToolbarTitleColor { get; set; } = null;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            IoCManager.Resolver.InjectProperties(this);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            
            SetContentView(ScreenResourceId);
            Cheeseknife.Bind(this);
            LoadExtras();
            
            ScreenToolbar = FindViewById(ScreenToolbarId) 
                as Android.Support.V7.Widget.Toolbar;
            if (ScreenToolbar != null)
                SetSupportActionBar(ScreenToolbar);

            LoadCustomScreenTint();
            
            ScreenCreate += OnCreate;
            RunOnUiThread(() => ScreenCreate?.Invoke());
        }

        protected virtual void OnCreate() { }
        
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
            if (ToolbarTitleColor == null) return;
            UpdateToolbarTitleColor(ToolbarTitleColor);
        }

        protected void UpdateToolbarTitleColor(string hexColor)
        {
            SupportActionBar.TitleFormatted = HtmlCompat.FromHtml(
                $"<font color='{hexColor}'>{SupportActionBar.Title}</font>",
                HtmlCompat.FromHtmlModeLegacy);
        }
        
        /// <inheritdoc cref="IAppScreen{TScreenCtx}"/>
        public void SetScreenTint(string hexToolbarBgColor, 
            string hexToolbarTitleColor = null,
            string hexSystemColor = null)
        {
            if (SupportActionBar == null) return;
            
            var colorDrawable = new ColorDrawable(Color.ParseColor(hexToolbarBgColor));
            SupportActionBar.SetBackgroundDrawable(colorDrawable);

            if (!string.IsNullOrEmpty(hexToolbarTitleColor))
                UpdateToolbarTitleColor(hexToolbarTitleColor);

            if (!string.IsNullOrEmpty(hexSystemColor) && Build.VERSION.SdkInt > BuildVersionCodes.Lollipop)
            {
                Window.ClearFlags(WindowManagerFlags.TranslucentNavigation);
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                var systemColor = Color.ParseColor(hexSystemColor);
                Window.SetStatusBarColor(systemColor);
                Window.SetNavigationBarColor(systemColor);
            }

            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
        }

        /// <summary>
        /// Sets home button to return previous activity,
        /// ParentActivity needs to be configured.
        /// </summary>
        protected void SetupHomeButtonInToolbar()
        {
            if (SupportActionBar == null) return;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowCustomEnabled(true);
        }

        /// <summary>
        /// Loads global screen tint if it is enabled.
        /// </summary>
        protected void LoadCustomScreenTint()
        {
            if (!GlobalTintSettings.UseCustomTint) return;
            
            SetScreenTint(GlobalTintSettings.ToolbarBackgroundColor,
                hexToolbarTitleColor: GlobalTintSettings.ToolbarTitleColor,
                hexSystemColor: GlobalTintSettings.SystemStatusColor);
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


        /// <summary>
        /// Load object extras from Activity Intent.
        /// </summary>
        protected void LoadExtras()
        {
            var properties = ObjectAttributesUtils
                .GetPropertiesInfoForAttribute<ExtraParamAttribute>(GetType());

            foreach (var propertyInfo in properties)
            {
                var propAttribute = propertyInfo.GetAttribute<ExtraParamAttribute>();
                if (propAttribute == null) continue;
                var extraName = string.IsNullOrEmpty(propAttribute.Name) 
                    ? propertyInfo.Name : propAttribute.Name;
                if (!Intent.HasExtra(extraName)) continue;

                var foundVal = Intent.GetObjectExtra(extraName, propertyInfo.PropertyType);
                propertyInfo.SetValue(this, foundVal);
            }
        }
        
        
    }
}