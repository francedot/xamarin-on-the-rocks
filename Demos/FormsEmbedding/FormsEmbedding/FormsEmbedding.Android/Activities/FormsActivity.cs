﻿using System.Reflection;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace FormsEmbedding.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class FormsActivity : FormsAppCompatActivity
    {
        public static bool IsFormsInitialized;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var pageName = Intent.Extras.GetString("PageType");
            var fullName = typeof(FormsApp).Namespace + ".Pages." + pageName;
            var pageType = typeof(FormsApp).Assembly.GetType(fullName);

            if (!IsFormsInitialized)
            {
                global::Xamarin.Forms.Forms.Init(this, bundle);
                IsFormsInitialized = true;
            }

            LoadApplication(new FormsApp(pageType));
        }
    }
}