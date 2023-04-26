using DBCHM.PdmModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace DBCHM.PDM
{
    /// <summary>
    /// PDM实体集合
    /// </summary>
    public class PdmModels
    {
        public PdmModels()
        {
            this.Tables = new List<TableInfo>();
        }

        /// <summary>
        /// 表集合
        /// </summary>
        public IList<TableInfo> Tables { get; private set; }
    }

    public class PdmReader
    {
        /// <summary>
        /// 读取指定Pdm文件的实体集合
        /// </summary>
        /// <param name="pdmFile">Pdm文件名(全路径名)</param>
        /// <returns>实体集合</returns>
        public PdmModels ReadFromFile(string pdmFile)
        {
            if (string.IsNullOrEmpty(pdmFile))
            {
                return null;
            }
            //加载文件.
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(pdmFile);
            //必须增加xml命名空间管理，否则读取会报错.
            var xmlnsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            xmlnsManager.AddNamespace("a", "attribute");
            xmlnsManager.AddNamespace("c", "collection");
            xmlnsManager.AddNamespace("o", "object");
            var theModels = new PdmModels();

            //读取所有表节点
            var xnTablesList = xmlDoc.SelectNodes("//c:Tables", xmlnsManager);
            if (xnTablesList != null)
                foreach (var xnTable in from XmlNode xmlTables in xnTablesList from XmlNode xnTable in xmlTables.ChildNodes where xnTable.Name != "o:Shortcut" select xnTable)
                {
                    theModels.Tables.Add(GetTable(xnTable));
                }
            return theModels;
        }

        //初始化"o:Table"的节点
        private TableInfo GetTable(XmlNode xnTable)
        {
            var mTable = new TableInfo();
            var xe = (XmlElement)xnTable;
            mTable.TableId = xe.GetAttribute("Id");
            var xnTProperty = xe.ChildNodes;
            foreach (XmlNode xnP in xnTProperty)
            {
                switch (xnP.Name)
                {
                    case "a:ObjectID":
                        mTable.ObjectID = xnP.InnerText;
                        break;

                    case "a:Name":
                        mTable.Name = xnP.InnerText;
                        break;

                    case "a:Code":
                        mTable.Code = xnP.InnerText;
                        break;

                    case "a:CreationDate":
                        mTable.CreationDate = String2DateTime(xnP.InnerText);
                        break;

                    case "a:Creator":
                        mTable.Creator = xnP.InnerText;
                        break;

                    case "a:ModificationDate":
                        mTable.ModificationDate = String2DateTime(xnP.InnerText);
                        break;

                    case "a:Modifier":
                        mTable.Modifier = xnP.InnerText;
                        break;

                    case "a:Comment":
                        mTable.Comment = xnP.InnerText;
                        break;

                    case "a:PhysicalOptions":
                        mTable.PhysicalOptions = xnP.InnerText;
                        break;

                    case "c:Columns":
                        InitColumns(xnP, mTable);
                        break;

                    case "c:Keys":
                        InitKeys(xnP, mTable);
                        break;

                    case "c:PrimaryKey":
                        InitPrimaryKey(xnP, mTable);
                        break;

                    case "a:Description":
                        mTable.Description = xnP.InnerText;
                        break;
                }
            }
            return mTable;
        }

        //PDM文件中的日期格式采用的是当前日期与1970年1月1日8点之差的秒树来保存.
        private DateTime _baseDateTime = new DateTime(1970, 1, 1, 8, 0, 0);

        private DateTime String2DateTime(string dateString)
        {
            Int64 theTicker = Int64.Parse(dateString);
            return _baseDateTime.AddSeconds(theTicker);
        }

        //初始化"c:Columns"的节点
        private void InitColumns(XmlNode xnColumns, TableInfo pTable)
        {
            foreach (XmlNode xnColumn in xnColumns)
            {
                pTable.AddColumn(GetColumn(xnColumn, pTable));
            }
        }

        //初始化c:Keys"的节点
        private void InitKeys(XmlNode xnKeys, TableInfo pTable)
        {
            foreach (XmlNode xnKey in xnKeys)
            {
                pTable.AddKey(GetKey(xnKey, pTable));
            }
        }

        //初始化c:PrimaryKey"的节点
        private void InitPrimaryKey(XmlNode xnPrimaryKey, TableInfo pTable)
        {
            pTable.PrimaryKeyRefCode = GetPrimaryKey(xnPrimaryKey);
        }

        private static Boolean ConvertToBooleanPg(Object obj)
        {
            if (obj != null)
            {
                string mStr = obj.ToString();
                mStr = mStr.ToLower();
                if ((mStr.Equals("y") || mStr.Equals("1")) || mStr.Equals("true"))
                {
                    return true;
                }
            }
            return false;
        }

        private ColumnInfo GetColumn(XmlNode xnColumn, TableInfo ownerTable)
        {
            var mColumn = new ColumnInfo(ownerTable);
            var xe = (XmlElement)xnColumn;
            mColumn.ColumnId = xe.GetAttribute("Id");
            var xnCProperty = xe.ChildNodes;
            foreach (XmlNode xnP in xnCProperty)
            {
                switch (xnP.Name)
                {
                    case "a:ObjectID":
                        mColumn.ObjectID = xnP.InnerText;
                        break;

                    case "a:Name":
                        mColumn.Name = xnP.InnerText;
                        break;

                    case "a:Code":
                        mColumn.Code = xnP.InnerText;
                        break;

                    case "a:CreationDate":
                        mColumn.CreationDate = String2DateTime(xnP.InnerText);
                        break;

                    case "a:Creator":
                        mColumn.Creator = xnP.InnerText;
                        break;

                    case "a:ModificationDate":
                        mColumn.ModificationDate = String2DateTime(xnP.InnerText);
                        break;

                    case "a:Modifier":
                        mColumn.Modifier = xnP.InnerText;
                        break;

                    case "a:Comment":
                        mColumn.Comment = xnP.InnerText;
                        break;

                    case "a:DataType":
                        mColumn.DataType = xnP.InnerText;
                        break;

                    case "a:Length":
                        mColumn.Length = xnP.InnerText;
                        break;

                    case "a:Identity":
                        mColumn.Identity = ConvertToBooleanPg(xnP.InnerText);
                        break;

                    case "a:Mandatory":
                        mColumn.Mandatory = ConvertToBooleanPg(xnP.InnerText);
                        break;

                    case "a:PhysicalOptions":
                        mColumn.PhysicalOptions = xnP.InnerText;
                        break;

                    case "a:ExtendedAttributesText":
                        mColumn.ExtendedAttributesText = xnP.InnerText;
                        break;

                    case "a:Precision":
                        mColumn.Precision = xnP.InnerText;
                        break;
                }
            }
            return mColumn;
        }

        private string GetPrimaryKey(XmlNode xnKey)
        {
            var xe = (XmlElement)xnKey;
            if (xe.ChildNodes.Count <= 0) return "";
            var theKp = (XmlElement)xe.ChildNodes[0];
            return theKp.GetAttribute("Ref");
        }

        private void InitKeyColumns(XmlNode xnKeyColumns, PdmKey Key)
        {
            var xe = (XmlElement)xnKeyColumns;
            var xnKProperty = xe.ChildNodes;
            foreach (var theRef in from XmlNode xnP in xnKProperty select ((XmlElement)xnP).GetAttribute("Ref"))
            {
                Key.AddColumnObjCode(theRef);
            }
        }

        private PdmKey GetKey(XmlNode xnKey, TableInfo ownerTable)
        {
            var mKey = new PdmKey(ownerTable);
            var xe = (XmlElement)xnKey;
            mKey.KeyId = xe.GetAttribute("Id");
            var xnKProperty = xe.ChildNodes;
            foreach (XmlNode xnP in xnKProperty)
            {
                switch (xnP.Name)
                {
                    case "a:ObjectID":
                        mKey.ObjectID = xnP.InnerText;
                        break;

                    case "a:Name":
                        mKey.Name = xnP.InnerText;
                        break;

                    case "a:Code":
                        mKey.Code = xnP.InnerText;
                        break;

                    case "a:CreationDate":
                        mKey.CreationDate = String2DateTime(xnP.InnerText);
                        break;

                    case "a:Creator":
                        mKey.Creator = xnP.InnerText;
                        break;

                    case "a:ModificationDate":
                        mKey.ModificationDate = String2DateTime(xnP.InnerText);
                        break;

                    case "a:Modifier":
                        mKey.Modifier = xnP.InnerText;
                        break;

                    case "c:Key.Columns":
                        InitKeyColumns(xnP, mKey);
                        break;
                }
            }
            return mKey;
        }
    }
}