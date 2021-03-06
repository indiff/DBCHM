# DBCHM - Release Notes

======================================
## v1.8.0.3-beta
* 优化oracle连接时超时出现 ORA-01013错误的情况（Oracle将不再查找自增列）
* 所有类型的文档，对Oracle表结构导出时，将不再显示自增列。
* 连接/测试时，数据库下拉框，将不再加载 默认数据库，如Sql Server 的默认数据库 master、tempdb...等数据库不会再加载

## v1.8.0.2-beta
* 修复hhk索引文件过大，生成chm时弹出 An internal error has occurred...的情况
* 连接/测试 按钮只进行测试连接，不进行表结构信息查询，减少测试连接时间。
* 列批注 在编辑前的选中状态下，可以从选定行开始粘贴多行文本内容 对多个列注释批量赋值。
* 连接数据库 连接超时的默认值由原60秒改为30秒
* 导出 部分勾选，对视图、存储过程不起作用的情况

## v1.8.0.1-beta
* PostgreSql 多模式 情况下 导出会有问题的情况
* 错误时，记录详细日志

## v1.8.0.0-beta
* 各类文档生成 的代码重构
* chm/html可以使用模板生成文档
* 支持数据库的视图、存储过程导出,默认格式化显示,并且关键字高亮
* 软件其他细节完善

