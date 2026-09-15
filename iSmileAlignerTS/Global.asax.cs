using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using iSmileAlignerTS.Models;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Net.Mail;
using System.Net.Mime;
using Newtonsoft.Json;
using System.Threading;
using System.Collections;
using System.Reflection;

namespace iSmileAlignerTS
{
    public class ObjectDumper
    {
        private int _currentIndent;
        private readonly int _indentSize;
        private readonly StringBuilder _stringBuilder;
        private readonly Dictionary<object, int> _hashListOfFoundElements;
        private readonly char _indentChar;
        private readonly int _depth;
        private int _currentLine;
        private int _currentCount;

        private ObjectDumper(int depth, int indentSize, char indentChar)
        {
            _depth = depth;
            _indentSize = indentSize;
            _indentChar = indentChar;
            _stringBuilder = new StringBuilder();
            _hashListOfFoundElements = new Dictionary<object, int>();
            _currentCount = 0;
        }

        public static string Dump(object element, int depth = 10, int indentSize = 2, char indentChar = ' ')
        {
            var instance = new ObjectDumper(depth, indentSize, indentChar);
            return instance.DumpElement(element, true);
        }

        private string DumpElement(object element, bool isTopOfTree = false)
        {
            if (_currentIndent > _depth || _currentCount > 1000) { return null; }
            _currentCount += 1;
            if (element == null || element is string)
            {
                Write(FormatValue(element));
            }
            else if (element is ValueType)
            {
                Type objectType = element.GetType();
                bool isWritten = false;
                if (objectType.IsGenericType)
                {
                    Type baseType = objectType.GetGenericTypeDefinition();
                    if (baseType == typeof(KeyValuePair<,>))
                    {
                        isWritten = true;
                        Write("Key:");
                        _currentIndent++;
                        DumpElement(objectType.GetProperty("Key").GetValue(element, null));
                        _currentIndent--;
                        Write("Value:");
                        _currentIndent++;
                        DumpElement(objectType.GetProperty("Value").GetValue(element, null));
                        _currentIndent--;
                    }
                }
                if (!isWritten)
                {
                    Write(FormatValue(element));
                }
            }
            else
            {
                var enumerableElement = element as IEnumerable;
                if (enumerableElement != null)
                {
                    foreach (object item in enumerableElement)
                    {
                        if (item is IEnumerable && !(item is string))
                        {
                            _currentIndent++;
                            DumpElement(item);
                            _currentIndent--;
                        }
                        else
                        {
                            DumpElement(item);
                        }
                    }
                }
                else
                {
                    Type objectType = element.GetType();
                    Write("{{{0}(HashCode:{1})}}", objectType.FullName, element.GetHashCode());
                    if (!AlreadyDumped(element))
                    {
                        _currentIndent++;
                        MemberInfo[] members = objectType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
                        foreach (var memberInfo in members)
                        {
                            var fieldInfo = memberInfo as FieldInfo;
                            var propertyInfo = memberInfo as PropertyInfo;

                            if (fieldInfo == null && (propertyInfo == null || !propertyInfo.CanRead || propertyInfo.GetIndexParameters().Length > 0))
                                continue;

                            var type = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;
                            object value;
                            try
                            {
                                value = fieldInfo != null
                                                   ? fieldInfo.GetValue(element)
                                                   : propertyInfo.GetValue(element, null);
                            }
                            catch (Exception e)
                            {
                                Write("{0} failed with:{1}", memberInfo.Name, (e.GetBaseException() ?? e).Message);
                                continue;
                            }

                            if (type.IsValueType || type == typeof(string))
                            {
                                Write("{0}: {1}", memberInfo.Name, FormatValue(value));
                            }
                            else
                            {
                                var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
                                Write("{0}: {1}", memberInfo.Name, isEnumerable ? "..." : "{ }");

                                _currentIndent++;
                                DumpElement(value);
                                _currentIndent--;
                            }
                        }
                        _currentIndent--;
                    }
                }
            }

            return isTopOfTree ? _stringBuilder.ToString() : null;
        }

        private bool AlreadyDumped(object value)
        {
            if (value == null)
                return false;
            int lineNo;
            if (_hashListOfFoundElements.TryGetValue(value, out lineNo))
            {
                Write("(reference already dumped - line:{0})", lineNo);
                return true;
            }
            _hashListOfFoundElements.Add(value, _currentLine);
            return false;
        }

        private void Write(string value, params object[] args)
        {
            var space = new string(_indentChar, _currentIndent * _indentSize);

            if (args != null)
                value = string.Format(value, args);

            _stringBuilder.AppendLine(space + value);
            _currentLine++;
        }

