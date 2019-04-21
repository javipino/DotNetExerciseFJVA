using Dapper;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Integration.DataInfraestructure.Implementations
{
    public class SqlQueryHelper<T> : IQueryHelper<T> where T : class
    {
        #region Constants, Properties and Attributes

        private const string PARAM_ID = "@";

        #endregion

        /// <summary>
        /// generate select query
        /// </summary>
        /// <returns></returns>
        public string SelectQuery()
        {
            string queryBase = string.Empty;
            queryBase = "select * from {0} ; ";
            
            string fieldsList = string.Empty;

            string query = string.Format(queryBase
                , GetTableName()
                );
            return query;
        }

        /// <summary>
        /// Generate select query filtering by pk
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string SelectQueryByKeys(T entity, ref DynamicParameters parameters)
        {
            string queryBase = "select * from {0} where 1 = 1 {1}";

            string query = string.Format(queryBase
                , GetTableName()                            // Table name
                , ParsePkCondition(entity, ref parameters)  // Filtering by PK conditions
                );
            return query;
        }

        /// <summary>
        /// generate insert query returning insert parameter as reference
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string InsertQuery(T entity, ref DynamicParameters parameters)
        {
            string queryBase = "insert into {0} ({1}) values ({2}); select scope_identity() as {3} ";
            string fieldsList = string.Empty;
            string parametersList = string.Empty;
            string idfield = string.Empty;
            string outputParameter = string.Empty;

            ParseFieldsList(entity, ref parameters, ref fieldsList, ref parametersList);
            ParseIdField(entity, ref parameters, ref idfield, ref outputParameter);

            string query = string.Format(queryBase
                , GetTableName()    // Table name
                , fieldsList        // List of fields
                , parametersList    // List of parameters
                , idfield           // Identity field
                );

            return query;
        }

        /// <summary>
        /// Generate update query
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string UpdateQuery(T entity, ref DynamicParameters parameters)
        {
            string queryBase = "update {0} set {1} where 1 = 1 {2}";
            string fieldsList = string.Empty;

            ParseFieldsSetList(entity, ref parameters, ref fieldsList);

            string query = string.Format(queryBase
                , GetTableName()                            // Table name
                , fieldsList                                // List of fields
                , ParsePkCondition(entity, ref parameters)  // Filtering by PK conditions
                );
            return query;
        }

        /// <summary>
        /// generate delete physically query
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string DeletePhysicalQuery(T entity, ref DynamicParameters parameters)
        {
            string queryBase = "delete {0} where 1 = 1 {1}";

            string query = string.Format(queryBase
                , GetTableName()                            // Table name
                , ParsePkCondition(entity, ref parameters)  // Filtering by PK conditions
                );
            return query;
        }

        public string TruncateQuery(T entity)
        {
            string queryBase = "truncate table {0}";

            string query = string.Format(queryBase
                , GetTableName()                            // Table name
                );
            return query;
        }

        /// <summary>
        /// Add entity pk to entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public T AddEntityId(T entity, object id)
        {
            var keyProps = entity.GetType().GetProperties();

            foreach (var prop in keyProps)
            {
                if (prop.GetCustomAttributes(typeof(KeyAttribute), false).Any() && id != null)
                    if (id.GetType() != prop.PropertyType)
                    {
                        id = Convert.ChangeType(id, prop.PropertyType);
                        prop.SetValue(entity, id);
                    }
            }
            return entity;
        }

        #region Private Methods

        /// <summary>
        /// Retrieves the type <T> table name
        /// </summary>
        private string GetTableName()
        {
            string tableName = string.Empty;
            Type tableType = typeof(T);
            if (tableType.GetCustomAttributes(typeof(TableAttribute), false).Length > 0)
            {
                tableName = (tableType.GetCustomAttribute(typeof(TableAttribute)) as TableAttribute).Name;
            }
            else
            {
                tableName = tableType.Name;
            }

            return tableName;
        }

        /// <summary>
        /// Retrieves the where clause to be added as parsing query conditions
        /// </summary>
        private string ParsePkCondition(T entity, ref DynamicParameters parameters)
        {
            string clause = string.Empty;

            Type tableType = typeof(T);
            var keyProps = tableType.GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0);

            for (int i = 0; i < keyProps.Count(); i++)
            {
                PropertyInfo prop = keyProps.ElementAt(i);
                clause += string.Format(" and {0} = {1}KeyParam{2} ", prop.Name, PARAM_ID, i);
                parameters.Add(string.Format("{0}KeyParam{1}", PARAM_ID, i), prop.GetValue(entity));
            }

            return clause;
        }

        /// <summary>
        /// Retrieves the where clause to be added as parsing query conditions
        /// </summary>
        private void ParseFieldsList(T entity, ref DynamicParameters parameters, ref string fieldsList, ref string parametersList)
        {
            Type tableType = typeof(T);
            var keyProps = tableType.GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length == 0);

            fieldsList = string.Empty;
            parametersList = string.Empty;

            for (int i = 0; i < keyProps.Count(); i++)
            {
                PropertyInfo prop = keyProps.ElementAt(i);

                fieldsList += string.Format("{0} {1}", (string.IsNullOrEmpty(fieldsList) ? string.Empty : ","), prop.Name);
                parametersList += string.Format("{0} {1}FieldParam{2}", (string.IsNullOrEmpty(parametersList) ? string.Empty : ","), PARAM_ID, i);
                parameters.Add(string.Format("{0}FieldParam{1}", PARAM_ID, i), prop.GetValue(entity));
            }
        }

        /// <summary>
        /// Retrieves the Id field
        /// </summary>
        private void ParseIdField(T entity, ref DynamicParameters parameters, ref string field, ref string outputParameter)
        {
            Type tableType = typeof(T);
            var keyProps = tableType.GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0);
            var prop = keyProps.FirstOrDefault();

            field = prop.Name;
            outputParameter = string.Format("{0}{1}", PARAM_ID, prop.Name);

            DbType dbt = (DbType)Enum.Parse(typeof(DbType), prop.PropertyType.Name);
            parameters.Add(outputParameter, dbType: dbt, direction: System.Data.ParameterDirection.Output);
        }

        /// <summary>
        /// Retrieves the "set" clause list for update operations
        /// </summary>
        private void ParseFieldsSetList(T entity, ref DynamicParameters parameters, ref string fieldsList)
        {
            Type tableType = typeof(T);
            var keyProps = tableType.GetProperties();

            fieldsList = string.Empty;

            for (int i = 0; i < keyProps.Count(); i++)
            {
                PropertyInfo prop = keyProps.ElementAt(i);
                if (prop.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0)
                    continue;

                fieldsList += string.Format("{0} {1} = {2}FieldParam{3}", (string.IsNullOrEmpty(fieldsList) ? string.Empty : ","), prop.Name, PARAM_ID, i);
                parameters.Add(string.Format("{0}FieldParam{1}", PARAM_ID, i), prop.GetValue(entity));
            }
        }

        #endregion
    }
}
