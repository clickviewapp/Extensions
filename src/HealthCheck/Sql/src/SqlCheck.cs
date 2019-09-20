namespace ClickView.Extensions.HealthCheck.Sql
{
    using System.Data.Common;
    using System.Data.SqlClient;
    using Checks.Db;

    public class SqlCheck : DbCheck
    {
        public SqlCheck(string name, string connectionString) : base(name, connectionString)
        {
        }

        protected override DbConnection GetConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
