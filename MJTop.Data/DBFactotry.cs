using DDTek.Oracle;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using MJTop.Data.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2;

namespace MJTop.Data
{
    internal class DBFactory
    {
        public static DB CreateInstance(DBType dbType, string connectionString, int cmdTimeOut)
        {
            switch (dbType)
            {
                case DBType.SqlServer:
                    return new SqlServerDB(dbType, SqlClientFactory.Instance, connectionString,cmdTimeOut);
                case DBType.MySql:
                    return new MySqlDB(dbType, MySqlClientFactory.Instance, connectionString, cmdTimeOut);
                case DBType.Oracle:
                    return new OracleDB(dbType, OracleClientFactory.Instance, connectionString, cmdTimeOut);
                case DBType.OracleDDTek:
                    return new OracleDDTekDB(dbType, OracleFactory.Instance, connectionString, cmdTimeOut);
                case DBType.PostgreSql:
                    return new PostgreSqlDB(dbType, NpgsqlFactory.Instance, connectionString, cmdTimeOut);
                case DBType.SQLite:
                    return new SQLiteDB(dbType, SQLiteFactory.Instance, connectionString, cmdTimeOut);
                case DBType.DB2:
                    return new DB2DDTekDB(dbType, DB2Factory.Instance, connectionString, cmdTimeOut);
                default:
                    throw new ArgumentException("未支持的数据库类型！");
            }
        }
    }
}
