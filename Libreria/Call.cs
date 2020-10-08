
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;       //uso questa classe per applicazioni multipiattaforma UWP e Xamarin
using System.Net.Http.Headers;

using ICSharpCode.SharpZipLib.GZip;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Threading;

using Android.App;
using Android;
using Android.Widget;
using Android.OS;

namespace Imvdb.LibreriaImvdb
{
    public class Call
    {
        private string appKey = "ZO93Xed56gtIwxTTQzqnZ5lNjiRewoT7zet23Pp2";
        Action<IList<Video>> callback;
        /* public Call(string appKey, Action<IList<Video>> callback)
         {
             this.appKey = appKey;
             this.callback = callback;
         }*/

        public async Task<string> RequestAccess(string query, char type)
        {

            String base64;

            byte[] byt = System.Text.Encoding.UTF8.GetBytes(appKey);
            base64 = Convert.ToBase64String(byt);
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            //così decomprimerò automaticamente i dati se questi mi arrivano compressi
            string result = "";
            string endpoint = "";
            if (type == 's')
                endpoint = @"http://imvdb.com/api/v1/search/videos?q="+query;
            if (type == 'v')
                endpoint = @"http://imvdb.com/api/v1/video/"+query+"?include=credits,bts,countries,sources,popularity,featured";
            string jsonResponse = "";
            using (HttpClient httpClient = new HttpClient(handler))
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip")); // così richiedo che i dati mi arrivino compressi
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64);
                httpClient.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("UTF-8"));
                httpClient.DefaultRequestHeaders.Add("UserAgent", "esempio1");


                HttpContent content = new StringContent("", Encoding.UTF8);

                HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);
                jsonResponse = await response.Content.ReadAsStringAsync();
                /*if (response.IsSuccessStatusCode==false)
                {
                    return "ERROR";
                }*/
            }

            return jsonResponse;
        }


        public List<Video> searchVideos(string query)
        {
            var task = Task.Run(() => RequestAccess(query, 's'));
            task.Wait();
            var result = task.Result;
            result = result.Substring(result.IndexOf("[")); //, result.IndexOf("]")+1);
            int end = result.Length - 1;
            result = result.Substring(0, end);
            System.Diagnostics.Debug.WriteLine(result);
            JArray a = JArray.Parse(result); //results: stringa che contiene la risposta del server
            int i = -1;
            int j = -1;
            //JArray b = a.Children();
            List<Artists> vid_artists = a.Children().Select(
                //foreach()
                status =>
                new Artists
                {
                    name = (string)status.SelectToken("artists[0].name"),
                    slug = (string)status.SelectToken("artists[0].slug"),
                    url = (string)status.SelectToken("artists[0].url")
                }
            ).ToList();
            List<Images> imgs = a.Children().Select(
               status =>
               new Images
               {
                   o = (string)status.SelectToken("image.o"),
                   l = (string)status.SelectToken("image.l"),
                   b = (string)status.SelectToken("image.b"),
                   t = (string)status.SelectToken("image.t"),
                   s = (string)status.SelectToken("image.s")
               }
               ).ToList();
            List<Video> videos = a.Children()
                .Select(status =>
                    new Video
                    {
                        id = long.Parse((string)status.SelectToken("id")),
                        song_title = (string)status.SelectToken("song_title"),
                        /*
                        song_slug = (string)status.SelectToken("song_slug"),
                        url = (string)status.SelectToken("url"),
                        multiple_versions = bool.Parse((string)status.SelectToken("multiple_versions")),
                        version_name = (string)status.SelectToken("version_name"),
                        version_number = int.Parse((string)status.SelectToken("version_number")),
                        is_imvdb_pick = bool.Parse((string)status.SelectToken("is_imvdb_pick")),
                        aspect_ratio = (string)status.SelectToken("aspect_ratio"),
                        year = (string)status.SelectToken("year"),
                        verified_credits = bool.Parse((string)status.SelectToken("verified_credits")),
                        */

                        artists = vid_artists,
                        image = imgs[j = j + 1]
                    }

             ).ToList();
            return videos;
        }


        public Video GetFull(long id)
        {
            Video fullvideo = new Video();
            var task = Task.Run(() => RequestAccess(id.ToString(), 'v'));
            task.Wait();
            var result = task.Result;
            System.Diagnostics.Debug.WriteLine("FULL VIDEO RESULT");
            System.Diagnostics.Debug.WriteLine(result);
            try
            {
                fullvideo = JsonConvert.DeserializeObject<Video>(result);
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception is thrown. Message is :" + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception is thrown. Message is :" + e.Message);
            }
            //System.Diagnostics.Debug.WriteLine(fullvideo.credits.cast[0].entity_name);
            return fullvideo;
        }
    }
}