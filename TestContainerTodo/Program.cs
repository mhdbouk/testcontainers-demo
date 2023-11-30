using Microsoft.EntityFrameworkCore;
using TestContainerTodo.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ITodoRepository, TodoRepository>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetService<TodoDbContext>();
db?.Database.EnsureCreated();

app.MapGet("/todo", async (ITodoRepository repository) =>
    await repository.GetAsync());

app.MapGet("/todo/{id}", async (int id, ITodoRepository repository) =>
    await repository.GetAsync(id)
        is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());

app.MapPost("/todo", async (Todo todo, ITodoRepository repository) =>
{
    await repository.AddAsync(todo);

    return Results.Created($"/todo/{todo.Id}", todo);
});

app.MapPut("/todo/{id}", async (int id, Todo inputTodo, ITodoRepository repository) =>
{
    var todo = await repository.GetAsync(id);

    if (todo is null) return Results.NotFound();

    inputTodo.Id = id;
    var status = await repository.UpdateAsync(inputTodo);
    
    return status ? Results.NoContent() : Results.NotFound();
});

app.Run();
