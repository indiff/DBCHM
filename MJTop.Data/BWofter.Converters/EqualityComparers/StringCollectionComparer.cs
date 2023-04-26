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

namespace BWofter.Converters.EqualityComparers
{
    using System;
    using System.Collections.Generic;

    /// <summary><para>A utility class that is used to compare string collections within the cache object.</para></summary>
    public class StringCollectionComparer : IEqualityComparer<ICollection<string>>
    {
        private static readonly Lazy<StringCollectionComparer> instance = new Lazy<StringCollectionComparer>(true);

        /// <summary><para>Returns the singleton of <see cref="StringCollectionComparer"/>.</para></summary>
        /// <returns>The singleton of <see cref="StringCollectionComparer"/>.</returns>
        public static IEqualityComparer<ICollection<string>> GetInstance() => instance.Value;

        /// <summary><para>Determines whether the specified objects are equal.</para></summary>
        /// <param name="x"><para>The first object of type <see cref="ICollection{T}"/> of <see cref="string"/> values to compare.</para></param>
        /// <param name="x"><para>The second object of type <see cref="ICollection{T}"/> of <see cref="string"/> values to compare.</para></param>
        /// <returns><para><see langword="true"/> if the specified objects are equal; otherwise, <see langword="false"/>.</para></returns>
        public bool Equals(ICollection<string> x, ICollection<string> y)
        {
            //Determines if x and y are the same instance and returns true.
            if (x == y)
            {
                return true;
            }
            //Determines if either x or y is null and returns false.
            else if (x == null || y == null)
            {
                return false;
            }
            //Determines if x is a different length from y and returns false.
            else if (x.Count != y.Count)
            {
                return false;
            }
            //Determines if x is a hash set and uses it as the contains target.
            else if (x is HashSet<string>)
            {
                //Iterate over the values in y and determine if x contains them. If any are not contained in x, return false.
                foreach (string s in y)
                {
                    if (!x.Contains(s))
                    {
                        return false;
                    }
                }
            }
            //The default option.
            else
            {
                //Iterate over the values in x and determine if y contains them. If any are not contained in y, return false.
                foreach (string s in x)
                {
                    if (!y.Contains(s))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary><para>Returns a hash code for the specified object.</para></summary>
        /// <param name="obj"><para>The <see cref="Object"/> for which a hash code is to be returned.</para></param>
        /// <returns><para>A hash code for the specified object.</para></returns>
        public int GetHashCode(ICollection<string> obj)
        {
            //Determine if obj is null and return 0.
            if (obj == null)
            {
                return 0;
            }
            int hashCode = 2;
            //Enter an overflow unchecked areaa, iterate over the strings in the collection, and create a compound hash code using the hash code of each string.
            unchecked
            {
                foreach (string s in obj)
                {
                    hashCode <<= (obj?.GetHashCode() ?? 10) * 230182;
                }
            }
            return hashCode;
        }
    }
}