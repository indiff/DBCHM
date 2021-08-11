using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace DocTools.Dtos
{
    /// <summary>
    /// 数据库Dto
    /// </summary>
    public class DBDto
    {
        public DBDto() { }

        public DBDto(string dbName, object tag = null)
        {
            this.DBName = dbName;
            this.Tag = tag;
        }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DBName { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DBType { get; set; }

        private List<TableDto> _Tables = null;

        private SortedDictionary<string, List<TableDto>> _TableDict = null;

        public SortedDictionary<string, List<TableDto>> TableDict {
           get
            {
                // 数据库表信息是否包含模块信息
                bool isModule = false;
                foreach ( var t in Tables)
                {
                    if (!String.IsNullOrEmpty(t.TableModule))
                    {
                        isModule = true;
                    }
                }

                if ( isModule )
                {
                    _TableDict = new SortedDictionary<string, List<TableDto>>();
                    Tables.ForEach(t => {
                        if (String.IsNullOrEmpty(t.TableModule))
                        {
                            List<TableDto> list = null;
                            if (!_TableDict.ContainsKey("未知"))
                            {
                                list = new List<TableDto>();
                                _TableDict.Add("未知", list);
                            } else
                            {
                                list = _TableDict["未知"];
                            }
                            t.TableModule = "未知";
                            list.Add(t);
                            
                        }
                        else
                        {
                            List<TableDto> list = null;
                            if (!_TableDict.ContainsKey( t.TableModule ))
                            {
                                list = new List<TableDto>();
                                _TableDict.Add(t.TableModule, list);
                            }
                            else
                            {
                                list = _TableDict[t.TableModule];
                            }
                            list.Add(t);
                            
                        }
                    });
                    return _TableDict;
                } else
                {
                    return null;
                }
                
            }
            set
            {
                _TableDict = value;
            }
        }

        /// <summary>
        /// 表结构信息
        /// </summary>
        public List<TableDto> Tables
        {
            get
            {
                if (_Tables == null)
                {
                    return new List<TableDto>();
                }
                else
                {
                    _Tables.ForEach(t =>
                    {
                        t.Comment = FilterIllegalDir(t.Comment);
                    });
                    return _Tables;
                }
            }
            set
            {
                _Tables = value;
            }
        }

        /// <summary>
        /// 数据库视图
        /// </summary>
        public Dictionary<string,string> Views { get; set; }

        /// <summary>
        /// 数据库存储过程
        /// </summary>
        public Dictionary<string, string> Procs { get; set; }

        /// <summary>
        /// 其他一些参数数据，用法如 winform 控件的 Tag属性
        /// </summary>
        public object Tag { get; set; }


        /// <summary>
        /// 处理非法字符路径
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string FilterIllegalDir(string str)
        {
            if (str.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                str = string.Join(" ", str.Split(Path.GetInvalidFileNameChars()));
            }
            return str;
        }

    }
}
