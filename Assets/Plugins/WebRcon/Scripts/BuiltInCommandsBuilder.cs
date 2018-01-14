using System;

namespace SickDev.WebRcon.Unity {
    public class BuiltInCommandsBuilder : CommandSystem.Unity.BuiltInCommandsBuilder {
        WebRconManager webRconManager;

        public BuiltInCommandsBuilder(WebRconManager manager) : base(manager.commandsManager) {
            webRconManager = manager;
        }

        public override void Build() {
            int commandsBefore = manager.GetCommands().Length;

            if(webRconManager.builtInCommands.analytics)
                Analytics();
            if(webRconManager.builtInCommands.performanceReporting)
                PerformanceReporting();
            if(webRconManager.builtInCommands.androidInput)
                AndroidInput();
            if(webRconManager.builtInCommands.animator)
                Animator();
            if(webRconManager.builtInCommands.appleReplayKit)
                AppleReplayKit();
            if(webRconManager.builtInCommands.appleTvRemote)
                AppleTvRemote();
            if(webRconManager.builtInCommands.application)
                Application();
            if(webRconManager.builtInCommands.audioListener)
                AudioListener();
            if(webRconManager.builtInCommands.audioSettings)
                AudioSettings();
            if(webRconManager.builtInCommands.audioSource)
                AudioSource();
            if(webRconManager.builtInCommands.caching)
                Caching();
            if(webRconManager.builtInCommands.camera)
                Camera();
            if(webRconManager.builtInCommands.canvas)
                Canvas();
            if(webRconManager.builtInCommands.color)
                Color();
            if(webRconManager.builtInCommands.color32)
                Color32();
            if(webRconManager.builtInCommands.colorUtility)
                ColorUtility();
            if(webRconManager.builtInCommands.crashReport)
                CrashReport();
            if(webRconManager.builtInCommands.crashReportHandler)
                CrashReportHandler();
            if(webRconManager.builtInCommands.cursor)
                Cursor();
            if(webRconManager.builtInCommands.debug)
                Debug();
            if(webRconManager.builtInCommands.playerConnection)
                PlayerConnection();
            if(webRconManager.builtInCommands.display)
                Display();
            if(webRconManager.builtInCommands.dynamicGI)
                DynamicGI();
            if(webRconManager.builtInCommands.font)
                Font();
            if(webRconManager.builtInCommands.gameObject)
                GameObject();
            if(webRconManager.builtInCommands.hash128)
                Hash128();
            if(webRconManager.builtInCommands.handheld)
                Handheld();
            if(webRconManager.builtInCommands.humanTrait)
                HumanTrait();
            if(webRconManager.builtInCommands.input)
                Input();
            if(webRconManager.builtInCommands.compass)
                Compass();
            if(webRconManager.builtInCommands.gyroscope)
                Gyroscope();
            if(webRconManager.builtInCommands.locationService)
                LocationService();
            if(webRconManager.builtInCommands.iOSDevice)
                IOSDevice();
            if(webRconManager.builtInCommands.iOSNotificationServices)
                IOSNotificationServices();
            if(webRconManager.builtInCommands.iOSOnDemandResources)
                IOSOnDemandResources();
            if(webRconManager.builtInCommands.layerMask)
                LayerMask();
            if(webRconManager.builtInCommands.lightmapSettings)
                LightmapSettings();
            if(webRconManager.builtInCommands.lightProbeProxyVolume)
                LightProbeProxyVolume();
            if(webRconManager.builtInCommands.lODGroup)
                LODGroup();
            if(webRconManager.builtInCommands.masterServer)
                MasterServer();
            if(webRconManager.builtInCommands.mathf)
                Mathf();
            if(webRconManager.builtInCommands.microphone)
                Microphone();
            if(webRconManager.builtInCommands.physics)
                Physics();
            if(webRconManager.builtInCommands.physics2D)
                Physics2D();
            if(webRconManager.builtInCommands.playerPrefs)
                PlayerPrefs();
            if(webRconManager.builtInCommands.proceduralMaterial)
                ProceduralMaterial();
            if(webRconManager.builtInCommands.profiler)
                Profiler();
            if(webRconManager.builtInCommands.qualitySettings)
                QualitySettings();
            if(webRconManager.builtInCommands.quaternion)
                Quaternion();
            if(webRconManager.builtInCommands.random)
                Random();
            if(webRconManager.builtInCommands.rect)
                Rect();
            if(webRconManager.builtInCommands.reflectionProbe)
                ReflectionProbe();
            if(webRconManager.builtInCommands.remoteSettings)
                RemoteSettings();
            if(webRconManager.builtInCommands.graphicsSettings)
                GraphicsSettings();
            if(webRconManager.builtInCommands.renderSettings)
                RenderSettings();
            if(webRconManager.builtInCommands.samsungTV)
                SamsungTV();
            if(webRconManager.builtInCommands.sceneManager)
                SceneManager();
            if(webRconManager.builtInCommands.sceneUtility)
                SceneUtility();
            if(webRconManager.builtInCommands.screen)
                Screen();
            if(webRconManager.builtInCommands.shader)
                Shader();
            if(webRconManager.builtInCommands.sortingLayer)
                SortingLayer();
            if(webRconManager.builtInCommands.systemInfo)
                SystemInfo();
            if(webRconManager.builtInCommands.texture)
                Texture();
            if(webRconManager.builtInCommands.time)
                Time();
            if(webRconManager.builtInCommands.touchScreenKeyboard)
                TouchScreenKeyboard();
            if(webRconManager.builtInCommands.vector2)
                Vector2();
            if(webRconManager.builtInCommands.vector3)
                Vector3();
            if(webRconManager.builtInCommands.vector4)
                Vector4();
            if(webRconManager.builtInCommands.vRInputTracking)
                VRInputTracking();
            if(webRconManager.builtInCommands.vRDevice)
                VRDevice();
            if(webRconManager.builtInCommands.vRSettings)
                VRSettings();

            int commandsAfter = manager.GetCommands().Length;
            UnityEngine.Debug.Log("WebRcon: Loaded " + (commandsAfter - commandsBefore) + " built-in commands");
        }

