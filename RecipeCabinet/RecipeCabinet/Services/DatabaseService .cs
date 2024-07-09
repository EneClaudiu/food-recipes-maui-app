using RecipeCabinet.Models;
using SQLite;

namespace RecipeCabinet.Services
{
    public class DatabaseService : IDatabaseService
    {
        private SQLiteAsyncConnection _localConnection;

        public DatabaseService()
        {
            //_ = DeleteDatabaseAsync();
            _ = InitializeLocalDatabaseAsync();
        }

        public SQLiteAsyncConnection GetLocalConnection()
        {
            return _localConnection;
        }

        public async Task InitializeLocalDatabaseAsync()
        {
            _localConnection = new SQLiteAsyncConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RecipeCabinet_Local.db3"));
            await _localConnection.CreateTableAsync<Recipe>();
        }

        public async Task<bool> DeleteDatabaseAsync()
        {
            try
            {
                if (_localConnection != null) 
                {
                    await _localConnection.CloseAsync();
                }
                var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RecipeCabinet_Local.db3");
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                    Console.WriteLine("Database successfully deleted.");
                    return true;
                }
                else
                {
                    Console.WriteLine("No database file found to delete.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting database: {ex.Message}");
                return false;
            }
        }
    }
}
