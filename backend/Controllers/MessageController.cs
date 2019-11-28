using backend.Database;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "BCF")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<BotController> _logger;
        private readonly UserRepository _userRepository;
        private readonly MessageRepository _messageRepository;

        public MessageController(
            ILogger<BotController> logger,
            UserRepository userRepository, 
            MessageRepository messageRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post(MessageRequestModel model)
        {
            var messageIds = new List<Guid>();

            foreach (var channelId in model.ChannelIds)
            {
                var dbModel = new Message
                {
                    MessageId = Guid.NewGuid(),
                    FromChannelId = User.Identity.Name,
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


            return Ok(new MessageResponseModel { MessageIds = messageIds });

        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await _userRepository.GetUserAsync(User.Identity.Name);
            var messages = await _messageRepository.GetMessagesByChannelIdAsync(user.UserId);
            return Ok(messages.Select(x=> new MessageModel {
                MessageId = x.MessageId,
                Type = x.Type,
                MessageString = x.MessageString,
                FromChannelId = x.FromChannelId,
                ExpiresAt = x.ExpiresAt,
                CreatedAt = x.CreatedAt,
                Delivered = x.Delivered
            }).OrderBy(x=>x.CreatedAt));
        }
        
        [HttpPut("{id}/Delivered")]
        public async Task<IActionResult> Put(Guid id)
        { 
            var message = await _messageRepository.GetMessageAsync(id);
            if(message.ChannelId != User.Identity.Name)
            {
                return Unauthorized();
            }
            message.Delivered = true;
            await _messageRepository.SaveMessageAsync(message);
            return Ok();
        }
    }
}
