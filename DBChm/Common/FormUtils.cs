using DBChm;
using MJTop.Data;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows.Forms;

namespace DBCHM
{
    public static class FormUtils
    {
        /// <summary>
        /// 静态构造函数执行1次，记载 可以处理DBCHM的数据库类型
        /// </summary>
        static FormUtils()
        {
            DictDBType = EnumExt<DBType>.All();

            DictDBType.Remove(DBType.OracleDDTek.ToString());
            DictDBType.Remove(DBType.SQLite.ToString());

        }
        /// <summary>
        /// 所有数据库类型 对应的端口
        /// </summary>
        public static readonly Dictionary<string, string> DictPort = new Dictionary<string, string>()
        {
            { DBType.SqlServer.ToString(),"1433"},
             { DBType.MySql.ToString(),"3306"},
                { DBType.Oracle.ToString(),"1521"},
                    //{ DBType.OracleDDTek.ToString(),"1521"},            
                { DBType.PostgreSql.ToString(),"5432"},
        };

        /// <summary>
        /// 所有数据库类型
        /// </summary>
        public static Dictionary<string, DBType> DictDBType
        {
            get; private set;
        }


        /// <summary>
        /// 是否正常的Close
        /// </summary>
        public static bool IsOK_Close { get; set; } = false;


        public static ProgressArg ProgArg { get; set; }

        

        /// <summary>
        /// Loading加载
        /// </summary>
        /// <param name="msg">loading提示消息</param>
        /// <param name="owner">当前窗体 this </param>
        /// <param name="work">异步执行方法</param>
        /// <param name="workArg">异步执行方法的参数</param>
        public static void ShowProcessing(string msg, Form owner, Action<object> work, object workArg = null)
        {
            try
            {
                FrmProcessing processingForm = new FrmProcessing(msg);
                dynamic expObj = new ExpandoObject();
                expObj.Form = processingForm;
                expObj.WorkArg = workArg;
                processingForm.SetWorkAction(work, expObj);
                processingForm.ShowDialog(owner);
                if (processingForm.WorkException != null)
                {
                    throw processingForm.WorkException;
                }
            }
            catch (System.Exception ex)
            {
                LogUtils.LogError("FrmProcessing", Developer.SysDefault, ex, msg);
            }
        }

    }
}
