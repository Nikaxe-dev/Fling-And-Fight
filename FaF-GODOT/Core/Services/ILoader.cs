using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FaF.Core.Services;

public interface ILoader<loadType>
{
    private IEnumerable<Type> GetDerivedTypes(Assembly assembly, Type baseType)
        => assembly.GetTypes().Where(t => t.IsAssignableTo(baseType));
    
    public void LoadDerived()
    {
        foreach (Type type in GetDerivedTypes(GetType().Assembly, typeof(loadType)))
            if (!type.IsAbstract)
                OnLoaded((loadType)Activator.CreateInstance(type));
    }

    protected void OnLoaded(loadType loaded);
}