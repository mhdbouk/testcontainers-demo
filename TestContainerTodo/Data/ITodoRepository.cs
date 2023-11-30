namespace TestContainerTodo.Data;

public interface ITodoRepository
{
    Task<List<Todo>> GetAsync();
    Task<Todo?> GetAsync(int id);
    Task AddAsync(Todo todo);
    Task<bool> UpdateAsync(Todo todo);
}