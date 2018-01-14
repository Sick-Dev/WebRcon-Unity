using UnityEngine;
using UnityEditor;
using System.Linq;

namespace SickDev.WebRcon.Unity {
    [CustomEditor(typeof(WebRconManager))]
    public class WebRconManagerEditor : Editor {
        GUIContent[] tabsContents = new GUIContent[] {
            new GUIContent("Behaviour"),
            new GUIContent("Built in Commands"),
        };

        int selectedTab;

        public override void OnInspectorGUI() {
            serializedObject.Update();
            DrawScript();
            DrawTabs();
            serializedObject.ApplyModifiedProperties();
        }

        void DrawScript() {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            GUI.enabled = true;
        }

        void DrawTabs() {
            selectedTab = GUILayout.Toolbar(selectedTab, tabsContents);
            switch(selectedTab) {
            case 0:
                DrawBehaviour();
                break;
            case 1:
                DrawBuiltInCommands();
                break;
            }
        }

        void DrawBehaviour() {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoInitialize"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_cKey"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_attachedLogType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("verboseLevel"));
        }

        void DrawBuiltInCommands() {
            SerializedProperty currentCommand = serializedObject.FindProperty("_builtInCommands");
            SerializedProperty[] commands = new SerializedProperty[currentCommand.Copy().CountRemaining()];
            for(int i = 0; currentCommand.Next(true); i++)
                commands[i] = currentCommand.Copy();
            commands = commands.OrderBy(x => x.name).ToArray();

            int firstColumnCount = commands.Length/2;
            SerializedProperty commandToggled = null;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            for(int i = 0; i < commands.Length; i++) {
                if (i == firstColumnCount+1)
                    EditorGUILayout.BeginVertical();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(commands[i], true);
                if(EditorGUI.EndChangeCheck())
                    commandToggled = commands[i];                
                if (i == firstColumnCount)
                    EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            ProcessCommandToggled(commandToggled);
        }

        void ProcessCommandToggled(SerializedProperty command) {}
    }
}