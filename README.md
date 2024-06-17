# TheLocator
A service locator implementation for Unity with consideration for gameobjects and their hierarchies, scenes and the global scope.  

## About
This is a Unity package that implements the service locator pattern in a specialized manner for Unity.

This implementation of the pattern defines containers/scopes for gameobjects, scenes and the global scope. It allows the user to get, register or unregister services. Getting of a service is delegated to higher containers if a lower one fails.

This package does not create any extra overhead and uses regular C# objects. It integrates into Unity and initializes itself with the `RuntimeInitializeOnLoadMethod` attribute.

## Installation
Having the necessary scripts on the project is all that is needed. This package is best installed with the Unity Package Manager via the git URL.

1. Open the Unity Package Manager.
2. Select "Add package from git URL..."  
3. Paste `https://github.com/SametHope/the-locator.git#main` as the URL.

### Optional
I also highly recommend installation of the [Unity3D-SerializableInterface](https://github.com/Thundernerd/Unity3D-SerializableInterface) package together with this package. It's installation is a bit more annoying though. If you have NPM and openupm-cli, follow instructions on the repo above, otherwise follow the guide below.

1. Make sure there are no compilation errors on the project.
2. Download the [Installer .unitypackage](https://package-installer.glitch.me/v1/installer/package.openupm.com/net.tnrd.serializableinterface?registry=https://package.openupm.com).
3. Import the whole package. This will trigger the installation of the actual package.

## Usage
This implementation of the service locator pattern has scopes for gameobjects, scenes and the global scope.  

Services must be handled by the user, being manually added and removed.

### Register Services
```cs
// Have the service to register available
var serviceObject = ExampleService as IExampleService;
var serviceObject = GetComponent<IExampleService>();

// Register a service for the gameobject
Locator.Register(serviceObject, targetGO); 

// Register a service for the scene of the gameobject
Locator.Register(serviceObject, targetGO.scene); 

// Register a global service
Locator.Register(serviceObject);

// Try methods will not log errors if the service is already registered
Locator.TryRegister(serviceObject, targetGO); 
Locator.TryRegister(serviceObject, targetGO.scene); 
Locator.TryRegister(serviceObject); 
```

### Unregister Services
```cs
// Unregister a service for the gameobject
Locator.Unregister<IExampleService>(targetGO);

// Unregister a service for the scene of the gameobject
Locator.Unregister<IExampleService>(targetGO.scene);

// Unregister a global service
Locator.Unregister<IExampleService>();

// Try methods will not log errors if the service is already not registered
Locator.TryUnregister<IExampleService>(targetGO);
Locator.TryUnregister<IExampleService>(targetGO.scene);
Locator.TryUnregister<IExampleService>();

// You may also unregister services via objects rather than type
// Type is then inferred
Locator.Unregister(serviceObject, TargetGO);
Locator.Unregister(serviceObject, TargetGO.scene);
Locator.Unregister(serviceObject);
...
```

### Get Services
```cs
// Get a service for the gameobject
// This will check the scene and later the global scope for the service if it is not found for the gameobject
var service = Locator.Get<IExampleService>(targetGO);

// Get a service for the gameobject, consider parents
// This will check parents of the gameobject before the scene and the global scope until the service is found
var service = Locator.Get<IExampleService>(targetGO, true);

// Get a service for the scene of the gameobject
// This will check the global scope if service is not found for the scene
var service = Locator.Get<IExampleService>(targetGO.scene);

// Get a global service
var service = Locator.Get<IExampleService>();

// Try methods will not log errors if the service is not found
Locator.TryGet<IExampleService>(targetGO, out var service);
Locator.TryGet<IExampleService>(targetGO.scene, out var service);
Locator.TryGet<IExampleService>(out var service);
```

### Further configuration
You may modify the way locator is works with some pre-defined fields.  

```cs
public static class Locator
{
  // Setting this field before RuntimeInitializeLoadType.BeforeSplashScreen) will override the implementation
  public static Func<ILocator> GetNewLocator { get; set; }
  // Setting this field will override the way errors are logged
  public static Action<object> LogError { get; set; }
}
```

## Behind The Scenes
Here is a peek at the default locators implementation which is pretty self explanatory. Rest of the package (`Locator` class) can be considered like a wrapper that provides means to access and change the `ILocator` implementation easily.
```cs
public class DefaultLocator : ILocator
{
  // key is gameobject instance id
  private readonly Dictionary<int, Dictionary<Type, object>> _gameObjectContainers;
  // key is scene path 
  private readonly Dictionary<string, Dictionary<Type, object>> _sceneContainers; 
  private readonly Dictionary<Type, object> _globalContainer;
  
  // Implementation of the ILocator
  ...
}
```