using UnityEditor; using UnityEngine; using UnityEditor.SceneManagement; using UnityEngine.SceneManagement;
using UnityEngine.Rendering; using UnityEngine.Rendering.Universal;
[InitializeOnLoad]
public static class SpaceWaveBootstrap {
  static SpaceWaveBootstrap(){ EditorApplication.update += TrySetup; }
  static bool done;
  static void TrySetup(){ if(done) return; done=true; EnsureURP(); EnsureScenes(); ConfigureAndroid(); Debug.Log("SpaceWave: auto-config done."); }
  static void EnsureURP(){
    if(GraphicsSettings.renderPipelineAsset == null){
      var guids = AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset");
      UniversalRenderPipelineAsset urp = null;
      if(guids.Length>0){ urp = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(AssetDatabase.GUIDToAssetPath(guids[0])); }
      else { urp = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>(); AssetDatabase.CreateAsset(urp,"Assets/SpaceWave/URP_Asset.asset"); AssetDatabase.SaveAssets(); }
      GraphicsSettings.renderPipelineAsset = urp; QualitySettings.renderPipeline = urp;
    }
  }
  static void EnsureScenes(){
    System.IO.Directory.CreateDirectory("Assets/SpaceWave/Scenes");
    CreateSceneIfMissing("MainMenu", BuildMenu);
    CreateSceneIfMissing("Game", BuildGame);
    CreateSceneIfMissing("Loading", BuildLoading);
    var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
    void Add(string p){ if(!list.Exists(s=>s.path==p)) list.Add(new EditorBuildSettingsScene(p,true)); }
    Add("Assets/SpaceWave/Scenes/MainMenu.unity");
    Add("Assets/SpaceWave/Scenes/Game.unity");
    Add("Assets/SpaceWave/Scenes/Loading.unity");
    EditorBuildSettings.scenes = list.ToArray();
  }
  static void CreateSceneIfMissing(string name, System.Action<Scene> builder){
    var path=$"Assets/SpaceWave/Scenes/{name}.unity";
    if(!System.IO.File.Exists(path)){ var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); builder(scene); EditorSceneManager.SaveScene(scene,path); }
  }
  static void BuildMenu(Scene scene){
    var camGO=new GameObject("Main Camera"); var cam=camGO.AddComponent<Camera>(); cam.orthographic=true; cam.orthographicSize=5; camGO.tag="MainCamera";
    var canvasGO=new GameObject("Canvas"); var canvas=canvasGO.AddComponent<UnityEngine.Canvas>(); canvas.renderMode=UnityEngine.RenderMode.ScreenSpaceOverlay;
    var scaler=canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>(); scaler.uiScaleMode=UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution=new Vector2(1920,1080);
    canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
    new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
    var title=new GameObject("Title"); title.transform.SetParent(canvasGO.transform);
    var t=title.AddComponent<TMPro.TextMeshProUGUI>(); t.text="SPACEWAVE"; t.fontSize=96; t.alignment=TMPro.TextAlignmentOptions.Center;
    var rt=(RectTransform)title.transform; rt.anchorMin=rt.anchorMax=new Vector2(0.5f,0.8f); rt.sizeDelta=new Vector2(800,200); rt.anchoredPosition=Vector2.zero;
    CreateButton(canvasGO.transform,"Play Endless",new Vector2(0.5f,0.55f), ()=> UnityEditor.EditorApplication.isPlaying=false);
    CreateButton(canvasGO.transform,"Levels",new Vector2(0.5f,0.45f), ()=> {});
    CreateButton(canvasGO.transform,"Shop",new Vector2(0.5f,0.35f), ()=> {});
    CreateButton(canvasGO.transform,"Settings",new Vector2(0.5f,0.25f), ()=> {});
  }
  static void CreateButton(Transform parent,string label,Vector2 anchor,System.Action onClick){
    var go=new GameObject(label); go.transform.SetParent(parent);
    var rt=go.AddComponent<RectTransform>(); rt.anchorMin=rt.anchorMax=anchor; rt.sizeDelta=new Vector2(400,100); rt.anchoredPosition=Vector2.zero;
    var img=go.AddComponent<UnityEngine.UI.Image>(); img.color=new Color(0.1f,0.1f,0.15f,0.85f);
    var btn=go.AddComponent<UnityEngine.UI.Button>();
    var txtGO=new GameObject("Text"); txtGO.transform.SetParent(go.transform);
    var txt=txtGO.AddComponent<TMPro.TextMeshProUGUI>(); txt.text=label; txt.alignment=TMPro.TextAlignmentOptions.Center; txt.fontSize=48;
    var trt=(RectTransform)txtGO.transform; trt.anchorMin=Vector2.zero; trt.anchorMax=Vector2.one; trt.offsetMin=Vector2.zero; trt.offsetMax=Vector2.zero;
    btn.onClick.AddListener(()=> onClick());
  }
  static void BuildGame(Scene scene){
    var camGO=new GameObject("Main Camera"); var cam=camGO.AddComponent<Camera>(); cam.orthographic=true; cam.orthographicSize=5; camGO.tag="MainCamera";
    var player=new GameObject("Player"); player.transform.position=new Vector3(-6f,0,0);
    player.AddComponent<SpaceWave.Gameplay.WaveController>(); player.AddComponent<SpriteRenderer>(); player.AddComponent<BoxCollider2D>();
    var genGO=new GameObject("ObstacleGenerator"); genGO.AddComponent<ObstacleGenerator>(); genGO.AddComponent<AudioManager>();
    var prefab=new GameObject("ObstaclePrefab"); prefab.AddComponent<SpriteRenderer>(); prefab.AddComponent<BoxCollider2D>(); prefab.AddComponent<ProceduralObstacle>();
    UnityEditor.PrefabUtility.SaveAsPrefabAsset(prefab, "Assets/SpaceWave/Prefabs/ObstaclePrefab.prefab"); Object.DestroyImmediate(prefab);
    var gen=genGO.GetComponent<ObstacleGenerator>();
    gen.GetType().GetField("obstaclePrefab", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance)
      .SetValue(gen, AssetDatabase.LoadAssetAtPath<GameObject>("Assets/SpaceWave/Prefabs/ObstaclePrefab.prefab"));
  }
  static void BuildLoading(Scene scene){
    var camGO=new GameObject("Main Camera"); var cam=camGO.AddComponent<Camera>(); cam.orthographic=true; cam.orthographicSize=5; camGO.tag="MainCamera";
    var canvas=new GameObject("Canvas"); canvas.AddComponent<UnityEngine.Canvas>().renderMode=UnityEngine.RenderMode.ScreenSpaceOverlay;
    canvas.AddComponent<UnityEngine.UI.CanvasScaler>(); canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
    var go=new GameObject("LoadingText"); go.transform.SetParent(canvas.transform);
    var txt=go.AddComponent<TMPro.TextMeshProUGUI>(); txt.text="Loading..."; txt.fontSize=72; txt.alignment=TMPro.TextAlignmentOptions.Center;
    var rt=(RectTransform)go.transform; rt.anchorMin=rt.anchorMax=new Vector2(0.5f,0.5f); rt.sizeDelta=new Vector2(600,200); rt.anchoredPosition=Vector2.zero;
  }
  static void ConfigureAndroid(){
    PlayerSettings.defaultIsFullScreen=true;
    PlayerSettings.allowedAutorotateToPortrait=false; PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;
    PlayerSettings.allowedAutorotateToLandscapeLeft=true; PlayerSettings.allowedAutorotateToLandscapeRight=true;
    PlayerSettings.defaultScreenWidth=1920; PlayerSettings.defaultScreenHeight=1080;
    PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
    PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
    PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel24;
#if !UNITY_2023_1_OR_NEWER
    PlayerSettings.Android.targetSdkVersion=AndroidSdkVersions.AndroidApiLevel34;
#endif
    QualitySettings.vSyncCount=0; Debug.Log("Android: IL2CPP ARM64, minSdk 24, targetSdk 34.");
  }
}
