# OnceMi.IdentityServer4

基于FreeSql实现的IdentityServer4，参考IdentityServer4.EntityFramework来进行实现。部分源码使用[https://github.com/xinguisoft/IdentityServer4.FreeSql](https://github.com/xinguisoft/IdentityServer4.FreeSql "https://github.com/xinguisoft/IdentityServer4.FreeSql")。

最开始打算直接使用由xinguisoft开源的[IdentityServer4.FreeSql](https://github.com/xinguisoft/IdentityServer4.FreeSql "IdentityServer4.FreeSql")项目，但是发现有很多错误，实现代码也比较简单粗暴，于是在此项目基础上，完善了此项目并独立出来，同时感谢作者的辛苦付出。

## Preview
Demo: 现在IdentityServer4转向收费，IdentityServer4也就不提供Demo了。

## How to use  
1. 下载项目  
2. 运行`OnceMi.IdentityServer4`  
默认采用Sqlite数据库，你可以切换到你喜欢的数据库，数据库配置在`appsettings.XXX.json`中(XXX表示运行环境，开发环境为Development)。第一次运行会初始化数据库，并写入测试数据。
默认不会生成用户数据，IdentityServer4不管理用户数据，所以需要运行OnceMi.Framework项目来初始化用户数据。

## Remark
1. 项目全部采用.NET 6开发，同时也建议升级到.NET 6。  
2. 默认账号是`developer`，密码是`123456`。  
3. ef文件夹中的为原版IdentityServer4.EntityFramework项目，可以参考。  
