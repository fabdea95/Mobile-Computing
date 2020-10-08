using Newtonsoft.Json;
using System.Collections.Generic;
namespace Imvdb.LibreriaImvdb
{
    public class Search
    {
        [JsonProperty("Search")]
        public List<Video> Videos { get; set; }
    }

    public class Video
    {
        public int i;
        public long id { get; set; }
        public char production_status { get; set; }
        public string song_title { get; set; }
        public string song_slug { get; set; }
        public string url { get; set; }
        public bool multiple_versions { get; set; }
        public string version_name { get; set; }
        public int version_number { get; set; }
        public bool is_imvdb_pick { get; set; }
        public string aspect_ratio { get; set; }
        public string year { get; set; }
        public bool verified_credits { get; set; }

        
        public List<Artists> artists { get; set; }
        
        public Images image { get; set; }

        public List<Sources> sources { get; set; }
        public List<Directors> directors { get; set; }
        public Credits credits { get; set; }
        public string release_date_stamp { get; set; }
        public string release_date_string { get; set; }
        public List<Featured> featured_artists { get; set; }
        public Popularity popularity { get; set; }
    }

    public class Artists
    {
        public string name { get; set; }
        public string slug { get; set; }
        public string url { get; set; }
        //public long discogs_Id { get; set; }
    }

    public class Images
    {
        public string o { get; set; }
        public string l { get; set; }
        public string b { get; set; }
        public string t { get; set; }
        public string s { get; set; }

    }
    public class Sources
    {
        public string source { get; set; }
        public string source_slug { get; set; }
        public string source_data { get; set; }
        public bool is_primary { get; set; }
    }

    public class Directors
    {
        public string position_name { get; set; }
        public string position_code { get; set; }
        public string entity_name { get; set; }
        public string entity_slug { get; set; }
        public long entity_id { get; set; }
        public string position_notes { get; set; }
        public long position_id { get; set; }
        public string entity_url { get; set; }
    }
    public class Credits
    {
        //[JsonProperty("Credits")]
        public int total_credits { get; set; }
        public List<Crew> crew { get; set; }
        public List<Cast> cast { get; set; }

    }
    public class Crew
    {
        public string position_name { get; set; }
        public string position_code { get; set; }
        public string entity_name { get; set; }
        public string entity_slug { get; set; }
        public long entity_id { get; set; }
        public string position_notes { get; set; }
        public long position_id { get; set; }
        public string entity_url { get; set; }
    }
    public class Cast
    {
        public string entity_name { get; set; }
        public string entity_slug { get; set; }
        public long entity_id { get; set; }
        public List<string> cast_roles { get; set; }
        public long position_id { get; set; }
    }
    public class Featured
    {
        public string name { get; set; }
        public string slug { get; set; }
        public string url { get; set; }
    }
    public class Popularity
    {
        public long views_all_time { get; set; }
        public string views_all_time_formatted { get; set; }
        public long shares_all_time { get; set; }
        public string shares_all_time_formatted { get; set; }
    }
    public class Bts
    {
        public string type { get; set; }
        public string source { get; set; }
        public string source_data { get; set; }
        public string title { get; set; }
    }
    public class Country
    {
        public string code { get; set; }
        public string name { get; set; }
    }

}
