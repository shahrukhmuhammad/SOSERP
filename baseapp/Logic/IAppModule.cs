using BaseApp.Entity;
using Insight.Database;
using System.Collections.Generic;
using System.Data;

namespace BaseApp
{
    public interface IAppModule
    {
        [Sql("AppModules_GetAll")]
        List<AppModule> GetAll();

        [Sql("AppModules_GetById")]
        AppModule GetById(string Id);

        [Sql("AppModules_Update")]
        void Update(string Id, bool Status, string Message);
    }
}

namespace BaseApp.System
{
    public class DatabaseAppModule : IAppModule
    {
        private IAppModule repo;

        public DatabaseAppModule(IDbConnection db)
        {
            // Ensure that the connection is opened (otherwise executing the command will fail)
            ConnectionState originalState = db.State;
            if (originalState != ConnectionState.Open)
                db.Open();
            try
            {
                // Create a command to get the server version
                // NOTE: The query's syntax is SQL Server specific

                repo = db.As<IAppModule>();
            }
            finally
            {
                // Close the connection if that's how we got it
                if (originalState == ConnectionState.Closed)
                    db.Close();
            }
        }

        public List<AppModule> GetAll()
        {
            return repo.GetAll();
        }

        public AppModule GetById(string Id)
        {
            return repo.GetById(Id);
        }

        public void Update(string Id, bool Status, string Message)
        {
            repo.Update(Id, Status, Message);
        }
    }
}