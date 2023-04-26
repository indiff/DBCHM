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

namespace BWofter.Converters.Expressions
{
    using System;
    using System.Data;
    using System.Linq.Expressions;
    using System.Reflection;

    public abstract class DataExpression : Expression
    {
        public override bool CanReduce => true;
        public override ExpressionType NodeType => ExpressionType.Extension;
        protected static readonly Type objectType = typeof(object);
        protected static readonly Type boolType = typeof(bool);
        protected static readonly Type typeType = typeof(Type);
        protected static readonly Type typeInfoType = typeof(TypeInfo);
        protected static readonly Type dataColumnType = typeof(DataColumn);
        protected static readonly Type dataTableType = typeof(DataTable);
        protected static readonly Type dataRowType = typeof(DataRow);
        protected static readonly Type intType = typeof(int);
        protected static readonly Type stringType = typeof(string);
        protected static readonly Type dataSetType = typeof(DataSet);
        protected static readonly Type convertType = typeof(Convert);

        public static IsAssignableFromExpression IsAssignableFrom(Type target, Type other) =>
            new IsAssignableFromExpression(Constant(target), Constant(other));

        public static IsAssignableFromExpression IsAssignableFrom(Expression other, Type target) =>
            new IsAssignableFromExpression(Constant(target), other);

        public static IsAssignableFromExpression IsAssignableFrom(Expression target, Expression other) =>
            new IsAssignableFromExpression(target, other);

        public static DataColumnExpression DataColumn(Expression dataTable, int columnNumber) =>
            new DataColumnExpression(dataTable, Constant(columnNumber));

        public static DataColumnExpression DataColumn(Expression dataTable, string columnName) =>
            new DataColumnExpression(dataTable, Constant(columnName));

        public static DataColumnExpression DataColumn(Expression dataTable, Expression columnName) =>
            new DataColumnExpression(dataTable, columnName);

        public static DataTableExpression DataTable(Expression dataMember, int tableNumber) =>
            new DataTableExpression(dataMember, Constant(tableNumber));

        public static DataTableExpression DataTable(Expression dataMember, string tableName) =>
            new DataTableExpression(dataMember, Constant(tableName));

        public static DataTableExpression DataTable(Expression dataMember, string tableName, string tableNamespace) =>
            new DataTableExpression(dataMember, Constant(tableName), Constant(tableNamespace));

        public static DataTableExpression DataTable(Expression dataMember, Expression tableName, string tableNamespace) =>
            new DataTableExpression(dataMember, tableName, Constant(tableNamespace));

        public static DataTableExpression DataTable(Expression dataMember, Expression tableName = null, Expression tableNamespace = null) =>
            new DataTableExpression(dataMember, tableName, tableNamespace);

        public static DataTypeExpression DataType(Expression dataColumn) =>
            new DataTypeExpression(dataColumn);

        public static DataFieldExpression DataField(Expression dataRow, int columnNumber) =>
            new DataFieldExpression(dataRow, DataColumn(DataTable(dataRow), columnNumber));

        public static DataFieldExpression DataField(Expression dataRow, string columnName) =>
            new DataFieldExpression(dataRow, DataColumn(DataTable(dataRow), columnName));

        public static DataFieldExpression DataField(Expression dataRow, Expression dataColumn) =>
            new DataFieldExpression(dataRow, dataColumn);

        public static TypeIsAssignExpression TypeIsAssign(Expression left, ParameterExpression right) =>
            new TypeIsAssignExpression(left, right, right.Type);

        public static TypeIsAssignExpression TypeIsAssign(Expression left, Expression right, Type type) =>
            new TypeIsAssignExpression(left, right, type);

        public static TryParseExpression TryParse(Expression inParameter, ParameterExpression outParameter) =>
            new TryParseExpression(inParameter, outParameter, outParameter.Type);

        public static TryParseExpression TryParse(Expression inParameter, Expression outParameter, Type type) =>
            new TryParseExpression(inParameter, outParameter, type);

        public static ChangeTypeExpression ChangeType(Expression operand, Type type) =>
            new ChangeTypeExpression(operand, type);

        public static DataFieldIsNullExpression DataFieldIsNull(Expression dataRow, int columnNumber) =>
            new DataFieldIsNullExpression(dataRow, DataColumn(DataTable(dataRow), columnNumber));

        public static DataFieldIsNullExpression DataFieldIsNull(Expression dataRow, string columnName) =>
            new DataFieldIsNullExpression(dataRow, DataColumn(DataTable(dataRow), columnName));

        public static DataFieldIsNullExpression DataFieldIsNull(Expression dataRow, Expression dataColumn) =>
            new DataFieldIsNullExpression(dataRow, dataColumn);

        public abstract override Expression Reduce();
    }
}