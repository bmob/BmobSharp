# BmobSharp

Bmob Api Client for .Net

## 支持平台

* Windows Deskstop 即.Net C#的项目
* WindowsPhone8
* Windows 8.1
* Unity

## 示例

参见[bmob-demo-csharp](https://github.com/bmob/bmob-demo-csharp)，examples目录下三个项目windows-destop、unity、windowphone8简单介绍了Bmob的基本功能，是了解bmob和学习bmob的不二之选。

## 文档

源代码中注释比较少，如果对源码中功能弄的不是很明白，查看[文档](https://github.com/bmob/bmob-demo-csharp/wiki)对理解代码和使用有更深的认识。

* <https://github.com/bmob/bmob-demo-csharp/wiki>

## 开发环境搭建

* 软件下载/安装

  * [Visual Studio Community 2015](https://www.visualstudio.com/zh-cn/visual-studio-homepage-vs.aspx)
  * [Unity3d](http://unity3d.com/cn/get-unity/download?ref=personal)

* 开发环境说明

 * Window10 / VS2015 / Unity 5.1.0f3
 * Mac OS X 10.9 / Xcode 5.1 / Unity 5.2.1f1

如果仅仅是编译项目，可以不需要安装Unity3d，下载Unity3d的Engine和Editor动态链接库文件就行了。


* 文件结构说明

  * core 全部源代码放在这个目录下。直接运行csproj就可以编辑运行代码了。
  * BmobTest 提供了基于desktop平台的测试用例，便于接口的调试。
  * 源码中提供了**build.simple.bat**用于一键生成各个平台的dll文件。
  
* Unity源码调试设置

 * 删除原来的`Assets/libs/Bmob-Unity.dll`。
 * 把源代码`core/src`目录拷贝到`Assets/classes/`下。
 * 打开Unity重新编译，把BmobUntiy对象拖拽到摄像机上，重新设置AppId和RestKey。
 
![](https://cloud.githubusercontent.com/assets/667902/10300818/2742146a-6c2e-11e5-8367-b2304abfc2dd.jpg)

* Unity输出日志位置：<http://docs.unity3d.com/Manual/LogFiles.html>

* Unity Mac环境配置

![](https://raw.githubusercontent.com/bmob/bmob-demo-csharp/master/images/unity-ios-simulator.png)

![](https://cloud.githubusercontent.com/assets/667902/10384088/a2ff6304-6e69-11e5-8f2e-3221e8ac7851.png)

![](https://cloud.githubusercontent.com/assets/667902/10384074/637611a6-6e69-11e5-9406-a68414742547.png)
