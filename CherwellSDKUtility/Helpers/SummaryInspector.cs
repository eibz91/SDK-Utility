using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace CherwellSDKUtility.Helpers
{
    public class SummaryInspector
    {
        #region Properties

        static readonly Dictionary<Assembly, XmlDocument> Cache = new Dictionary<Assembly, XmlDocument>();
        static readonly Dictionary<Assembly, Exception> FailCache = new Dictionary<Assembly, Exception>();

        #endregion

        #region Public Methods

        public static XmlElement XmlFromMember(MethodInfo methodInfo)
        {
            string parametersString = "";

            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
            {
                if (parametersString.Length > 0)
                {
                    parametersString += ",";
                }

                parametersString += parameterInfo.ParameterType.FullName;
            }

            return parametersString.Length > 0 ? XmlFromName(methodInfo.DeclaringType, 'M', methodInfo.Name + "(" + parametersString + ")") 
                : XmlFromName(methodInfo.DeclaringType, 'M', methodInfo.Name);
        }

        public static XmlElement XmlFromMember(MemberInfo memberInfo)
        {
            return XmlFromName(memberInfo.DeclaringType, memberInfo.MemberType.ToString()[0], memberInfo.Name);
        }
        public static XmlElement XmlFromType(Type type)
        {
            return XmlFromName(type, 'T', "");
        }

        public static XmlDocument XmlFromAssembly(Assembly assembly)
        {
            if (FailCache.ContainsKey(assembly))
                throw FailCache[assembly];

            try
            {
                if (!Cache.ContainsKey(assembly))
                    Cache[assembly] = XmlFromAssemblyNonCached(assembly);

                return Cache[assembly];
            }
            catch (Exception exception)
            {
                FailCache[assembly] = exception;
                throw;
            }
        }

        #endregion

        #region Private Methods

        private static XmlElement XmlFromName(Type type, char prefix, string name)
        {
            string fullName;

            if (String.IsNullOrEmpty(name))
            {
                fullName = prefix + ":" + type.FullName;
            }
            else
            {
                fullName = prefix + ":" + type.FullName + "." + name;
            }

            XmlDocument xmlDocument = XmlFromAssembly(type.Assembly);
            XmlElement matchedElement = null;

            foreach (XmlElement xmlElement in xmlDocument["doc"]["members"])
            {
                if (xmlElement.Attributes["name"].Value.Equals(fullName))
                {
                    if (matchedElement != null)
                    {
                        throw new SummaryInspectorException("Multiple matches to query", null);
                    }

                    matchedElement = xmlElement;
                }
            }

            if (matchedElement == null)
            {
                throw new SummaryInspectorException("Could not find documentation for specified element", null);
            }

            return matchedElement;
        }

        private static XmlDocument XmlFromAssemblyNonCached(Assembly assembly)
        {
            string assemblyFilename = assembly.CodeBase;
            const string prefix = "file:///";

            if (assemblyFilename.StartsWith(prefix))
            {
                StreamReader streamReader;

                try
                {
                    streamReader = new StreamReader(Path.ChangeExtension(assemblyFilename.Substring(prefix.Length), ".xml"));
                }
                catch (FileNotFoundException exception)
                {
                    throw new SummaryInspectorException("XML documentation not present (make sure it is turned on in project properties when building)", exception);
                }

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(streamReader);
                return xmlDocument;
            }
            
            throw new SummaryInspectorException("Could not ascertain assembly filename", null);
        }

        #endregion
    }

    [Serializable]
    public class SummaryInspectorException : Exception
    {
        public SummaryInspectorException(string message, Exception innerException) : base(message, innerException) {}
    }
}
