using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Nio.Channels;

namespace simple_Camera
{
    [Activity(Label = "SettingsActivity")]
    public class SettingsActivity : Activity
    {
        TextView tvShowPaths;
        Button copy;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_settings);

            tvShowPaths = (TextView)FindViewById(Resource.Id.tvShowPaths);
            tvShowPaths.Text = "Witam w Settingsach";
            copy = (Button)FindViewById(Resource.Id.copy);

            copy.Click += CopyFunction;

        }

        private void CopyFunction(object sender, EventArgs e)
        {
            if(Directory.Exists(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).ToString() + "/PrzegladyZdjecia"))
            {
                Toast.MakeText(this, "Działa wiec kopiujemy do nowej", ToastLength.Long).Show();
                try
                {
                    string sourceDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).ToString() + "/PrzegladyZdjecia";
                    string destDir = MainActivity.GetBaseFolderPath(true) + "/CheckThisOutOnSD";
                    string[] picList = Directory.GetFiles(sourceDir, "*.jpg");
                    
                    if(!Directory.Exists(destDir))
                        Directory.CreateDirectory(destDir);

                    foreach (string f in picList)
                    {
                        Toast.MakeText(this, f, ToastLength.Long).Show();
                        string fName = f.Substring(sourceDir.Length + 1);
                        System.IO.File.Copy(Path.Combine(sourceDir, fName), Path.Combine(destDir, fName), true);
                    }

                    foreach (string f in picList)
                    {
                        System.IO.File.Delete(f);
                    }

                } catch (DirectoryNotFoundException dirNotFound) {
                    Toast.MakeText(this, dirNotFound.ToString(), ToastLength.Long).Show();
                }
                //Copy(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).ToString() + "/newDirectory", Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).ToString() + "/CheckThis");
                Toast.MakeText(this, "DONE", ToastLength.Long).Show();
            }
        }
    }
}