using System;
using System.Data.Common;
using Top._51Try.Data;

namespace DBCHM
{
    public static class DBUtils
    {
        public static DB Instance
        { get; set; }

        public static bool TryConnn(DbProviderFactory factory, string connectionString)
        {
            try
            {
                var conn = factory.CreateConnection();
                conn.ConnectionString = connectionString;
                conn.Open();
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
