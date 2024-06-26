﻿using BrowserChat.Entity.DTO;
using BrowserChat.Security.Core.Application;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrowserChat.Security.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserReadDTO>> Get()
        {
            return await _mediator.Send(new CurrentUser.CurrentUserCommand());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserReadDTO>> Login(
            Login.UsuarioLoginCommand paramValues)
        {
            return await _mediator.Send(paramValues);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserReadDTO>> Register(
            Register.UserRegisterCommand paramValues)
        {
            return await _mediator.Send(paramValues);
        }
    }
}
