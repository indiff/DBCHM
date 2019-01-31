namespace TryOpenXml.Dtos
{
    /// <summary>
    /// 数据库表字段dto
    /// </summary>
    public class ColumnDto
    {

        /// <summary>
        /// 序号
        /// </summary>
        public string ColumnOrder { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string ColumnTypeName { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public string Length { get; set; }

        /// <summary>
        /// 小数位
        /// </summary>
        public string Scale { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public string IsPK { get; set; }

        /// <summary>
        /// 自增
        /// </summary>
        public string IsIdentity { get; set; }

        /// <summary>
        /// 允许空
        /// </summary>
        public string CanNull { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultVal { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { get; set; }

    }
}
