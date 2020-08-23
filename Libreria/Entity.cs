namespace Imvdb.LibreriaImvdb
{
    public class Entity
    {
        public long Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public long Discogs_Id { get; set; }
        public string By_Lyne { get; set; }
        public struct Distinct_Position
        {
            public long Hits { get; set; }
            public long Position_Code { get; set; }
            public string Position_Name { get; set; }
        }
        public string Bio { get; set; }
        public string Image { get; set; }
        public long Artist_Video_Count { get; set; }
        public long Featured_Video_Count { get; set; }
    }
}