using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Bytehive.Scraper.Utilites
{
    public class XPathExtensionVariable : IXsltContextVariable
    {
        // Namespace of user-defined variable.
        private string prefix;
        // The name of the user-defined variable.
        private string varName;

        // Constructor used in the overridden ResolveVariable function of custom XsltContext.
        public XPathExtensionVariable(string prefix, string varName)
        {
            this.prefix = prefix;
            this.varName = varName;
        }

        // Function to return the value of the specified user-defined variable.
        // The GetParam method of the XsltArgumentList property of the active
        // XsltContext object returns value assigned to the specified variable.
        public object Evaluate(System.Xml.Xsl.XsltContext xsltContext)
        {
            XsltArgumentList vars = ((BytehiveScraperContext)xsltContext).ArgList;
            return vars.GetParam(varName, prefix);
        }

        // Determines whether this variable is a local XSLT variable.
        // Needed only when using a style sheet.
        public bool IsLocal
        {
            get
            {
                return false;
            }
        }

        // Determines whether this parameter is an XSLT parameter.
        // Needed only when using a style sheet.
        public bool IsParam
        {
            get
            {
                return false;
            }
        }

        public System.Xml.XPath.XPathResultType VariableType
        {
            get
            {
                return XPathResultType.Any;
            }
        }
    }

}
