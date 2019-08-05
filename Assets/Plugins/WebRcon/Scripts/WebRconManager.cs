using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using SickDev.CommandSystem;

namespace SickDev.WebRcon.Unity {
    public sealed class WebRconManager : MonoBehaviour {
        public class WebConsoleUnity : WebConsole {
            public override string pluginApi { get { return "Unity"; } }

            public WebConsoleUnity(string cKey, Configuration commandSystemConfiguration) : base(cKey, commandSystemConfiguration) { }
        }

        public enum VerboseLevel { None = 0, OnlyError, Normal, Extended}

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
        VerboseLevel _verboseLevel = VerboseLevel.Normal;
        [SerializeField]
        bool _showStackTrace;
        [SerializeField]
        BuiltInCommandsBuilder.BuiltInCommandsPreferences _builtInCommands;
        [SerializeField][HideInInspector]
        Buffer buffer;

        public event Action onConnected;
        public event OnDisconnectedHandler onDisconnected;
        public event OnExceptionThrownHandler onExceptionThrown;
        public event OnErrorHandler onError;

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

        public VerboseLevel verboseLevel {
            get { return _verboseLevel; }
            set { _verboseLevel = value; }
        }

        public bool showStackTrace {
            get { return _showStackTrace; }
            set { _showStackTrace = value; }
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
            console = new WebConsoleUnity(cKey, new Configuration(
                Application.platform != RuntimePlatform.WebGLPlayer,
                "Assembly-CSharp-firstpass",
                "Assembly-CSharp"
            ));
        }

        void SetupHandlers() {
            console.onConnected += OnConnected;
            console.onDisconnected += OnDisconnected;
            console.onExceptionThrown += OnExceptionThrown;
            console.onError += OnError;
            console.onCommand += OnCommand;
            Application.logMessageReceivedThreaded += OnLogMessageReceived;
        }

        void OnConnected() {
            buffer.justConnected = true;
        }

        void OnDisconnected(ErrorCode error) {
            buffer.disconnectedError = error;
        }

        void OnExceptionThrown(Exception exception) {
            Debug.LogError("Exception: " + exception.ToString());
            buffer.exceptionBuffer.Add(exception);
        }

        void OnError(ErrorCode error) {
            buffer.errorBuffer.Add(error);
        }

        void OnCommand(CommandMessage command) {
            buffer.commandBuffer.Add(command);
        }

        public void Initialize() {
            Initialize(cKey);
        }

        public void Initialize(string cKey) {
            if (console.isInitialized)
                throw new AlreadyInitializedException();
            if(string.IsNullOrEmpty(cKey))
                throw new ArgumentException("cKey is null or empty. Please, ensure to use a valid cKey");
            ResetBuffers();
            console.cKey = cKey;
            console.Initialize("test.webrcon.com");
        }

        void ResetBuffers() {
            buffer = new Buffer();
        }

        void Update() {
            ProcessOnConnected();
            ProcessOnDisconnected();
            ProcessOnExceptionThrown();
            ProcessOnError();
            ProcessOnCommand();
        }

        void ProcessOnConnected() {
            if(buffer.justConnected) {
                buffer.justConnected = false;
                if (verboseLevel >= VerboseLevel.Normal)
                    Debug.Log("WebRcon: Connected successfully");
                new BuiltInCommandsBuilder(this).Build();
                if(onConnected != null)
                    onConnected();
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

        void ProcessOnExceptionThrown() {
            for(int i = 0; i < buffer.exceptionBuffer.Count; i++) {
                Exception exception = buffer.exceptionBuffer[i];
                if (verboseLevel >= VerboseLevel.OnlyError)
                    Debug.LogError("WebRcon: "+exception.Message + "\n" + exception.StackTrace);
                if(onExceptionThrown != null)
                    onExceptionThrown(exception);
            }
            buffer.exceptionBuffer.Clear();
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
                CommandMessage command = buffer.commandBuffer[i];
                Tab tab = console.GetTab(command.tabId);
                CommandExecuter executer = commandsManager.GetCommandExecuter(command.parsedCommand);

                if (!executer.isValidCommand) {
                    if (verboseLevel >= VerboseLevel.Normal)
                        tab.Log("The Command \"" + command.parsedCommand.command + "\" does not exist");
                }
                else if (!executer.canBeExecuted) {
                    if (verboseLevel >= VerboseLevel.Normal)
                        tab.Log("The Command \"" + command.parsedCommand.raw + "\" does not match any of its overloads");
                }
                else {
                    if (verboseLevel >= VerboseLevel.Extended)
                        tab.Log("Command processed");
                    try {
                        object result = executer.Execute();
                        if (executer.hasReturnValue) {
                            string resultString = ConvertCommandResultToString(result);
                            console.GetTab(command.tabId).Log("Return value: " +resultString);
                        }
                    }
                    catch (CommandSystemException exception) {
                        if (verboseLevel >= VerboseLevel.OnlyError)
                            tab.Log(exception.ToString());
                    }
                }
            }
            buffer.commandBuffer.Clear();
        }

        string ConvertCommandResultToString(object result) {
            if (result == null)
                return "null";
            else if (result is Array) {
                Array resultArray = (Array)result;
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < resultArray.Length; i++) {
                    builder.Append(resultArray.GetValue(i).ToString());
                    if (i < resultArray.Length - 1)
                        builder.Append(", ");
                }
                return builder.ToString();
            }
            else
                return result.ToString();
        }

        void OnLogMessageReceived(string condition, string stackTrace, LogType type) {
            if(!console.isConnected)
                return;

            if((attachedLogType & type) == type) {
                string text = condition;
                if (showStackTrace)
                    text += "\n" + stackTrace.Remove(stackTrace.Length-1);//Remove last break line
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

        [Serializable] 
        class Buffer {
            public bool justConnected;
            public ErrorCode? disconnectedError;
            public List<Exception> exceptionBuffer = new List<Exception>();
            public List<ErrorCode> errorBuffer = new List<ErrorCode>();
            public List<CommandMessage> commandBuffer = new List<CommandMessage>();
        }
    }
}