        [Serializable]
        public class BuiltInCommandsPreferences {
            public bool analytics = true;
            public bool performanceReporting = true;
            public bool androidInput = true;
            public bool animator = true;
            public bool appleReplayKit = true;
            public bool appleTvRemote = true;
            public bool application = true;
            public bool audioListener = true;
            public bool audioSettings = true;
            public bool audioSource = true;
            public bool caching = true;
            public bool camera = true;
            public bool canvas = true;
            public bool color = true;
            public bool color32 = true;
            public bool colorUtility = true;
            public bool crashReport = true;
            public bool crashReportHandler = true;
            public bool cursor = true;
            public bool debug = true;
            public bool playerConnection = true;
            public bool display = true;
            public bool dynamicGI = true;
            public bool font = true;
            public bool gameObject = true;
            public bool hash128 = true;
            public bool handheld = true;
            public bool humanTrait = true;
            public bool input = true;
            public bool compass = true;
            public bool gyroscope = true;
            public bool locationService = true;
            public bool iOSDevice = true;
            public bool iOSNotificationServices = true;
            public bool iOSOnDemandResources = true;
            public bool layerMask = true;
            public bool lightmapSettings = true;
            public bool lightProbeProxyVolume = true;
            public bool lODGroup = true;
            public bool masterServer = true;
            public bool mathf = true;
            public bool microphone = true;
            public bool physics = true;
            public bool physics2D = true;
            public bool playerPrefs = true;
            public bool proceduralMaterial = true;
            public bool profiler = true;
            public bool qualitySettings = true;
            public bool quaternion = true;
            public bool random = true;
            public bool rect = true;
            public bool reflectionProbe = true;
            public bool remoteSettings = true;
            public bool graphicsSettings = true;
            public bool renderSettings = true;
            public bool samsungTV = true;
            public bool sceneManager = true;
            public bool sceneUtility = true;
            public bool screen = true;
            public bool shader = true;
            public bool sortingLayer = true;
            public bool systemInfo = true;
            public bool texture = true;
            public bool time = true;
            public bool touchScreenKeyboard = true;
            public bool vector2 = true;
            public bool vector3 = true;
            public bool vector4 = true;
            public bool vRInputTracking = true;
            public bool vRDevice = true;
            public bool vRSettings = true;
        }
    }
}
