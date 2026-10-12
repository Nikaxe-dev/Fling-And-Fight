using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace FaF.Core.Services;

public interface INodeLoader<loadType>
{
    private IEnumerable<Type> GetDerivedTypes(Assembly assembly, Type baseType)
        => assembly.GetTypes().Where(t => t.IsAssignableTo(baseType));
    
    public void LoadDerived()
    {
        foreach (Type type in GetDerivedTypes(GetType().Assembly, typeof(loadType)))
            if (!type.IsAbstract)
                OnLoaded((loadType)Activator.CreateInstance(type));
    }

    protected void OnLoaded(loadType loaded)
    {
        if (loaded is Node loadedNode)
        {
            loadedNode.Name = loadedNode.GetType().Name.TrimPrefix("Server").TrimPrefix("Client");
            if (this is Node thisNode)
            {
                thisNode.AddChild(loadedNode);
                loadedNode.Owner = thisNode;
            }
        }
    }
}