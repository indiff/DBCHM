using MJTop.Data.DatabaseInfo;
using System.Data.Common;

namespace MJTop.Data.Database
{
    public class DB2DDTekDB : DB
    {
        public DB2DDTekDB(DBType dbType, DbProviderFactory dbFactory, string connectionString, int cmdTimeOut)
            : base(dbType, dbFactory, connectionString)
        {
            this.CmdTimeout = cmdTimeOut;
            this.Info = new DB2DBInfo(this);
        }
    }
}