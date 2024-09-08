#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides methods to manage services across different scopes: <see cref="Global"/>, <see cref="Scene"/>, 
/// and <see cref="GameObject"/>. Services can be <c>registered</c>, <c>retrieved</c>, and <c>unregistered</c>
/// within these scopes.
/// </summary>
/// <remarks>
/// <para>
/// Scopes are ordered from lowest to highest as follows:
/// <list type="bullet">
/// <item><description><see cref="GameObject"/>/<see cref="Component"/>></description></item>
/// <item><description><see cref="Scene"/></description></item>
/// <item><description><see cref="Global"/></description></item>
/// </list>
/// </para>
/// <para>
/// If a service isn't found in a lower scope, the system will search in the higher scopes.
/// Services are registered by type, with an optional name to differentiate between multiple services of the same type.
/// </para>
/// <para>
/// Methods without the "Try" prefix will throw exceptions if a service isn't found or is already registered.
/// To unregister services, you can use methods without specifying the service parameter to remove services by type and name.
/// </para>
/// </remarks>
public static class Locator
{
    /// <summary>
    /// Contains the registered services for <see cref="GameObject"/>/<see cref="Component"/> scopes.
    /// <para>
    /// </para>
    /// Do not modify this dictionary directly unless you have a reason to. Use Get, Register, and Unregister methods instead.
    /// </summary>
    public static Dictionary<(int instanceID, string? name), object> Containers_GameObject { get; set; }
    /// <summary>
    /// Contains the registered services for <see cref="Scene"/> scopes.
    /// <para>
    /// </para>
    /// Do not modify this dictionary directly unless you have a reason to. Use Get, Register, and Unregister methods instead.
    /// </summary>
    public static Dictionary<(string scenePath, string? name), object> Containers_Scene { get; set; }
    /// <summary>
    /// Contains the registered services for <see cref="Global"/> scope.
    /// <para>
    /// </para>
    /// Do not modify this dictionary directly unless you have a reason to. Use Get, Register, and Unregister methods instead.
    /// </summary>
    public static Dictionary<(Type type, string? name), object> Containers_Global { get; set; }

    // This method is necessary to work with enter play mode options
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Bootstrap()
    {
        Containers_GameObject = new();
        Containers_Scene = new();
        Containers_Global = new();
    }

    #region Global Scope Calls
    #region Get
    /// <summary>
    /// Get the global service of type <typeparamref name="T"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static T Get<T>(string? serviceName = null) where T : class
    {
        var t = typeof(T);
        if(Containers_Global.TryGetValue((typeof(T), serviceName), out object globalService))
        {
            return (T)globalService;
        }

        throw new Exception($"Could not get global service type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} because it is not registered.");
    }
    /// <summary>
    /// Try to get the global service of type <typeparamref name="T"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static bool TryGet<T>(out T service, string? serviceName = null) where T : class
    {
        var t = typeof(T);
        try { service = Get<T>(serviceName); return true; }
        catch { service = null; return false; }
    }
    #endregion
    #region Register
    /// <summary>
    /// Register the global service of type <typeparamref name="T"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static void Register<T>(T service, string? serviceName = null) where T : class
    {
        if(service == null)
        {
            throw new Exception($"Could not register global service of type {typeof(T)} because service is null.");
        }

        if(Containers_Global.ContainsKey((typeof(T), serviceName)))
        {
            throw new Exception($"Could not register global service of type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} because service is already registered.");
        }

        Containers_Global.Add((typeof(T), serviceName), service);
    }
    /// <summary>
    /// Try to register the global service of type <typeparamref name="T"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static bool TryRegister<T>(T service, string? serviceName = null) where T : class
    {
        try { Register(service, serviceName); return true; }
        catch { return false; }
    }
    #endregion
    #region Unregister
    /// <summary>
    /// Unregister the global service of type <typeparamref name="T"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static void Unregister<T>(T service, string? serviceName = null) where T : class
    {
        if(service == null)
        {
            throw new Exception($"Could not unregister global service of type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} because service is null.");
        }
        if(!Containers_Global.ContainsKey((typeof(T), serviceName)))
        {
            throw new Exception($"Could not unregister global service of type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} because service is not registered.");
        }

        Containers_Global.Remove((typeof(T), serviceName));
    }
    /// <summary>
    /// Unregister the global service of type <typeparamref name="T"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static void Unregister<T>(string? serviceName = null) where T : class
    {
        if(!Containers_Global.ContainsKey((typeof(T), serviceName)))
        {
            throw new Exception($"Could not unregister global service of type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} because service is not registered.");
        }

        Containers_Global.Remove((typeof(T), serviceName));
    }

