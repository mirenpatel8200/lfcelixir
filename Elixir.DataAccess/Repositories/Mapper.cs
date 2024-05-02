using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq.Expressions;

namespace Elixir.DataAccess.Repositories
{
    public class Mapper<TEntity> : Mapper where TEntity : new()
    {
        private readonly OleDbDataReader _dataReader;

        public Mapper(OleDbDataReader dataReader)
        {
            _dataReader = dataReader;
        }

        public MappingObject<TEntity> SetStartIndex(int startIndex)
        {
            return new MappingObject<TEntity>(startIndex, _dataReader);
        }
    }

    public class MappingObject<TEntity> : IMappedInitialized<TEntity> where TEntity : new()
    {
        private int _rowCounter;

        private readonly OleDbDataReader _dataReader;
        private readonly Dictionary<string, object> _objectProps = new Dictionary<string, object>();

        public MappingObject(int startIndex, OleDbDataReader dataReader)
        {
            _rowCounter = startIndex;
            _dataReader = dataReader;
        }

        private IMappedInitialized<TEntity> MapLocal<TProp>(Expression<Func<TEntity, TProp>> expression, object valueToSet)
        {
            var memberExpr = expression.Body as MemberExpression;
            var member = memberExpr?.Member;
            var propName = member?.Name;

            _objectProps.Add(propName, valueToSet);

            _rowCounter++;
            return this;
        }

        public IMappedInitialized<TEntity> Map<TProp>(Expression<Func<TEntity, TProp>> expression)
        {
            var valueToSet = Mapper.GetTableValue<TProp>(_dataReader, _rowCounter);
            return MapLocal(expression, valueToSet);
        }

        public IMappedInitialized<TEntity> Skip<TProp>(Expression<Func<TEntity, TProp>> expression)
        {
            return MapLocal(expression, default(TProp));
        }

        public TEntity Create()
        {
            var t = new TEntity();
            var eType = typeof(TEntity);

            foreach (var oProp in _objectProps)
            {
                var prop = eType.GetProperty(oProp.Key);
                prop.SetValue(t, oProp.Value);
            }

            return t;
        }
    }

    public class Mapper
    {
        public static TValue GetTableValue<TValue>(OleDbDataReader dataReader, int rowNumber)
        {
            return dataReader.IsDBNull(rowNumber) ? default(TValue) : (TValue)dataReader.GetValue(rowNumber);
        }
    }

    public interface IMappedInitialized<TEntity>
    {
        /// <summary>
        /// Maps table column to specified property. Order has to be exactly the same as in database.
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        IMappedInitialized<TEntity> Map<TProp>(Expression<Func<TEntity, TProp>> expression);

        /// <summary>
        /// Skips a property. Order of properties should be exactly the same as in database.
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        IMappedInitialized<TEntity> Skip<TProp>(Expression<Func<TEntity, TProp>> expression);

        /// <summary>
        /// Creates an object with initialized properties.
        /// </summary>
        /// <returns></returns>
        TEntity Create();
    }
}
