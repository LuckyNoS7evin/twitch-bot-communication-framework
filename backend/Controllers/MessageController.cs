using backend.Database;
using backend.ExtensionMethods;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        public MessageController(ILogger<BotController> logger, UserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
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

            string decryptedMessage = "";
            try
            {
                decryptedMessage = model.Message.Decrypt(user.Secret);
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _userRepository.GetUsersByClientIdAsync("ixgbPHJopeaEcGxcLaWuoQ==");
            if (users == null || users.Count == 0)
            {
                return Unauthorized();
            }
            var user = users.FirstOrDefault();

            var messageString = "This is my message";

            return Ok(messageString.Encrypt(user.Secret));
        }

    }
}