    /// <summary>
    /// Try to unregister the global service of type <typeparamref name="T"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static bool TryUnregister<T>(T service, string? serviceName = null) where T : class
    {
        try { Unregister(service, serviceName); return true; }
        catch { return false; }
    }
    /// <summary>
    /// Try to unregister the global service of type <typeparamref name="T"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static bool TryUnregister<T>(string? serviceName = null) where T : class
    {
        try { Unregister<T>(serviceName); return true; }
        catch { return false; }
    }
    #endregion
    #endregion

    #region Scene Scope Calls
    #region Get
    /// <summary>
    /// Register the service of type <typeparamref name="T"/> for the scene <paramref name="scene"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static T Get<T>(Scene scene, string? serviceName = null) where T : class
    {
        var t = typeof(T);
        if(Containers_Scene.TryGetValue((scene.path, serviceName), out object sceneService))
        {
            return (T)sceneService;
        }

        if(TryGet(out T globalService, serviceName))
        {
            return globalService;
        }

        throw new Exception($"Could not get service of type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} for the scene {scene.path} because it is not registered.");
    }
    /// <summary>
    /// Try to get the service of type <typeparamref name="T"/> for the scene <paramref name="scene"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static bool TryGet<T>(Scene scene, out T service, string? serviceName = null) where T : class
    {
        // T BECAME OBJECT HERE WHYY
        var t = typeof(T);
        try { service = Get<T>(scene, serviceName); return true; }
        catch { service = null; return false; }
    }
    #endregion
    #region Register
    /// <summary>
    /// Register the service of type <typeparamref name="T"/> for the scene <paramref name="scene"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static void Register<T>(T service, Scene scene, string? serviceName = null) where T : class
    {
        if(service == null)
        {
            throw new Exception($"Could not register service of type {typeof(T)} for the scene {scene.path} because service is null.");
        }

        if(Containers_Scene.ContainsKey((scene.path, serviceName)))
        {
            throw new Exception($"Could not register service of type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} for the scene {scene.path} because service is already registered.");
        }

        Containers_Scene.Add((scene.path, serviceName), service);
    }
    /// <summary>
    /// Try to register the service of type <typeparamref name="T"/> for the scene <paramref name="scene"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static bool TryRegister<T>(T service, Scene scene, string? serviceName = null) where T : class
    {
        try { Register(service, scene, serviceName); return true; }
        catch { return false; }
    }
    #endregion
    #region Unregister
    /// <summary>
    /// Unregister the service of type <typeparamref name="T"/> for the scene <paramref name="scene"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static void Unregister<T>(T service, Scene scene, string? serviceName = null) where T : class
    {
        if(service == null)
        {
            throw new Exception($"Could not unregister service of type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} for the scene {scene.path} because service is null.");
        }
        if(!Containers_Scene.ContainsKey((scene.path, serviceName)))
        {
            throw new Exception($"Could not unregister service of type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} for the scene {scene.path} because service is not registered.");
        }

        Containers_Scene.Remove((scene.path, serviceName));
    }
    /// <summary>
    /// Unregister the service of type <typeparamref name="T"/> for the scene <paramref name="scene"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static void Unregister<T>(Scene scene, string? serviceName = null) where T : class
    {
        if(!Containers_Scene.ContainsKey((scene.path, serviceName)))
        {
            throw new Exception($"Could not unregister service of type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} for the scene {scene.path} because service is not registered.");
        }

        Containers_Scene.Remove((scene.path, serviceName));
    }

