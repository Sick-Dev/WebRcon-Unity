using UnityEngine;
using UnityEngine.UI;
using SickDev.WebRcon.Unity;
using SickDev.CommandSystem;

public class SimpleCalculator : MonoBehaviour {
    [SerializeField]
    InputField ckeyInput;
    [SerializeField]
    Text statusBar;
    [SerializeField]
    Text resultsText;
    [SerializeField]
    Button connectButton;

    void Start() {
        WebRconManager.singleton.onConnected += () => {
            statusBar.text = "Connected! =D\nPlease, type \"/help\" in the web console to see a list of available commands";
            connectButton.enabled = false;
        };
        WebRconManager.singleton.onDisconnected += reason => {
            statusBar.text = "Disconnected";
        };

        WebRconManager.singleton.commandsManager.Add(new FuncCommand<float, float, float>(Sum));
        WebRconManager.singleton.commandsManager.Add(new FuncCommand<float, float, float>(Subtract));
        WebRconManager.singleton.commandsManager.Add(new FuncCommand<float, float, float>(Multiply));
        WebRconManager.singleton.commandsManager.Add(new FuncCommand<float, float, float>(Divide));
        WebRconManager.singleton.commandsManager.Add(new ActionCommand(Clear));
    }

    public void Connect() {
        WebRconManager.singleton.cKey = ckeyInput.text;
        WebRconManager.singleton.Initialize();
    }

    float Sum(float num1, float num2) {
        float result = num1 + num2;
        resultsText.text += string.Format("\n{0} + {1} = {2}", num1, num2, result);
        return result;
    }

    float Subtract(float num1, float num2) {
        float result = num1 - num2;
        resultsText.text += string.Format("\n{0} - {1} = {2}", num1, num2, result);
        return result;
    }

    float Multiply(float num1, float num2) {
        float result = num1 * num2;
        resultsText.text += string.Format("\n{0} * {1} = {2}", num1, num2, result);
        return result;
    }

    float Divide(float num1, float num2) {
        float result = num1 / num2;
        resultsText.text += string.Format("\n{0} / {1} = {2}", num1, num2, result);
        return result;
    }

    void Clear() {
        resultsText.text = string.Empty;
    }
}