using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Imvdb.LibreriaImvdb;
using System.Collections.Generic;
using Android.Text;
using Android.Text.Style;
using Android.Graphics;
using System.Net;
using System;
using Android.Content;
using Android.Views;

namespace musicTW
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        EditText mEdit;
        Button button;
        string mystring = "", ytstring= "", vmstring= "", watch_url = "";
        Call client;
        List<Video> videos;
        Navigation nav;
        Video selectedVideo;
        ImageButton ytbutton, vmbutton;
        Imvdb.LibreriaImvdb.Entity selectedEntity;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            this.nav = new Navigation();

            //SetContentView(Resource.Layout.activity_main);
            button = FindViewById<Button>(Resource.Id.Button);
            mEdit = FindViewById<EditText>(Resource.Id.search_query);

            //button.Click += buttonClicked;
            client = new Call();
            button.Click += (e, o) => {
                this.Search();

            };
        }

        public bool Search() /*RICERCA*/
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
                var videoList = FindViewById<ListView>(Resource.Id.Result);    //seleziono listview
                videoList.Adapter = new SearchResults(this, videos);
                //SetContentView(Resource.Layout.Results);
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


        public void FullVideoInfo(Video selectedVideo) /*VISUALIZZA INFO VIDEO*/
        {
            SetContentView(Resource.Layout.full);
            FindViewById<TextView>(Resource.Id.SongTitle).Text = selectedVideo.song_title;
            SpannableString content;
            int i = 0;
            mystring = "";

            System.Diagnostics.Debug.WriteLine("artist name: " + selectedVideo.artists[0].name);
            mystring = selectedVideo.artists[0].name;//mystring + a.name + " ";
            content = new SpannableString(mystring);
            content.SetSpan(new UnderlineSpan(), 0, mystring.Length, 0);
            FindViewById<TextView>(Resource.Id.Artist).Text = selectedVideo.artists[0].name;
            /*
            TextView artist = FindViewById<TextView>(Resource.Id.Artist);
            artist.Click += (e, o) => {
                SetContentView(Resource.Layout.entity);
                selectedEntity = client.GetFullEntity(selectedVideo.artists[0].slug);
                FullEntity(selectedEntity);
            };
            */
            //FindViewById<TextView>(Resource.Id.Artist).CallOnClick();
            for(i=0;i< selectedVideo.sources.Count;i++) {
                if (selectedVideo.sources[i].source == "youtube")
                {
                    ytstring = "https://www.youtube.com/watch?v=" + selectedVideo.sources[i].source_data;
                    ytbutton = (ImageButton)FindViewById(Resource.Id.youtube);
                    ytbutton.Visibility = ViewStates.Visible;
                    ytbutton.Click += (e, o) =>
                    {
                        this.WatchVideo(e, o, ytstring);
                    };
                }
                if (selectedVideo.sources[i].source == "vimeo")
                {
                    vmstring = "https://www.vimeo.com/" + selectedVideo.sources[i].source_data;
                    vmbutton = (ImageButton)FindViewById(Resource.Id.vimeo);
                    vmbutton.Visibility = ViewStates.Visible;
                    vmbutton.Click += (e, o) =>
                    {
                        this.WatchVideo(e, o, vmstring);
                    };
                }
            }         
           
            if (selectedVideo.featured_artists.Count > 0)
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
                mystring = mystring + "\t" + c.position_name +  ":\n";

            }
            FindViewById<TextView>(Resource.Id.Crew1).Text = mystring;
            mystring = "";
            foreach (Crew c in selectedVideo.credits.crew)
            {
                mystring = mystring +  c.entity_name + "\n";

            }
            FindViewById<TextView>(Resource.Id.Crew2).Text = mystring;
            mystring = "";
            foreach (Cast c in selectedVideo.credits.cast)
            {
                mystring = mystring + "\t" + c.entity_name + "\n";

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

        public void FullEntity(Imvdb.LibreriaImvdb.Entity selectedEntity)
        {
            SetContentView(Resource.Layout.entity);
            FindViewById<TextView>(Resource.Id.ArtistName).Text = selectedEntity.Name;
            FindViewById<TextView>(Resource.Id.Bio).Text = selectedEntity.Bio;
            Bitmap imageBitmap = null;
            try
            {
                using (var webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(selectedEntity.Image);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                        FindViewById<ImageView>(Resource.Id.Full_img_e).SetImageBitmap(imageBitmap);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception is thrown. Message is :" + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception is thrown. Message is :" + e.Message);
            }

        }

        public void WatchVideo(object sender, System.EventArgs e, string link)
        {
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(link));
            //Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(watch_url));
            StartActivity(intent);
        }

        public override void OnBackPressed()    /*GESTISCE LE TRANSIZIONI TRA LE VARIE SCHERMATE*/
        {

            if (this.nav.BackCheck == 0)        /* se ci si trova sulla schermata principale, chiudi app*/
                base.OnBackPressed();
            else                                /*altrimenti, aggiorna flag schermata*/
            {
                nav.BackCheck--;
                switch (nav.BackCheck)
                {
                    case 0:                     /*se flag=0, schermata_principale*/
                        SetContentView(Resource.Layout.activity_main);
                        button = FindViewById<Button>(Resource.Id.Button);
                        mEdit = FindViewById<EditText>(Resource.Id.search_query);
                        button.Click += (e, o) => {
                            this.Search();
                        };
                        break;
                    case 1:                     /*se flag=1, risultati ricerca*/
                        SetContentView(Resource.Layout.Results);
                        var videoList = FindViewById<ListView>(Resource.Id.Result);    //seleziono listview
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