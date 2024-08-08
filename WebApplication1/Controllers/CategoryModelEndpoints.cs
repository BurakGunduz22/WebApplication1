using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using WebApplication1.Data;
using WebApplication1.Models;
namespace WebApplication1.Controllers;

public static class CategoryModelEndpoints
{
    public static void MapCategoryModelEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Categories").WithTags(nameof(CategoryModel));

        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            return await db.CategoryModel.ToListAsync();
        })
        .WithName("GetAllCategories")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<CategoryModel>, NotFound>> (int id, ApplicationDbContext db) =>
        {
            return await db.CategoryModel.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is CategoryModel model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetCategoryById")
        .WithOpenApi();
        group.MapGet("/subcategories/{categoryId}", async Task<Results<Ok<List<CategoryModel>>, NotFound>> (int categoryId, ApplicationDbContext db) =>
        {
            var subcategories = await db.CategoryModel.AsNoTracking()
                .Where(model => model.ParentId == categoryId)
                .ToListAsync();

            return subcategories.Any() ? TypedResults.Ok(subcategories) : TypedResults.NotFound();
        })
        .WithName("GetSubCategories")
        .WithOpenApi();
        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, CategoryModel categoryModel, ApplicationDbContext db) =>
        {
            var affected = await db.CategoryModel
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, categoryModel.Id)
                    .SetProperty(m => m.ParentId, categoryModel.ParentId)
                    .SetProperty(m => m.Name, categoryModel.Name)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateCategory")
        .WithOpenApi();

        group.MapPost("/", async (CategoryModel categoryModel, ApplicationDbContext db) =>
        {
            db.CategoryModel.Add(categoryModel);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Categories/{categoryModel.Id}",categoryModel);
        })
        .WithName("CreateCategory")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, ApplicationDbContext db) =>
        {
            var affected = await db.CategoryModel
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteCategory")
        .WithOpenApi();
    }
}
