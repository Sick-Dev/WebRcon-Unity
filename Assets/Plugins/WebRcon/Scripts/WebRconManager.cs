using System;
using System.Collections.Generic;
using UnityEngine;
using SickDev.CommandSystem;

namespace SickDev.WebRcon.Unity {
    public sealed class WebRconManager : MonoBehaviour {
        enum VerboseLevel { None = 0, OnlyError, Normal, Extended}

        static WebRconManager _prefab;
        static WebRconManager prefab {
            get {
                if(_prefab == null)
                    _prefab = Resources.Load<WebRconManager>(typeof(WebRconManager).Name);
                return _prefab;
            }
        }

        static WebRconManager _singleton;
        public static WebRconManager singleton {
            get {
                if(_singleton == null)
                    Instantiate();
                return _singleton;
            }
        }

        [SerializeField]
        int selectedTab;
        [SerializeField]
        bool autoInitialize;
        [SerializeField]
        string _cKey;
        [SerializeField, EnumFlags]
        LogType _attachedLogType = (LogType) (-1);
        [SerializeField]
        VerboseLevel verboseLevel = VerboseLevel.Normal;
        [SerializeField]
        BuiltInCommandsBuilder.BuiltInCommandsPreferences _builtInCommands;

        Buffer buffer;
        public event Action onLinked;
        public event Action onUnlinked;
        public event OnDisconnectedHandler onDisconnected;
        public event OnInnerExceptionThrownHandler onInnerExceptionThrown;
        public event OnErrorHandler onError;
        public event OnCommandHandler onCommand;

        public WebConsole console { get; private set; }

        public CommandsManager commandsManager { get { return console.commandsManager; } }
        public BuiltInCommandsBuilder.BuiltInCommandsPreferences builtInCommands { get { return _builtInCommands; } }

        public string cKey {
            get { return _cKey; }
            set { _cKey = value; }
        }

        public LogType attachedLogType {
            get { return _attachedLogType; }
            set { _attachedLogType = value; }
        }

        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeOnLoad() {
            if(!prefab.autoInitialize)
                return;
            Instantiate();
            singleton.Initialize();
        }

        static void Instantiate() {
            _singleton = FindObjectOfType<WebRconManager>();
            if(_singleton == null)
                _singleton = Instantiate(prefab);
        }

        void Awake() {
            if(singleton != this) {
                Debug.LogWarning("There can only exist one instance of "+typeof(WebRconManager).Name);
                DestroyImmediate(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);

            CreateWebConsole();
            SetupHandlers();
            ResetBuffers();
        }

        void CreateWebConsole() {
            console = new WebConsole(cKey, new Configuration(
                Application.platform != RuntimePlatform.WebGLPlayer,
                "Assembly-CSharp-firstpass",
                "Assembly-CSharp"
            ));
        }

        void SetupHandlers() {
            console.onLinked += OnLinked;
            console.onUnlinked += OnUnlinked;
            console.onDisconnected += OnDisconnected;
            console.onInnerExceptionThrown += OnInternalExceptionThrown;
            console.onError += OnError;
            console.onCommand += OnCommand;
            Application.logMessageReceivedThreaded += OnLogMessageReceived;
        }

        void OnLinked() {
            buffer.justLinked = true;
        }

        void OnUnlinked() {
            buffer.justUnlinked = true;
        }

        void OnDisconnected(ErrorCode error) {
            buffer.disconnectedError = error;
        }

        void OnInternalExceptionThrown(Exception exception) {
            Debug.LogError("Inner Exception: " + exception.ToString());
            buffer.innerExceptionBuffer.Add(exception);
        }

        void OnError(ErrorCode error) {
            buffer.errorBuffer.Add(error);
        }

        void OnCommand(CommandMessage command) {
            buffer.commandBuffer.Add(command);
        }

        public void Initialize() {
            if(string.IsNullOrEmpty(cKey))
                throw new ArgumentException("cKey is null or empty. Please, ensure to use a valid cKey");
            ResetBuffers();
            console.cKey = cKey;
            console.Initialize();
        }

        void ResetBuffers() {
            buffer = new Buffer();
        }

        void Update() {
            ProcessOnLinked();
            ProcessOnUnlinked();
            ProcessOnDisconnected();
            ProcessOnInnerExceptionThrown();
            ProcessOnError();
            ProcessOnCommand();
        }

        void ProcessOnLinked() {
            if(buffer.justLinked) {
                buffer.justLinked = false;
                if (verboseLevel >= VerboseLevel.Normal)
                    Debug.Log("WebRcon: Linked successfully");
                new BuiltInCommandsBuilder(this).Build();
                if(onLinked != null)
                    onLinked();
            }
        }

        void ProcessOnUnlinked() {
            if(buffer.justUnlinked) {
                buffer.justUnlinked = false;
                if (verboseLevel >= VerboseLevel.Normal)
                    Debug.LogWarning("WebRcon: Unlinked");
                if(onUnlinked != null)
                    onUnlinked();
            }
        }

        void ProcessOnDisconnected() {
            if(buffer.disconnectedError != null) {
                ErrorCode code = buffer.disconnectedError.Value;
                buffer.disconnectedError = null;
                if (verboseLevel >= VerboseLevel.Normal)
                    Debug.LogWarning("WebRcon: The connection was closed (Code: "+code.ToString()+")");
                if(onDisconnected != null)
                    onDisconnected(code);
            }
        }

        void ProcessOnInnerExceptionThrown() {
            for(int i = 0; i < buffer.innerExceptionBuffer.Count; i++) {
                Exception exception = buffer.innerExceptionBuffer[i];
                if (verboseLevel >= VerboseLevel.OnlyError)
                    Debug.LogError("WebRcon: "+exception.Message + "\n" + exception.StackTrace);
                if(onInnerExceptionThrown != null)
                    onInnerExceptionThrown(exception);
            }
            buffer.innerExceptionBuffer.Clear();
        }

        void ProcessOnError() {
            for(int i = 0; i < buffer.errorBuffer.Count; i++) {
                if (verboseLevel >= VerboseLevel.OnlyError)
                    Debug.LogError("WebRcon: Error (Code: "+buffer.errorBuffer[i].ToString()+")");
                if(onError != null)
                    onError(buffer.errorBuffer[i]);
            }
            buffer.errorBuffer.Clear();
        }

        void ProcessOnCommand() {
            for(int i = 0; i < buffer.commandBuffer.Count; i++) {
                if (verboseLevel >= VerboseLevel.Extended)
                    Debug.Log("WebRcon: Command processed: \""+buffer.commandBuffer[i].parsedCommand.raw+"\"");
                if(onCommand != null)
                        onCommand(buffer.commandBuffer[i]);
                console.ExecuteCommand(buffer.commandBuffer[i]);
            }
            buffer.commandBuffer.Clear();
        }

        void OnLogMessageReceived(string condition, string stackTrace, LogType type) {
            if(!console.isLinked)
                return;

            if((attachedLogType & type) == type) {
                string text = condition + "\n" + stackTrace;
                console.defaultTab.Log(text);
            }
        }

        public void Close() {
            console.Close();
        }

        void OnDestroy() {
            Application.logMessageReceivedThreaded -= OnLogMessageReceived;
            Close();
        }

        class Buffer {
            public bool justLinked;
            public bool justUnlinked;
            public ErrorCode? disconnectedError;
            public List<Exception> innerExceptionBuffer = new List<Exception>();
            public List<ErrorCode> errorBuffer = new List<ErrorCode>();
            public List<CommandMessage> commandBuffer = new List<CommandMessage>();
        }
    }
}