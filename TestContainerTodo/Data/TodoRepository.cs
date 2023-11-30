using Microsoft.EntityFrameworkCore;

namespace TestContainerTodo.Data;

public class TodoRepository : ITodoRepository
{
    private readonly TodoDbContext _context;
    public TodoRepository(TodoDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public Task<List<Todo>> GetAsync()
        => _context.Todos.ToListAsync();
    
    public Task<Todo?> GetAsync(int id)
        => _context.Todos.FirstOrDefaultAsync(todo => todo.Id == id);
    
    public async Task AddAsync(Todo todo)
    {
        await _context.Todos.AddAsync(todo);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> UpdateAsync(Todo todo)
    {
        var dbTodo = await _context.Todos.FindAsync(todo.Id);

        if (dbTodo is null) return false;

        dbTodo.Title = todo.Title;
        dbTodo.IsCompleted = todo.IsCompleted;

        await _context.SaveChangesAsync();

        return true;
    }
}
