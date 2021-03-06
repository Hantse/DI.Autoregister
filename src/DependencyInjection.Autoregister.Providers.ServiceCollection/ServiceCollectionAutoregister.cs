﻿using DependencyInjection.Autoregister.Abstraction.Helpers;
using DependencyInjection.Autoregister.Abstraction.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace DependencyInjection.Autoregister.Providers
{
    public static class AddAutoRegistration
    {
        public static IServiceCollection AddAutoRegister(this IServiceCollection services)
        {
            RegisterFromAssemlyLoad(services, Assembly.GetExecutingAssembly());
            return services;
        }
        public static IServiceCollection AddAutoRegister(this IServiceCollection services, Assembly assembly)
        {
            RegisterFromAssemlyLoad(services, assembly);
            return services;
        }
        public static IServiceCollection AddAutoRegister(this IServiceCollection services, Type assemblyType)
        {
            RegisterFromAssemlyLoad(services, Assembly.GetAssembly(assemblyType));
            return services;
        }


        public static IServiceCollection AddAutoRegister(this IServiceCollection services, string startWith)
        {
            RegisterFromAssemlyLoad(services, Assembly.GetExecutingAssembly(), startWith);
            return services;
        }
        public static IServiceCollection AddAutoRegister(this IServiceCollection services, Assembly assembly, string startWith)
        {
            RegisterFromAssemlyLoad(services, assembly, startWith);
            return services;
        }
        public static IServiceCollection AddAutoRegister(this IServiceCollection services, Type assemblyType, string startWith)
        {
            RegisterFromAssemlyLoad(services, Assembly.GetAssembly(assemblyType), startWith);
            return services;
        }

        public static IServiceCollection AddAutoRegister(this IServiceCollection services, string[] startsWith)
        {
            RegisterFromAssemlyLoad(services, Assembly.GetExecutingAssembly(), startsWith);
            return services;
        }
        public static IServiceCollection AddAutoRegister(this IServiceCollection services, Assembly assembly, string[] startsWith)
        {
            RegisterFromAssemlyLoad(services, assembly, startsWith);
            return services;
        }
        public static IServiceCollection AddAutoRegister(this IServiceCollection services, Type assemblyType, string[] startsWith)
        {
            RegisterFromAssemlyLoad(services, Assembly.GetAssembly(assemblyType), startsWith);
            return services;
        }

        private static void RegisterFromAssemlyLoad(IServiceCollection services, Assembly assembly)
        {
            var registrations = AssemblyIterator.LoadFromAssembly(assembly);

            foreach (var reg in registrations)
            {
                Register(services, reg.Type, reg.ServiceType, reg.ImplementationType);
            }
        }

        private static void RegisterFromAssemlyLoad(IServiceCollection services, Assembly assembly, string startWith)
        {
            var registrations = AssemblyIterator.LoadFromAssembly(assembly, startWith);

            foreach (var reg in registrations)
            {
                Register(services, reg.Type, reg.ServiceType, reg.ImplementationType);
            }
        }
        private static void RegisterFromAssemlyLoad(IServiceCollection services, Assembly assembly, string[] startsWith)
        {
            var registrations = AssemblyIterator.LoadFromAssembly(assembly, startsWith);

            foreach (var reg in registrations)
            {
                Register(services, reg.Type, reg.ServiceType, reg.ImplementationType);
            }
        }

        private static void Register(IServiceCollection services, ServiceRegistrationType registrationType, Type serviceType, Type implementationType)
        {
            if (registrationType == ServiceRegistrationType.SCOPED)
            {
                if (serviceType != null)
                    services.AddScoped(serviceType, implementationType);
                else
                    services.AddScoped(implementationType);
            }
            else if (registrationType == ServiceRegistrationType.TRANSIENT)
            {
                if (serviceType != null)
                    services.AddTransient(serviceType, implementationType);
                else
                    services.AddTransient(implementationType);
            }
            else
            {
                if (serviceType != null)
                    services.AddSingleton(serviceType, implementationType);
                else
                    services.AddSingleton(implementationType);
            }
        }
    }
}
