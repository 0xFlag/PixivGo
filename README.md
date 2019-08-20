# PixivGo
Pixiv插画，预览下载，批量下载，关键词搜索，不用梯子<br />
<br />
编程语言：C#<br />
编程软件：Visual Studio 2012<br />
编程环境：.Net Framework 4.5<br />
Version版本: 1.0<br />
API 调用地址：https://api.imjad.cn/pixiv_v2.md （感谢这位大佬提供）<br />
<br />
功能：<br />
1.预览下载（预览图片，图片信息）<br />
2.批量下载<br />
3.搜索功能（关键词搜索）<br />
这里我预览图片不上传了，懒_(:з」∠)_ <br />
<br />
注意：<br />
1.预览和下载都用了Referer，可能响应慢会导致程序无响应（等待即可）Org<br />
2.批量下载的格式一定不能搞错，看文本内提示<br />
3.批量下载量过多也可能会导致程序无响应然后崩溃Org<br />
<br />
关于Bug：<br />
1.一个作品内有多张插画的话无法读取，把json转C#实体类后发现会有重复函数，一个是list一个不是，所以就不能读取Org（目前在找办法）<br />
2.API有提供排行榜搜索，不知道为什么，搜索结果404，所以就没写进搜索功能<br />
3.本来想美化下程序的，去试了下，感jiao好麻烦，而且花里胡哨，所以就不美化了qaq<br />
<br />
上传日期：2019/08/20<br />
