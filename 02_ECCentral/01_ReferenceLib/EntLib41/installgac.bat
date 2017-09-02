"gacutil" /if Microsoft.Practices.EnterpriseLibrary.AppSettings.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Caching.Cryptography.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Caching.Cryptography.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Caching.Database.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Caching.Database.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Caching.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Common.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Configuration.Design.HostAdapter.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Configuration.Design.UI.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Configuration.EnvironmentalOverrides.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Data.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Data.SqlCe.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.WCF.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.WCF.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Logging.Database.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Logging.Database.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Logging.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.PolicyInjection.CallHandlers.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.PolicyInjection.CallHandlers.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.PolicyInjection.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.PolicyInjection.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Security.Cache.CachingStore.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Security.Cache.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Security.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Security.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Validation.Configuration.Design.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Validation.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Validation.Integration.AspNet.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF.dll
"gacutil" /if Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.dll
"gacutil" /if Microsoft.Practices.ObjectBuilder2.dll
"gacutil" /if Microsoft.Practices.Unity.Configuration.dll
"gacutil" /if Microsoft.Practices.Unity.dll
"gacutil" /if Microsoft.Practices.Unity.Interception.Configuration.dll
"gacutil" /if Microsoft.Practices.Unity.Interception.dll



@echo   off  
set   CURRENT_DIR=%cd%   

reg add "HKLM\SOFTWARE\Microsoft\.NETFramework\v2.0.50727\AssemblyFoldersEx\Enterprise Library4.1" /ve /d %CURRENT_DIR% /f

pause




  

