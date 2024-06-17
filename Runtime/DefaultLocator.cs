using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SametHope.TheLocator
{
    /// <summary>
    /// Default implementation of <see cref="ILocator"/>. It uses dictionaries to store services.
    /// </summary>
    public class DefaultLocator : ILocator
    {
        private readonly Dictionary<int, Dictionary<Type, object>> _gameObjectContainers; // key is gameobject instance id
        private readonly Dictionary<string, Dictionary<Type, object>> _sceneContainers; // key is scene path
        private readonly Dictionary<Type, object> _globalContainer;

        public DefaultLocator()
        {
            _gameObjectContainers = new();
            _sceneContainers = new();
            _globalContainer = new();
        }

        #region Global
        public bool TryGet<T>(out T service) where T : class
        {
            if (_globalContainer.TryGetValue(typeof(T), out object value))
            {
                service = (T)value;
                return true;
            }
            service = null;
            return false;
        }
        public bool TryRegister<T>(T service) where T : class
        {
            if (!_globalContainer.ContainsKey(typeof(T)))
            {
                _globalContainer.Add(typeof(T), service);
                return true;
            }
            return false;
        }
        public bool TryUnregister<T>(T service) where T : class
        {
            return TryUnregister<T>();
        }
        public bool TryUnregister<T>() where T : class
        {
            return _globalContainer.Remove(typeof(T));
        }
        #endregion

        #region Scene
        public bool TryGet<T>(Scene scene, out T service) where T : class
        {
            if (_sceneContainers.TryGetValue(scene.path, out var container) && container.TryGetValue(typeof(T), out object value))
            {
                service = (T)value;
                return true;
            }

            return TryGet(out service);
        }
        public bool TryRegister<T>(T service, Scene scene) where T : class
        {
            if (!_sceneContainers.TryGetValue(scene.path, out var container))
            {
                container = new();
                _sceneContainers.Add(scene.path, container);
            }
            if (!container.ContainsKey(typeof(T)))
            {
                container.Add(typeof(T), service);
                return true;
            }
            return false;
        }
        public bool TryUnregister<T>(T service, Scene scene) where T : class
        {
            return TryUnregister<T>(scene);
        }
        public bool TryUnregister<T>(Scene scene) where T : class
        {
            if (_sceneContainers.TryGetValue(scene.path, out var container))
            {
                return container.Remove(typeof(T));
            }
            return false;
        }
        #endregion

        #region GameObject
        public bool TryGet<T>(GameObject gameObject, out T service, bool includeParents = false) where T : class
        {
            if (_gameObjectContainers.TryGetValue(gameObject.GetInstanceID(), out var container) && container.TryGetValue(typeof(T), out object value))
            {
                service = (T)value;
                return true;
            }
            if (includeParents)
            {
                Transform parent = gameObject.transform.parent;
                while (parent != null)
                {
                    if (_gameObjectContainers.TryGetValue(parent.gameObject.GetInstanceID(), out container) && container.TryGetValue(typeof(T), out value))
                    {
                        service = (T)value;
                        return true;
                    }
                    parent = parent.parent;
                }
            }

            return TryGet(gameObject.scene, out service);
        }
        public bool TryRegister<T>(T service, GameObject gameObject) where T : class
        {
            if (!_gameObjectContainers.TryGetValue(gameObject.GetInstanceID(), out var container))
            {
                container = new();
                _gameObjectContainers.Add(gameObject.GetInstanceID(), container);
            }
            if (!container.ContainsKey(typeof(T)))
            {
                container.Add(typeof(T), service);
                return true;
            }
            return false;
        }
        public bool TryUnregister<T>(T service, GameObject gameObject) where T : class
        {
            return TryUnregister<T>(gameObject);
        }
        public bool TryUnregister<T>(GameObject gameObject) where T : class
        {
            if (_gameObjectContainers.TryGetValue(gameObject.GetInstanceID(), out var container))
            {
                return container.Remove(typeof(T));
            }
            return false;
        }

        public bool TryGet<T>(MonoBehaviour monoBehaviour, out T service, bool includeParents = false) where T : class
        {
            return TryGet(monoBehaviour.gameObject, out service, includeParents);
        }
        public bool TryRegister<T>(T service, MonoBehaviour monoBehaviour) where T : class
        {
            return TryRegister(service, monoBehaviour.gameObject);
        }
        public bool TryUnregister<T>(T service, MonoBehaviour monoBehaviour) where T : class
        {
            return TryUnregister(service, monoBehaviour.gameObject);
        }
        public bool TryUnregister<T>(MonoBehaviour monoBehaviour) where T : class
        {
            return TryUnregister<T>(monoBehaviour.gameObject);
        }
        #endregion
    }
}
