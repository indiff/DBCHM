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

namespace BWofter.Converters.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    /// <summary><para>A static class that provides LINQ extensions for the <see cref="DataColumnCollection"/> type.</para></summary>
    public static class DataColumnCollectionExtensions
    {
        /// <summary><para>Returns an <see cref="IEnumerable{T}"/> of <see cref="DataColumn"/> values that represents <paramref name="collection"/>.</para></summary>
        /// <param name="collection"><para>The target of the extension method.</para></param>
        /// <returns><para><see cref="IEnumerable{T}"/> of <see cref="DataColumn"/> values or <see cref="Enumerable.Empty{TResult}"/>.</para></returns>
        public static IEnumerable<DataColumn> Select(this DataColumnCollection collection) =>
            collection.Select(dc => dc);

        /// <summary><para>Returns an <see cref="IEnumerable{T}"/> of <see cref="DataColumn"/> values that represents <paramref name="collection"/>.</para></summary>
        /// <param name="collection"><para>The target of the extension method.</para></param>
        /// <param name="selector"><para>The selector of the extension method.</para></param>
        /// <returns><para><see cref="IEnumerable{T}"/> of <see cref="DataColumn"/> values or <see cref="Enumerable.Empty{TResult}"/>.</para></returns>
        public static IEnumerable<T> Select<T>(this DataColumnCollection collection, Func<DataColumn, T> selector)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            //Determines if there are no entries in the collection. If not, return an empty enumerable. Otherwise, process the selector.
            if (collection.Count == 0)
            {
                return Enumerable.Empty<T>();
            }
            else
            {
                return collection.Cast<DataColumn>().Select(selector);
            }
        }

        /// <summary><para>Returns an <see cref="IEnumerable{T}"/> of <see cref="DataColumn"/> values that represents <paramref name="collection"/>.</para></summary>
        /// <param name="collection"><para>The target of the extension method.</para></param>
        /// <param name="predicate"><para>The predicate of the extension method.</para></param>
        /// <returns><para><see cref="IEnumerable{T}"/> of <see cref="DataColumn"/> values or <see cref="Enumerable.Empty{TResult}"/>.</para></returns>
        public static IEnumerable<DataColumn> Where(this DataColumnCollection collection, Func<DataColumn, bool> predicate)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (collection.Count == 0)
            {
                return Enumerable.Empty<DataColumn>();
            }
            else
            {
                return collection.Cast<DataColumn>().Where(predicate);
            }
        }
    }
}