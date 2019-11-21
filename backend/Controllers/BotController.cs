using backend.Database;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes="Twitch")]
    public class BotController : ControllerBase
    {
        private readonly ILogger<BotController> _logger;
        private readonly ChannelRepository _channelRepository;
        private readonly TwitchService _twitchService;

        public BotController(ILogger<BotController> logger, ChannelRepository channelRepository, TwitchService twitchService)
        {
            _logger = logger;
            _channelRepository = channelRepository;
            _twitchService = twitchService;
        }

        [HttpGet]
        public async Task<List<ChannelBotLinkModel>> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //got get from DB
            var channels = await _channelRepository.GetBotsByChannelId(userId);

            return channels.Select(x=> new ChannelBotLinkModel
            {
                BotAccepted = x.BotAccepted,
                BotDisplayName = x.BotDisplayName,
                BotId = x.BotId,
                ChannelAccepted = x.ChannelAccepted,
                ChannelDisplayName = x.ChannelDisplayName,
                ChannelId = x.ChannelId,
                PendingResponse = x.PendingResponse
            }).ToList();
        }

        [HttpPost]
        public async Task<ActionResult<ChannelBotLinkModel>> Post(BotLinkRequestModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != model.BotId && userId != model.ChannelId) return Forbid();

            var botLink = await Get();

            if (botLink != null && botLink.Count > 0 && botLink.Where(c => c.ChannelId == model.ChannelId).Where(b => b.BotId == model.BotId).Any())
            {
                return BadRequest("ALREADY_LINKED_OR_PENDING");
            }

            //go check twitch that ids are real
            var users = await _twitchService.GetUserDisplayNames(new List<string> { model.BotId, model.ChannelId });
            //put pending request in DB
            var newChannel = new Channel
            {
                BotAccepted = userId == model.BotId,
                BotDisplayName = users[model.BotId],
                BotId = model.BotId,
                ChannelAccepted = userId == model.ChannelId,
                ChannelDisplayName = users[model.ChannelId],
                ChannelId = model.ChannelId,
                PendingResponse = true
            };

            await _channelRepository.SaveChannelAsync(newChannel);

            return Ok(new ChannelBotLinkModel
            {
                BotAccepted = newChannel.BotAccepted,
                BotDisplayName = newChannel.BotDisplayName,
                BotId = newChannel.BotId,
                ChannelAccepted = newChannel.ChannelAccepted,
                ChannelDisplayName = newChannel.ChannelDisplayName,
                ChannelId = newChannel.ChannelId,
                PendingResponse = newChannel.PendingResponse
            });
        }

        [HttpPut]
        public async Task<ActionResult<ChannelBotLinkModel>> Put(BotLinkAcceptModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != model.BotId && userId != model.ChannelId) return Forbid();

            //got get from DB
            var botLink = await _channelRepository.GetBotsByChannelId(userId);

            if (botLink == null || botLink.Count() == 0) return BadRequest("NO_PENDING_REQUEST_FOUND");

            // find a pending in list
            var pending = botLink.Where(c => c.ChannelId == model.ChannelId).Where(b => b.BotId == model.BotId).Where(c =>
                {
                    if (userId == model.ChannelId)
                    {
                        return c.ChannelAccepted == false;
                    }
                    return c.BotAccepted == false;
                }).FirstOrDefault();

            if (pending == null) return BadRequest("NO_PENDING_REQUEST_FOUND");

            // update request with accepted or delined
            pending.PendingResponse = false;
            
            if (model.BotId == userId)
            {
                pending.BotAccepted = model.Accepted;
            }
            else
            {
                pending.ChannelAccepted = model.Accepted;
            }

            await _channelRepository.SaveChannelAsync(pending);

            return Ok(new ChannelBotLinkModel
            {
                BotAccepted = pending.BotAccepted,
                BotDisplayName = pending.BotDisplayName,
                BotId = pending.BotId,
                ChannelAccepted = pending.ChannelAccepted,
                ChannelDisplayName = pending.ChannelDisplayName,
                ChannelId = pending.ChannelId,
                PendingResponse = pending.PendingResponse
            });
        }

    }
}
