using ENDE.Api.Services;
using ENDE.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Walnut.Data.Repositories;
using Walnut.Models.Dto;

namespace Walnut.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public MessageController(
            IAuthService authService,
            IUserRepository userRepository,
            IMessageRepository messageRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMessages(string receiverUsername)
        {
            Request.Headers.TryGetValue("Authorization", out var tokens);
            if (tokens.ToString() == string.Empty)
            {
                return BadRequest("Missing data");
            }
            var token = tokens.ToString().Split(' ')[1];
            var guid = await _authService.GetGuidFromTokenAsync(token);
            if (guid == null)
            {
                return Unauthorized();
            }
            var sender = await _userRepository.GetUserByGuidAsync(guid);
            if (sender == null)
            {
                return NotFound("Sender not found!");
            }
            var receiver = await _userRepository.GetUserByUsernameAsync(receiverUsername);
            if (receiver == null)
            {
                return NotFound("Receiver not found!");
            }
            var messages = await _messageRepository.GetAllMessages(sender.Id, receiver.Id);
            if (messages == null)
            {
                return NotFound("Couldn't find any messages");
            }
            return Ok(messages.Select(m => new MessageDto
            {
                SenderUsername = m.Sender!.Username,
                ReceiverUsername = m.Receiver!.Username,
                Content = m.Content,
                Date = m.Date
            }));
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(MessageSendRequestDto request)
        {
            Request.Headers.TryGetValue("Authorization", out var tokens);
            if (tokens.ToString() == string.Empty)
            {
                return BadRequest("Missing data");
            }
            var token = tokens.ToString().Split(' ')[1];
            var guid = await _authService.GetGuidFromTokenAsync(token);
            if (guid == null)
            {
                return Unauthorized();
            }
            var sender = await _userRepository.GetUserByGuidAsync(guid);
            if (sender == null)
            {
                return NotFound("Sender not found!");
            }
            var receiver = await _userRepository.GetUserByUsernameAsync(request.ReceiverUsername);
            if (receiver == null)
            {
                return NotFound("Receiver not found!");
            }
            var message = await _messageRepository.AddMessageAsync(sender, receiver, request.Content);
            if (message == null)
            {
                return BadRequest("Invalid message");
            }
            return Ok($"Message sent: {sender.Username} -> {receiver.Username}");
        }
    }
}
