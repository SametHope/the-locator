using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SametHope.TheLocator
{
    public static class Locator
    {
        /// <summary>
        /// Use <see cref="RuntimeInitializeOnLoadMethodAttribute"/> with <see cref="RuntimeInitializeLoadType.SubsystemRegistration"/> or <see cref="RuntimeInitializeLoadType.AfterAssembliesLoaded"/> to override this field in order to use a custom locator.
        /// </summary>
        public static Func<ILocator> GetNewLocator { get; set; }
        /// <summary>
        /// Use this field to modify tha way errors are logged. By default it uses <see cref="Debug.LogError(object)"/>.<br></br>
        /// You can modify this field anytime.
        /// </summary>
        public static Action<object> LogError { get; set; }
        private static ILocator _locator { get; set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Initialize()
        {
            _locator = GetNewLocator?.Invoke() ?? new DefaultLocator();
            LogError ??= Debug.LogError;
        }

        #region Global calls
        public static T Get<T>() where T : class
        {
            if (!TryGet(out T service))
            {
                Debug.LogError($"Service of type {typeof(T)} is not registered globally.");
            }
            return service;
        }
        public static void Register<T>(T service) where T : class
        {
            if (!TryRegister(service))
            {
                Debug.LogError($"Service of type {typeof(T)} is already registered globally.");
            }
        }
        public static void Unregister<T>(T service) where T : class
        {
            if (!TryUnregister(service))
            {
                Debug.LogError($"Service of type {typeof(T)} is not registered globally.");
            }
        }
        public static void Unregister<T>() where T : class
        {
            if (!TryUnregister<T>())
            {
                Debug.LogError($"Service of type {typeof(T)} is not registered globally.");
            }
        }

        public static bool TryGet<T>(out T service) where T : class => _locator.TryGet(out service);
        public static bool TryRegister<T>(T service) where T : class => _locator.TryRegister(service);
        public static bool TryUnregister<T>(T service) where T : class => _locator.TryUnregister(service);
        public static bool TryUnregister<T>() where T : class => _locator.TryUnregister<T>();
        #endregion

        #region Scene calls
        public static T Get<T>(Scene scene) where T : class
        {
            if (!TryGet(scene, out T service))
            {
                Debug.LogError($"Service of type {typeof(T)} is not registered for the scene {scene.path} or globally.");
            }
            return service;
        }
        public static void Register<T>(T service, Scene scene) where T : class
        {
            if (!TryRegister(service, scene))
            {
                Debug.LogError($"Service of type {typeof(T)} is already registered for the scene {scene.path} or globally.");
            }
        }
        public static void Unregister<T>(T service, Scene scene) where T : class
        {
            if (!TryUnregister(service, scene))
            {
                Debug.LogError($"Service of type {typeof(T)} is not registered for the scene {scene.path} or globally.");
            }
        }
        public static void Unregister<T>(Scene scene) where T : class
        {
            if (!TryUnregister<T>(scene))
            {
                Debug.LogError($"Service of type {typeof(T)} is not registered for the scene {scene.path} or globally.");
            }
        }

        public static bool TryGet<T>(Scene scene, out T service) where T : class => _locator.TryGet(scene, out service);
        public static bool TryRegister<T>(T service, Scene scene) where T : class => _locator.TryRegister(service, scene);
        public static bool TryUnregister<T>(T service, Scene scene) where T : class => _locator.TryUnregister(service, scene);
        public static bool TryUnregister<T>(Scene scene) where T : class => _locator.TryUnregister<T>(scene);
        #endregion

        #region GameObject calls
        public static T Get<T>(GameObject gameObject, bool includeParents = false) where T : class
        {
            if (!TryGet(gameObject, out T service, includeParents))
            {
                Debug.LogError($"Service of type {typeof(T)} is not registered for the gameobject {gameObject.name},{(includeParents ? " its parents," : "")} its scene {gameObject.scene.path} or globally.");
            }
            return service;
        }
        public static void Register<T>(T service, GameObject gameObject) where T : class
        {
            if (!TryRegister(service, gameObject))
            {
                Debug.LogError($"Service of type {typeof(T)} is already registered for the gameobject {gameObject.name}.");
            }
        }
        public static void Unregister<T>(T service, GameObject gameObject) where T : class
        {
            if (!TryUnregister(service, gameObject))
            {
                Debug.LogError($"Service of type {typeof(T)} is not registered for the gameobject {gameObject.name}.");
            }
        }
        public static void Unregister<T>(GameObject gameObject) where T : class
        {
            if (!TryUnregister<T>(gameObject))
            {
                Debug.LogError($"Service of type {typeof(T)} is not registered for the gameobject {gameObject.name}.");
            }
        }

        public static bool TryGet<T>(GameObject gameObject, out T service, bool includeParents = false) where T : class => _locator.TryGet(gameObject, out service, includeParents);
        public static bool TryRegister<T>(T service, GameObject gameObject) where T : class => _locator.TryRegister(service, gameObject);
        public static bool TryUnregister<T>(T service, GameObject gameObject) where T : class => _locator.TryUnregister(service, gameObject);
        public static bool TryUnregister<T>(GameObject gameObject) where T : class => _locator.TryUnregister<T>(gameObject);

        // MonoBehaviour redirects
        public static T Get<T>(MonoBehaviour monoBehaviour, bool includeParents = false) where T : class
        {
            if (!TryGet(monoBehaviour, out T service, includeParents))
            {
                Debug.LogError($"Service of type {typeof(T)} is not registered for the gameobject {monoBehaviour.gameObject.name},{(includeParents ? " its parents," : "")} its scene {monoBehaviour.gameObject.scene.path} or globally.");
            }
            return service;
        }
        public static void Register<T>(T service, MonoBehaviour monoBehaviour) where T : class
        {
            if (!TryRegister(service, monoBehaviour))
            {
                Debug.LogError($"Service of type {typeof(T)} is already registered for the gameobject {monoBehaviour.gameObject.name}.");
            }
        }
        public static void Unregister<T>(T service, MonoBehaviour monoBehaviour) where T : class
        {
            if (!TryUnregister(service, monoBehaviour))
            {
                Debug.LogError($"Service of type {typeof(T)} is not registered for the gameobject {monoBehaviour.gameObject.name}.");
            }
        }
        public static void Unregister<T>(MonoBehaviour monoBehaviour) where T : class
        {
            if (!TryUnregister<T>(monoBehaviour))
            {
                Debug.LogError($"Service of type {typeof(T)} is not registered for the gameobject {monoBehaviour.gameObject.name}.");
            }
        }

        public static bool TryGet<T>(MonoBehaviour monoBehaviour, out T service, bool includeParents = false) where T : class => _locator.TryGet(monoBehaviour.gameObject, out service, includeParents);
        public static bool TryRegister<T>(T service, MonoBehaviour monoBehaviour) where T : class => _locator.TryRegister(service, monoBehaviour.gameObject);
        public static bool TryUnregister<T>(T service, MonoBehaviour monoBehaviour) where T : class => _locator.TryUnregister(service, monoBehaviour.gameObject);
        public static bool TryUnregister<T>(MonoBehaviour monoBehaviour) where T : class => _locator.TryUnregister<T>(monoBehaviour.gameObject);
        #endregion
    }
}
