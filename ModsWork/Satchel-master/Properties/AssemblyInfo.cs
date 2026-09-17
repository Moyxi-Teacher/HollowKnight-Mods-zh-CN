using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Satchel")]
[assembly: AssemblyDescription("Satchel - Hollow Knight modding utilities")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Satchel")]
[assembly: AssemblyCopyright("Copyright ©  2026")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

/* 用户决策(2026-09-11)：
   - 关闭强名称签名（原版 DLL 均为 PublicKeyToken=null），不设置 AssemblyKeyFile。
   - 中文名由 AI 拟定，后续用户确认；版本号 = 原版数字 + “汉化”后缀。
   - 原版 Satchel 元数据版本号显示异常；按源码硬编码 ver="0.9.4" 拟定 0.9.4.0，
     即 AssemblyVersion/FileVersion = 0.9.4.0（数字部分，保持兼容），
     AssemblyInformationalVersion = 0.9.4.0汉化（菜单悬停显示）。 */
[assembly: AssemblyVersion("0.9.4.0")]
[assembly: AssemblyFileVersion("0.9.4.0")]
[assembly: AssemblyInformationalVersion("0.9.4.0汉化")]

[assembly: ComVisible(false)]
[assembly: Guid("ec832acd-2374-45b5-ad18-376cef2ac643")]
