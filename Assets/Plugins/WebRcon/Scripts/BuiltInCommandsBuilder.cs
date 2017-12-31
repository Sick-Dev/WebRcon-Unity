using SickDev.CommandSystem;

namespace SickDev.WebRcon.Unity {
    public class BuiltInCommandsBuilder : CommandSystem.Unity.BuiltInCommandsBuilder {
        public BuiltInCommandsBuilder(CommandsManager manager) : base(manager) { }

        public override void Build() {
            int commandsBefore = manager.GetCommands().Length;

            Analytics();
            PerformanceReporting();
            AndroidInput();
            Animator();
            AppleReplayKit();
            AppleTvRemote();
            Application();
            AudioListener();
            AudioSettings();
            AudioSource();
            Caching();
            Camera();
            Canvas();
            Color();
            Color32();
            ColorUtility();
            CrashReport();
            CrashReportHandler();
            Cursor();
            Debug();
            PlayerConnection();
            Display();
            DynamicGI();
            Font();
            GameObject();
            Hash128();
            Handheld();
            HumanTrait();
            Input();
            Compass();
            Gyroscope();
            LocationService();
            IOSDevice();
            IOSNotificationServices();
            IOSOnDemandResources();
            LayerMask();
            LightmapSettings();
            LightProbeProxyVolume();
            LODGroup();
            MasterServer();
            Mathf();
            Microphone();
            Physics();
            //Physics2D();
            //PlayerPrefs();
            //ProceduralMaterial();
            //Profiler();
            //QualitySettings();
            //Quaternion();
            //Random();
            //Rect();
            //ReflectionProbe();
            //RemoteSettings();
            //GraphicsSettings();
            //RenderSettings();
            //SamsungTV();
            //SceneManager();
            //SceneUtility();
            //Screen();
            //Shader();
            //SortingLayer();
            //SystemInfo();
            //Texture();
            //Time();
            //TouchScreenKeyboard();
            //Vector2();
            //Vector3();
            //Vector4();
            //VRInputTracking();
            //VRDevice();
            //VRSettings();

            int commandsAfter = manager.GetCommands().Length;
            UnityEngine.Debug.Log("WebRcon: Loaded " + (commandsAfter - commandsBefore) + " built-in commands");
        }
    }
}