        private string FormatValue(object o)
        {
            if (o == null)
                return ("null");

            if (o is DateTime)
                return (((DateTime)o).ToShortDateString());

            if (o is string)
                return "\"" + (string)o + "\"";

            if (o is char)
            {
                if (o.Equals('\0'))
                {
                    return "''";
                }
                else
                {
                    return "'" + (char)o + "'";
                }
            }

            if (o is ValueType)
                return (o.ToString());

            if (o is IEnumerable)
                return ("...");

            return ("{ }");
        }
    }

    public class MvcApplication : System.Web.HttpApplication
    {
        private static string privateViewBagErrorMsg = null;
        private static string privateLogMsgs = null;
        private static string privateOriginalWebViewSearches = "";
        private static string privateOriginalRazorViewSearches = "";
        private static string privateOriginalWebPartialSearches = "";
        private static string privateOriginalRazorPartialSearches = "";


        // Fehlermeldung erfassen und protokollieren
        //
        public static string ErrorString(string msg,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            logMsg(msg, memberName, sourceFilePath, sourceLineNumber);

            privateViewBagErrorMsg= (privateViewBagErrorMsg == null) ? "" : privateViewBagErrorMsg + Environment.NewLine;
            privateViewBagErrorMsg= privateViewBagErrorMsg + msg + Environment.NewLine;
            privateViewBagErrorMsg = privateViewBagErrorMsg + "Method: " + memberName + Environment.NewLine;
            privateViewBagErrorMsg = privateViewBagErrorMsg + "File: " + sourceFilePath + Environment.NewLine;
            privateViewBagErrorMsg = privateViewBagErrorMsg + "Zeile: " + sourceLineNumber.ToString("#0") + Environment.NewLine;
            return privateViewBagErrorMsg;
        }


        public static string ErrorString()
        {
            return privateViewBagErrorMsg;
        }


        public static string LogString()
        {
            return (privateLogMsgs == null) ? "" : privateLogMsgs;
        }

        
        public static void ErrorStringReset()
        {
            privateViewBagErrorMsg= null;
        }


