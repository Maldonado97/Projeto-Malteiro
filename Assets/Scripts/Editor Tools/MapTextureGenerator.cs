using UnityEditor;
using UnityEngine;

public class MapTextureGenerator : MonoBehaviour
{
    //[SerializeField] FilePathAttribute pathFolder;
    Camera camera;
    private void Awake()
    {
        camera = GetComponent<Camera>();
    }
    [ContextMenu("Screenshot")]
    void Screenshot()
    {
        TakeScreenshot($"{Application.dataPath}/Map Textures/RawMap.png");
        Debug.Log($"Saved screenshot at {Application.dataPath}");
    }
    void TakeScreenshot(string fullPath)
    {
        if(camera == null)
        {
            camera = GetComponent<Camera>();
            Debug.Log("Map texture generator aquired Camera");
        }

        //RenderTexture rt = new RenderTexture(256, 256, 0);
        RenderTexture rt = new RenderTexture(256, 256, 0);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        camera.targetTexture = null;

        if (Application.isEditor)
        {
            DestroyImmediate(rt);
        }
        else
        {
            Destroy(rt);
        }

        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}
