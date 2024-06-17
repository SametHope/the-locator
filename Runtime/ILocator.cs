using UnityEngine;
using UnityEngine.SceneManagement;

namespace SametHope.TheLocator
{
    /// <summary>
    /// Interface for the locator. Implement this interface to create a custom locator that replaces the default one.
    /// </summary>
    public interface ILocator
    {
        // Global
        public bool TryGet<T>(out T service) where T : class;
        public bool TryRegister<T>(T service) where T : class;
        public bool TryUnregister<T>(T service) where T : class;
        public bool TryUnregister<T>() where T : class;

        // Scene
        public bool TryGet<T>(Scene scene, out T service) where T : class;
        public bool TryRegister<T>(T service, Scene scene) where T : class;
        public bool TryUnregister<T>(T service, Scene scene) where T : class;
        public bool TryUnregister<T>(Scene scene) where T : class;

        // GameObject
        public bool TryGet<T>(GameObject gameObject, out T service, bool includeParents = false) where T : class;
        public bool TryRegister<T>(T service, GameObject gameObject) where T : class;
        public bool TryUnregister<T>(T service, GameObject gameObject) where T : class;
        public bool TryUnregister<T>(GameObject gameObject) where T : class;
    }
}
