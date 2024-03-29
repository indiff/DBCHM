[![GitHub all releases](https://img.shields.io/github/downloads/indiff/DBCHM/total)](https://github.com/indiff/DBCHM/releases/tag/v1.8.0.4)

## 🧨操作方法
![GIF 2022-3-12 23-20-18](https://user-images.githubusercontent.com/501276/158024086-a74b1709-109e-4e7d-abf3-c82e47b8681c.gif)
![image](https://user-images.githubusercontent.com/501276/158024130-be80481e-6524-4be2-afdd-533e96d1a911.png)

## ✔ 更新日志
- [x] 修复chm生成bug、支持返回目录、索引主题支持表名和表注释搜索
- [x] 支持 Oracle 存储过程，按type分类， 名称获取类型和相应的功能描述 2021/9/25 1.8.0.4 (文件名不能包含/ 或者 \ 字符)
- [x] 支持表结构进行分组
- [x] 修复分组后不能显示批注的bug
- [x] 更新packages
- [x] 调整关于页面和皮肤
## 👍发布版本
```
git tag -a v1.8.0.4 -m "Oracle存储过程分类,提取功能注释"
git push origin v1.8.0.4

删除标签 
git push origin --delete v1.8.0.4
git tag -d v1.8.0.4
```
## 🎯 支持的数据库
- [x] SqlServer
- [x] MySQL
- [x] Oracle
- [x] PostgreSQL
- [x] DB2
- [x] SQLite


## 💪贡献者

- [trycache](https://gitee.com/trycache) 主要开发者
- [空无一物](https://gitee.com/dotnetchina/) 先驱者
- [indiff](https://github.com/indiff/) 扩展支持分组功能


## 配置方法
- 表名匹配
![1658281570966](https://user-images.githubusercontent.com/501276/179877968-71d97d1b-35e6-4053-8820-600792a711b7.png)
- 表前缀匹配
![image](https://user-images.githubusercontent.com/501276/179878028-701bf656-69ca-44c7-96f7-6ab6360acd5f.png)

## 欢迎关注
- [qttabbar](https://github.com/indiff/qttabbar)

