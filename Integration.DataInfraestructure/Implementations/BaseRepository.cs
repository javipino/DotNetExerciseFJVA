using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Integration.DataInfraestructure.Implementations
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        #region Constants, Properties and Attributes

        private IQueryHelper<T> _queryHelper;
        private IDbConnectionHelper _dbHelper;
        private IDbConnection _connection;
        private ILogger _logger;
        private IUnitOfWork _unitOfWork;

        #endregion

        #region Constructors

        public BaseRepository(IUnitOfWork unitOfWork, IQueryHelper<T> queryHelper, IDbConnectionHelper dbHelper, ILogger<BaseRepository<T>> logger)
        {
            //Make "GetConnection" only one time
            _connection = unitOfWork.GetConnection();
            _queryHelper = queryHelper;
            _dbHelper = dbHelper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region General-purpose methods

        /// <summary>
        /// Execute an insert query over Db
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> Get()
        {
            IEnumerable<T> result = null;

            return await Task.Factory.StartNew(() =>
            {
                string selQuery = _queryHelper.SelectQuery();

                if (!string.IsNullOrEmpty(selQuery))
                {
                    result = _connection.Query<T>(
                        selQuery,
                        commandType: CommandType.Text,
                        transaction: _unitOfWork.Transaction);
                }
            }).ContinueWith(x => result);
        }

        /// <summary>
        /// Execute an update query over db connection
        /// </summary>
        /// <param name="entity">entity with identify the fech element</param>
        /// <returns></returns>
        public async Task<T> GetByPK(T entity)
        {
            var dynamicParameters = new DynamicParameters();
            string query = _queryHelper.SelectQueryByKeys(entity, ref dynamicParameters);

            return await _connection.QueryFirstOrDefaultAsync<T>(
                query,
                dynamicParameters,
                commandType: CommandType.Text,
                transaction: _unitOfWork.Transaction);
        }

        /// <summary>
        /// Execute Insert query over db connection
        /// </summary>
        /// <param name="entity">model information</param>
        /// <returns></returns>
        public T Insert(T entity)
        {
            var dynamicParameters = new DynamicParameters();
            string query = _queryHelper.InsertQuery(entity, ref dynamicParameters);

            var aux = _connection.ExecuteScalarAsync(
                query,
                dynamicParameters,
                commandType: CommandType.Text,
                transaction: _unitOfWork.Transaction
            ).GetAwaiter().GetResult();

            T result = _queryHelper.AddEntityId(entity, aux);

            return result;
        }
               
        /// <summary>
        /// Execute an update query over db connection
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> Update(T entity)
        {
            var dynamicParameters = new DynamicParameters();
            string query = _queryHelper.UpdateQuery(entity, ref dynamicParameters);

            return await _connection.ExecuteAsync(
                query,
                dynamicParameters,
                commandType: CommandType.Text,
                transaction: _unitOfWork.Transaction);
        }

        /// <summary>
        /// Execute a physically delete query
        /// </summary>
        /// <param name="entity">identify entity</param>
        /// <returns></returns>
        public async Task<int> DeletePhysical(T entity)
        {
            var dynamicParameters = new DynamicParameters();
            string query = _queryHelper.DeletePhysicalQuery(entity, ref dynamicParameters);

            return await _connection.ExecuteAsync(
                query,
                dynamicParameters,
                commandType: CommandType.Text,
                transaction: _unitOfWork.Transaction);
        }

        /// <summary>
        /// Execute a truncate query over db
        /// </summary>
        /// <param name="entity">idintify table</param>
        /// <returns></returns>
        public async Task<int> Truncate(T entity)
        {
            string query = _queryHelper.TruncateQuery(entity);

            return await _connection.ExecuteAsync(
                query,
                commandType: CommandType.Text,
                transaction: _unitOfWork.Transaction);
        }

        #endregion
    }
}
