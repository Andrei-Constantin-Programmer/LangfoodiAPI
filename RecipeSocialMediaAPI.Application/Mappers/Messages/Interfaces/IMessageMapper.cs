﻿using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;

public interface IMessageMapper
{
    MessageDto MapMessageToMessageDTO(Message message);
}