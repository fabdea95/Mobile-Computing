using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Imvdb.LibreriaImvdb;

namespace mtw
{
    public class SearchResults : BaseAdapter <Video> 
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

            
            view.FindViewById<TextView>(Resource.Id.SongTitle).Text = video.song_title;
            view.FindViewById<TextView>(Resource.Id.Artist).Text = video.Artist.name;
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
        //~SearchResults();
    }
}