namespace MJTop.Data
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DBType
    {
        /// <summary>
        /// SqlServer数据库
        /// </summary>
        SqlServer,

        /// <summary>
        /// MySql数据库
        /// </summary>
        MySql,

        /// <summary>
        /// Oracle数据库(内置使用 Oracle.ManagedDataAccess 客户端)
        /// </summary>
        Oracle,

        /// <summary>
        /// Oracle数据库(内置使用 DDTek.Oracle 客户端)
        /// </summary>
        OracleDDTek,

        /// <summary>
        /// PostgreSql数据库
        /// </summary>
        PostgreSql,

        /// <summary>
        /// SQLite数据库
        /// </summary>
        SQLite,

        /// <summary>
        /// DB2数据库
        /// </summary>
        DB2
    }

    /// <summary>
    /// 主键类型
    /// </summary>
    public enum PrimaryKeyType
    {
        /// <summary>
        /// 没有主键或未知
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// 主键的值是自增方式
        /// </summary>
        AUTO,

        /// <summary>
        /// 主键的值是插入前设置的方式
        /// </summary>
        SET
    }

    /// <summary>
    /// 保存类型
    /// </summary>
    public enum SaveType
    {
        /// <summary>
        /// 插入
        /// </summary>
        Insert,

        /// <summary>
        /// 更新
        /// </summary>
        Update
    }

    /// <summary>
    /// 相似类型
    /// </summary>
    public enum LikeType
    {
        /// <summary>
        /// 数值类型
        /// </summary>
        Number,

        /// <summary>
        /// 字符类型
        /// </summary>
        String,

        /// <summary>
        /// 日期类型
        /// </summary>
        DateTime
    }

    /// <summary>
    /// 分组计算类型
    /// </summary>
    //public enum GroupCalc
    //{
    //    /// <summary>
    //    /// 分组求每组总数
    //    /// </summary>
    //    COUNT,
    //    /// <summary>
    //    /// 分组求每组总和
    //    /// </summary>
    //    SUM,
    //    /// <summary>
    //    ///  分组求每组平均值
    //    /// </summary>
    //    AVG,
    //    /// <summary>
    //    /// 分组求每组最大值
    //    /// </summary>
    //    MAX,
    //    /// <summary>
    //    /// 分组求每组最小值
    //    /// </summary>
    //    MIN
    //}
}