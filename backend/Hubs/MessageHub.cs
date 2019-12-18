using backend.Database;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Hubs
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly MessageRepository _messageRepository;

        public MessageHub(MessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public override Task OnConnectedAsync()
        {
            Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            return base.OnConnectedAsync();
        }

        public async Task SendAsync(MessageRequestModel model)
        {
            var messageIds = new List<Guid>();

            foreach (var channelId in model.ChannelIds)
            {
                var dbModel = new Message
                {
                    MessageId = Guid.NewGuid(),
                    FromChannelId = Context.User.Identity.Name,
                    ChannelId = channelId,
                    Delivered = false,
                    MessageString = model.MessageString,
                    Type = model.Type,
                    ExpiresAt = DateTime.UtcNow.AddDays(1),
                    CreatedAt = DateTime.UtcNow
                };
                await _messageRepository.SaveMessageAsync(dbModel);
                messageIds.Add(dbModel.MessageId);
            }

        }

        public async Task DeliveredAsync(Guid id)
        {
            var message = await _messageRepository.GetMessageAsync(id);
            if (message.ChannelId != Context.User.Identity.Name)
            {
                return;
            }
            message.Delivered = true;
            await _messageRepository.SaveMessageAsync(message);
        }
    }
}
