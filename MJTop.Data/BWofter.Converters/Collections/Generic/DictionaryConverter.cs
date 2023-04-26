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

namespace BWofter.Converters.Collections.Generic
{
    using BWofter.Converters.Data;
    using BWofter.Converters.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    /// <summary><para>A static class used to convert dictionaries into entities.</para></summary>
    /// <typeparam name="TEntity"><para>The entity type to convert to.</para></typeparam>
    public static class DictionaryConverter<TEntity> where TEntity : class, new()
    {
        /// <summary><para>Iterates over the <see cref="IDictionary{TKey, TValue}"/> of <see cref="string"/> keys and <see cref="object"/> values in the
        /// <paramref name="dictionary"/>, mapping their fields to the entity type.</para></summary>
        /// <param name="dictionary"><para>The <see cref="IDictionary{TKey, TValue}"/> instance of <see cref="string"/> keys and <see cref="object"/> values
        /// to map to the entity type.</para></param>
        /// <returns><para>An instance of the entity type.</para></returns>
        public static TEntity ToEntity(IDictionary<string, object> dictionary)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            return ToEntities(new[] { dictionary }).FirstOrDefault();
        }

        /// <summary><para>Iterates over the <see cref="IDictionary{TKey, TValue}"/> of <see cref="string"/> keys and <see cref="object"/> values in the
        /// <paramref name="dictionary"/>, mapping their fields to the entity type.</para></summary>
        /// <param name="dictionary"><para>The <see cref="IDictionary{TKey, TValue}"/> instance of <see cref="string"/> keys and <see cref="object"/> values
        /// to map to the entity type.</para></param>
        /// <param name="columnToMemberMap"><para>The <see cref="IDictionary{TKey, TValue}"/> to map the <see cref="string"/> keys to properties.</para></param>
        /// <returns><para>An instance of the entity type.</para></returns>
        public static TEntity ToEntity(IDictionary<string, object> dictionary, IDictionary<string, string> columnToMemberMap)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (columnToMemberMap == null) throw new ArgumentNullException(nameof(columnToMemberMap));
            return ToEntities(new[] { dictionary }, columnToMemberMap).FirstOrDefault();
        }

        /// <summary><para>Iterates over the <see cref="IDictionary{TKey, TValue}"/> of <see cref="string"/> keys and <see cref="object"/> values in the
        /// <paramref name="dictionaries"/>, mapping their fields to the entity type.</para></summary>
        /// <param name="dictionaries"><para>The <see cref="IDictionary{TKey, TValue}"/> instances of <see cref="string"/> keys and <see cref="object"/> values
        /// to map to the entity type.</para></param>
        /// <returns><para>A yielded instance of the entity type.</para></returns>
        public static IEnumerable<TEntity> ToEntities(IEnumerable<IDictionary<string, object>> dictionaries)
        {
            IDictionary<string, object> first = dictionaries?.FirstOrDefault();
            if (dictionaries == null) throw new ArgumentNullException(nameof(dictionaries));
            if (first == null) throw new ArgumentException($"Parameter {nameof(dictionaries)} should have at least 1 value, 0 given.");
            //Get a table definition for the dictionaries then call the data table converter.
            DataTable dataTable = first.AsDataTableDefinition();
            return DataTableConverter<TEntity>.ToEntities(dictionaries.Select(d => DictionaryToDataRow(d, dataTable)));
        }

        /// <summary><para>Iterates over the <see cref="IDictionary{TKey, TValue}"/> of <see cref="string"/> keys and <see cref="object"/> values in the
        /// <paramref name="dictionaries"/>, mapping their fields to the entity type.</para></summary>
        /// <param name="dictionaries"><para>The <see cref="IDictionary{TKey, TValue}"/> instances of <see cref="string"/> keys and <see cref="object"/> values
        /// to map to the entity type.</para></param>
        /// <param name="columnToMemberMap"><para>The <see cref="IDictionary{TKey, TValue}"/> to map the <see cref="string"/> keys to properties.</para></param>
        /// <returns><para>A yielded instance of the entity type.</para></returns>
        public static IEnumerable<TEntity> ToEntities(IEnumerable<IDictionary<string, object>> dictionaries, IDictionary<string, string> columnToMemberMap)
        {
            IDictionary<string, object> first = dictionaries?.FirstOrDefault();
            if (dictionaries == null) throw new ArgumentNullException(nameof(dictionaries));
            if (columnToMemberMap == null) throw new ArgumentNullException(nameof(columnToMemberMap));
            if (first == null) throw new ArgumentException($"Parameter {nameof(dictionaries)} should have at least 1 value, 0 given.");
            //Get a table definition for the dictionaries then call the data table converter.
            DataTable dataTable = first.AsDataTableDefinition();
            //Converts the sting/string column to member map into a data column/string column to member map.
            Dictionary<DataColumn, string> translatedColumnToMemberMap = new Dictionary<DataColumn, string>();
            foreach (KeyValuePair<string, string> entry in columnToMemberMap)
            {
                translatedColumnToMemberMap.Add(dataTable.Columns[entry.Key], entry.Value);
            }
            return DataTableConverter<TEntity>.ToEntities(dictionaries.Select(d => DictionaryToDataRow(d, dataTable)), translatedColumnToMemberMap);
        }

        //Generates a new data row for each dictionary in the the ienumerable.
        private static DataRow DictionaryToDataRow(IDictionary<string, object> rowData, DataTable dataTable)
        {
            //Create a new data row and add it to the data table.
            DataRow dataRow = dataTable.NewRow();
            dataTable.Rows.Add(dataRow);
            //Iterate over the entries in the dictionary and add them to the row.
            foreach (KeyValuePair<string, object> entry in rowData)
            {
                dataRow[entry.Key] = entry.Value;
            }
            return dataRow;
        }
    }
}