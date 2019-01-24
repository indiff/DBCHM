using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJTop.Data
{
    public class TS
    {
        private DB Db { get; set; }
        public TS(DB db)
        {
            this.Db = db;
            this.IsTran = false;
        }
        internal DbConnection TranConn { get; set; }
        internal DbTransaction Tran { get; set; }
        private bool IsTran { get; set; }

        public virtual void Begin()
        {
            this.TranConn = Db.CreateConn();
            this.TranConn.Open();
            this.Tran = this.TranConn.BeginTransaction();
            this.IsTran = true;
        }

        public virtual void Begin(IsolationLevel isolationLevel)
        {
            this.TranConn = Db.CreateConn();
            this.TranConn.Open();
            this.Tran = this.TranConn.BeginTransaction(isolationLevel);
            this.IsTran = true;
        }

        public virtual void Commit()
        {
            this.Tran.Commit();
            this.IsTran = false;
        }

        public virtual void Rollback()
        {
            this.Tran.Rollback();
            this.IsTran = false;
        }
    }
}
