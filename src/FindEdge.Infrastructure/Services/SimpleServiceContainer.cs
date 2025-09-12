using System;
using System.Collections.Generic;
using FindEdge.Core.Interfaces;

namespace FindEdge.Infrastructure.Services
{
    /// <summary>
    /// Implémentation simple du conteneur de services
    /// </summary>
    public class SimpleServiceContainer : IServiceContainer
    {
        private readonly Dictionary<Type, object> _services = new();
        private readonly Dictionary<Type, Func<object>> _factories = new();

        public void Register<T>(T service) where T : class
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _services[typeof(T)] = service;
        }

        public void Register<T>(Func<T> factory) where T : class
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _factories[typeof(T)] = () => factory();
        }

        public T Resolve<T>() where T : class
        {
            var type = typeof(T);

            // Chercher d'abord dans les services enregistrés
            if (_services.TryGetValue(type, out var service))
                return (T)service;

            // Chercher dans les factories
            if (_factories.TryGetValue(type, out var factory))
                return (T)factory();

            throw new InvalidOperationException($"Service de type {type.Name} non enregistré");
        }

        public bool IsRegistered<T>() where T : class
        {
            var type = typeof(T);
            return _services.ContainsKey(type) || _factories.ContainsKey(type);
        }
    }
}
