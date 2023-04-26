/*
BSD 3-Clause License

Copyright (c) 2017, B. Wofter
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the copyright holder nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace BWofter.Converters.Data
{
    using BWofter.Converters.EqualityComparers;
    using BWofter.Converters.Expressions;
    using BWofter.Converters.Extensions;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary><para>A static class used to convert data tables into entities.</para></summary>
    /// <typeparam name="TEntity"><para>The entity type to convert to.</para></typeparam>
    public static class DataTableConverter<TEntity> where TEntity : class, new()
    {
        private static readonly Type type = typeof(TEntity);
        private static readonly ConcurrentDictionary<ICollection<string>, Func<DataRow, TEntity>> converters = new ConcurrentDictionary<ICollection<string>, Func<DataRow, TEntity>>(StringCollectionComparer.GetInstance());

        /// <summary><para>Iterates over the <see cref="DataRow"/> values in the <paramref name="dataTable"/>, mapping their fields
        /// to the entity type.</para></summary>
        /// <param name="dataTable"><para>The <see cref="DataTable"/> to map to the entity type.</para></param>
        /// <returns><para>A yielded instance of the entity type.</para></returns>
        public static IEnumerable<TEntity> ToEntities(DataTable dataTable)
        {
            if (dataTable == null) throw new ArgumentNullException(nameof(dataTable));
            return ToEntities(dataTable, new Dictionary<DataColumn, string>(dataTable.Columns.Select(GetDataColumnKeyValuePair).ToDictionary()));
        }

        /// <summary><para>Iterates over the <see cref="DataRow"/> values in the <paramref name="dataTable"/>, mapping their fields
        /// to the entity type.</para></summary>
        /// <param name="dataTable"><para>The <see cref="DataTable"/> to map to the entity type.</para></param>
        /// <param name="columnToMemberMap"><para>The <see cref="IDictionary{TKey, TValue}"/> to map the <see cref="DataColumn"/> values to properties.</para></param>
        /// <returns><para>A yielded instance of the entity type.</para></returns>
        public static IEnumerable<TEntity> ToEntities(DataTable dataTable, IDictionary<DataColumn, string> columnToMemberMap)
        {
            if (dataTable == null) throw new ArgumentNullException(nameof(dataTable));
            if (columnToMemberMap == null) throw new ArgumentNullException(nameof(columnToMemberMap));
            Func<DataRow, TEntity> converter = GetConverter(columnToMemberMap);
            if (converter == null) throw new InvalidOperationException($"Unable to generate a converter for the data table. This could be due to a bug in the expression generator.");
            //Iterate over the data rows in the data table, yielding the results back to the caller.
            foreach (DataRow row in dataTable.Rows)
            {
                yield return converter(row);
            }
        }

        /// <summary><para>Iterates over the <see cref="DataRow"/> values in <paramref name="dataRows"/>, mapping their fields
        /// to the entity type.</para></summary>
        /// <param name="dataRows"><para>The <see cref="DataRowCollection"/> to map to the entity type.</para></param>
        /// <returns><para>A yielded instance of the entity type.</para></returns>
        public static IEnumerable<TEntity> ToEntities(DataRowCollection dataRows)
        {
            if (dataRows == null) throw new ArgumentNullException(nameof(dataRows));
            if (dataRows.Count == 0) throw new ArgumentException($"Parameter {nameof(dataRows)} should have at least 1 value, 0 given.");
            return ToEntities(dataRows, dataRows[0].Table.Columns.Select(GetDataColumnKeyValuePair).ToDictionary());
        }

        /// <summary><para>Iterates over the <see cref="DataRow"/> values in <paramref name="dataRows"/>, mapping their fields
        /// to the entity type.</para></summary>
        /// <param name="dataRows"><para>The <see cref="DataRowCollection"/> to map to the entity type.</para></param>
        /// <param name="columnToMemberMap"><para>The <see cref="IDictionary{TKey, TValue}"/> to map the <see cref="DataColumn"/> values to properties.</para></param>
        /// <returns><para>A yielded instance of the entity type.</para></returns>
        public static IEnumerable<TEntity> ToEntities(DataRowCollection dataRows, IDictionary<DataColumn, string> columnToMemberMap)
        {
            if (dataRows == null) throw new ArgumentNullException(nameof(dataRows));
            if (columnToMemberMap == null) throw new ArgumentNullException(nameof(columnToMemberMap));
            Func<DataRow, TEntity> converter = GetConverter(columnToMemberMap);
            if (converter == null) throw new InvalidOperationException($"Unable to generate a converter for the data table. This could be due to a bug in the expression generator.");
            //Iterate over the data rows in the data table, yielding the results back to the caller.
            foreach (DataRow row in dataRows)
            {
                yield return converter(row);
            }
        }

        /// <summary><para>Iterates over the <see cref="DataRow"/> values in <paramref name="dataRows"/>, mapping their fields
        /// to the entity type.</para></summary>
        /// <param name="dataRows"><para>The <see cref="IEnumerable{T}"/> of <see cref="DataRow"/> values to map to the entity type.</para></param>
        /// <returns><para>A yielded instance of the entity type.</para></returns>
        public static IEnumerable<TEntity> ToEntities(IEnumerable<DataRow> dataRows)
        {
            DataRow first = dataRows?.FirstOrDefault();
            if (dataRows == null) throw new ArgumentNullException(nameof(dataRows));
            if (first == null) throw new ArgumentException($"Parameter {nameof(dataRows)} should have at least 1 value, 0 given.");
            return ToEntities(dataRows, first.Table.Columns.Select(GetDataColumnKeyValuePair).ToDictionary());
        }

        /// <summary><para>Iterates over the <see cref="DataRow"/> values in <paramref name="dataRows"/>, mapping their fields
        /// to the entity type.</para></summary>
        /// <param name="dataRows"><para>The <see cref="IEnumerable{T}"/> of <see cref="DataRow"/> values to map to the entity type.</para></param>
        /// <param name="columnToMemberMap"><para>The <see cref="IDictionary{TKey, TValue}"/> to map the <see cref="DataColumn"/> values to properties.</para></param>
        /// <returns><para>A yielded instance of the entity type.</para></returns>
        public static IEnumerable<TEntity> ToEntities(IEnumerable<DataRow> dataRows, IDictionary<DataColumn, string> columnToMemberMap)
        {
            if (dataRows == null) throw new ArgumentNullException(nameof(dataRows));
            if (columnToMemberMap == null) throw new ArgumentNullException(nameof(columnToMemberMap));
            Func<DataRow, TEntity> converter = GetConverter(columnToMemberMap);
            if (converter == null) throw new InvalidOperationException($"Unable to generate a converter for the data table. This could be due to a bug in the expression generator.");
            //Iterate over the data rows in the data table, yielding the results back to the caller.
            foreach (DataRow row in dataRows)
            {
                yield return converter(row);
            }
        }

        /// <summary><para>Iterates over the <see cref="DataRow"/> values in <paramref name="dataRows"/>, mapping their fields
        /// to the entity type.</para></summary>
        /// <param name="dataRows"><para>The <see cref="Array"/> of <see cref="DataRow"/> values to map to the entity type.</para></param>
        /// <returns><para>A yielded instance of the entity type.</para></returns>
        public static IEnumerable<TEntity> ToEntities(params DataRow[] dataRows)
        {
            if (dataRows == null) throw new ArgumentNullException(nameof(dataRows));
            if (dataRows.Length == 0) throw new ArgumentException($"Parameter {nameof(dataRows)} should have at least 1 value, 0 given.");
            return ToEntities(dataRows, dataRows[0].Table.Columns.Select(GetDataColumnKeyValuePair).ToDictionary());
        }

        /// <summary><para>Iterates over the <see cref="DataRow"/> values in <paramref name="dataRows"/>, mapping their fields
        /// to the entity type.</para></summary>
        /// <param name="columnToMemberMap"><para>The <see cref="Dictionary{TKey, TValue}"/> to map the <see cref="DataColumn"/> values to properties.</para></param>
        /// <param name="dataRows"><para>The <see cref="Array"/> of <see cref="DataRow"/> values to map to the entity type.</para></param>
        /// <returns><para>A yielded instance of the entity type.</para></returns>
        public static IEnumerable<TEntity> ToEntities(IDictionary<DataColumn, string> columnToMemberMap, params DataRow[] dataRows)
        {
            if (dataRows == null) throw new ArgumentNullException(nameof(dataRows));
            if (columnToMemberMap == null) throw new ArgumentNullException(nameof(columnToMemberMap));
            Func<DataRow, TEntity> converter = GetConverter(columnToMemberMap);
            if (converter == null) throw new InvalidOperationException($"Unable to generate a converter for the data table. This could be due to a bug in the expression generator.");
            //Iterate over the data rows in the data table, yielding the results back to the caller.
            foreach (DataRow row in dataRows)
            {
                yield return converter(row);
            }
        }

        //Creates a cached generator for the converter to use for all data tables with a matching set of data columns. Due to our assumption that data types are loose in
        //data tables, this will generate an extremely generic converter that doesn't statically use the data table's data typing.
        private static Func<DataRow, TEntity> GetConverter(IDictionary<DataColumn, string> columnToMemberMap)
        {
            List<string> columnNames = columnToMemberMap.Select(k => k.Key.ColumnName).ToList();
            if (!converters.TryGetValue(columnNames, out Func<DataRow, TEntity> value))
            {
                NewExpression instantiate = Expression.New(type);
                ParameterExpression dataRow = Expression.Parameter(typeof(DataRow), "dataRow");
                //Declare a dictionary to prevent creating more than 1 instance of a temporary parser type.
                ConcurrentDictionary<Type, ParameterExpression> parameters = new ConcurrentDictionary<Type, ParameterExpression>();
                List<MemberBinding> memberBindings = new List<MemberBinding>();
                //Iterate over the column map and generate the expressions needed.
                foreach (KeyValuePair<DataColumn, string> columnToMember in columnToMemberMap)
                {
                    if (TryGetMemberInfo(columnToMember.Value, columnToMember.Key.Table.CaseSensitive, out MemberInfo memberInfo))
                    {
                        //Get a member expression. This is used for type resolution.
                        Type memberType = Expression.MakeMemberAccess(instantiate, memberInfo).Type,
                            realType = Nullable.GetUnderlyingType(memberType) ?? memberType;
                        //Get the data column expression used to access the data row field.
                        DataColumnExpression dataColumn = DataExpression.DataColumn(DataExpression.DataTable(dataRow), columnToMember.Key.ColumnName);
                        //Get the is null expression used to determine if the data field is null.
                        DataFieldIsNullExpression callDataFieldIsNull = DataExpression.DataFieldIsNull(dataRow, dataColumn);
                        ConditionalExpression dataFieldIsNull = Expression.Condition(callDataFieldIsNull, Expression.Default(memberType),
                            Expression.Default(memberType));
                        //Get the is assignable from expression used to determine if the data column can be converted to the member type.
                        IsAssignableFromExpression callIsAssignableFrom = DataExpression.IsAssignableFrom(DataExpression.DataType(dataColumn), memberType);
                        //Get the is assignable from true result. If member type is string, then call the trim method on the string.
                        Expression isAssignableFromTrue = Expression.Convert(DataExpression.DataField(dataRow, dataColumn), memberType);
                        if (typeof(string).IsAssignableFrom(memberType))
                        {
                            isAssignableFromTrue = Expression.Call(isAssignableFromTrue, "Trim", Type.EmptyTypes);
                        }
                        //Get the conditional expression used to process the data column type conversion.
                        ConditionalExpression isAssignableFrom = Expression.Condition(callIsAssignableFrom,
                            isAssignableFromTrue, Expression.Default(memberType));
                        if (realType.GetMethods().Any(m => m.Name == "TryParse"))
                        {
                            //Get the is string parameter expression used for try parsing.
                            ParameterExpression stringLocal = parameters.GetOrAdd(typeof(string), Expression.Variable(typeof(string))),
                                outLocal = parameters.GetOrAdd(realType, Expression.Variable(realType));
                            //Get the type is assign and try parse expressions used for try parsing.
                            TypeIsAssignExpression typeIsAssign = DataExpression.TypeIsAssign(
                                DataExpression.DataField(dataRow, dataColumn), stringLocal);
                            TryParseExpression callTryParse = DataExpression.TryParse(stringLocal, outLocal);
                            //Get the conditional expression used to process the try parse conversion.
                            ConditionalExpression tryParse = Expression.Condition(
                                Expression.AndAlso(typeIsAssign, callTryParse),
                                Expression.Convert(outLocal, memberType), Expression.Default(memberType));
                            //Update is assignable from with the try parse method.
                            isAssignableFrom = isAssignableFrom.Update(isAssignableFrom.Test, isAssignableFrom.IfTrue, tryParse);
                        }
                        if (typeof(IConvertible).IsAssignableFrom(realType))
                        {
                            //Get the change type expression.
                            ChangeTypeExpression callChangeType = DataExpression.ChangeType(
                                DataExpression.DataField(dataRow, dataColumn), realType);
                            //Get the conversion expression for the change type. If member type is string, then call the trim method on the string.
                            Expression changeType = Expression.Convert(callChangeType, memberType);
                            if (typeof(string).IsAssignableFrom(memberType))
                            {
                                changeType = Expression.Call(changeType, "Trim", Type.EmptyTypes);
                            }
                            //Determine if try parse is set. If so, update it and is assignable from. Otherwise, add change type to is assignable from.
                            if (isAssignableFrom.IfFalse is ConditionalExpression tryParse)
                            {
                                tryParse = tryParse.Update(tryParse.Test, tryParse.IfTrue, changeType);
                                isAssignableFrom = isAssignableFrom.Update(isAssignableFrom.Test, isAssignableFrom.IfTrue, tryParse);
                            }
                            else
                            {
                                isAssignableFrom = isAssignableFrom.Update(isAssignableFrom.Test, isAssignableFrom.IfTrue, changeType);
                            }
                        }
                        dataFieldIsNull = dataFieldIsNull.Update(dataFieldIsNull.Test, dataFieldIsNull.IfTrue, isAssignableFrom);
                        //Silently ignore conversion errors with the try catch expression. This is mostly for testing purposes and might be removed
                        //in the future.
                        memberBindings.Add(Expression.Bind(memberInfo, Expression.TryCatch(dataFieldIsNull,
                            Expression.Catch(typeof(Exception), Expression.Default(memberType)))));
                    }
                }
                //Get the converter lambda expression, creating the initialization block with its parameters.
                Expression<Func<DataRow, TEntity>> converter = Expression.Lambda<Func<DataRow, TEntity>>(
                    Expression.Block(parameters.Values, Expression.MemberInit(instantiate, memberBindings)), dataRow);
                //Use expression reducer to reduce the conversion lambda expression, then compile and return the delegate.
                converter = (Expression<Func<DataRow, TEntity>>)new ExpressionReducer().Visit(converter);
                value = converters.GetOrAdd(new HashSet<string>(columnNames), converter.Compile());
            }
            return value;
        }

        //Attempts to get a member info object from the entity type the converter is converting to, returning true if one is found.
        private static bool TryGetMemberInfo(string memberName, bool caseSensitive, out MemberInfo memberInfo)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            //Determine if the case sensitivite bool is false and add the ignore case flag to the binding search.
            if (!caseSensitive)
            {
                flags |= BindingFlags.IgnoreCase;
            }
            //Determine if a property exists with the name member name and has the appropriate flags and set the out variable to this property info.
            if (type.GetProperty(memberName, flags) is PropertyInfo propertyInfo)
            {
                memberInfo = propertyInfo;
            }
            //Determine if a field exists with the name member name and has the appropriate flags and set the out variable to this field info.
            else if (type.GetField(memberName, flags) is FieldInfo fieldInfo)
            {
                memberInfo = fieldInfo;
            }
            //If no property or field exists in the type with the name given, assume that there is no valid target and set the out variable to null.
            else
            {
                memberInfo = null;
            }
            return memberInfo != null;
        }

        //Creates a key value pair for the data column/column's name. This is used whenever no dictionary is provided to seed the converter's data column => property map.
        private static KeyValuePair<DataColumn, string> GetDataColumnKeyValuePair(DataColumn dataColumn) =>
            new KeyValuePair<DataColumn, string>(dataColumn, dataColumn.ColumnName);
    }
}