using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJTop.Data
{
    /// <summary>
    /// 表触发器
    /// </summary>
    public class TableTrigger
    {
        /// <summary>
        /// 表对应执行的Action集合
        /// </summary>
        internal IgCaseDictionary<List<Action>> ExecActions { get; set; }

        private DB Db { get; set; }

        internal TableTrigger(DB db)
        {
            this.Db = db;
            this.ExecActions = new IgCaseDictionary<List<Action>>();
        }

        /// <summary>
        /// 添加 表对应要执行的Action
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="actions">多个Action，object：方法执行后传入的值</param>
        public void Add(string tableName, params Action[] actions)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("表名不能为空！");
            }
            this.Db.CheckTabStuct(tableName);
            this.ExecActions.AddRange(tableName, actions);
        }


        /// <summary>
        /// 删除 表对应执行的Action集合数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>是否删除成功</returns>
        public bool Remove(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("表名不能为空！");
            }
            Db.CheckTabStuct(tableName);          
            return this.ExecActions.Remove(tableName);
        }

        /// <summary>
        /// 清除 所有表的Action集合
        /// </summary>
        public void Clear()
        {
            this.ExecActions.Clear();
        }

        /// <summary>
        /// 获取当前表的Action集合
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>当前表的Action集合</returns>
        public List<Action> GetActions(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("表名不能为空！");
            }
            List<Action> lstAct = null;
            if (this.ExecActions.TryGetValue(tableName, out lstAct))
            {
                return lstAct;
            }
            else
            {
                return new List<Action>();
            }
        }

    }
}
