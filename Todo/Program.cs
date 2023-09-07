using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDbContext<TodoContext>(options => options.UseInMemoryDatabase("TodoList"));
}

var app = builder.Build();
{
    app.MapGet("/", () => "Hello, Todo!");

    app.MapGet("/todos", async (TodoContext context) =>
        await context.Todos.ToListAsync());

    app.MapGet("/todos/{id}", async (int id, TodoContext context) =>
        await context.Todos.FindAsync(id) is Todo todo ? Results.Ok(todo) : Results.NotFound());

    app.MapPost("/todos", async (Todo todo, TodoContext context) =>
    {
        context.Todos.Add(todo);
        await context.SaveChangesAsync();

        return Results.Created($"/todos/{todo.Id}", todo);
    });

    app.MapPut("/todos/{id}", async (int id, Todo todo, TodoContext context) =>
    {
        var item = await context.Todos.FindAsync(id);
        if (item is null) return Results.NotFound();

        item.Name = todo.Name;
        item.Completed = todo.Completed;

        await context.SaveChangesAsync();

        return Results.NoContent();
    });

    app.MapDelete("/todos/{id}", async (int id, TodoContext context) =>
    {
        if (await context.Todos.FindAsync(id) is Todo todo)
        {
            context.Todos.Remove(todo);
            await context.SaveChangesAsync();

            return Results.NoContent();
        }

        return Results.NotFound();
    });

    app.Run();
}

class Todo
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool Completed { get; set; }
}

class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }

    public DbSet<Todo> Todos => Set<Todo>();
}