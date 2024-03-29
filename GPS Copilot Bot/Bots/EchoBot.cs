﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EchoBot .NET Template version v4.17.1

using Azure.AI.OpenAI;
using GPS_Copilot.Models;
using GPS_Copilot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        ConversationState _conversationState;
        UserState _userState;


        const string WELCOME_TEXT = " Hello and welcome, I am your AI assistant and I am here to assist you with the following areas. Please select one?";
        const string WELCOME_AFTER_INIT_PROMPT_TEXT = "Welcome, i will be your guide on {0}. This is your playground where you can modify and fine-tune the behaviour of your bot using prompt engineering techniques. An initial system prompt has been created to help you get started and it is stored in Azure Blob Storage. You can access and modify this prompt to suit your needs using Azure Storage Explorer. Here is the link to the file: {1}.";
        const string READY_TO_PROCEED_TEXT = "Are you ready to proceed?";
        const string RESTART_TEXT = "restart";


        public EchoBot(ConversationState conversationState, UserState userState)
        {
            _conversationState = conversationState;
            _userState = userState;
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            var conversationStateAccessors = _conversationState.CreateProperty<Messages>(nameof(Messages));
            var conversationMessage = await conversationStateAccessors.GetAsync(turnContext, () => new Messages());

            var contextHelperAccessors = _conversationState.CreateProperty<ContextHelper>(nameof(ContextHelper));
            var contextHelper = await contextHelperAccessors.GetAsync(turnContext, () => new ContextHelper());

            OpenAI oAI = new OpenAI();
            string responseMessage;
            string systemMessage;
            BlobData blob = new BlobData();
            string blobUri;

            if (string.Compare(turnContext.Activity.Text, RESTART_TEXT, true) == 0)
            {
                contextHelper.IsSystemMessageLoaded = false;
                //await turnContext.SendActivityAsync(MessageFactory.Text(WELCOME_TEXT, WELCOME_TEXT), cancellationToken);
                await SendSuggestedActionsAsync(turnContext, cancellationToken);
            }

            else if (!contextHelper.IsSystemMessageLoaded)
            {
                conversationMessage.MessageList.Clear();

                conversationMessage.MessageList.Add(new Message()
                {
                    Text = WELCOME_TEXT,
                    Role = Role.System
                });

                conversationMessage.MessageList.Add(new Message()
                {
                    Text = turnContext.Activity.Text,
                    Role = Role.User
                });

                contextHelper.AreaSelected = turnContext.Activity.Text;

                contextHelper.IsSystemMessageLoaded = true;

                //Create blob file                
                blobUri = await blob.CreateBlobFileFromStringIfNotExist($"{contextHelper.AreaSelected}.txt", new Data().SystemInitialPrompt);


                //Read Blob file
                systemMessage = await blob.ReadTextFromBlobAsync($"{contextHelper.AreaSelected}.txt");

                conversationMessage.MessageList.Add(new Message()
                {
                    Text = systemMessage,
                    Role = Role.System
                });
                await turnContext.SendActivityAsync(MessageFactory.Text(String.Format(WELCOME_AFTER_INIT_PROMPT_TEXT, contextHelper.AreaSelected, blobUri)));

                await turnContext.SendActivityAsync(MessageFactory.Text(READY_TO_PROCEED_TEXT));
            }
            else
            {
                conversationMessage.MessageList.Add(new Message()
                {
                    Text = turnContext.Activity.Text,
                    Role = Role.User
                });

                responseMessage = oAI.CallOpenAI(conversationMessage);

                conversationMessage.MessageList.Add(new Message()
                {
                    Text = responseMessage,
                    Role = Role.System
                });

                await turnContext.SendActivityAsync(MessageFactory.Text(responseMessage, responseMessage), cancellationToken);
            }

        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    //await turnContext.SendActivityAsync(MessageFactory.Text(WELCOME_TEXT, WELCOME_TEXT), cancellationToken);
                    await SendSuggestedActionsAsync(turnContext, cancellationToken);
                }
            }
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        private static async Task SendSuggestedActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text(WELCOME_TEXT);

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "Partner Business Plan", Type = ActionTypes.ImBack, Value = "Partner Business Plan"},
                    new CardAction() { Title = "Specializations", Type = ActionTypes.ImBack, Value = "Specializations"},
                    new CardAction() { Title = "Designations", Type = ActionTypes.ImBack, Value = "Designations" },
                    new CardAction() { Title = "General Questions", Type = ActionTypes.ImBack, Value = "General Questions" },
                },
            };
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

    }
}
