﻿using DependencyInjection.Autoregister.Abstraction.Attributes;
using DependencyInjection.Autoregister.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyInjection.Autoregister.Abstraction.Helpers
{
    public static class AssemblyIterator
    {
        private static List<ServiceRegistration> _serviceRegistrations = new List<ServiceRegistration>();

        private static List<string> _exludeNamespaceStartWith = new List<string>()
        {
            "Microsoft",
            "System"
        };

        public static ServiceRegistration[] LoadFromAssembly(Assembly assembly)
        {
            _serviceRegistrations = new List<ServiceRegistration>();
            GetTypesWithHelpAttribute(assembly, a => !_exludeNamespaceStartWith.Any(s => a.FullName.StartsWith(s)));
            return _serviceRegistrations.ToArray();
        }

        public static ServiceRegistration[] LoadFromAssembly(Assembly assembly, string startWith)
        {
            _serviceRegistrations = new List<ServiceRegistration>();
            GetTypesWithHelpAttribute(assembly, a => a.FullName.StartsWith(startWith));
            return _serviceRegistrations.ToArray();
        }

        public static ServiceRegistration[] LoadFromAssembly(Assembly assembly, string[] startsWith)
        {
            _serviceRegistrations = new List<ServiceRegistration>();
            GetTypesWithHelpAttribute(assembly, a => startsWith.Any(s => a.FullName.StartsWith(s)));
            return _serviceRegistrations.ToArray();
        }

        private static void GetTypesWithHelpAttribute(Assembly assembly, Func<AssemblyName, bool> predicate)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(DependencyRegistration), true).Length > 0)
                {
                    AddToStack(type);
                }
            }

            GetTypesWithHelpAttributeFromChild(assembly, predicate);
        }

        public static void GetTypesWithHelpAttributeFromChild(Assembly assembly, Func<AssemblyName, bool> predicate)
        {
            foreach (AssemblyName asem in assembly.GetReferencedAssemblies().Where(predicate))
            {
                Assembly loadAssembly = Assembly.Load(asem);

                foreach (Type type in loadAssembly.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof(DependencyRegistration), true).Length > 0)
                    {
                        AddToStack(type);
                    }
                }

                GetTypesWithHelpAttributeFromChild(loadAssembly, predicate);
            }
        }

        private static void AddToStack(Type type)
        {
            ServiceRegistrationType registrationType = GetRegistrationType(type);
            Type interfaceOf = GetRegistrationInterfaceType(type);
            Type[] interfaces = type.GetInterfaces();

            if (interfaceOf == null && interfaces.Length > 0)
            {
                for (int i = 0; i < interfaces.Length; i++)
                {
                    _serviceRegistrations.Add(new ServiceRegistration
                    {
                        ServiceType = interfaces[i],
                        ImplementationType = type,
                        Type = registrationType
                    });
                }
            }
            else
            {
                _serviceRegistrations.Add(new ServiceRegistration
                {
                    ServiceType = interfaceOf,
                    ImplementationType = type,
                    Type = registrationType
                });
            }
        }

        private static Type GetRegistrationInterfaceType(Type type)
        {
            return GetAttribute(type).InterfaceOf;
        }

        private static ServiceRegistrationType GetRegistrationType(Type type)
        {
            return GetAttribute(type).RegistrationType;
        }

        private static DependencyRegistration GetAttribute(Type type)
        {
            return type.GetCustomAttributes(typeof(DependencyRegistration), true).First() as DependencyRegistration;
        }
    }
}
