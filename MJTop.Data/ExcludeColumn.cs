using System.Collections.Specialized;

namespace MJTop.Data
{
    /// <summary>
    /// 排除列
    /// </summary>
    public class ExcludeColumn
    {
        /// <summary>
        /// 表列集合
        /// </summary>
        internal NameValueCollection Coll { get; set; }

        private DB Db { get; set; }

        internal ExcludeColumn(DB db)
        {
            this.Db = db;
            this.Coll = new NameValueCollection();
        }

        /// <summary>
        /// 需排除的 表名与多个列名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnNames">一个或多个列名</param>
        public void Add(string tableName, params string[] columnNames)
        {
            Db.CheckTabStuct(tableName, columnNames);

            if (columnNames != null && columnNames.Length > 0)
            {
                foreach (var columnName in columnNames)
                {
                    this.Coll.Add(tableName, columnName);
                }
            }
        }

        /// <summary>
        /// 需排除的 表名与列名集合
        /// </summary>
        /// <param name="tableColumns">表列集合</param>
        public void AddRange(NameValueCollection tableColumns)
        {
            if (tableColumns == null)
            {
                foreach (var tableName in tableColumns.AllKeys)
                {
                    string[] columnNames;
                    if (tableColumns.TryGetValues(tableName, out columnNames))
                    {
                        Db.CheckTabStuct(tableName, columnNames);

                        foreach (var columnName in columnNames)
                        {
                            this.Coll.Add(tableName, columnName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 清空排除项
        /// </summary>
        public void Clear()
        {
            this.Coll.Clear();
        }

        /// <summary>
        /// 删除对应表的 排除项
        /// </summary>
        /// <param name="tableName">表名</param>
        public void Remove(string tableName)
        {
            Db.CheckTabStuct(tableName);
            this.Coll.Remove(tableName);
        }

        /// <summary>
        /// 删除指定的排除项
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnName">列名</param>
        /// <returns>返回bool</returns>
        public bool Remove(string tableName, string columnName)
        {
            Db.CheckTabStuct(tableName, columnName);

            return this.Coll.Remove(tableName, columnName);
        }
    }
}