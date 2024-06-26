﻿using AutoMapper;
using BrowserChat.Entity.DTO;
using BrowserChat.Security.Core.Data;
using BrowserChat.Security.Core.Entities;
using BrowserChat.Security.Core.JWTLogic;
using BrowserChat.Value;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BrowserChat.Security.Core.Application
{
    public class Register
    {
        public class UserRegisterCommand : IRequest<UserReadDTO>
        {
            public string Name { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class UserRegisterHandler : IRequestHandler<UserRegisterCommand, UserReadDTO>
        {
            private readonly SecurityContext _context;
            private readonly UserManager<User> _usrManager;
            private readonly IMapper _mapper;
            private readonly IJWTGenerator _jwtGenerator;

            public UserRegisterHandler(
                SecurityContext context,
                UserManager<User> usrManager,
                IMapper mapper,
                IJWTGenerator jwtGenerator)
            {
                _context = context;
                _usrManager = usrManager;
                _mapper = mapper;
                _jwtGenerator = jwtGenerator;
            }

            public async Task<UserReadDTO> Handle(
                UserRegisterCommand request,
                CancellationToken cancellationToken)
            {
                var exists = await _context.Users.Where(u => u.Email == request.Email).AnyAsync();
                if (exists)
                    throw new Exception(Constant.MessagesAndExceptions.Security.EmailAlreadyExists);

                exists = await _context.Users.Where(u => u.UserName == request.UserName).AnyAsync();
                if (exists)
                    throw new Exception(Constant.MessagesAndExceptions.Security.UnerNameAlreadyExists);

                var user = new User
                {
                    Name = request.Name,
                    Surname = request.Surname,
                    Email = request.Email,
                    UserName = request.UserName
                };

                var resultado = await _usrManager.CreateAsync(user, request.Password);
                if (resultado.Succeeded)
                {
                    var userDTO = this._mapper.Map<User, UserReadDTO>(user);
                    userDTO.Token = this._jwtGenerator.CreateToken(user);

                    return userDTO;
                }

                throw new Exception(Constant.MessagesAndExceptions.Security.CouldNotRegisterUser);
            }
        }
    }
}