    /// <summary>
    /// Try to unregister the service of type <typeparamref name="T"/> for the scene <paramref name="scene"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static bool TryUnregister<T>(T service, Scene scene, string? serviceName = null) where T : class
    {
        try { Unregister(service, scene, serviceName); return true; }
        catch { return false; }
    }
    /// <summary>
    /// Try to unregister the service of type <typeparamref name="T"/> for the scene <paramref name="scene"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static bool TryUnregister<T>(Scene scene, string? serviceName = null) where T : class
    {
        try { Unregister<T>(scene, serviceName); return true; }
        catch { return false; }
    }
    #endregion
    #endregion

    #region GameObject Scope Calls
    #region Get
    /// <summary>
    /// Get the service of type <typeparamref name="T"/> for the gameobject <paramref name="gameObject"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static T Get<T>(GameObject gameObject, string? serviceName = null) where T : class
    {
        var t = typeof(T);
        if(Containers_GameObject.TryGetValue((gameObject.GetInstanceID(), serviceName), out object goService))
        {
            return (T)goService;
        }

        if(TryGet(gameObject.scene, out T sceneService, serviceName))
        {
            return sceneService;
        }

        throw new Exception($"Could not get service of type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} for the gameobject {gameObject.name} because it is not registered.");
    }
    /// <summary>
    /// Try to get the service of type <typeparamref name="T"/> for the gameobject <paramref name="gameObject"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static bool TryGet<T>(GameObject gameObject, out T service, string? serviceName = null) where T : class
    {
        var t = typeof(T);
        try { service = Get<T>(gameObject, serviceName); return true; }
        catch { service = null; return false; }
    }
    // Component redirects
    /// <summary>
    /// Get the service of type <typeparamref name="T"/> for the <see cref="GameObject"/> of the component <paramref name="component"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static T Get<T>(Component component, string? serviceName = null) where T : class
    {
        return Get<T>(component.gameObject, serviceName);
    }
    /// <summary>
    /// Try to get the service of type <typeparamref name="T"/> for the <see cref="GameObject"/> of the component <paramref name="component"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static bool TryGet<T>(Component component, out T service, string? serviceName = null) where T : class
    {
        try { service = Get<T>(component, serviceName); return true; }
        catch { service = null; return false; }
    }
    #endregion
    #region Register
    /// <summary>
    /// Register the service of type <typeparamref name="T"/> for the gameobject <paramref name="gameObject"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static void Register<T>(T service, GameObject gameObject, string? serviceName = null) where T : class
    {
        if(service == null)
        {
            throw new Exception($"Could not register service of type {typeof(T)} for the gameobject {gameObject.name} because service is null.");
        }

        if(Containers_GameObject.ContainsKey((gameObject.GetInstanceID(), serviceName)))
        {
            throw new Exception($"Could not register service of type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} for the gameobject {gameObject.name} because service is already registered.");
        }

        Containers_GameObject.Add((gameObject.GetInstanceID(), serviceName), service);
    }
    /// <summary>
    /// Try to register the service of type <typeparamref name="T"/> for the gameobject <paramref name="gameObject"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static bool TryRegister<T>(T service, GameObject gameObject, string? serviceName = null) where T : class
    {
        try { Register(service, gameObject, serviceName); return true; }
        catch { return false; }
    }
    // Component redirects
    /// <summary>
    /// Register the service of type <typeparamref name="T"/> for the <see cref="GameObject"/> of the component <paramref name="component"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static void Register<T>(T service, Component component, string? serviceName = null) where T : class
    {
        Register(service, component.gameObject, serviceName);
    }
    /// <summary>
    /// Try to register the service of type <typeparamref name="T"/> for the <see cref="GameObject"/> of the component <paramref name="component"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static bool TryRegister<T>(T service, Component component, string? serviceName = null) where T : class
    {
        try { Register(service, component, serviceName); return true; }
        catch { return false; }
    }
    #endregion
    #region Unregister
    /// <summary>
    /// Unregister the service of type <typeparamref name="T"/> for the gameobject <paramref name="gameObject"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static void Unregister<T>(T service, GameObject gameObject, string? serviceName = null) where T : class
    {
        if(service == null)
        {
            throw new Exception($"Could not unregister service of type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} for the gameobject {gameObject.name} because service is null.");
        }
        if(gameObject == null)
        {
            throw new Exception($"Could not unregister service of type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} because gameobject is null.");
        }
        if(!Containers_GameObject.ContainsKey((gameObject.GetInstanceID(), serviceName)))
        {
            throw new Exception($"Could not unregister service of type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} for the gameobject {gameObject.name} because service is not registered.");
        }

        Containers_GameObject.Remove((gameObject.GetInstanceID(), serviceName));
    }
    /// <summary>
    /// Unregister the service of type <typeparamref name="T"/> for the gameobject <paramref name="gameObject"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static void Unregister<T>(GameObject gameObject, string? serviceName = null) where T : class
    {
        if(gameObject == null)
        {
            throw new Exception($"Could not unregister service of type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} because gameobject is null.");
        }
        if(!Containers_GameObject.ContainsKey((gameObject.GetInstanceID(), serviceName)))
        {
            throw new Exception($"Could not unregister service of type {typeof(T)}{(serviceName == null ? "" : $" with the name {serviceName}")} for the gameobject {gameObject.name} because service is not registered.");
        }

        Containers_GameObject.Remove((gameObject.GetInstanceID(), serviceName));
    }