        public static string CurrentCulture
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture.Name;
            }
        }

        // LogEintrag vornehmen
        //
        public static void logMsg(string msg,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            string path = System.Environment.GetEnvironmentVariable("temp");
            if (path == null || path.Length == 0) path = @"c:\temp";
            path = path + @"\__ismilealignertswebapp-" + DateTime.Now.ToString(@"yyyy-MM-dd") + ".log";
            string m = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + msg + ";" + memberName + ";" + sourceLineNumber.ToString() + ";" + sourceFilePath;
            StreamWriter sw = null;
            try
            {
                sw = File.AppendText(path);
                sw.WriteLine(m);
                sw.Close();
                sw.Dispose();
                sw = null;
            }
            catch (Exception)
            {
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            privateViewBagErrorMsg= null;
            privateLogMsgs = null;
            logMsg("ding ding");

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            foreach (var v in ViewEngines.Engines)
            {
                string t = v.GetType().ToString();
                if (t == "System.Web.Mvc.WebFormViewEngine")
                {
                    WebFormViewEngine x = (v as WebFormViewEngine);
                    if (x.ViewLocationFormats != null) logMsg("web " + string.Join(";", x.ViewLocationFormats));
                    privateOriginalWebViewSearches = string.Join(";", x.ViewLocationFormats);
                    privateOriginalWebPartialSearches = string.Join(";", x.PartialViewLocationFormats);
                    x.ViewLocationCache = DefaultViewLocationCache.Null;
                }
                else if (t == "System.Web.Mvc.RazorViewEngine")
                {
                    RazorViewEngine x = (v as RazorViewEngine);
                    if (x.ViewLocationFormats != null) logMsg("razor " + string.Join(";", x.ViewLocationFormats));
                    privateOriginalRazorViewSearches = string.Join(";", x.ViewLocationFormats);
                    privateOriginalWebPartialSearches = string.Join(";", x.PartialViewLocationFormats);
                    x.ViewLocationCache = DefaultViewLocationCache.Null;
                }
            }

            logMsg("web view " + privateOriginalWebViewSearches);
            logMsg("web part " + privateOriginalWebPartialSearches);
            logMsg("razor view " + privateOriginalRazorViewSearches);
            logMsg("razor part " + privateOriginalWebPartialSearches);
        }

        public static void ViewEngineSetViewPath(string e, string pattern)
        {
            if (ViewEngines.Engines != null && ViewEngines.Engines.Count > 0)
            {
                foreach (var v in ViewEngines.Engines)
                {
                    if (v != null)
                    {
                        string t = v.GetType().ToString();
                        if (e == "System.Web.Mvc.WebFormViewEngine" && t == e)
                        {
                            WebFormViewEngine x = (v as WebFormViewEngine);
                            x.ViewLocationFormats = pattern.Split(';');
                            x.ViewLocationCache = DefaultViewLocationCache.Null;
                            logMsg("web engine " + pattern);
                        }
                        else if (e == "System.Web.Mvc.RazorViewEngine" && t == e)
                        {
                            RazorViewEngine x = (v as RazorViewEngine);
                            x.ViewLocationFormats = pattern.Split(';');
                            x.PartialViewLocationFormats = pattern.Split(';');
                            x.ViewLocationCache = DefaultViewLocationCache.Null;
                            logMsg("razor engine " + pattern);
                        }
                    }
                }
            }
        }

        public static void ExpandViewEnginesSearchPath(string lang)
        {
            if (string.IsNullOrWhiteSpace(lang)) return;
            string langtoadd = lang.ToLower();
            int pos = lang.IndexOf('-');
            if (pos > 0) langtoadd = lang.Substring(0, pos);

            string neuweb = "";
            string neurazor = "";

            for (; langtoadd != ""; )
            {
                string langext= "";
                string neuerpath = "";
                string muster= "";
                string einzelpfad = "";

                pos = langtoadd.IndexOf(";");                           // de-AT;de
                if (pos > 0)
                {
                    langext = langtoadd.Substring(0, pos);              // de-AT
                    langtoadd = langtoadd.Substring(pos + 1);
                }
                else
                {
                    langext = langtoadd;                                // de
                    langtoadd = "";
                }

                neuerpath = "";
                muster = privateOriginalWebViewSearches;                // ~/Views/{1}/{0}.aspx;~/Views/{1}/{0}.ascx;~/Views/Shared/{0}.aspx;~/Views/Shared/{0}.ascx
                for(pos=0; pos >= 0;)
                {
                    pos = muster.IndexOf(';');                     
                    if (pos > 0)
                    {
                        einzelpfad = muster.Substring(0, pos);          // ~/Views/{1}/{0}.aspx
                        muster = muster.Substring(pos + 1);
                    }
                    else
                    {
                        einzelpfad = muster;                            // ~/Views/Shared/{0}.ascx
                        muster = "";
                    }
                    int dotpos = einzelpfad.LastIndexOf(@"/{0}");
                    if (dotpos > 0)
                    {
                        if (neuerpath != "") neuerpath += ";";
                        neuerpath += einzelpfad.Substring(0, dotpos +  1) + langext + einzelpfad.Substring(dotpos);
                    }
                }
                if (neuerpath != "")
                {
                    if (neuweb != "") neuweb += ";";
                    neuweb += neuerpath;
                }

                neuerpath = "";
                muster = privateOriginalRazorViewSearches;              // ~/Views/{1}/{0}.cshtml;~/Views/{1}/{0}.vbhtml;~/Views/Shared/{0}.cshtml;~/Views/Shared/{0}.vbhtml
                for (pos = 0; pos >= 0; )
                {
                    pos = muster.IndexOf(';');
                    if (pos > 0)
                    {
                        einzelpfad = muster.Substring(0, pos);          // ~/Views/{1}/{0}.cshtml
                        muster = muster.Substring(pos + 1);
                    }
                    else
                    {
                        einzelpfad = muster;                            // ~/Views/Shared/{0}.vbhtml
                        muster = "";
                    }
                    int dotpos = einzelpfad.LastIndexOf(@"/{0}");
                    if (dotpos > 0)
                    {
                        if (neuerpath != "") neuerpath += ";";
                        neuerpath += einzelpfad.Substring(0, dotpos + 1) + langext + einzelpfad.Substring(dotpos);
                    }
                }
                if (neuerpath != "")
                {
                    if (neurazor != "") neurazor += ";";
                    neurazor += neuerpath;
                }
            }

            if (neuweb != "") ViewEngineSetViewPath("System.Web.Mvc.WebFormViewEngine", neuweb + ";" + privateOriginalWebViewSearches);
            if (neurazor != "") ViewEngineSetViewPath("System.Web.Mvc.RazorViewEngine", neurazor + ";" + privateOriginalRazorViewSearches);
        }

        /*
        protected void Application_BeginRequest(Object source, EventArgs e)
        {
            if (!Context.Request.IsSecureConnection && !Request.Url.Host.Contains("localhost") && Request.Url.AbsolutePath.Contains("/Account/"))
            {
                Response.Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));
            }
        }
        */
    }
}
