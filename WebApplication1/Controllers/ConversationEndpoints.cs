using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using WebApplication1.Data;
using WebApplication1.Models;
namespace WebApplication1.Controllers;

public static class ConversationEndpoints
{
    public static void MapConversationEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Conversation").WithTags(nameof(Conversation));
        
        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            return await db.Conversations.ToListAsync();
        })
        .WithName("GetAllConversations")
        .WithOpenApi();

        group.MapGet("/conversation={id}", async Task<Results<Ok<Conversation>, NotFound>> (int id, ApplicationDbContext db) =>
        {
            return await db.Conversations.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Conversation model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetConversationById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Conversation conversation, ApplicationDbContext db) =>
        {
            var affected = await db.Conversations
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, conversation.Id)
                    .SetProperty(m => m.ConversationName, conversation.ConversationName)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateConversation")
        .WithOpenApi();

        group.MapPost("/", async (Conversation conversation, ApplicationDbContext db) =>
        {
            db.Conversations.Add(conversation);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Conversation/{conversation.Id}",conversation);
        })
        .WithName("CreateConversation")
        .WithOpenApi();
           
        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, ApplicationDbContext db) =>
        {
            var affected = await db.Conversations
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteConversation")
        .WithOpenApi();

        // Updated endpoint to get conversations by UserId
        group.MapGet("/user={userId}", async Task<Results<Ok<List<Conversation>>, NotFound>> (string userId, ApplicationDbContext db) =>
        {
            var conversations = await db.Conversations
                .Where(c => c.SenderId == userId || c.RecieverId == userId)
                .ToListAsync();

            return conversations.Count > 0 ? TypedResults.Ok(conversations) : TypedResults.NotFound();
        })
        .WithName("GetConversationsByUserId")
        .WithOpenApi();
    }
}
