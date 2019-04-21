using Shared.ViewModel.AppSettings;
using System.Data.SqlClient;

namespace Integration.DataInfraestructure.Implementations
{
    public class DbConnectionHelper: IDbConnectionHelper
    {
        #region Attributes

        private ConnectionSettings _settings;

        #endregion

        public DbConnectionHelper(ConnectionSettings settings)
        {
            _settings = settings;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_settings.LocalDb);
        }
    }
}
