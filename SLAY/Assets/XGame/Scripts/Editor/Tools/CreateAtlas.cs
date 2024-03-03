using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;
using UnityEditor.U2D;
using System.IO;

public class CreateAtlas : Editor
{
    [MenuItem("Assets/CreateAtlas2Res")]
    static void AtlasCreate()
    {
        string path = "";
        if (Selection.assetGUIDs != null && Selection.assetGUIDs.Length == 1)
        {
            path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        }
        if (!path.Contains("UISprites/")) return;
        //创建图集
        var pos = path.LastIndexOf("/");
        var name = path.Substring(pos + 1, path.Length - pos - 1);
        if (name.Contains("/")) return;
        var atlasPath = Path.Combine(path, "..\\..\\Resources\\SpriteAtlas\\");
        // Debug.Log("aaa" + atlasPath);
        // return;
        string atlas = atlasPath + name + ".spriteatlas";
        if (File.Exists(atlas))
        {
            Debug.Log("图集找到" + atlas);
        }
        else
        {
            SpriteAtlas sa = new SpriteAtlas();

            SpriteAtlasPackingSettings packSet = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                padding = 4,
            };
            sa.SetPackingSettings(packSet);


            SpriteAtlasTextureSettings textureSet = new SpriteAtlasTextureSettings()
            {
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
            sa.SetTextureSettings(textureSet);
            AssetDatabase.CreateAsset(sa, atlas);
            //图片的文件夹加入图集。
            Debug.Log("aaaa" + path);
            Object texture = AssetDatabase.LoadMainAssetAtPath(path);
            SpriteAtlasExtensions.Add(sa, new Object[] { texture });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("图集创建:" + atlas);
        }
    }

}
