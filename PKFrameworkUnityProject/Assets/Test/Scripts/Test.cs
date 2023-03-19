using System.Collections;
using System.Collections.Generic;
using PKFramework.Runtime;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    public AudioClip a;
    public AnimatorController ac;
    public Texture t;
    public GameObject prefab;
    public Font f;
    public Sprite sp;
    
    void Start()
    {
        a = AssetComponent.Instance.LoadAsync<AudioClip>("Test/GameAsset/Audio/scavengers_chop1", new LoadAssetCallbacks());
        ac = AssetComponent.Instance.LoadAsync<AnimatorController>("Test/GameAsset/Animation/AnimatorControllers/Enemy1", new LoadAssetCallbacks());
        t = AssetComponent.Instance.LoadAsync<Texture>("Test/GameAsset/Textures/Scavengers_SpriteSheet", new LoadAssetCallbacks());
        prefab = AssetComponent.Instance.LoadAsync<GameObject>("Test/GameAsset/Prefabs/Exit", new LoadAssetCallbacks());
        f = AssetComponent.Instance.LoadAsync<Font>("Test/GameAsset/Fonts/PressStart2P-Regular", new LoadAssetCallbacks());
        sp = AssetComponent.Instance.LoadAsync<Sprite>("Test/GameAsset/Sprites/Scavengers_SpriteSheet", new LoadAssetCallbacks());
    }
    
    void Update()
    {
        
    }
}
