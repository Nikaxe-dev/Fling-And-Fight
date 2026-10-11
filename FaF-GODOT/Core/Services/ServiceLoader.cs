using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FaF.Core.Services;

public abstract partial class ServiceLoader(string[] dependencies, Type[] loadedTypes, Type[] excludeTypes) : Service(dependencies)
{
    private IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType)
        => assembly.GetTypes().Where(t => t.BaseType == baseType);
    
    private readonly List<Service> LoadedServices = [];

    protected override void _LoadService()
    {
        base._LoadService();
        AddServices();
        LoadServices();
    }

    private void LoadServiceChild(Service service)
    {
        if (LoadedServices.Contains(service))
            return;
        
        foreach (string serviceName in service.Dependencies)
            if (GetNode(serviceName) is Service dependency)
                LoadServiceChild(dependency);
        
        LoadedServices.Add(service);
        service.LoadService();
    }

    private void LoadServices()
    {
        foreach (var child in GetChildren())
            if (child is Service service)
                LoadServiceChild(service);
    }

    private void AddServices()
    {
        foreach (Type loadedType in loadedTypes)
        {
            foreach (Type serviceType in FindDerivedTypes(GetType().Assembly, loadedType).Where(t => !t.IsAbstract).Where(t => !excludeTypes.Contains(t)))
            {
                Service service = (Service)Activator.CreateInstance(serviceType);
                service.Name = serviceType.Name;
                AddChild(service);
            }
        }
    }

    protected virtual void _ServiceLoaded(Service service) {}
    protected virtual void _ServicePreloaded(Service service) {}
}