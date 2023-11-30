using Microsoft.EntityFrameworkCore;
using Testcontainers.SqlEdge;
using TestContainerTodo.Data;

namespace IntegrationTests;

public class TodoRepositoryTests
{
    [Fact]
    public async Task AddTodo_Test()
    {
        // Arrange
        
        var container = new SqlEdgeBuilder()
                        .WithImage("mcr.microsoft.com/azure-sql-edge")
                        .Build();

        await container.StartAsync();
        
        var context = new TodoDbContext(new DbContextOptionsBuilder<TodoDbContext>()
                                        .UseSqlServer(container.GetConnectionString())
                                        .Options
        );
        
        await context.Database.EnsureCreatedAsync();

        var repository = new TodoRepository(context);
        
        // Act
        await repository.AddAsync(new Todo()
        {
            Title = "Test New Todo",
            IsCompleted = false
        });
        
        
        // Assert
        var dbTodos = await context.Todos.ToListAsync();
        Assert.Single(dbTodos);
        Assert.Equal("Test New Todo", dbTodos[0].Title);
        Assert.False(dbTodos[0].IsCompleted);
        
        await container.DisposeAsync();
    }
    
    [Fact]
    public async Task GetTodo_Test()
    {
        // Arrange
        
        var container = new SqlEdgeBuilder()
                        .WithImage("mcr.microsoft.com/azure-sql-edge")
                        .Build();

        await container.StartAsync();
        
        var context = new TodoDbContext(new DbContextOptionsBuilder<TodoDbContext>()
                                        .UseSqlServer(container.GetConnectionString())
                                        .Options
        );
        
        await context.Database.EnsureCreatedAsync();

        var repository = new TodoRepository(context);

        await context.Todos.AddAsync(new Todo()
        {
            Title = "Test New Todo",
            IsCompleted = true
        });
        
        await context.Todos.AddAsync(new Todo()
        {
            Title = "Test New Todo 2",
            IsCompleted = false
        });

        await context.SaveChangesAsync();
        
        // Act
        var list = await repository.GetAsync();

        // Assert
        Assert.Equal(2, list.Count);
        Assert.Equal("Test New Todo", list[0].Title);
        Assert.True(list[0].IsCompleted);
        Assert.Equal("Test New Todo 2", list[1].Title);
        Assert.False(list[1].IsCompleted);
        
        await container.DisposeAsync();
    }
}
