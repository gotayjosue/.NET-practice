using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITaskService>(new InMemoryTaskService());

var app = builder.Build();

//Middlewares

// Redirect requests from /tasks/{id} to /todos/{id}
app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/$1"));

//Loging middleware
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: [{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Started");
    await next(context);
    Console.WriteLine($"Response: [{context.Response.StatusCode} {context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Completed");
});


var todos = new List<Todo>();

//Get all todos
app.MapGet("/todos", (ITaskService service) => service.GetAllTodos());

//Get a todo by id
app.MapGet("/todos/{id}", (int id, ITaskService service) =>
{
    var task = service.GetTaskById(id);
    return task is not null ? Results.Ok(task) : Results.NotFound();
});

// Seed with some initial data
app.MapPost("/todos", (Todo task, ITaskService service) =>
{
    service.AddTodo(task);
    return Results.Created($"/todos/{task.Id}", task);
})
.AddEndpointFilter(async (context, next) =>
{
    var task = context.Arguments[0] as Todo;
    var errors = new Dictionary<string, string>();

    if (task.DueDate < DateTime.UtcNow)
    {
        errors.Add(nameof(Todo.DueDate), "Due date cannot be in the past.");
    }
    if (task.IsCompleted)
    {
        errors.Add(nameof(Todo.IsCompleted), "New tasks cannot be marked as completed.");
    }

    if (errors.Count > 0)
    {
        return Results.BadRequest(errors);
    }

    return await next(context);
});

app.MapDelete("/todos/{id}", (int id, ITaskService service) =>
{
    service.DeleteTodoById(id);
    return Results.NotFound();
});

app.Run();

public record Todo(int Id, string Name, DateTime DueDate, bool IsCompleted);

interface ITaskService
{
   Todo? GetTaskById(int id);
   List<Todo> GetAllTodos();
   void DeleteTodoById(int id);
   Todo AddTodo(Todo task);
}

class InMemoryTaskService : ITaskService
{
    private readonly List<Todo> _todos = [];

    public Todo? GetTaskById(int id) => _todos.FirstOrDefault(t => t.Id == id);

    public List<Todo> GetAllTodos() => _todos;

    public void DeleteTodoById(int id)
    {
        _todos.RemoveAll(t => t.Id == id);

    }

    public Todo AddTodo(Todo task)
    {
        _todos.Add(task);
        return task;
    }
}