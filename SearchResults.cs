using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

using Imvdb.LibreriaImvdb;
using System.Security.Policy;
using System.Net;
using Android;
//using mtw;

namespace m2w
{
    public class SearchResults : BaseAdapter<Video>
    {
        private readonly Activity _context;
        private readonly IList<Video> _videos;

        public SearchResults(Activity context, IList<Video> videos)
        {
            _context = context;
            _videos = videos;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView
                        ?? _context.LayoutInflater.Inflate(Resource.Layout.Video, null);
            var video = _videos[position];
            //view.FindViewById<ImageView>(Resource.Id.Anteprima).SetImageURI((Uri).Parse(video.Image.t));
            var imageBitmap = GetImageBitmapFromUrl(video.image.b);
            var imagen = view.FindViewById<ImageView>(Resource.Id.Anteprima_img);
            imagen.SetImageBitmap(imageBitmap);
            view.FindViewById<TextView>(Resource.Id.SongTitle).Text = video.song_title;
            view.FindViewById<TextView>(Resource.Id.Artist).Text = video.artists[0].name;
            return view;
        }

        public override int Count
        {
            get { return _videos.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public override Video this[int position]
        {
            get { return _videos[position]; }
        }
        public Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;
            try
            {
                using (var webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception is thrown. Message is :" + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception is thrown. Message is :" + e.Message);
            }
            return imageBitmap;
        }
    }
}