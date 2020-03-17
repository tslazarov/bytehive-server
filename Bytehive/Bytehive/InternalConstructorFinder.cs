using Autofac.Core.Activators.Reflection;
using System;
using System.Linq;
using System.Reflection;

namespace Bytehive
{
    public class InternalConstructorFinder : IConstructorFinder
    {
        public ConstructorInfo[] FindConstructors(Type t) => t.GetTypeInfo().DeclaredConstructors.Where(c => !c.IsPrivate && !c.IsPublic).ToArray();
    }
}