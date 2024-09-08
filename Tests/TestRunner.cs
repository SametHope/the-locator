using UnityEngine;

public class TestRunner : MonoBehaviour
{
    static bool _allGood = true;
    private void Start()
    {
        TestStatics();
        TestNonExistents();
        TestGeneral();
        TestMultiName();
        TestMultiSameScope();
        PrepareTestStaticsForNextTime();

        if(_allGood)
        {
            Debug.Log("All Tests Passed");
        }
        else
        {
            Debug.LogError("Some Tests Failed");
        }
    }
    private void TestStatics()
    {
        CheckCondition(Locator.Containers_Global.Count == 0, true);
        CheckCondition(Locator.Containers_Scene.Count == 0, true);
        CheckCondition(Locator.Containers_GameObject.Count == 0, true);
    }
    private void PrepareTestStaticsForNextTime()
    {
        Locator.Register(new ExampleService());
        Locator.Register(new ExampleService(), "hey");
        Locator.Register(new ExampleService(), gameObject.scene);
        Locator.Register(new ExampleService(), gameObject.scene, "hey");
        Locator.Register(new ExampleService(), gameObject);
        Locator.Register(new ExampleService(), gameObject, "hey");
    }
    private void TestNonExistents()
    {
        // Get Non-Existent Global
        CheckException(() => Locator.Get<TestRunner>(), true);
        CheckException(() => Locator.Get<TestRunner>("1"), true);

        // Get Non-Existent Scene
        CheckException(() => Locator.Get<TestRunner>(gameObject.scene), true);
        CheckException(() => Locator.Get<TestRunner>(gameObject.scene, "2"), true);

        // Get Non-Existent GameObject
        CheckException(() => Locator.Get<TestRunner>(gameObject), true);
        CheckException(() => Locator.Get<TestRunner>(gameObject, "3"), true);

        // Get Non-Existent Component
        CheckException(() => Locator.Get<TestRunner>(this), true);
        CheckException(() => Locator.Get<TestRunner>(this, "4"), true);


        // Register Non-Existent Global
        CheckException(() => Locator.Register<TestRunner>(null), true);
        CheckException(() => Locator.Register<TestRunner>(null, "5"), true);

        // Register Non-Existent Scene
        CheckException(() => Locator.Register<TestRunner>(null, gameObject.scene), true);
        CheckException(() => Locator.Register<TestRunner>(null, gameObject.scene, "6"), true);

        // Register Non-Existent GameObject
        CheckException(() => Locator.Register<TestRunner>(null, gameObject), true);
        CheckException(() => Locator.Register<TestRunner>(null, gameObject, "7"), true);

        // Register Non-Existent Component
        CheckException(() => Locator.Register<TestRunner>(null, this), true);
        CheckException(() => Locator.Register<TestRunner>(null, this, "8"), true);


        // Unregister Non-Existent Global
        CheckException(() => Locator.Unregister<TestRunner>(), true);
        CheckException(() => Locator.Unregister<TestRunner>("9"), true);
        CheckException(() => Locator.Unregister<TestRunner>(this), true);
        CheckException(() => Locator.Unregister<TestRunner>(this, "10"), true);

        // Unregister Non-Existent Scene
        CheckException(() => Locator.Unregister<TestRunner>(gameObject.scene), true);
        CheckException(() => Locator.Unregister<TestRunner>(gameObject.scene, "11"), true);
        CheckException(() => Locator.Unregister<TestRunner>(this, gameObject.scene), true);
        CheckException(() => Locator.Unregister<TestRunner>(this, gameObject.scene, "12"), true);

        // Unregister Non-Existent GameObject
        CheckException(() => Locator.Unregister<TestRunner>(gameObject), true);
        CheckException(() => Locator.Unregister<TestRunner>(gameObject, "13"), true);
        CheckException(() => Locator.Unregister<TestRunner>(this, gameObject), true);
        CheckException(() => Locator.Unregister<TestRunner>(this, gameObject, "14"), true);

        // Unregister Non-Existent Component
        CheckException(() => Locator.Unregister<TestRunner>(this), true);
        CheckException(() => Locator.Unregister<TestRunner>(this, "15"), true);
        CheckException(() => Locator.Unregister<TestRunner>(this, this), true);
        CheckException(() => Locator.Unregister<TestRunner>(this, this, "16"), true);
    }
    private void TestGeneral()
    {
        var exService = new ExampleService();

        // Test global No Name
        CheckException(() => Locator.Register<ExampleService>(exService), false);
        CheckException(() => Locator.Get<ExampleService>(), false);
        CheckException(() => Locator.Get<ExampleService>(gameObject.scene), false);
        CheckException(() => Locator.Get<ExampleService>(gameObject), false); // ?
        CheckException(() => Locator.Get<ExampleService>(this), false);
        CheckException(() => Locator.Register(exService), true);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject.scene), true);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject), true);
        CheckException(() => Locator.Unregister<ExampleService>(this), true);
        CheckException(() => Locator.Unregister<ExampleService>(), false);
        CheckException(() => Locator.Register(exService), false);
        CheckException(() => Locator.Unregister<ExampleService>(exService), false);

        // Test global With Name
        CheckException(() => Locator.Register<ExampleService>(exService, "hey"), false);
        CheckException(() => Locator.Get<ExampleService>("hey"), false);
        CheckException(() => Locator.Get<ExampleService>(gameObject.scene, "hey"), false);
        CheckException(() => Locator.Get<ExampleService>(gameObject, "hey"), false);
        CheckException(() => Locator.Get<ExampleService>(this, "hey"), false);
        CheckException(() => Locator.Register(exService, "hey"), true);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject.scene, "hey"), true);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject, "hey"), true);
        CheckException(() => Locator.Unregister<ExampleService>(this, "hey"), true);
        CheckException(() => Locator.Unregister<ExampleService>("hey"), false);
        CheckException(() => Locator.Register(exService, "hey"), false);
        CheckException(() => Locator.Unregister<ExampleService>(exService, "hey"), false);

        // Test global mixed
        CheckException(() => Locator.Register<ExampleService>(exService, "hey"), false);
        CheckException(() => Locator.Get<ExampleService>(), true);
        CheckException(() => Locator.Get<ExampleService>("other"), true);
        CheckException(() => Locator.Get<ExampleService>(gameObject.scene, "other"), true);
        CheckException(() => Locator.Get<ExampleService>(gameObject, "other"), true);
        CheckException(() => Locator.Get<ExampleService>(this, "other"), true);
        CheckException(() => Locator.Get<ExampleService>("hey"), false);
        CheckException(() => Locator.Get<ExampleService>(gameObject.scene, "hey"), false);
        CheckException(() => Locator.Get<ExampleService>(gameObject, "hey"), false);
        CheckException(() => Locator.Get<ExampleService>(this, "hey"), false);
        CheckException(() => Locator.Unregister<ExampleService>("hey"), false);

        // Test scene No Name
        CheckException(() => Locator.Register<ExampleService>(exService, gameObject.scene), false);
        CheckException(() => Locator.Get<ExampleService>(gameObject.scene), false);
        CheckException(() => Locator.Get<ExampleService>(gameObject), false);
        CheckException(() => Locator.Get<ExampleService>(this), false);
        CheckException(() => Locator.Register(exService, gameObject.scene), true);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject.scene), false);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject), true);
        CheckException(() => Locator.Unregister<ExampleService>(this), true);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject.scene), true);

        // Test scene With Name and mixed
        CheckException(() => Locator.Register<ExampleService>(exService, gameObject.scene, "hey"), false);
        CheckException(() => Locator.Get<ExampleService>(gameObject.scene, "hey"), false);
        CheckException(() => Locator.Get<ExampleService>(gameObject, "hey"), false);
        CheckException(() => Locator.Get<ExampleService>(this, "hey"), false);
        CheckException(() => Locator.Register(exService, gameObject.scene, "hey"), true);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject.scene, "hey"), false);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject.scene, "hey"), true);
        CheckException(() => Locator.Get<ExampleService>(this), true);
        CheckException(() => Locator.Get<ExampleService>(this, "other"), true);
        CheckException(() => Locator.Get<ExampleService>(gameObject, "other"), true);
        CheckException(() => Locator.Get<ExampleService>(gameObject.scene, "other"), true);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject.scene), true);

        // Test GameObject mixed
        CheckException(() => Locator.Register<ExampleService>(exService, gameObject), false);
        CheckException(() => Locator.Get<ExampleService>(gameObject), false);
        CheckException(() => Locator.Get<ExampleService>(gameObject.scene), true);
        CheckException(() => Locator.Get<ExampleService>(), true);
        CheckException(() => Locator.Register<ExampleService>(exService, gameObject, "hey"), false);
        CheckException(() => Locator.Get<ExampleService>(gameObject, "hey"), false);
        CheckException(() => Locator.Get<ExampleService>(gameObject.scene, "hey"), true);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject), false);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject), true);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject, "hey"), false);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject, "hey"), true);
    }
    private void TestMultiName()
    {
        var a = new ExampleService();
        var b = new ExampleService();
        var c = new ExampleService();

        CheckException(() => Locator.Register(a, "a"), false);
        CheckException(() => Locator.Register(b, "b"), false);
        CheckException(() => Locator.Register(c, "c"), false);

        CheckException(() => Locator.Get<ExampleService>("a"), false);
        CheckException(() => Locator.Get<ExampleService>("b"), false);
        CheckException(() => Locator.Get<ExampleService>("c"), false);

        CheckException(() => Locator.Get<ExampleService>(), true);
        CheckException(() => Locator.Get<ExampleService>("d"), true);

        CheckException(() => Locator.Unregister<ExampleService>("d"), true);
        CheckException(() => Locator.Unregister<ExampleService>(), true);

        CheckCondition(Locator.Containers_Global.Count == 3, true);

        CheckCondition(Locator.Get<ExampleService>("a").R == Locator.Get<ExampleService>("a").R, true);
        CheckCondition(Locator.Get<ExampleService>("b").R == Locator.Get<ExampleService>("b").R, true);
        CheckCondition(Locator.Get<ExampleService>("c").R == Locator.Get<ExampleService>("c").R, true);

        CheckCondition(Locator.Get<ExampleService>("a").R != Locator.Get<ExampleService>("b").R, true);
        CheckCondition(Locator.Get<ExampleService>("a").R != Locator.Get<ExampleService>("c").R, true);

        CheckCondition(Locator.Get<ExampleService>("a").R == Locator.Get<ExampleService>(this, "a").R, true);
        CheckCondition(Locator.Get<ExampleService>(this, "a").R == Locator.Get<ExampleService>(this, "a").R, true);
        CheckCondition(Locator.Get<ExampleService>(gameObject, "a").R == Locator.Get<ExampleService>(this, "a").R, true);
        CheckCondition(Locator.Get<ExampleService>(gameObject.scene, "a").R == Locator.Get<ExampleService>(this, "a").R, true);

        CheckCondition(Locator.Get<ExampleService>("a").R != Locator.Get<ExampleService>(this, "b").R, true);
        CheckCondition(Locator.Get<ExampleService>(this, "a").R != Locator.Get<ExampleService>(this, "b").R, true);
        CheckCondition(Locator.Get<ExampleService>(gameObject, "a").R != Locator.Get<ExampleService>(this, "b").R, true);
        CheckCondition(Locator.Get<ExampleService>(gameObject.scene, "a").R != Locator.Get<ExampleService>(this, "b").R, true);

        CheckCondition(Locator.Get<ExampleService>("a").R != Locator.Get<ExampleService>(this, "c").R, true);
        CheckCondition(Locator.Get<ExampleService>(this, "a").R != Locator.Get<ExampleService>(this, "c").R, true);
        CheckCondition(Locator.Get<ExampleService>(gameObject, "a").R != Locator.Get<ExampleService>(this, "c").R, true);
        CheckCondition(Locator.Get<ExampleService>(gameObject.scene, "a").R != Locator.Get<ExampleService>(this, "c").R, true);


        CheckException(() => Locator.Unregister<ExampleService>("a"), false);
        CheckException(() => Locator.Unregister<ExampleService>("b"), false);
        CheckException(() => Locator.Unregister<ExampleService>("c"), false);


        var d = new ExampleService();
        var e = new ExampleService();
        var f = new ExampleService();
        var g = new ExampleService();

        CheckException(() => Locator.Register(d, this, "d"), false);
        CheckException(() => Locator.Register(e, gameObject, "e"), false);
        CheckException(() => Locator.Register(f, gameObject.scene, "f"), false);
        CheckException(() => Locator.Register(g, "g"), false);

        CheckCondition(Locator.Get<ExampleService>(this, "d").R != Locator.Get<ExampleService>(gameObject, "e").R, true);
        CheckCondition(Locator.Get<ExampleService>(gameObject, "e").R != Locator.Get<ExampleService>(gameObject.scene, "f").R, true);
        CheckCondition(Locator.Get<ExampleService>(gameObject.scene, "f").R != Locator.Get<ExampleService>("g").R, true);

        CheckCondition(Locator.Get<ExampleService>(this, "e").R == Locator.Get<ExampleService>(gameObject, "e").R, true);
        CheckCondition(Locator.Get<ExampleService>(this, "f").R == Locator.Get<ExampleService>(gameObject.scene, "f").R, true);
        CheckCondition(Locator.Get<ExampleService>(this, "g").R == Locator.Get<ExampleService>("g").R, true);

        CheckException(() => Locator.Unregister<ExampleService>(this, "g"), true);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject, "f"), true);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject.scene, "e"), true);
        CheckException(() => Locator.Unregister<ExampleService>("a"), true);

        CheckException(() => Locator.Unregister<ExampleService>(this, "d"), false);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject, "e"), false);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject.scene, "f"), false);
        CheckException(() => Locator.Unregister<ExampleService>("g"), false);

        CheckCondition(Locator.Containers_Global.Count == 0, true);
        CheckCondition(Locator.Containers_Scene.Count == 0, true);
        CheckCondition(Locator.Containers_GameObject.Count == 0, true);
    }
    private void TestMultiSameScope()
    {
        CheckException(() => Locator.Register(new ExampleService(), null), false);
        CheckException(() => Locator.Register(new ExampleService(), (GameObject)null, "x"), true);

        CheckException(() => Locator.Unregister<ExampleService>((GameObject)null, "x"), true);
        CheckException(() => Locator.Unregister<ExampleService>(null), false);

        CheckException(() => Locator.Register(new ExampleService(), (Component)null), true);
        CheckException(() => Locator.Register(new ExampleService(), (Component)null, "x"), true);

        CheckException(() => Locator.Unregister<ExampleService>((Component)null, "x"), true);
        CheckException(() => Locator.Unregister<ExampleService>((Component)null), true);

        CheckException(() => Locator.Register(new ExampleService(), gameObject, "a"), false);
        CheckException(() => Locator.Register(new ExampleService(), gameObject, "b"), false);
        CheckException(() => Locator.Register(new ExampleService(), gameObject, "c"), false);

        CheckException(() => Locator.Register(new ExampleService(), gameObject.scene, "d"), false);
        CheckException(() => Locator.Register(new ExampleService(), gameObject.scene, "e"), false);
        CheckException(() => Locator.Register(new ExampleService(), gameObject.scene, "f"), false);

        CheckException(() => Locator.Unregister<ExampleService>(gameObject, "a"), false);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject, "b"), false);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject, "c"), false);

        CheckException(() => Locator.Unregister<ExampleService>(gameObject.scene, "d"), false);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject.scene, "e"), false);
        CheckException(() => Locator.Unregister<ExampleService>(gameObject.scene, "f"), false);

    }
    public static void CheckException(System.Action action, bool expected)
    {
        bool threw = false;

        try
        {
            action();
        }
        catch
        {
            threw = true;
        }

        if(threw != expected)
        {
            _allGood = false;
            Debug.LogError("Test Failed");
        }
    }

    public static void CheckCondition(bool condition, bool expected)
    {
        if(condition != expected)
        {
            _allGood = false;
            Debug.LogError("Test Failed");
        }
    }
}

public class ExampleService { public float R = Random.value; }