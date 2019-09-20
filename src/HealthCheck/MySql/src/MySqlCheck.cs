using MySqlConnection = MySql.Data.MySqlClient.MySqlConnection;

namespace ClickView.Extensions.HealthCheck.MySql
{
    using System.Data.Common;
    using Checks.Db;

    public class MySqlCheck : DbCheck
    {
        public MySqlCheck(string name, string connectionString) : base(name, connectionString)
        {
        }

        protected override DbConnection GetConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}
