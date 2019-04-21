using System;
using System.Data;

namespace Integration.DataInfraestructure
{
    public interface IUnitOfWork: IDisposable
    {
        IDbTransaction Transaction { get; }

        void SaveChanges();
        IDbConnection GetConnection();
    }
}
