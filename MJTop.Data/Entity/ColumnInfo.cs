using System;
using System.ComponentModel;
using System.Data;

namespace MJTop.Data
{
    /// <summary>
    /// 列信息
    /// </summary>
    [Serializable]
    public class ColumnInfo
    {
        /// <summary>
        /// 序号
        /// </summary>
        [DisplayName("序号")]
        public int Colorder
        {
            get;
            set;
        }

        /// <summary>
        /// 列名
        /// </summary>
        [DisplayName("列名")]
        public string ColumnName
        {
            get;
            set;
        }


        /// <summary>
        /// 数据类型
        /// </summary>
        [DisplayName("数据类型")]
        public string TypeName
        {
            get;
            set;
        }

        /// <summary>
        /// 列说明
        /// </summary>
        [DisplayName("列说明")]
        public string DeText
        {
            get;
            set;
        }
        
        /// <summary>
        /// 字段长度 max 或特殊数据类型 使用 -1 表示！
        /// </summary>
        [DisplayName("长度")]
        public long? Length
        {
            get;
            set;
        }

        /// <summary>
        /// 小数点后保留位数
        /// </summary>
        [DisplayName("小数位数")]
        public int? Scale
        {
            get;
            set;
        }


        /// <summary>
        /// 是否自增列
        /// </summary>
        [DisplayName("是否为自增")]
        public bool IsIdentity
        {
            get;
            set;
        }



        /// <summary>
        /// 是否主键
        /// </summary>
        [DisplayName("是否为主键")]
        public bool IsPK
        {
            get;
            set;
        }

        /// <summary>
        /// 是否可为Null
        /// </summary>
        [DisplayName("是否可为空")]
        public bool CanNull
        {
            get;
            set;
        }

        /// <summary>
        /// 默认值
        /// </summary>
        [DisplayName("默认值")]
        public string DefaultVal
        {
            get;
            set;
        }

        /// <summary>
        /// 近似类型
        /// </summary>
        [DisplayName("近似类型")]
        public LikeType LikeType
        {
            get
            {
                if (this.DbType == DbType.Decimal || this.DbType == DbType.Double
                    || this.DbType == DbType.Int16
                    || this.DbType == DbType.Int32
                     || this.DbType == DbType.Int64
                    )
                {
                    return LikeType.Number;
                }
                else if (this.DbType == DbType.Date || this.DbType == DbType.DateTime
                    || this.DbType == DbType.DateTime2 || this.DbType == DbType.DateTimeOffset)
                {
                    return LikeType.DateTime;
                }
                else
                {
                    return LikeType.String;
                }
            }
        }

        /// <summary>
        /// DbType类型
        /// </summary>
        [DisplayName("DbType类型")]
        public DbType DbType
        {
            get;
            set;
        }

    }
}
