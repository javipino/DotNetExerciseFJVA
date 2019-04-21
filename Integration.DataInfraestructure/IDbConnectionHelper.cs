using System.Data.SqlClient;

namespace Integration.DataInfraestructure
{
    public interface IDbConnectionHelper
    {
        SqlConnection CreateConnection();
    }
}
