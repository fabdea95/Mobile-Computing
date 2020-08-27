using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

using ICSharpCode.SharpZipLib.GZip;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using Windows.Web.Http;   //uso questa classe in UWP
using System.Threading;
using System.Net.Http;       //uso questa classe per applicazioni multipiattaforma UWP e Xamarin
using System.Net.Http.Headers;
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
        public Call(string appKey, Action<IList<Video>> callback)
        {
            this.appKey = appKey;
            this.callback = callback;
        }

        public async void RequestAccess(string query)
        {
            try
            {
                /*BASIC VERSION: 
                //Create a web request 
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.imvdb.com/api/v1/search/videos?q=Abba+Mamma+Mia");
                //myHttpWebRequest.Method = "POST"; 

                //Get the headers associated with the request.
                WebHeaderCollection myWebHeaderCollection = myHttpWebRequest.Headers;

                Console.WriteLine("Configuring Webrequest using 'Add' method");

                //Send the appKey. 
                myWebHeaderCollection.Add("IMVDB-APP-KEY", appKey);

                //Get the associated response for the above request.
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

                //Print the headers for the request.
                //printHeaders(myWebHeaderCollection);
                //Console.WriteLine(myHttpWebResponse);
                //System.Diagnostics.Debug.WriteLine("RESPONSE: ");
                //System.Diagnostics.Debug.WriteLine(myHttpWebResponse);
                //Console.WriteLine("OK");
                Console.WriteLine("RESPONSE: "+((HttpWebResponse)myHttpWebResponse).StatusDescription);
                myHttpWebResponse.Close();
                */

                //string tokenAutorizzazione;

                String base64;

                byte[] byt = System.Text.Encoding.UTF8.GetBytes(appKey);
                base64 = Convert.ToBase64String(byt);
                var handler = new HttpClientHandler()
                {                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                //così decomprimerò automaticamente i dati se questi mi arrivano compressi
                string result = "";
                using (HttpClient httpClient = new HttpClient(handler))
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip")); // così richiedo che i dati mi arrivino compressi
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64);
                    httpClient.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("UTF-8"));
                    httpClient.DefaultRequestHeaders.Add("UserAgent", "esempio1");
                    string endpoint = @"http://imvdb.com/api/v1/";
                    HttpContent content = new StringContent("", Encoding.UTF8);
                    HttpResponseMessage response = await httpClient.PostAsync(endpoint + "search/videos?q=" + query, content);
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        // string jsonResponse = await response.Content.ReadAsStringAsync();
                        //do something with json response here
                        System.Diagnostics.Debug.WriteLine("Risposta");
                        //System.Diagnostics.Debug.WriteLine(jsonResponse);
                        result = jsonResponse;
                        result = result.Substring(result.IndexOf("[")); //, result.IndexOf("]")+1);
                        int end = result.Length - 1;
                        result = result.Substring(0, end);
                        System.Diagnostics.Debug.WriteLine(result);
                    }
                }
                /*
                Video vid_risp = JsonConvert.DeserializeObject<Video>(result);
                System.Diagnostics.Debug.WriteLine(vid_risp.id);
                */

                JArray a = JArray.Parse(result); //results: stringa che contiene la risposta del server
                int i = -1;

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
                IList<Video> videos = a.Children()
                    .Select(status =>
                        new Video
                        {
                            id = long.Parse((string)status.SelectToken("id")),
                            song_title = (string)status.SelectToken("song_title"),
                            song_slug = (string)status.SelectToken("song_slug"),
                            url = (string)status.SelectToken("url"),
                            multiple_versions = bool.Parse((string)status.SelectToken("multiple_versions")),
                            version_name = (string)status.SelectToken("version_name"),
                            version_number = int.Parse((string)status.SelectToken("version_number")),
                            is_imvdb_pick = bool.Parse((string)status.SelectToken("is_imvdb_pick")),
                            aspect_ratio = (string)status.SelectToken("aspect_ratio"),
                            year = (string)status.SelectToken("year"),
                            verified_credits = bool.Parse((string)status.SelectToken("verified_credits")),
                            
                            Artist = vid_artists[i=i+1]
                            
                        }
                        
                 ).ToList();
                

                System.Diagnostics.Debug.WriteLine(vid_artists[0].name);

                System.Diagnostics.Debug.WriteLine(videos[0].id);
                System.Diagnostics.Debug.WriteLine(videos[0].song_title);
                System.Diagnostics.Debug.WriteLine(videos[0].song_slug);
                System.Diagnostics.Debug.WriteLine(videos[0].Artist.name);

                System.Diagnostics.Debug.WriteLine(videos[1].id);
                System.Diagnostics.Debug.WriteLine(videos[1].song_title);
                System.Diagnostics.Debug.WriteLine(videos[1].song_slug);
                System.Diagnostics.Debug.WriteLine(videos[1].Artist.name);



                callback(videos);

                //System.Diagnostics.Debug.WriteLine(vid_risp.Artists.name);
            }
            //Catch exception if trying to add a restricted header.
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            catch (WebException e)
            {
                Console.WriteLine("\nWebException is thrown. \nMessage is :" + e.Message);
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    Console.WriteLine("Server : {0}", ((HttpWebResponse)e.Response).Server);

                    System.Diagnostics.Debug.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    System.Diagnostics.Debug.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    System.Diagnostics.Debug.WriteLine("Server : {0}", ((HttpWebResponse)e.Response).Server);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception is thrown. Message is :" + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception is thrown. Message is :" + e.Message);
            }
        }
        
        public async void GetFull(long id)
        {
            const string endpoint = @"https://imvdb.com/api/v1/video/";
            string url = endpoint + id + "?include=sources,popularity,featured,credits,bts,countries";
            String base64;

            byte[] byt = System.Text.Encoding.UTF8.GetBytes(appKey);
            base64 = Convert.ToBase64String(byt);
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            //così decomprimerò automaticamente i dati se questi mi arrivano compressi
            string result = "";
            using (HttpClient httpClient = new HttpClient(handler))
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip")); // così richiedo che i dati mi arrivino compressi
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64);
                httpClient.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("UTF-8"));
                //httpClient.DefaultRequestHeaders.Add("UserAgent", "esempio1");

                HttpContent content = new StringContent("", Encoding.UTF8);
                HttpResponseMessage response = await httpClient.PostAsync(url, content);
                string jsonResponse = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("Risposta");
                    //System.Diagnostics.Debug.WriteLine(jsonResponse);
                    result = jsonResponse;
                    System.Diagnostics.Debug.WriteLine(result);
                }
                Video selectedVideo = JsonConvert.DeserializeObject<Video>(result);
                System.Diagnostics.Debug.WriteLine(selectedVideo.release_date_string);
            }
            //Video fullvideo;
            //callback(fullvideo);
        }
    }
}