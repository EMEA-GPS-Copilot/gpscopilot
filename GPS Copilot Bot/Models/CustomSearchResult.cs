using Azure.Search.Documents.Indexes;
using System.Collections.Generic;

namespace GPS_Copilot_Bot.Models
{
    public partial class CustomSearchResultWrapper
    {
        public List<CustomSearchResult> Values { get; set; }
    }

    public partial class CustomSearchResult
    {
        [SimpleField(IsKey = true, IsFilterable = true)]
        public string Content { get; set; }
        public string Filepath { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }

        public string id { get; set; }
        public string Chunk_id { get; set; }
        public string Last_updated { get; set; }
    }
}
