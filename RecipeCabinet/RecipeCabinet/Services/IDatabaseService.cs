using SQLite;

namespace RecipeCabinet.Services
{
    public interface IDatabaseService
    {
        SQLiteAsyncConnection GetLocalConnection();
        Task InitializeLocalDatabaseAsync();
    }
}
