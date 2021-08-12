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
using System.Data.Common;

namespace MJTop.Data
{
    internal class DBFactory
    {
        internal static DB CreateInstance(DBType dbType, string connectionString, int cmdTimeOut)
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

        internal static void TryConnect(DBType dbType, string connectionString, out List<string> dbNames)
        {
            dbNames = new List<string>();
            DbConnection conn = null;
            DbCommand cmd = null;
            var dbSql = string.Empty;
            switch (dbType)
            {
                case DBType.SqlServer:
                    conn = SqlClientFactory.Instance.CreateConnection();
                    cmd = SqlClientFactory.Instance.CreateCommand();
                    cmd.CommandText = "select name from sys.sysdatabases where name not in ('master','tempdb','model','msdb') Order By name asc";
                    cmd.Connection = conn;
                    break;
                case DBType.MySql:
                    conn = MySqlClientFactory.Instance.CreateConnection();
                    cmd = MySqlClientFactory.Instance.CreateCommand();
                    cmd.CommandText = "select schema_name from information_schema.SCHEMATA where schema_name not in ('information_schema','performance_schema','mysql','sys') order by  schema_name asc";
                    cmd.Connection = conn;
                    break;
                case DBType.Oracle:
                    conn = OracleClientFactory.Instance.CreateConnection();
                    cmd = OracleClientFactory.Instance.CreateCommand();
                    //cmd.CommandText = "";
                    cmd.Connection = conn;
                    break;
                case DBType.OracleDDTek:
                    conn = OracleFactory.Instance.CreateConnection();
                    cmd = OracleFactory.Instance.CreateCommand();
                    //cmd.CommandText = "";
                    cmd.Connection = conn;
                    break;
                case DBType.PostgreSql:
                    conn = NpgsqlFactory.Instance.CreateConnection();
                    cmd = NpgsqlFactory.Instance.CreateCommand();
                    cmd.CommandText = "select datname from pg_database where datistemplate = false and datname not in ('postgres') order by oid desc";
                    cmd.Connection = conn;
                    break;
                case DBType.SQLite:
                    conn = SQLiteFactory.Instance.CreateConnection();
                    cmd = SQLiteFactory.Instance.CreateCommand();
                    //cmd.CommandText = "";
                    cmd.Connection = conn;
                    break;
                case DBType.DB2:
                    conn = DB2Factory.Instance.CreateConnection();
                    cmd = DB2Factory.Instance.CreateCommand();
                    //cmd.CommandText = "";
                    cmd.Connection = conn;
                    break;
                default:
                    throw new ArgumentException("未支持的数据库类型！");
            }


            try
            {
                conn.ConnectionString = connectionString;
                conn.Open();

                if (conn is DDTek.Oracle.OracleConnection oraConn)
                {
                    dbNames.Add(oraConn.ServiceName);
                }
                else
                {
                    dbNames.Add(conn.Database);
                }

                if (cmd != null && !string.IsNullOrWhiteSpace(cmd.CommandText))
                {
                    try
                    {
                        var reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var name = reader.GetFieldValue<string>(0);
                                if (!dbNames.Contains(name))
                                {
                                    dbNames.Add(name);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogError("TryConnect", Developer.SysDefault, ex, dbType, connectionString, dbNames );
                    }
                }
            }
            finally
            {
                cmd?.Dispose();
                conn?.Close();
            }
        }
    }
}
