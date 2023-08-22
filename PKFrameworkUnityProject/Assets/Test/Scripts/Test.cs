using System.Collections;
using System.Collections.Generic;
using PKFramework.Runtime;
using PKFramework.Runtime.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestBase
{
    
}

public class Test1 : TestBase
{
}

public class Test2 : TestBase
{
}

public class Test : MonoBehaviour
{
    public AudioClip a;
    public Texture t;
    public GameObject prefab;
    public Font f;
    public Sprite sp;
    
    void Start()
    {
        a = AssetComponent.Instance.LoadAsync<AudioClip>("Test/GameAsset/Audio/scavengers_chop1", new LoadAssetCallbacks());
        t = AssetComponent.Instance.LoadAsync<Texture>("Test/GameAsset/Textures/Scavengers_SpriteSheet", new LoadAssetCallbacks());
        prefab = AssetComponent.Instance.LoadAsync<GameObject>("Test/GameAsset/Prefabs/Exit", new LoadAssetCallbacks());
        f = AssetComponent.Instance.LoadAsync<Font>("Test/GameAsset/Fonts/PressStart2P-Regular", new LoadAssetCallbacks());
        sp = AssetComponent.Instance.LoadAsync<Sprite>("Test/GameAsset/Sprites/Scavengers_SpriteSheet", new LoadAssetCallbacks());

        TestBase tb = new TestBase();
        Test1 t1 = new Test1();
        Test2 t2 = new Test2();
        TestMethod(tb);
        TestMethod(t1);
        TestMethod(t2);

        UIManager.Instance.OpenSync<MainWindow>();
        UIManager.Instance.OpenSync<Window1>();
        UIManager.Instance.OpenSync<Window2>();
        WWW w = new WWW("");
    }

    void TestMethod(TestBase tb)
    {
        Debug.Log(tb.GetType());
    }

    private float timer = 0;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.5f)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                timer = 0;
                UIManager.Instance.CloseTopWindow(UILayer.CommonUI);
            }
        }

        
    }
}
