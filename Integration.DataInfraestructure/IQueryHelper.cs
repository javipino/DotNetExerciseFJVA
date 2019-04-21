using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Integration.DataInfraestructure
{
    public interface IQueryHelper<T> where T : class
    {
        string SelectQuery();
        string SelectQueryByKeys(T entity, ref DynamicParameters parameters);
        string InsertQuery(T entity, ref DynamicParameters parameters);
        string UpdateQuery(T entity, ref DynamicParameters parameters);
        string DeletePhysicalQuery(T entity, ref DynamicParameters parameters);
        string TruncateQuery(T entity);
        T AddEntityId(T entity, object id);
    }
}
