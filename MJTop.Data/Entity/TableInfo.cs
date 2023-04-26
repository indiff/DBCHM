using System;
using System.Collections.Generic;

namespace MJTop.Data
{
    /// <summary>
    /// 表信息
    /// </summary>
    [Serializable]
    public class TableInfo
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 表描述
        /// </summary>
        public string TabComment { get; set; }

        /// <summary>
        /// 表数据行数
        /// </summary>
        public int TableRows { get; set; }

        /// <summary>
        /// 该表包含的所有列
        /// </summary>
        public List<ColumnInfo> Colnumns { get; set; }

        /// <summary>
        /// 主键列名
        /// </summary>
        public string PriKeyColName { get; set; }

        /// <summary>
        /// 主键类型
        /// </summary>
        public PrimaryKeyType PriKeyType { get; set; }
    }
}