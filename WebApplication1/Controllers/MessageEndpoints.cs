using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.DTOs;

namespace WebApplication1.Controllers
{
    public static class MessageEndpoints
    {
        public static void MapMessageEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/Message").WithTags(nameof(Message));

            group.MapGet("/", async (ApplicationDbContext db) =>
            {
                var messages = await db.Messages.ToListAsync();
                var messageDtos = messages.Select(m => new MessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    SenderId = m.SenderId,
                    ConversationId = m.ConversationId
                }).ToList();

                return messageDtos;
            })
            .WithName("GetAllMessages")
            .WithOpenApi();

            group.MapGet("/{id}", async Task<Results<Ok<MessageDto>, NotFound>> (int id, ApplicationDbContext db) =>
            {
                var message = await db.Messages
                    .Include(m => m.Conversation)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(model => model.Id == id);

                if (message is null)
                {
                    return TypedResults.NotFound();
                }

                var messageDto = new MessageDto
                {
                    Id = message.Id,
                    Content = message.Content,
                    Timestamp = message.Timestamp,
                    SenderId = message.SenderId,
                    ConversationId = message.ConversationId
                };

                return TypedResults.Ok(messageDto);
            })
            .WithName("GetMessageById")
            .WithOpenApi();

            group.MapGet("/conversation={conversationId}", async Task<Results<Ok<List<MessageDto>>, NotFound>> (int conversationId, ApplicationDbContext db) =>
            {
                var messages = await db.Messages.AsNoTracking()
                    .Where(model => model.ConversationId == conversationId)
                    .ToListAsync();

                if (!messages.Any())
                {
                    return TypedResults.NotFound();
                }

                var messageDtos = messages.Select(m => new MessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    SenderId = m.SenderId,
                    ConversationId = m.ConversationId
                }).ToList();

                return TypedResults.Ok(messageDtos);
            })
            .WithName("GetMessagesByConversationId")
            .WithOpenApi();

            group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, MessageDto messageDto, ApplicationDbContext db) =>
            {
                var affected = await db.Messages
                    .Where(model => model.Id == id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(m => m.Id, messageDto.Id)
                        .SetProperty(m => m.Content, messageDto.Content)
                        .SetProperty(m => m.Timestamp, messageDto.Timestamp)
                        .SetProperty(m => m.SenderId, messageDto.SenderId)
                        .SetProperty(m => m.ConversationId, messageDto.ConversationId)
                    );

                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
            .WithName("UpdateMessage")
            .WithOpenApi();

            group.MapPost("/", async Task<IResult> (MessageDto messageDto, ApplicationDbContext db) =>
            {
                var conversation = await db.Conversations.FindAsync(messageDto.ConversationId);
                if (conversation == null)
                {
                    conversation = new Conversation
                    {
                        Id = messageDto.ConversationId,
                        LastMessageDate = messageDto.Timestamp
                    };
                    db.Conversations.Add(conversation);
                }

                var message = new Message
                {
                    Content = messageDto.Content,
                    Timestamp = messageDto.Timestamp,
                    SenderId = messageDto.SenderId,
                    ConversationId = messageDto.ConversationId
                };

                db.Messages.Add(message);

                conversation.LastMessageDate = message.Timestamp;
                db.Conversations.Update(conversation);

                await db.SaveChangesAsync();
                return TypedResults.Created($"/api/Message/{message.Id}", messageDto);
            })
            .WithName("CreateMessage")
            .WithOpenApi();

            group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, ApplicationDbContext db) =>
            {
                var affected = await db.Messages
                    .Where(model => model.Id == id)
                    .ExecuteDeleteAsync();

                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
            .WithName("DeleteMessage")
            .WithOpenApi();
        }
    }
}