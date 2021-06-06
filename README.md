# OnceMi.IdentityServer4

基于FreeSql实现的IdentityServer4，参考IdentityServer4.EntityFramework来进行实现。部分源码使用[https://github.com/xinguisoft/IdentityServer4.FreeSql](https://github.com/xinguisoft/IdentityServer4.FreeSql "https://github.com/xinguisoft/IdentityServer4.FreeSql")。

最开始打算直接使用由xinguisoft开源的[IdentityServer4.FreeSql](https://github.com/xinguisoft/IdentityServer4.FreeSql "IdentityServer4.FreeSql")项目，但是发现有很多错误，实现代码也比较简单粗暴，于是在此项目基础上，完善了此项目并独立出来，同时感谢作者的辛苦付出。

## Preview
Demo:目前在写Admin，等搞定了在搭建一个Demo服务器，先看图吧。
  
![](https://github.com/oncemi/IdentityServer4.FreeSql/raw/main/1.docs/img/01.jpg)

------------

![](https://github.com/oncemi/IdentityServer4.FreeSql/raw/main/1.docs/img/02.jpg)


## How to use  
1. 下载项目  
2. 运行`OnceMi.IdentityServer4`
默认采用Sqlite数据库，你可以切换到你喜欢的数据库，数据库配置在`appsettings.json`中。第一次运行会初始化数据库库，并写入测试数据。

## Remark
1. 项目全部采用.NET5开发，同时也建议升级到.NET5。  
2. 测试数据位于`testdata.json`，你可以修改或者删掉它。  
3. 默认账号是`admin`，密码是`123456`。  
4. ef文件中的为原版IdentityServer4.EntityFramework项目，可以参考。  
  
