using Azure;
using Azure.Search.Documents;
using GPS_Copilot_Bot.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection.Metadata;

namespace GPS_Copilot.Services
{
    public class Search 
    {
        // Build a config object and retrieve user settings.
        private IConfiguration config;
        private string? endpoint;
        private string? key;
        SearchClient client;

        public Search(string indexName)
        {

            config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .Build();
            // Get the service endpoint and API key from the environment
            string endpoint = config["SearchEndpoint"];
            string key = config["SearchKey"];

            // Create a client
            AzureKeyCredential credential = new AzureKeyCredential(key);
            client = new SearchClient(new Uri(endpoint), indexName, credential);
        }

        public string CallSearch(string searchQuery)
        {
            var result = client.Search<CustomSearchResultWrapper>(searchQuery);

            try
            {
                return result.GetRawResponse().Content.ToDynamicFromJson().value[0].content.ToString();
            }
            catch
            {
                return null;
            } ;
        }


    }
}
