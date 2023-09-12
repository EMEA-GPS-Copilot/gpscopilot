using Azure.AI.OpenAI;
using Azure;
using GPS_Copilot.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using System;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using Azure.Storage.Blobs.Specialized;

namespace GPS_Copilot.Services
{
    public class BlobData
    {
        // Build a config object and retrieve user settings.
        private IConfiguration config;
        private string? blobConnectionString;
        private string? blobContainerName;
        private string? blobSasToken;
        public BlobData()
        {

            config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .Build();
            blobConnectionString = config["blobConnectionString"];
            blobContainerName = config["blobContainerName"];
            blobSasToken = config["blobSasToken"];
        }

        public void GetBlobServiceClientSAS(ref BlobServiceClient blobServiceClient, string accountName, string sasToken)
        {
            string blobUri = "https://" + accountName + ".blob.core.windows.net";

            blobServiceClient = new BlobServiceClient
            (new Uri($"{blobUri}?{sasToken}"), null);
        }

        public async Task<string> ReadTextFromBlobAsync(string blobName)
        {
            string text = null;

            // Create BlobServiceClient with SAS token  
            BlobServiceClient blobServiceClient = null;
            GetBlobServiceClientSAS(ref blobServiceClient, blobConnectionString, blobSasToken);

            // Get reference to container  
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);

            // Create container if it doesn't exist  
            containerClient.CreateIfNotExists();

            // Get reference to blob  
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            using (var streamReader = new System.IO.StreamReader(await blobClient.OpenReadAsync()))
            {
                text = await streamReader.ReadToEndAsync();
            }

            return text;
        }


        public async Task<string> CreateBlobFileFromStringIfNotExist(string fileName, string fileContent)
        {
            // Create BlobServiceClient with SAS token  
            BlobServiceClient blobServiceClient = null;
            GetBlobServiceClientSAS(ref blobServiceClient, blobConnectionString, blobSasToken);

            // Get reference to container  
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);

            // Create container if it doesn't exist  
            containerClient.CreateIfNotExists();

            // Get reference to blob  
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            // Check if the file already exists  
            if (!await blobClient.ExistsAsync())
            {
                // Convert the string content to a byte array  
                byte[] contentBytes = Encoding.UTF8.GetBytes(fileContent);

                // Upload the byte array to the blob  
                using (MemoryStream stream = new MemoryStream(contentBytes))
                {
                    await blobClient.UploadAsync(stream, new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders
                        {
                            ContentType = "text/plain"
                        }
                    });
                }
            }

            return blobClient.Uri.AbsoluteUri;
        }
    }
}