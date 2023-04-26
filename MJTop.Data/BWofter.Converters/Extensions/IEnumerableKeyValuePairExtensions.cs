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
    using System.Linq;

    /// <summary><para>A static class that provides LINQ extensions for <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/> values.</para></summary>
    public static class IEnumerableKeyValuePairExtensions
    {
        /// <summary><para>Returns a <see cref="Dictionary{TKey, TValue}"/> with the key type <typeparamref name="TKey"/> and value type <typeparamref name="TValue"/>,
        /// initialized with the values in <paramref name="dictionarySource"/>.</para></summary>
        /// <typeparam name="TKey"><para>The <see cref="Dictionary{TKey, TValue}"/> key type.</para></typeparam>
        /// <typeparam name="TValue"><para>The <see cref="Dictionary{TKey, TValue}"/> value type.</para></typeparam>
        /// <param name="dictionarySource"><para>An <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/> values that contain the initial data of the dictionary.</para></param>
        /// <returns><para><see cref="Dictionary{TKey, TValue}"/> with the key type <typeparamref name="TKey"/> and value type <typeparamref name="TValue"/>.</para></returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dictionarySource)
        {
            if (dictionarySource == null) throw new ArgumentNullException(nameof(dictionarySource));
            //Instantiate a new dictionary with a number of entries equal to the length of the source, then sets gD equal to the reference of dictionary. This is to prevent extra casting operations.
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(dictionarySource.Count());
            ICollection<KeyValuePair<TKey, TValue>> gD = dictionary;
            foreach (KeyValuePair<TKey, TValue> entry in dictionarySource)
            {
                //Add the key value pair to gD.
                gD.Add(entry);
            }
            return dictionary;
        }
    }
}