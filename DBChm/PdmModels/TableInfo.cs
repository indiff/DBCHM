using System;
using System.Collections.Generic;

namespace DBCHM.PdmModels
{
/// <summary>  
    /// 表信息  
    /// </summary>  
    public class TableInfo  
    {  
        public TableInfo()  
        {  
            keys = new List<PdmKey>();  
            columns = new List<ColumnInfo>();  
  
        }  
        string tableId;  
        /// <summary>  
        /// 表ID  
        /// </summary>  
        public string TableId  
        {  
            get { return tableId; }  
            set { tableId = value; }  
        }  
        string objectID;  
        /// <summary>  
        /// 对象ID  
        /// </summary>  
        public string ObjectID  
        {  
            get { return objectID; }  
            set { objectID = value; }  
        }  
        string name;  
        /// <summary>  
        /// 表名  
        /// </summary>  
        public string Name  
        {  
            get { return name; }  
            set { name = value; }  
        }  
        string code;  
        /// <summary>  
        /// 表代码,对应数据库表名  
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
        /// 注释  
        /// </summary>  
        public string Comment  
        {  
            get { return comment; }  
            set { comment = value; }  
        }  
  
        string physicalOptions;  
        /// <summary>  
        /// 物理选项  
        /// </summary>  
        public string PhysicalOptions  
        {  
            get { return physicalOptions; }  
            set { physicalOptions = value; }  
        }  
  
  
        IList<ColumnInfo> columns;  
        /// <summary>  
        /// 表列集合  
        /// </summary>  
        public IList<ColumnInfo> Columns  
        {  
            get { return columns; }  
        }  
  
        IList<PdmKey> keys;  
        /// <summary>  
        /// 表Key集合  
        /// </summary>  
        public IList<PdmKey> Keys  
        {  
            get { return keys; }  
        }  
  
        public void AddColumn(ColumnInfo mColumn)  
        {  
            if (columns == null)  
                columns = new List<ColumnInfo>();  
            columns.Add(mColumn);  
        }  
  
        public void AddKey(PdmKey mKey)  
        {  
            if (keys == null)  
                keys = new List<PdmKey>();  
            keys.Add(mKey);  
        }  
        /// <summary>  
        /// 主键Key代码.=>KeyId  
        /// </summary>  
        public string PrimaryKeyRefCode { get; set; }  
        /// <summary>  
        /// 主关键字  
        /// </summary>  
        public PdmKey PrimaryKey  
        {  
            get  
            {  
                foreach (var key in keys)  
                {  
                    if (key.KeyId == PrimaryKeyRefCode)  
                    {  
                        return key;  
                    }  
                }  
                return null;  
            }  
        }  
        /// <summary>  
        /// 表的描述=>PDM Notes.  
        /// </summary>  
        public string Description { get; set; }  
    }  
}
