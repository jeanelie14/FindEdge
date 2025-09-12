using System;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le conteneur de services (DI)
    /// </summary>
    public interface IServiceContainer
    {
        /// <summary>
        /// Enregistre un service
        /// </summary>
        void Register<T>(T service) where T : class;

        /// <summary>
        /// Enregistre un service avec une factory
        /// </summary>
        void Register<T>(Func<T> factory) where T : class;

        /// <summary>
        /// Résout un service
        /// </summary>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Vérifie si un service est enregistré
        /// </summary>
        bool IsRegistered<T>() where T : class;
    }
}