    /// <summary>
    /// Try to unregister the service of type <typeparamref name="T"/> for the gameobject <paramref name="gameObject"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static bool TryUnregister<T>(T service, GameObject gameObject, string? serviceName = null) where T : class
    {
        try { Unregister(service, gameObject, serviceName); return true; }
        catch { return false; }
    }
    /// <summary>
    /// Try to unregister the service of type <typeparamref name="T"/> for the gameobject <paramref name="gameObject"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static bool TryUnregister<T>(GameObject gameObject, string? serviceName = null) where T : class
    {
        try { Unregister<T>(gameObject, serviceName); return true; }
        catch { return false; }
    }
    // Component redirects
    /// <summary>
    /// Unregister the service of type <typeparamref name="T"/> for the <see cref="GameObject"/> of the component <paramref name="component"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static void Unregister<T>(T service, Component component, string? serviceName = null) where T : class
    {
        Unregister(service, component.gameObject, serviceName);
    }
    /// <summary>
    /// Unregister the service of type <typeparamref name="T"/> for the <see cref="GameObject"/> of the component <paramref name="component"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static void Unregister<T>(Component component, string? serviceName = null) where T : class
    {
        Unregister<T>(component.gameObject, serviceName);
    }
    /// <summary>
    /// Try to unregister the service of type <typeparamref name="T"/> for the <see cref="GameObject"/> of the component <paramref name="component"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static bool TryUnregister<T>(T service, Component component, string? serviceName = null) where T : class
    {
        try { Unregister(service, component, serviceName); return true; }
        catch { return false; }
    }
    /// <summary>
    /// Try to unregister the service of type <typeparamref name="T"/> for the <see cref="GameObject"/> of the component <paramref name="component"/> with the optional name <paramref name="serviceName"/>.
    /// </summary>
    public static bool TryUnregister<T>(Component component, string? serviceName = null) where T : class
    {
        try { Unregister<T>(component, serviceName); return true; }
        catch { return false; }
    }
    #endregion
    #endregion
}
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
