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
    using System.Linq.Expressions;
    public sealed class DataFieldExpression : DataExpression
    {
        public Expression DataRow { get; }
        public new Expression DataColumn { get; }
        public override Type Type => objectType;
        public DataFieldExpression(Expression dataRow, Expression dataColumn)
        {
            DataRow = dataRow ?? throw new ArgumentNullException(nameof(dataRow));
            DataColumn = dataColumn ?? throw new ArgumentNullException(nameof(dataColumn));
        }
        public override Expression Reduce()
        {
            if (!dataRowType.IsAssignableFrom(DataRow.Type))
            {
                throw new InvalidOperationException($"");
            }
            if (!dataColumnType.IsAssignableFrom(DataColumn.Type))
            {
                throw new InvalidOperationException($"");
            }
            return Property(DataRow, "Item", DataColumn);
        }
    }
}
