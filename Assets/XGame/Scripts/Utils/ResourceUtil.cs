using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
namespace XGame
{
    public static class ResourceUtil
    {
        static readonly Dictionary<string, SpriteAtlas> loadedSpriteAtlasDic = new Dictionary<string, SpriteAtlas>();
        static readonly Dictionary<string, Sprite> loadedSpriteDic = new Dictionary<string, Sprite>();

        public static T Load<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }

        public static T Instantiate<T>(string path) where T : Object
        {
            T prefab = Load<T>(path);
            if (prefab == null)
            {
                LogUtil.Err("没有加载到预制体," + path);
                return null;
            }
            return GameObject.Instantiate<T>(prefab);
        }
        public static T Instantiate<T>(Transform p, string path) where T : Object
        {
            T prefab = Load<T>(path);
            if (prefab == null)
            {
                LogUtil.Err("没有加载到预制体," + path);
                return null;
            }
            return GameObject.Instantiate<T>(prefab, p);
        }


        public static Sprite GetSprite(string spriteAtlas_Name, string sprite_name)
        {
            string spritePath = spriteAtlas_Name + "/" + sprite_name;
            if (loadedSpriteDic.TryGetValue(spritePath, out Sprite loadedSprite))
            {
                if (loadedSprite is null)
                {
                    LogUtil.Err("获取精灵图片错误：已加载精灵图片字典中存在该键，但对应的精灵图片为空！精灵图片的路径为：" + spritePath);
                    loadedSpriteDic.Remove(spritePath);
                    return null;
                }
                else
                    return loadedSprite;
            }
            else
            {
                if (loadedSpriteAtlasDic.TryGetValue(spriteAtlas_Name, out SpriteAtlas loadedSpriteAtlas))
                {
                    if (loadedSpriteAtlas is null)
                    {
                        LogUtil.Err("获取精灵图片错误：已经加载的精灵图集字典中存在该图集的键，但对应的图集为空！图集名称为：" + spriteAtlas_Name);
                        loadedSpriteAtlasDic.Remove(spriteAtlas_Name);
                        return null;
                    }
                    else
                    {
                        Sprite targetSprite = loadedSpriteAtlas.GetSprite(sprite_name);
                        if (targetSprite is null)
                        {
                            LogUtil.Err("获取精灵图片错误：精灵图集中不存在该名称的精灵图片！精灵图片的名称为：" + sprite_name);
                            return null;
                        }
                        else
                        {
                            loadedSpriteDic.Add(spritePath, targetSprite);
                            return targetSprite;
                        }
                    }
                }
                else
                {
                    string spriteAtlasPath = "SpriteAtlas/" + spriteAtlas_Name;
                    SpriteAtlas targetSpriteAtlas = Load<SpriteAtlas>(spriteAtlasPath);
                    if (targetSpriteAtlas is null)
                    {
                        LogUtil.Err("获取精灵图片错误：请确认图集的路径是正确的！图集路径：" + spriteAtlasPath);
                        return null;
                    }
                    else
                    {
                        loadedSpriteAtlasDic.Add(spriteAtlas_Name, targetSpriteAtlas);
                        Sprite targetSprite = targetSpriteAtlas.GetSprite(sprite_name);
                        if (targetSprite is null)
                        {
                            LogUtil.Err("获取精灵图片错误：精灵图集中不存在该名称的精灵图片！精灵图片的名称为：" + spriteAtlas_Name + ":" + sprite_name);
                            return null;
                        }
                        else
                        {
                            loadedSpriteDic.Add(spritePath, targetSprite);
                            return targetSprite;
                        }
                    }
                }
            }
        }

        public static string GetEStr(string path)
        {
            return path;

        }
       
    }
}

