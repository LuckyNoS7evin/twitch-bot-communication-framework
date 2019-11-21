using backend.Database;
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
    [Authorize(AuthenticationSchemes="Twitch")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<BotController> _logger;
        private readonly UserRepository _userRepository;

        public UserController(ILogger<BotController> logger, UserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<UserModel> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //got get from DB
            var user = await _userRepository.GetUserAsync(userId);

            if (user == null)
            {
                // create user
                user = new User
                {
                    UserId = userId,
                    ClientId = GenerateCode(16),
                    Secret = GenerateCode(16)
                };

                await _userRepository.SaveUserAsync(user);
            }

            return new UserModel
            {
                UserId = user.UserId,
                ClientId = user.ClientId,
                Secret = user.Secret
            };
        }

        private string GenerateCode(int length)
        {
            RandomNumberGenerator cryptoRandomDataGenerator = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[length];
            cryptoRandomDataGenerator.GetBytes(buffer);
            string uniq = Convert.ToBase64String(buffer);
            return uniq;
        }

    }
}
