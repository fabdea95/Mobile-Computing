using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Collections.Generic;
using Imvdb.LibreriaImvdb;
using Android.Text;
using Android.Graphics;
using Android.Text.Style;
using System.Net;
using System;
using Android.Content;
//using Android.Content.Res;

namespace m2w
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        EditText mEdit;
        Button button;
        string mystring = "", watch_url="";
        Call client;
        List<Video> videos;
        Navigation nav;
        Video selectedVideo;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource

            SetContentView(Resource.Layout.activity_main);
            //var client = new Call("ZO93Xed56gtIwxTTQzqnZ5lNjiRewoT7zet23Pp2");
            //client.RequestAccess();
            this.nav = new Navigation();

            //SetContentView(Resource.Layout.activity_main);
            button = FindViewById<Button>(Resource.Id.Button);
            mEdit = FindViewById<EditText>(Resource.Id.search_query);

            //button.Click += buttonClicked;
            client = new Call();
            button.Click += (e, o) => {
                this.Search();
                //string query =  mEdit.Text;
                //query = query.Replace(" ", "+");

                //SetContentView(Resource.Layout.Results);

                //client.RequestAccess(query);

            };
        }

        public bool Search()
        {

            string query = mEdit.Text;
            query = query.Replace(" ", "+");
            if (string.IsNullOrWhiteSpace(query))
                mEdit.Hint = "Insert a title!";
            else
            {
                nav.BackCheck = 1;
                SetContentView(Resource.Layout.Results);
                videos = client.searchVideos(query);
                var videoList = FindViewById<ListView>(Resource.Id.Results);    //seleziono listview
                videoList.Adapter = new SearchResults(this, videos);
                videoList.ItemClick += (object sender, AdapterView.ItemClickEventArgs args) =>
                {
                    
                    nav.BackCheck = 2;
                    selectedVideo = videos[args.Position];
                    System.Diagnostics.Debug.WriteLine("VIDEO ID: " + selectedVideo.id);

                    selectedVideo = client.GetFull(selectedVideo.id);
                    FullVideoInfo(selectedVideo);
                    
                };
            }
            return true;
        }

        public void FullVideoInfo(Video selectedVideo){
            SetContentView(Resource.Layout.full);
            FindViewById<TextView>(Resource.Id.SongTitle).Text = selectedVideo.song_title;
            SpannableString content;
            int i = 0;
            mystring = "";
            //while (i < selectedVideo.artists.Count)
            //foreach(Artists a in selectedVideo.artists)
            //{
            /*
                if (i == 0)
            {
                mystring = mystring + selectedVideo.artists[i].name;
            }
            else
            {
                mystring = mystring + ", " + selectedVideo.artists[i].name;
            }
            i = i + 1;*/
            System.Diagnostics.Debug.WriteLine("artist name: "+ selectedVideo.artists[0].name);
            mystring = selectedVideo.artists[0].name;//mystring + a.name + " ";
            //}
            content = new SpannableString(mystring);
            content.SetSpan(new UnderlineSpan(), 0, mystring.Length, 0);
            FindViewById<TextView>(Resource.Id.Artist).TextFormatted = content;


            //FindViewById<TextView>(Resource.Id.Artist).CallOnClick();
            mystring = "https://www.youtube.com/watch?v=" + selectedVideo.sources[0].source_data;
            content = new SpannableString(mystring);
            content.SetSpan(new UnderlineSpan(), 0, mystring.Length, 0);
            FindViewById<TextView>(Resource.Id.Watch).TextFormatted = FindViewById<TextView>(Resource.Id.Watch).TextFormatted = content;
            TextView watch = FindViewById<TextView>(Resource.Id.Watch);
            watch.Click += WatchVideo;
            if (selectedVideo.featured_artists.Count>0)
            {
                FindViewById<TextView>(Resource.Id.Featured).Text = selectedVideo.featured_artists[0].name;
            }
            FindViewById<TextView>(Resource.Id.Views).Text = selectedVideo.popularity.views_all_time_formatted;
            FindViewById<TextView>(Resource.Id.VideoId).Text = selectedVideo.id.ToString();
            FindViewById<TextView>(Resource.Id.Release).Text = selectedVideo.release_date_string;
            if (selectedVideo.directors.Count > 0)
            {
                FindViewById<TextView>(Resource.Id.Director).Text = selectedVideo.directors[0].entity_name;
            }
            i = 0; mystring = "";
            System.Diagnostics.Debug.WriteLine("count: " + selectedVideo.credits.crew.Count);
            //System.Diagnostics.Debug.WriteLine("activity main: " + selectedVideo.credits.crew[0].entity_name);
            //while (selectedVideo.credits.crew[i] != null) {
            foreach (Crew c in selectedVideo.credits.crew)
            {
                mystring = mystring + c.position_name + ": " + c.entity_name + "\n";

            }
            FindViewById<TextView>(Resource.Id.Crew).Text = mystring;
            mystring = "";
            foreach (Cast c in selectedVideo.credits.cast)
            {
                mystring = mystring + c.entity_name + "\n";

            }
            FindViewById<TextView>(Resource.Id.Cast).Text = mystring;
            Bitmap imageBitmap = null;
            try
            {
                using (var webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(selectedVideo.image.o);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                        FindViewById<ImageView>(Resource.Id.Full_img).SetImageBitmap(imageBitmap);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception is thrown. Message is :" + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception is thrown. Message is :" + e.Message);
            }

        }


        public void WatchVideo(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(watch_url));
            StartActivity(intent);
        }

        public override void OnBackPressed()
        {
            // Check if the user is in the poster view or not. If not it exit the app, otherwise adds back all the results to the table and change the 
            // BackCheck value to true
            if (this.nav.BackCheck==0)
                base.OnBackPressed();               
            else
            {
                nav.BackCheck--;
                switch (nav.BackCheck)
                {
                    case 0:
                        SetContentView(Resource.Layout.activity_main);
                        button = FindViewById<Button>(Resource.Id.Button);
                        mEdit = FindViewById<EditText>(Resource.Id.search_query);
                        button.Click += (e, o) => {
                            this.Search();
                        };
                        break;
                    case 1:
                        SetContentView(Resource.Layout.Results);
                        var videoList = FindViewById<ListView>(Resource.Id.Results);    //seleziono listview
                        videoList.Adapter = new SearchResults(this, videos);
                        nav.BackCheck = 1;
                        videoList.ItemClick += (object sender, AdapterView.ItemClickEventArgs args) =>
                        {

                            nav.BackCheck = 2;
                            selectedVideo = videos[args.Position];
                            System.Diagnostics.Debug.WriteLine("VIDEO ID: " + selectedVideo.id);

                            selectedVideo = client.GetFull(selectedVideo.id);
                            System.Diagnostics.Debug.WriteLine("id: ", selectedVideo.id);
                            FullVideoInfo(selectedVideo);
                        };
                        break;
                }
                
            }
        }
    }
}