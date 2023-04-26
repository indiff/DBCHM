using System;
using System.Collections.Generic;

namespace DBCHM.PdmModels
{
    public class PdmKey
    {
        private string keyId;

        /// <summary>
        /// 关键字标识
        /// </summary>
        public string KeyId
        {
            get { return keyId; }
            set { keyId = value; }
        }

        private string objectID;

        /// <summary>
        /// 对象Id
        /// </summary>
        public string ObjectID
        {
            get { return objectID; }
            set { objectID = value; }
        }

        private string name;

        /// <summary>
        /// Key名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string code;

        /// <summary>
        /// Key代码,对应数据库中的Key.
        /// </summary>
        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        private DateTime creationDate;

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }

        private string creator;

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator
        {
            get { return creator; }
            set { creator = value; }
        }

        private DateTime modificationDate;

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModificationDate
        {
            get { return modificationDate; }
            set { modificationDate = value; }
        }

        private string modifier;

        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifier
        {
            get { return modifier; }
            set { modifier = value; }
        }

        private IList<ColumnInfo> columns;

        /// <summary>
        /// Key涉及的列
        /// </summary>
        public IList<ColumnInfo> Columns
        {
            get { return columns; }
        }

        public void AddColumn(ColumnInfo mColumn)
        {
            if (columns == null)
                columns = new List<ColumnInfo>();
            columns.Add(mColumn);
        }

        private List<string> _ColumnObjCodes = new List<string>();

        /// <summary>
        /// Key涉及的列代码，根据辞可访问到列信息.对应列的ColumnId
        /// </summary>
        public List<string> ColumnObjCodes
        {
            get { return _ColumnObjCodes; }
        }

        public void AddColumnObjCode(string ObjCode)
        {
            _ColumnObjCodes.Add(ObjCode);
        }

        private TableInfo _OwnerTable = null;

        public PdmKey(TableInfo table)
        {
            _OwnerTable = table;
        }
    }
}