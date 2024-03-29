﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocTools.Dtos
{
    /// <summary>
    /// 数据库表dto
    /// </summary>
    public class TableDto : IComparable<TableDto>
    {
        /// <summary>
        /// 序号
        /// </summary>
        [Display(Name = "序号")]
        public string TableOrder { get; set; }

        /// <summary>
        /// 模块
        /// </summary>
        [Display(Name = "模块")]
        public string TableModule { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [Display(Name = "表名")]
        public string TableName { get; set; }

        /// <summary>
        /// 行数
        /// </summary>
        [Display(Name = "行数")]
        public int TableRows { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        [Display(Name = "表说明")]
        public string Comment { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        [Display(Name = "数据库类型")]
        public string DBType { get; set; }

        /// <summary>
        /// 表格列集合
        /// </summary>
        [Display(Name = "列数据")]
        public List<ColumnDto> Columns { get; set; }

        public int CompareTo(TableDto other)
        {
            if ( this.TableRows == 0 && other.TableRows == 0 ) {  return 0; }

            if (this.TableRows != other.TableRows)
            {
                return this.TableRows.CompareTo(other.TableRows);
            }
            else return 0;
        }
    }
}