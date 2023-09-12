using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using static System.Net.Mime.MediaTypeNames;
using System;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Azure;
using System.Linq;
using GPS_Copilot.Models;

namespace GPS_Copilot.Services
{
    public class OpenAI
    {
        // Build a config object and retrieve user settings.
        private IConfiguration config;
        private string? oaiEndpoint;
        private string? oaiKey;
        private string? oaiModelName;
        private string? maxTokens;
        private OpenAIClient client;
        public ChatCompletionsOptions chatCompletionsOptions;
        public ChatCompletions chatCompletions;
        int maxTokensInt = 4000;
        public OpenAI(string systemPrompt)
        {

            config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .Build();
            oaiEndpoint = config["AzureOAIEndpoint"];
            oaiKey = config["AzureOAIKey"];
            oaiModelName = config["AzureOAIModelName"];
            maxTokens = config["maxTokens"];
            client = new OpenAIClient(new Uri(oaiEndpoint), new AzureKeyCredential(oaiKey));

            int.TryParse(maxTokens, out maxTokensInt);

            chatCompletionsOptions = InitChatCompetionOptions(systemPrompt, maxTokensInt);
        }

        public OpenAI()
        {

            config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .Build();
            oaiEndpoint = config["AzureOAIEndpoint"];
            oaiKey = config["AzureOAIKey"];
            oaiModelName = config["AzureOAIModelName"];
            maxTokens = config["maxTokens"];
            client = new OpenAIClient(new Uri(oaiEndpoint), new AzureKeyCredential(oaiKey));

            int.TryParse(maxTokens, out maxTokensInt);

            chatCompletionsOptions = InitChatCompetionOptions(null, maxTokensInt);
        }

        public string CallOpenAI(Messages messages, string additionalSystemPrompt = null)
        {

            // Build completion options object
            foreach (var message in messages.MessageList)
            {
                if (message.Role == Role.User)
                {
                    chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, message.Text));
                }
                else if (message.Role == Role.System)
                {
                    chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, message.Text));
                }
            }

            if (!string.IsNullOrEmpty(additionalSystemPrompt))
            {
                chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, additionalSystemPrompt));
            }

            // Send request to Azure OpenAI model
            ChatCompletions response = client.GetChatCompletions(
                deploymentOrModelName: oaiModelName,
                chatCompletionsOptions);

            chatCompletionsOptions.Messages.Add(response.Choices[0].Message);

            return response.Choices[0].Message.Content;

        }

        private ChatCompletionsOptions InitChatCompetionOptions(string systemPrompt, int maxTokens = 4000, float temperature = 0.7f)
        {

            // Build completion options object
            ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
            {
                MaxTokens = maxTokens,
                Temperature = temperature
            };

            if (!string.IsNullOrEmpty(systemPrompt))
            {
                chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, systemPrompt));
            }
        
            return chatCompletionsOptions;
        }
}
}
