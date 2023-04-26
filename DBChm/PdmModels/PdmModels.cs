using System.Collections.Generic;

namespace DBCHM.PdmModels
{
    /// <summary>
    /// PDM实体集合
    /// </summary>
    public class PdmModels
    {
        public PdmModels()
        {
            Tables = new List<TableInfo>();
        }

        /// <summary>
        /// 表集合
        /// </summary>
        public IList<TableInfo> Tables { get; private set; }
    }
}