using System;
using System.Data.Common;
using System.Reflection;
using Top._51Try.Data;

namespace DBCHM
{
    public static class DBUtils
    {

        /// <summary>
        /// 数据库对象
        /// Top._51Try.Data 因在迭代测试开发中，尚不完善，存在一定Bug，后期完善后，再开源。
        /// </summary>
        public static DB Instance
        { get; set; }
    }
}
