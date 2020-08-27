using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Imvdb.LibreriaImvdb;
using System;
using Android.Content;

namespace mtw
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class mtwActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            //var client = new Call("ZO93Xed56gtIwxTTQzqnZ5lNjiRewoT7zet23Pp2");
            //client.RequestAccess();

            //SetContentView(Resource.Layout.activity_main);
            var button = FindViewById<Button>(Resource.Id.Button);
            var mEdit = FindViewById<EditText>(Resource.Id.search_query);
            //var viewR = FindViewById<TextView>(Resource.Id.Results);
            //button.Click += buttonClicked;
            button.Click += (e, o) => {
                string query = mEdit.Text;
                query = query.Replace(" ", "+");
                //viewR.Text = query;
                SetContentView(Resource.Layout.Results);
                var client = new Call("ZO93Xed56gtIwxTTQzqnZ5lNjiRewoT7zet23Pp2",
                    videos =>
                    {
                        RunOnUiThread(() =>
                        {

                            var videoList = FindViewById<ListView>(Resource.Id.Results);    //seleziono listview
                            videoList.Adapter = new SearchResults(this, videos);
                            videoList.ItemClick += (object sender, AdapterView.ItemClickEventArgs args) =>
                            {
                                var selectedVideo = videos[args.Position];
                                SetContentView(Resource.Layout.full);
                                //client.GetFull(selectedVideo);
                                
                                FindViewById<TextView>(Resource.Id.SongTitle).Text = selectedVideo.song_title;
                                FindViewById<TextView>(Resource.Id.Artist).Text = selectedVideo.Artist.name;
                                FindViewById<TextView>(Resource.Id.Year).Text = selectedVideo.year;
                                FindViewById<TextView>(Resource.Id.ImvdbPick).Text = selectedVideo.is_imvdb_pick.ToString();
                                FindViewById<TextView>(Resource.Id.Url).Text = selectedVideo.url;
                                
                                FindViewById<TextView>(Resource.Id.VideoId).Text = selectedVideo.id.ToString();
                                /*
                                var client2 = new Call("ZO93Xed56gtIwxTTQzqnZ5lNjiRewoT7zet23Pp2",
                                fullvideo =>
                                {
                                    RunOnUiThread(() =>
                                    {
                                        var full = fullvideo;
                                        FindViewById<TextView>(Resource.Id.SongTitle).Text = selectedVideo.song_title;
                                        FindViewById<TextView>(Resource.Id.Artist).Text = selectedVideo.Artist.name;
                                        FindViewById<TextView>(Resource.Id.Year).Text = selectedVideo.year;
                                        FindViewById<TextView>(Resource.Id.ImvdbPick).Text = selectedVideo.is_imvdb_pick.ToString();
                                        FindViewById<TextView>(Resource.Id.Url).Text = selectedVideo.url;
                                        //full.release_date_string;
                                    });
                                }
                                );*/
                            };


                        });

                    }
                    //fullvideo 
                    );
               
                client.RequestAccess(query);
                //long fullid = long.Parse(FindViewById<TextView>(Resource.Id.VideoId).Text);
                //client.GetFull(fullid);
                //videos[]
                /*var client2 = new Call("ZO93Xed56gtIwxTTQzqnZ5lNjiRewoT7zet23Pp2",
                    videos =>
                    {
                        RunOnUiThread(() =>
                        {
                            client.GetFull(long.Parse(FindViewById<TextView>(Resource.Id.VideoId).Text));
                            //SetContentView(Resource.Layout.full);
                            //client.GetFull(selectedVideo);

                           

                        });
                    });*/
                   
                   
            
                
                //var fullView = FindViewById(Resource.Layout.full);
                //SetContentView(Resource.Layout.full);
                //System.Diagnostics.Debug.WriteLine(FindViewById<TextView>(Resource.Id.VideoId).Text);
                //client.GetFull(long.Parse(FindViewById<TextView>(Resource.Id.VideoId).Text));

            };
            

        }
    }
}