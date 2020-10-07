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

namespace m2w
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        EditText mEdit;
        Button button;
        string mystring = "";
        Call client;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource

            SetContentView(Resource.Layout.activity_main);
            //var client = new Call("ZO93Xed56gtIwxTTQzqnZ5lNjiRewoT7zet23Pp2");
            //client.RequestAccess();

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
            List<Video> videos;

            string query = mEdit.Text;
            query = query.Replace(" ", "+");
            if (string.IsNullOrWhiteSpace(query))
                mEdit.Hint = "Insert a title!";
            else
            {
                SetContentView(Resource.Layout.Results);
                videos = client.searchVideos(query);
                var videoList = FindViewById<ListView>(Resource.Id.Results);    //seleziono listview
                videoList.Adapter = new SearchResults(this, videos);
                videoList.ItemClick += (object sender, AdapterView.ItemClickEventArgs args) =>
                {
                    int i= 0;
                    Video selectedVideo = videos[args.Position];
                    System.Diagnostics.Debug.WriteLine("VIDEO ID: " + selectedVideo.id);

                    selectedVideo = client.GetFull(selectedVideo.id);

                    SetContentView(Resource.Layout.full);
                    FindViewById<TextView>(Resource.Id.SongTitle).Text = selectedVideo.song_title;
                    SpannableString content;
                    while(i<selectedVideo.artists.Count)
                    {
                        if (i == 0)
                        {
                            mystring = mystring + selectedVideo.artists[i].name;
                        }
                        else
                        {
                            mystring = mystring +", "+ selectedVideo.artists[i].name;
                        }
                        i=i+1;
                    }
                    content = new SpannableString(mystring);
                    content.SetSpan(new UnderlineSpan(), 0, selectedVideo.artists[0].name.Length, 0);  
                    FindViewById<TextView>(Resource.Id.Artist).TextFormatted = content; 


                    //FindViewById<TextView>(Resource.Id.Artist).CallOnClick();
                    mystring = "https://www.youtube.com/watch?v=" + selectedVideo.sources[0].source_data;
                    content = new SpannableString(mystring);
                    content.SetSpan(new UnderlineSpan(), 0, mystring.Length, 0);
                    FindViewById<TextView>(Resource.Id.Watch).TextFormatted = FindViewById<TextView>(Resource.Id.Watch).TextFormatted = content;
                    TextView watch = FindViewById<TextView>(Resource.Id.Watch);
                    watch.Click += WatchVideo;
                    FindViewById<TextView>(Resource.Id.Featured).Text = selectedVideo.featured_artists[0].name;
                    FindViewById<TextView>(Resource.Id.Views).Text = selectedVideo.popularity.views_all_time_formatted;
                    FindViewById<TextView>(Resource.Id.VideoId).Text = selectedVideo.id.ToString();
                    FindViewById<TextView>(Resource.Id.Release).Text = selectedVideo.release_date_string;
                    FindViewById<TextView>(Resource.Id.Director).Text = selectedVideo.directors[0].entity_name;
                    i = 0; mystring = "";
                    System.Diagnostics.Debug.WriteLine("count: " + selectedVideo.credits.crew.Count);
                    //System.Diagnostics.Debug.WriteLine("activity main: " + selectedVideo.credits.crew[0].entity_name);
                    //while (selectedVideo.credits.crew[i] != null) {
                    foreach(Crew c in selectedVideo.credits.crew) { 
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
                };
            }
            return true;
        }

        public void WatchVideo(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(mystring));
            StartActivity(intent);
        }
    }
}