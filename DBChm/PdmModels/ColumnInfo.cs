using System;

namespace DBCHM.PdmModels
{
    /// <summary>  
    /// 表列信息  
    /// </summary>  
    public class ColumnInfo
    {
        private TableInfo _OwnerTable;
        /// <summary>  
        /// 所属表  
        /// </summary>  
        public TableInfo OwnerTable
        {
            get { return _OwnerTable; }
        }
        public ColumnInfo(TableInfo OwnerTable)
        {
            this._OwnerTable = OwnerTable;
        }
        /// <summary>  
        /// 是否主键  
        /// </summary>  
        public bool IsPrimaryKey
        {
            get
            {
                PdmKey theKey = _OwnerTable.PrimaryKey;
                if (theKey != null)
                {
                    if (theKey.ColumnObjCodes.Contains(columnId))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        string columnId;
        /// <summary>  
        /// 列标识  
        /// </summary>  
        public string ColumnId
        {
            get { return columnId; }
            set { columnId = value; }
        }
        string objectID;
        /// <summary>  
        /// 对象Id,全局唯一.  
        /// </summary>  
        public string ObjectID
        {
            get { return objectID; }
            set { objectID = value; }
        }
        string name;
        /// <summary>  
        /// 列名  
        /// </summary>  
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        string code;
        /// <summary>  
        /// 列代码，对应数据库表字段名  
        /// </summary>  
        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        DateTime creationDate;
        /// <summary>  
        /// 创建日期  
        /// </summary>  
        public DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }
        string creator;
        /// <summary>  
        /// 创建人  
        /// </summary>  
        public string Creator
        {
            get { return creator; }
            set { creator = value; }
        }
        DateTime modificationDate;
        /// <summary>  
        /// 修改日期  
        /// </summary>  
        public DateTime ModificationDate
        {
            get { return modificationDate; }
            set { modificationDate = value; }
        }
        string modifier;
        /// <summary>  
        /// 修改人  
        /// </summary>  
        public string Modifier
        {
            get { return modifier; }
            set { modifier = value; }
        }
        string comment;
        /// <summary>  
        /// 注视  
        /// </summary>  
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }
        string dataType;
        /// <summary>  
        /// 数据类型  
        /// </summary>  
        public string DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }
        string length;
        /// <summary>  
        /// 数据长度  
        /// </summary>  
        public string Length
        {
            get { return length; }
            set { length = value; }
        }
        bool identity;
        /// <summary>  
        /// 是否自增量  
        /// </summary>  
        public bool Identity
        {
            get { return identity; }
            set { identity = value; }
        }
        bool mandatory;
        /// <summary>  
        /// 是否可空  
        /// </summary>  
        public bool Mandatory
        {
            get { return mandatory; }
            set { mandatory = value; }
        }
        string extendedAttributesText;
        /// <summary>  
        /// 扩展属性  
        /// </summary>  
        public string ExtendedAttributesText
        {
            get { return extendedAttributesText; }
            set { extendedAttributesText = value; }
        }
        /// <summary>  
        /// 物理选项  
        /// </summary>  
        public string PhysicalOptions { get; set; }
        /// <summary>  
        /// 精度  
        /// </summary>  
        public string Precision { get; set; }
        /// <summary>  
        /// 描述  
        /// </summary>  
        public string Description { get; set; }
    }  
}
