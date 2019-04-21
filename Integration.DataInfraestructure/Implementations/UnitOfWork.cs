using Microsoft.Extensions.Logging;
using System;
using System.Data;

namespace Integration.DataInfraestructure.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private Guid _id;
        private bool _disposed = false;
        private IDbConnection _connection;
        private IDbTransaction _scope;
        private ILogger _logger;

        public UnitOfWork(IDbConnectionHelper connectors, ILoggerFactory loggerFactory)
        {
            _connection = connectors.CreateConnection();
            _logger = loggerFactory.CreateLogger<UnitOfWork>();
        }

        public IDbTransaction Transaction{
            get {
                return _scope;
            }
        }

        public IDbConnection GetConnection()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _id = Guid.NewGuid();
                _connection.Open();
                _logger.LogTrace(string.Format("Connection opened: {0}", _id.ToString()));
            }

            if (_scope == null)
            {
                _scope = _connection.BeginTransaction();
            }
            return _connection;
        }
        
        public void SaveChanges()
        {
            _scope.Commit();
            _scope.Dispose();
            _scope = _connection.BeginTransaction();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (_scope.Connection != null && _scope.Connection.State == ConnectionState.Open)
                    {
                        _scope.Rollback();
                        _scope.Dispose();
                    }

                    if (_connection.State == ConnectionState.Open)
                    {
                        _connection.Close();
                        _connection.Dispose();
                        _logger.LogTrace(string.Format("Connection closed: {0}", _id.ToString()));
                    }
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
