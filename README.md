## DBCHM - 最简单、最实用的数据库文档生成工具

### 项目介绍

一直想要在找一个数据库文档工具，但是找了很久，就是没有发现可以管理常用数据库字段批注功能的通用型工具，于是决定自己开发一个数据库批注管理型的文档工具。由于开发中常常会使用一些帮助文档，其中chm文档的使用方便，检索方便，比较好用，所以一开始就叫做了 DBCHM。
### 支持的数据库
- [x] SqlServer
- [x] MySQL
- [x] Oracle
- [x]  PostgreSQL
- [x] DB2
- [x] SQLite

### 主要功能
- 对数据库表，列的批注 获取、编辑、保存。
- 文档信息包含：序号 | 列名 | 数据类型 | 长度 | 小数位数 | 主键 | 自增 | 允许空 | 默认值 | 列说明
- 根据文件导入进行更新批注：
    - 	[x] pdm 由`powerdesigner`设计数据库时产生。
    - 	[x] xml  由`visual studio`设置项目属性，勾选  XML文档文件后产生。
	- 	[x] xml 由`dbchm`的 XML导出而产生。
- 支持的导出的文件格式：
    - 	[x] chm
    - 	[x] word
    - 	[x] excel
    - 	[x] pdf
    - 	[x] html
    - 	[x] xml

DBCHM效果展示：
------------------------
#### 1 数据库连接管理
![数据库连接管理](https://gitee.com/lztkdr/DBCHM/raw/master/DBChm/Images/DBCHM001.png)

#### 2 表名模糊匹配
![表名模糊搜索](https://gitee.com/lztkdr/DBCHM/raw/master/DBChm/Images/DBCHM002.png)

#### 3 执行批注更新
![表批注更新](https://gitee.com/lztkdr/DBCHM/raw/master/DBChm/Images/DBCHM003.png)

#### 4 导出CHM文件
![导出CHM文件](https://gitee.com/lztkdr/DBCHM/raw/master/DBChm/Images/DBCHM004.png)

#### 5 表结构信息
![表结构信息](https://gitee.com/lztkdr/DBCHM/raw/master/DBChm/Images/DBCHM005.png)

#### 6 其他格式的效果，请下载体验哈:wink:！！

### 贡献者
- @[trycache](https://gitee.com/trycache) 主要开发者
- @[空无一物](https://gitee.com/lztkdr/) 先驱者

###  使用前提/源码编译/注意事项
- [最新发行版本下载](https://gitee.com/lztkdr/DBCHM/releases)
- 导出chm文件时，电脑需`htmlhelp.exe`，[见附件](https://gitee.com/lztkdr/DBCHM/attach_files)
- 电脑需`.net framework 4.5.2`
- 源码编印环境：`Visual Studio 2017 以上`版本
- 导出文档前，数据库使用账号要给予`sa级别`的权限，不然表结构可能因查询权限不足，会查不出来数据
- 新功能内测，最新功能尝鲜，请在Q群共享中获取:yum:！
### DBCHM社群
- QQ交流群：[![加入QQ群](https://img.shields.io/badge/QQ群-132941648-blue.svg)](http://shang.qq.com/wpa/qunwpa?idkey=43619cbe3b2a10ded01b5354ac6928b30cc91bda45176f89a191796b7a7c0e26) ，推荐点击按钮入群，当然如果无法成功操作，请自行搜索群号132941648进行添加 ），其它疑问或idea欢迎入群交流！