using Azure.AI.OpenAI;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GPS_Copilot.Models;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GPS_Copilot.Services
{
    public class Data
    {
        public Data()
        {

            SystemInitialPrompt = File.ReadAllText("./Data/SystemInitPrompt_PDP.txt");
        }
        public string SystemInitialPrompt { get; set; }
    }
}
