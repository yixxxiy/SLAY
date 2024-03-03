using UnityEngine;

#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Collections.Generic;
#endif
using System.IO;

public static class XCodePostProcess
{
#if UNITY_IOS
    static string[] adAccounts = new string[]
    {
        "4pfyvq9l8r.skadnetwork",
        "su67r6k2v3.skadnetwork",
        "cstr6suwn9.skadnetwork",
        "ludvb6z3bs.skadnetwork",
        "f38h382jlk.skadnetwork",
        "v9wttpbfk9.skadnetwork",
        "n38lu8286q.skadnetwork",
        "9g2aggbj52.skadnetwork",
        "nu4557a4je.skadnetwork",
        "wzmmz9fp6w.skadnetwork",
        "v4nxqhlyqp.skadnetwork",
        "22mmun2rn5.skadnetwork",
        "238da6jt44.skadnetwork",
        "424m5254lk.skadnetwork",
        "ecpz2srf59.skadnetwork",
        "4dzt52r2t5.skadnetwork",
        "gta9lk7p23.skadnetwork"
    };
    [PostProcessBuild(100)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS)
        {
            Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
            return;
        }

        //得到xcode工程的路径
        string path = Path.GetFullPath(pathToBuiltProject);

        string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));
        string targetGUID = proj.GetUnityMainTargetGuid();

        proj.SetBuildProperty(targetGUID, "ENABLE_BITCODE", "NO");
        proj.SetBuildProperty(targetGUID, "ARCHS", "arm64");
        proj.SetBuildProperty(targetGUID, "VALID_ARCHS", "arm64");
        proj.SetBuildProperty(targetGUID, "GCC_OPTIMIZATION_LEVEL", "s");
        proj.SetBuildProperty(targetGUID, "DEBUG_INFORMATION_FORMAT", "dwarf-with-dsym");


        string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        plist.root.SetBoolean("Application has localized display name", true);
        if (!plist.root.values.ContainsKey("Privacy - Tracking Usage Description"))
            plist.root.values.Add("Privacy - Tracking Usage Description", new PlistElementString("Your data will be used to provide you a better and personalized ad experience."));
        else
            plist.root.SetString("Privacy - Tracking Usage Description", "Your data will be used to provide you a better and personalized ad experience.");
        if(!plist.root.values.ContainsKey("SKAdNetworkItems"))
            plist.root.CreateArray("SKAdNetworkItems");
        PlistElementArray adAccountArray = plist.root.values["SKAdNetworkItems"].AsArray();
        List<PlistElement> adAccountList = adAccountArray.values;
        foreach(string needAddAdAccount in adAccounts)
        {
            bool hasAdd = false;
            foreach (PlistElementDict element in adAccountList)
            {
                if(needAddAdAccount== element["SKAdNetworkIdentifier"].AsString())
                {
                    hasAdd = true;
                    break;
                }
            }
            if (!hasAdd)
            {
                PlistElementDict dict = new PlistElementDict();
                dict.SetString("SKAdNetworkIdentifier", needAddAdAccount);
                adAccountList.Add(dict);
            }
        }
        adAccountArray.values = adAccountList;
        plist.WriteToFile(plistPath);

        //string unityappControllerPath = pathToBuiltProject + "/Classes/UnityAppController.mm";
        //XClass UnityAppController = new XClass(unityappControllerPath);
        //UnityAppController.WriteBelow("#import \"UnityAppController.h\"", "#include \"FBAudienceNetwork/FBAdSettings.h\"");
        //广告追踪
        //UnityAppController.WriteBelow("- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions\n{\n", "\t[FBAdSettings setAdvertiserTrackingEnabled:YES];\n");

        File.WriteAllText(projPath, proj.WriteToString());

    }
#endif
}

public partial class XClass : System.IDisposable
{

    private string filePath;

    public XClass(string fPath)
    {
        filePath = fPath;
        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogError(filePath + "路径下文件不存在");
            return;
        }
    }


    public void WriteBelow(string below, string text)
    {
        StreamReader streamReader = new StreamReader(filePath);
        string text_all = streamReader.ReadToEnd();
        streamReader.Close();

        int beginIndex = text_all.IndexOf(below);
        if (beginIndex == -1)
        {
            Debug.LogError(filePath + "中没有找到标致" + below);
            return;
        }

        int endIndex = text_all.LastIndexOf("\n", beginIndex + below.Length);

        text_all = text_all.Substring(0, endIndex) + "\n" + text + "\n" + text_all.Substring(endIndex);

        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(text_all);
        streamWriter.Close();
    }

    public void WriteAbove(string above, string text)
    {
        StreamReader streamReader = new StreamReader(filePath);
        string text_all = streamReader.ReadToEnd();
        streamReader.Close();

        int beginIndex = text_all.IndexOf(above);
        if (beginIndex == -1)
        {
            Debug.LogError(filePath + "中没有找到标致" + above);
            return;
        }

        //int endIndex = text_all.LastIndexOf("\n", beginIndex + above.Length);

        text_all = text_all.Substring(0, beginIndex) + "\n" + text + "\n" + text_all.Substring(beginIndex);

        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(text_all);
        streamWriter.Close();
    }

    public void Replace(string below, string newText)
    {
        StreamReader streamReader = new StreamReader(filePath);
        string text_all = streamReader.ReadToEnd();
        streamReader.Close();

        int beginIndex = text_all.IndexOf(below);
        if (beginIndex == -1)
        {
            Debug.LogError(filePath + "中没有找到标致" + below);
            return;
        }

        text_all = text_all.Replace(below, newText);
        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(text_all);
        streamWriter.Close();

    }

    public void Dispose()
    {

    }
}