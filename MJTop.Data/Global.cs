using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJTop.Data
{
    /// <summary>
    /// 全局静态字典
    /// </summary>
    public class Global
    {
        /// <summary>
        /// Type类型对应的DbType类型 字典
        /// </summary>
        public readonly static Dictionary<Type, DbType> TypeMap = new Dictionary<Type, DbType>
        {
            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Decimal,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(bool)] = DbType.Boolean,
            [typeof(string)] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(TimeSpan)] = DbType.Time,
            [typeof(byte[])] = DbType.Binary,
            [typeof(byte?)] = DbType.Byte,
            [typeof(sbyte?)] = DbType.SByte,
            [typeof(short?)] = DbType.Int16,
            [typeof(ushort?)] = DbType.UInt16,
            [typeof(int?)] = DbType.Int32,
            [typeof(uint?)] = DbType.UInt32,
            [typeof(long?)] = DbType.Int64,
            [typeof(ulong?)] = DbType.UInt64,
            [typeof(float?)] = DbType.Single,
            [typeof(double?)] = DbType.Decimal,
            [typeof(decimal?)] = DbType.Decimal,
            [typeof(bool?)] = DbType.Boolean,
            [typeof(char?)] = DbType.StringFixedLength,
            [typeof(Guid?)] = DbType.Guid,
            [typeof(DateTime?)] = DbType.DateTime,
            [typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
            [typeof(TimeSpan?)] = DbType.Time,
            [typeof(object)] = DbType.Object
        };

        /// <summary>
        /// 不同数据库 参数化 时所使用的字符
        /// </summary>
        public readonly static Dictionary<DBType, string> ParameterCharMap = new Dictionary<DBType, string>
        {
          { DBType.SqlServer,"@" },
          { DBType.MySql,"@" },
          { DBType.Oracle,":" },
          { DBType.PostgreSql,":" },
          { DBType.SQLite,"@" },
          { DBType.DB2,":" },
        };


        #region 各种数据库
        /// <summary>
        /// SqlServer的 数据类型对应DbType字典
        /// </summary>
        public readonly static Dictionary<string, DbType> Dict_SqlServer_DbType = new Dictionary<string, DbType>()
        {
            { "bigint",DbType.Int64 },
            { "binary",DbType.Binary },
            { "bit",DbType.Boolean },
            { "char",DbType.AnsiStringFixedLength },
            { "date",DbType.Date },
            { "datetime",DbType.DateTime },
            { "datetime2",DbType.DateTime2 },
            { "datetimeoffset",DbType.DateTimeOffset },
            { "decimal",DbType.Decimal },
            { "float",DbType.Single },
            { "geography",DbType.Object },
            { "geometry",DbType.Object },
            { "hierarchyid",DbType.Object },
            { "image",DbType.Binary },
            { "int",DbType.Int32 },
            { "money",DbType.Currency },
            { "nchar",DbType.StringFixedLength },
            { "ntext",DbType.String },
            { "numeric",DbType.Decimal },
            { "nvarchar",DbType.StringFixedLength },
            { "real",DbType.Decimal },
            { "smalldatetime",DbType.DateTime },
            { "smallint",DbType.Int16 },
            { "smallmoney",DbType.Currency },
            { "sql_variant",DbType.Object },
            //{ "sysname",DbType.AnsiString },
            { "text",DbType.AnsiString },
            { "time",DbType.Time },
            { "timestamp",DbType.Binary },
            { "tinyint",DbType.Byte },
            { "uniqueidentifier",DbType.Guid },
            { "varbinary",DbType.Binary },
            { "varchar",DbType.AnsiStringFixedLength },
            { "xml",DbType.Xml }
        };

        /// <summary>
        /// SqlServer的 数据类型对应CSharpType 字典
        /// </summary>
        public readonly static Dictionary<string, string> Dict_SqlServer_CSharpType = new Dictionary<string, string>()
        {
            { "bigint","long" },
            { "binary","byte[]" },
            { "bit","bool" },
            { "char","string" },
            { "date","DateTime" },
            { "datetime","DateTime" },
            { "datetime2","DateTime" },
            { "datetimeoffset","DateTimeOffset" },
            { "decimal","decimal" },
            { "float","float" },
            { "geography","object" },
            { "geometry","object"  },
            { "hierarchyid","object" },
            { "image","byte[]" },
            { "int","int" },
            { "money","decimal" },
            { "nchar","string"  },
            { "ntext","string" },
            { "numeric","decimal" },
            { "nvarchar","string" },
            { "real","decimal" },
            { "smalldatetime","DateTime" },
            { "smallint","int"},
            { "smallmoney","decimal"  },
            { "sql_variant","object"  },
            //{ "sysname",DbType.AnsiString },
            { "text","string" },
            { "time","string"  },
            { "timestamp","byte[]" },
            { "tinyint","int" },
            { "uniqueidentifier","Guid" },
            { "varbinary","byte[]" },
            { "varchar","string"  },
            { "xml","string"}
        };

        /// <summary>
        /// SqlServer的 数据类型对应 默认值 字典
        /// </summary>
        public readonly static Dictionary<string, string> Dict_SqlServer_DefValue = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "getdate","DateTime.Now" },
            { "newid","Guid.NewGuid()" }
        };


        /// <summary>
        /// Oracle的 数据类型对应DbType字典
        /// </summary>
        public readonly static Dictionary<string, DbType> Dict_Oracle_DbType = new Dictionary<string, DbType>()
        {
            { "BOOLEAN",DbType.Boolean },

            { "CHAR",DbType.AnsiStringFixedLength },
            { "VARCHAR2",DbType.AnsiStringFixedLength },
            { "NCHAR",DbType.StringFixedLength },
            { "NVARCHAR2",DbType.StringFixedLength },

            { "DATE",DbType.DateTime },
            { "TIMESTAMP",DbType.DateTime },

            { "LONG",DbType.AnsiString },//用于存储可变长度字符串。
            { "RAW",DbType.Binary },//此数据类型用于存储二进制数据或字符串。字符变量是由Oracle在字符集之间自动转换的。
            { "LONG RAW",DbType.Binary },//此数据类型用户存储二进制数据或字符串。与RAW不同之处是它不在字符集之间进行转换。

            { "BLOB",DbType.Binary },//将大型二进制对象存储在数据库中
            { "CLOB",DbType.AnsiString },//将大型字符数据存储在数据库中
            { "NCLOB",DbType.String },//存储大型UNICODE字符数据
            { "BFILE",DbType.Binary },//将大型二进制对象存储在操作系统文件中

            { "ROWID",DbType.AnsiString },
            { "NROWID",DbType.String },

            { "NUMBER",DbType.Decimal },
            { "DECIMAL",DbType.Decimal },
            { "INTEGER",DbType.Int32 },
            { "FLOAT",DbType.Decimal },
            { "REAL",DbType.Decimal }
        };

        /// <summary>
        /// Oracle的 数据类型对应CSharpType 字典
        /// </summary>
        public readonly static Dictionary<string, string> Dict_Oracle_CSharpType = new Dictionary<string, string>()
        {
            { "BOOLEAN","bool"},
            { "CHAR","string" },
            { "VARCHAR2","string" },
            { "NCHAR","string" },
            { "NVARCHAR2","string" },

            { "DATE","DateTime" },
            { "TIMESTAMP","DateTime"},

            { "LONG","string" },//用于存储可变长度字符串。
            { "RAW","byte[]" },//此数据类型用于存储二进制数据或字符串。字符变量是由Oracle在字符集之间自动转换的。
            { "LONG RAW","byte[]" },//此数据类型用户存储二进制数据或字符串。与RAW不同之处是它不在字符集之间进行转换。

            { "BLOB","byte[]"},//将大型二进制对象存储在数据库中
            { "CLOB","string" },//将大型字符数据存储在数据库中
            { "NCLOB","string" },//存储大型UNICODE字符数据
            { "BFILE","byte[]" },//将大型二进制对象存储在操作系统文件中

            { "ROWID","string"  },
            { "NROWID","string"  },

            { "NUMBER","decimal"},
            { "DECIMAL","decimal" },
            { "INTEGER","int" },
            { "FLOAT","float" },
            { "REAL","decimal" }
        };

        /// <summary>
        /// Oracle的 数据类型对应 默认值 字典
        /// </summary>
        public readonly static Dictionary<string, string> Dict_Oracle_DefValue = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
           
        };

        /// <summary>
        /// MySql的 数据类型对应DbType字典
        /// </summary>
        public readonly static Dictionary<string, DbType> Dict_MySql_DbType = new Dictionary<string, DbType>()
        {
            { "bit",DbType.Boolean },
            { "bool",DbType.Boolean },
            { "boolean",DbType.Boolean },

            { "tinyint",DbType.SByte },
            { "smallint",DbType.Int16 },
            { "mediumint",DbType.Int32 },
            { "int",DbType.Int32 },
            { "integer",DbType.Int32 },
            { "bigint",DbType.Int64 },

            { "enum",DbType.Int32 },
            { "set",DbType.Int32 },

            { "binary",DbType.Binary },
            { "varbinary",DbType.Binary },
            { "tinyblob",DbType.Binary },
            { "mediumblob",DbType.Binary },
            { "blob",DbType.Binary },
            { "longblob",DbType.Binary },

            { "date",DbType.DateTime },
            { "year",DbType.Int32 },
            { "time",DbType.Time },
            { "datetime",DbType.DateTime },
            { "timestamp",DbType.DateTime },

            { "decimal",DbType.Decimal },
            { "dec",DbType.Decimal },
            { "double",DbType.Decimal },
            { "float",DbType.Single },
            { "real",DbType.Single },

            { "char",DbType.AnsiStringFixedLength },
            { "varchar",DbType.AnsiStringFixedLength },
            { "nchar",DbType.StringFixedLength },
            { "nvarchar",DbType.StringFixedLength },
            { "tinytext",DbType.String },
            { "text",DbType.String },
            { "mediumtext",DbType.String },
            { "longtext",DbType.String }

        };

        /// <summary>
        /// MySql的 数据类型对应CSharpType 字典
        /// </summary>
        public readonly static Dictionary<string, string> Dict_MySql_CSharpType = new Dictionary<string, string>()
        {
            { "bit","bool" },
            { "bool","bool"  },
            { "boolean","bool"  },

            { "tinyint","int" },
            { "smallint","int" },
            { "mediumint","int" },
            { "int","int" },
            { "integer","int" },
            { "bigint","long" },

            { "enum","int" },
            { "set","int"},

            { "binary","byte[]" },
            { "varbinary","byte[]" },
            { "tinyblob","byte[]" },
            { "mediumblob","byte[]"  },
            { "blob","byte[]"  },
            { "longblob","byte[]" },

            { "date","DateTime" },
            { "year","int" },
            { "time","string"},
            { "datetime","DateTime" },
            { "timestamp","DateTime"},

            { "decimal","decimal" },
            { "dec","decimal" },
            { "double","double" },
            { "float","float"},
            { "real","decimal" },

            { "char","string" },
            { "varchar","string" },
            { "nchar","string"  },
            { "nvarchar","string"  },
            { "tinytext","string"  },
            { "text","string"  },
            { "mediumtext","string"  },
            { "longtext","string"  }

        };

        /// <summary>
        /// MySql的 数据类型对应 默认值 字典
        /// </summary>
        public readonly static Dictionary<string, string> Dict_MySql_DefValue = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"CURRENT_TIMESTAMP","DateTime.Now" },
            {"CURRENT_TIMESTAMP(6)","DateTime.Now" }
        };


        /// <summary>
        /// PostgreSQL的 数据类型对应DbType字典
        /// </summary>
        public readonly static Dictionary<string, DbType> Dict_PostgreSql_DbType = new Dictionary<string, DbType>()
        {
            { "smallint",DbType.Int16 },
            { "integer",DbType.Int32 },
            { "bigint",DbType.Int64 },
            { "decimal",DbType.Decimal },
            { "numeric",DbType.Decimal },
            { "real",DbType.Int32 },
            { "double precision",DbType.Decimal },
            { "smallserial",DbType.Int16 },
            { "serial",DbType.Int32 },
            { "bigserial",DbType.Int64 },

            { "money",DbType.Decimal },

            { "character varying",DbType.StringFixedLength },
            { "character",DbType.StringFixedLength },
            { "text",DbType.StringFixedLength },

            { "bytea",DbType.Binary },

            { "timestamp",DbType.DateTime },
            { "date",DbType.DateTime },
            { "time",DbType.Time },
            { "interval",DbType.Int32 },

            { "boolean",DbType.Boolean },

            { "enum",DbType.String },

            { "point",DbType.Object},
            { "line",DbType.Object},
            { "lseg",DbType.Object},
            { "box",DbType.Object},
            { "path",DbType.Object},
            { "polygon",DbType.Object},
            { "circle",DbType.Object},

            { "cidr",DbType.String},//IPv4和IPv6网络
            { "inet",DbType.String},//IPv4和IPv6主机以及网络
            { "macaddr",DbType.String},//MAC地址

            { "bit",DbType.Boolean},
            { "bit varying",DbType.String},

            { "tsvector",DbType.String},
            { "tsquery",DbType.String},

            { "UUID",DbType.Guid},

            { "Xml",DbType.Xml},

            { "json",DbType.String},
            { "jsonb",DbType.String},

            { "int4range",DbType.String},
            { "int8range",DbType.String},
            { "numrange",DbType.String},
            { "tsrange",DbType.String},
            { "tstzrange",DbType.String},
            { "daterange",DbType.String}

        };

        /// <summary>
        /// PostgreSQL的 数据类型对应CSharpype 字典
        /// </summary>
        public readonly static Dictionary<string, string> Dict_PostgreSql_CSharpType = new Dictionary<string, string>()
        {
            { "smallint","int" },
            { "integer","int"},
            { "bigint","long" },
            { "decimal","decimal"},
            { "numeric","decimal" },
            { "real","decimal" },
            { "double precision","double" },
            { "smallserial","int" },
            { "serial","int" },
            { "bigserial","long" },

            { "money","decimal"  },

            { "character varying","string"  },
            { "character","string"  },
            { "text","string"  },

            { "bytea","byte[]"  },

            { "timestamp","DateTime" },
            { "date","DateTime" },
            { "time","string"  },
            { "interval","int"},

            { "boolean","bool" },

            { "enum","int" },

            { "point","object"},
            { "line","object"},
            { "lseg","object"},
            { "box","object"},
            { "path","object"},
            { "polygon","object"},
            { "circle","object"},

            { "cidr","string"},//IPv4和IPv6网络
            { "inet","string"},//IPv4和IPv6主机以及网络
            { "macaddr","string"},//MAC地址

            { "bit","bool"},
            { "bit varying","string"},

            { "tsvector","string"},
            { "tsquery","string"},

            { "UUID","Guid"},

            { "Xml","string"},

            { "json","string"},
            { "jsonb","string"},

            { "int4range","string"},
            { "int8range","string"},
            { "numrange","string"},
            { "tsrange","string"},
            { "tstzrange","string"},
            { "daterange","string"}

        };

        /// <summary>
        /// PostgreSql的 数据类型对应 默认值 字典
        /// </summary>
        public readonly static Dictionary<string, string> Dict_PostgreSql_DefValue = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {

        };

        /// <summary>
        /// Sqlite的 数据类型对应DbType字典
        /// </summary>
        public readonly static Dictionary<string, DbType> Dict_Sqlite_DbType = new Dictionary<string, DbType>()
        {
            { "int",DbType.Int32},
            { "integer",DbType.Int32},
            { "tinyint",DbType.Byte},
            { "smallint",DbType.Int16},
            { "mediumint",DbType.Int32},
            { "bitint",DbType.Int64},
            { "unsigned big int",DbType.Int64},
            { "int2",DbType.Int32},
            { "int8",DbType.Int64},

            { "character",DbType.AnsiStringFixedLength},
            { "varchar",DbType.AnsiStringFixedLength},
            { "varying character",DbType.AnsiStringFixedLength},
            { "nchar",DbType.StringFixedLength},
            { "native character",DbType.StringFixedLength},
            { "nvarchar",DbType.StringFixedLength},
            { "text",DbType.String},
            { "clob",DbType.String},


            { "blob",DbType.Binary},
            { "no datatype specified",DbType.Object},

            { "real",DbType.Single},
            { "double",DbType.Decimal},
            { "double precision",DbType.Double},
            { "float",DbType.Single},

            { "numeric",DbType.Decimal},
            { "decimal",DbType.Decimal},
            { "boolean",DbType.Boolean},
            { "date",DbType.Date},
            { "datetime",DbType.DateTime}
        };

        /// <summary>
        /// Sqlite的 数据类型对应CSharpType 字典
        /// </summary>
        public readonly static Dictionary<string, string> Dict_Sqlite_CSharpType = new Dictionary<string, string>()
        {
            { "int","int"},
            { "integer","int"},
            { "tinyint","int"},
            { "smallint","int"},
            { "mediumint","int"},
            { "bitint","long"},
            { "unsigned big int","long"},
            { "int2","int"},
            { "int8","long"},

            { "character","string"},
            { "varchar","string"},
            { "varying character","string"},
            { "nchar","string"},
            { "native character","string"},
            { "nvarchar","string"},
            { "text","string"},
            { "clob","string"},


            { "blob","byte[]"},
            { "no datatype specified","object"},

            { "real","decimal"},
            { "double","double"},
            { "double precision","double"},
            { "float","float"},

            { "numeric","decimal"},
            { "decimal","decimal"},
            { "boolean","bool"},
            { "date","DateTime"},
            { "datetime","DateTime"}
        };


        /// <summary>
        /// Sqlite的 数据类型对应 默认值 字典
        /// </summary>
        public readonly static Dictionary<string, string> Dict_Sqlite_DefValue = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {

        };

        /// <summary>
        /// DB2的 数据类型对应DbType字典
        /// </summary>
        public readonly static Dictionary<string, DbType> Dict_DB2_DbType = new Dictionary<string, DbType>()
        {
            { "CHAR",DbType.AnsiStringFixedLength },
            { "CHARACTER",DbType.AnsiStringFixedLength },
            { "VARCHAR",DbType.AnsiStringFixedLength },
            { "LONG VARCHAR",DbType.AnsiStringFixedLength },

            { "GRAPHIC",DbType.String },
            { "VARGRAPHIC",DbType.String },
            { "LONG GRAPHIC",DbType.String },
            
            { "DATE",DbType.DateTime },
            { "TIMESTAMP",DbType.DateTime },
            
            { "SMALLINT",DbType.Int32 },
            { "INTEGER",DbType.Int32 },
            { "BIGINT",DbType.Int64 },

            { "BLOB",DbType.Binary },//保存2GB长度以内的二进制数据。
            { "CLOB",DbType.AnsiString },//保存2GB长度以内的单字节文本数据
            { "DBCLOB",DbType.String },//保存1GB长度以内的双字节文本数据。
            
            { "NUMBER",DbType.Decimal },
            { "DOUBLE",DbType.Decimal },
            { "DECIMAL",DbType.Decimal },            
            { "FLOAT",DbType.Decimal },
            { "REAL",DbType.Decimal }
        };

        /// <summary>
        /// DB2的 数据类型对应CSharpType 字典
        /// </summary>
        public readonly static Dictionary<string, string> Dict_DB2_CSharpType = new Dictionary<string, string>()
        {
            { "CHAR","string" },
            { "CHARACTER","string"  },
            { "VARCHAR","string"  },
            { "LONG VARCHAR","string"  },

            { "GRAPHIC","string"  },
            { "VARGRAPHIC","string" },
            { "LONG GRAPHIC","string" },

            { "DATE","DateTime" },
            { "TIMESTAMP","DateTime" },

            { "SMALLINT","int" },
            { "INTEGER","int"  },
            { "BIGINT","long" },

            { "BLOB","byte[]" },//保存2GB长度以内的二进制数据。
            { "CLOB","string" },//保存2GB长度以内的单字节文本数据
            { "DBCLOB","string" },//保存1GB长度以内的双字节文本数据。
            
            { "NUMBER","decimal" },
            { "DOUBLE","decimal" },
            { "DECIMAL","decimal" },
            { "FLOAT","decimal" },
            { "REAL","decimal" }
        };

        /// <summary>
        /// DB2的 数据类型对应 默认值 字典
        /// </summary>
        public readonly static Dictionary<string, string> Dict_DB2_DefValue = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {

        };

        #endregion

        /// <summary>
        /// DbType对应的转换方法
        /// </summary>
        public readonly static Dictionary<DbType, Func<object, object>> Dict_Convert_Type = new Dictionary<DbType, Func<object, object>>()
        {
                { DbType.AnsiString,
                    (obj) => obj.ToString()
                },
                { DbType.Binary,
                    (obj) => obj
                },
                { DbType.Byte,(obj) =>
                    {
                        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        byte res;
                        if (byte.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                { DbType.Boolean, (obj) =>
                    {
                        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }
                        bool res;
                        if (bool.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                { DbType.Currency, (obj) =>
                    {
                        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        decimal res;
                        if (decimal.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                { DbType.Date, (obj) =>
                    {
                        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        DateTime res;
                        if (DateTime.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                { DbType.DateTime, (obj) =>
                    {
                        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        DateTime res;
                        if (DateTime.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                { DbType.Decimal, (obj) =>
                    {
                        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        decimal res;
                        if (decimal.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                { DbType.Double, (obj) =>
                    {
                        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        decimal res;
                        if (decimal.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                { DbType.Guid, (obj) =>
                {
                         if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        Guid res;
                        if (Guid.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                { DbType.Int16,(obj) =>
                    {
                         if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        int res;
                        if (int.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                { DbType.Int32,(obj) =>
                     {
                        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        int res;
                        if (int.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                { DbType.Int64,(obj) =>
                    {
                        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        long res;
                        if (long.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                { DbType.Object,
                    (obj)=>obj
                },
                { DbType.SByte,(obj) =>
                     {
                        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        int res;
                        if (int.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                { DbType.Single,(obj) =>
                     {
                        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        int res;
                        if (int.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                { DbType.String,
                    (obj)=>obj.ToString()
                },
                { DbType.Time,
                    (obj)=>obj.ToString()
                },
                { DbType.UInt16,(obj) =>
                     {
                        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        int res;
                        if (int.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                { DbType.UInt32,(obj) =>
                     {
                        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        int res;
                        if (int.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                { DbType.UInt64,(obj) =>
                     {
                        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        long res;
                        if (long.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                //{ DbType.VarNumeric, (obj) =>
                //    {
                //        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                //        {
                //            return DBNull.Value;
                //        }

                //        decimal res;
                //        if (decimal.TryParse(obj.ToString(), out res))
                //        {
                //            return res;
                //        }
                //        return new ArgumentException(obj + "类型转换失败！");
                //    }
                //},
                { DbType.AnsiStringFixedLength,
                    (obj)=>obj.ToString()
                },
                { DbType.StringFixedLength,
                    (obj)=>obj.ToString()
                },
                { DbType.Xml,
                    (obj)=>obj.ToString()
                },
                { DbType.DateTime2, (obj) =>
                    {
                        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        DateTime res;
                        if (DateTime.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
                { DbType.DateTimeOffset, (obj) =>
                    {
                        if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            return DBNull.Value;
                        }

                        DateTime res;
                        if (DateTime.TryParse(obj.ToString(), out res))
                        {
                            return res;
                        }
                        return new ArgumentException(obj + "类型转换失败！");
                    }
                },
        };
    }
}
