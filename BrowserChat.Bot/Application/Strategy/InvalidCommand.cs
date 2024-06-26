﻿using BrowserChat.Bot.AsyncServices;
using BrowserChat.Entity;
using BrowserChat.Value;

namespace BrowserChat.Bot.Application.Strategy
{
    internal class InvalidCommand : CommandBase, ICommandStrategy
    {
        public InvalidCommand(BotResponsePublisher publisher) : base(publisher) { }

        public void ProcessCommand(BotRequest request)
        {
            Publish(
                new BotResponse
                {
                    RoomId = request.RoomId,
                    Message = Constant.MessagesAndExceptions.Bot.Other.InvalidCommand,
                    CommandError = true,
                    CommandErrorType = BotCommandErrorType.InvalidComand
                }
            );
        }
    }
}
