using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MJTop.Data
{
    /// <summary>
    /// 忽略Key值大小写的字典集合
    /// </summary>
    /// <typeparam name="TValue">Value类型</typeparam>
    public class IgCaseDictionary<TValue> : Dictionary<string, TValue>
    {
        /// <summary>
        /// 大写/小写
        /// </summary>
        public KeyCase Case { get; private set; } = KeyCase.Lower;

        private ReaderWriterLockSlim Locker { get; set; }

        /// <summary>
        /// 构造函数，默认全部小写
        /// </summary>
        public IgCaseDictionary()
        {
            this.Case =  KeyCase.Lower;
            this.Locker = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// 构造函数,指定存储 大写？小写
        /// </summary>
        /// <param name="keyCase">大写/小写</param>
        public IgCaseDictionary(KeyCase keyCase)
        {
            this.Case = keyCase;
            this.Locker = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public new void Add(string key, TValue value)
        {
            try
            {
                Locker.EnterWriteLock();

                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentException("key不能为空！[" + key + "]", "key");
                }
                base.Add((Case == KeyCase.Lower ? key.ToLower() : key.ToUpper()), value);
            }
            catch (Exception ex)
            {
                LogUtils.LogError("IgCaseDictionary", Developer.SysDefault, ex, "Key:" + key + "Value:" + value);
                throw ex;
            }
            finally
            {
                Locker.ExitWriteLock();
            }
        }


        public new TValue this[string key]
        {
            get
            {
                try
                {
                    Locker.EnterReadLock();

                    if (string.IsNullOrWhiteSpace(key))
                    {
                        throw new ArgumentException("key不能为空！[" + key + "]", "key");
                    }
                    return base[(Case == KeyCase.Lower ? key.ToLower() : key.ToUpper())];
                }
                catch(Exception ex)
                {
                    LogUtils.LogError("IgCaseDictionary", Developer.SysDefault, ex, "Key:" + key);
                    throw ex;
                }
                finally
                {
                    Locker.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    Locker.EnterWriteLock();

                    if (string.IsNullOrWhiteSpace(key))
                    {
                        throw new ArgumentException("key不能为空！[" + key + "]", "key");
                    }
                    base[(Case == KeyCase.Lower ? key.ToLower() : key.ToUpper())] = value;
                }
                finally
                {
                    Locker.ExitWriteLock();
                }
            }
        }

        public new bool ContainsKey(string key)
        {
            try
            {
                Locker.EnterReadLock();

                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentException("key不能为空！[" + key + "]", "key");
                }
                return base.ContainsKey((Case == KeyCase.Lower ? key.ToLower() : key.ToUpper()));
            }
            finally
            {
                Locker.ExitReadLock();
            }
        }

        public new bool Remove(string key)
        {
            try
            {
                Locker.EnterWriteLock();

                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentException("key不能为空！[" + key + "]", "key");
                }
                return base.Remove((Case == KeyCase.Lower ? key.ToLower() : key.ToUpper()));
            }
            finally
            {
                Locker.ExitWriteLock();
            }
        }

        public new bool TryGetValue(string key, out TValue value)
        {
            try
            {
                Locker.EnterReadLock();

                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentException("key不能为空！[" + key + "]", "key");
                }
                return base.TryGetValue((Case == KeyCase.Lower ? key.ToLower() : key.ToUpper()), out value);
            }
            finally
            {
                Locker.ExitReadLock();
            }
        }

        public Dictionary<string, TValue> Dictionary()
        {
            return this;
        }
    }

    /// <summary>
    /// 键大小写枚举
    /// </summary>
    public enum KeyCase
    {
        Lower,
        Upper
    }
}
