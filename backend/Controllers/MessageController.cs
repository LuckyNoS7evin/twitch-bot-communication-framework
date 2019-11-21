using backend.Database;
using backend.ExtensionMethods;
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
            if (!HttpContext.Request.Headers.ContainsKey("client-id"))
            {
                return Unauthorized();
            }
            
            var users = await _userRepository.GetUsersByClientIdAsync(HttpContext.Request.Headers["client-id"]);
            if (users == null || users.Count == 0)
            {
                return Unauthorized();
            }
            var user = users.FirstOrDefault();

            string decryptedMessage;
            try
            {
                decryptedMessage = model.Message.Decrypt(user.Secret, HttpContext.Request.Headers["client-id"]);
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            if(string.IsNullOrEmpty(decryptedMessage))
            {
                return BadRequest("Message Invalid");
            }
            try
            {
                var messageModel = JsonConvert.DeserializeObject<MessageModel>(decryptedMessage);

                var messageIds = new List<Guid>();
                
                foreach (var channelId in messageModel.ChannelIds)
                { 
                    var dbModel = new Message
                    {
                        MessageId = Guid.NewGuid(),
                        ChannelId = channelId,
                        Delivered = false,
                        MessageString = messageModel.MessageString,
                        Type = messageModel.Type,
                        ExpiresAt = DateTime.UtcNow.AddDays(1)
                    };
                    await _messageRepository.SaveMessageAsync(dbModel);
                    messageIds.Add(dbModel.MessageId);
                }
               

                return Ok(new MessageResponseModel { MessageIds = messageIds });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
          
        }

        // [HttpGet]
        // public async Task<IActionResult> Get()
        // {
        //     var user = await _userRepository.GetUserAsync("14900522");
        //     if (user == null)
        //     {
        //         return Unauthorized();
        //     }

        //     var messageString = JsonConvert.SerializeObject(new MessageModel { 
        //         ChannelIds =  new List<string> { "14900522" },
        //         Type = Common.MessageType.GENERAL,
        //         MessageString = "BlaBlaBla"
        //     });

        //     return Ok(messageString.Encrypt(user.Secret, user.ClientId));
        // }

    }
}
