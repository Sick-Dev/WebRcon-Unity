**WebRcon-Unity** is the official Unity plugin to work with [WebRcon](http://www.webrcon.com/).
This plugin works as an extension of the official [WebRcon-CSharp](https://github.com/Sick-Dev/WebRcon-CSharp) plugin, and uses it as its core.
Hence, this is just an adaptation to be used together with Unity. For a more in-depth guide, please, refer to this [readme](https://github.com/Sick-Dev/WebRcon-CSharp).

1. [Requirements](#requirements)
2. [Installation](#installation)
3. [Usage](#usage)
4. [FAQ](#faq)
5. [License](#license)
6. [About](#about)

# Requirements
In order to use this plugin, you first need:
- Unity 5.6 or higher

# Installation
To use this plugin, you can either download the **Unity package** or copy the contents of [Plugins folder](/Assets/) into your Unity project.

# Usage
## Initialization
The folder _WebRcon/Resources/_ contains the prefab _WebRconManager_.
![WebRconManager Inspector](screenshots/Screenshots/WebRconManager-Inspector.jpg?raw=true)
When the "Auto Initialize" property is checked, it will be automatically instantiated in your scene when hitting play.
The only thing you have to worry about is setting your ckey in the inspector.

If You want to have full control of its initialization, then the following code will suffice:
```C#
using SickDev.WebRcon.Unity;
...
WebRconManager.singleton.cKey = "0M6EQVX8PI"; //Only if you haven't set it in the inspector
WebRconManager.singleton.Initialize();
WebRconManager.singleton.onLinked += () => {
    //Your code
};
```

## Logging messages to WebConsole
```C#
WebRconManager.singleton.console.defaultTab.Log("Hello World!");
```
The property "WebRconManager.console" references the same WebConsole used by the core [WebRcon-CSharp](https://github.com/Sick-Dev/WebRcon-CSharp) plugin.

To show messages on the web console, call the "Log" method on any "Tab" object.
If no other tabs have been created explicitely, you can use the "defaultTab".

To create and send message to another tab, simply call the method "CreateTab".
```C#
Tab newTab = WebRconManager.singleton.console.CreateTab("Tab name");
newTab.Log("This message is sent to the new tab");
```

## Registering Commands
```C#
public static int Max(int a, int b) {
  if(a > b)
    return a;
  else
    return b;
}
...
Command command = new FuncCommand<int, bool>(IsNumberEven);
WebRconManager.singleton.commandsManager.Add(command);
```
Once a command is registered, it can be called from the WebConsole.

Commands management is implemented by the [CommandSystem](https://github.com/Cobo3/CommandSystem), which allows to parse strings into ready-to-use commands.
The [CommandSystem](https://github.com/Cobo3/CommandSystem) already contains a full [in-depth guide](https://github.com/Cobo3/CommandSystem) of how it works, so feel free to read it  and familiarize yourself with it.

## Closing the connection
The connection is automatically closed whenever the WebRconManager object is destroyed, that is, when the game quits.
However, the connection can also be closed manually:
```C#
WebRconManager.singleton.console.Close();
```
It is recommended to manually close the connection at the end of the execution of your application.

## Events
- "onLinked" : Called when the connection status becomes **linked**.
- "onUnlinked" : Called when the connection status becomes **unlinked**.
- "onDisconnected" : Called when the connection status becomes **disconnected**. Returns the reason as an ErrorCode.
- "onError" : Called from the server when something is wrong. Returns the specific ErrorCode.
- "onExceptionThrown" : Called when an asynchronous operation throws an exception. The exception can come from either plugin source code or from the execution of a custom command.
- "onCommand" : Raised when a command is called from the WebConsole. Use it to manually manage the execution of commands. If no delegate is assigned to this event, the [CommandSystem](https://github.com/Cobo3/CommandSystem) will execute the command automatically.

## Built-in Commands
![WebRconManager Inspector](screenshots/Screenshots/WebRconManager-Inspector 2.jpg?raw=true)
You can check and uncheck the built-in commands that you want to be bundled with your game through the WebRconManager inspector.
**Note**: When targeting Android, enabling Microhone and Location will also add those same permissions to the final .apk.

# FAQ
### What is WebRcon?
Visit [WebRcon Website](http://www.webrcon.com) to obtain all needed info.

### What is the cKey?
The cKey is the generated code that will link your application to a WebConsole.

### How can I choose what messaged get logged?
By default, WebRconManager will log any message that would go into the editor’s Console, such as Debug.Log calls, internal messages or exceptions.
You can filter what kind of messages gets logged changing the setting "Attached Log Level" on the prefab.

### Can I use this plugin other than on Unity?
You can find the base C# plugin for WebRcon on [Github](https://github.com/Sick-Dev/WebRcon-CSharp).

# License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

# About
Created by SickDev.
- **Bubexel** - [GitHub](https://github.com/serk7) - [WebPage](http://www.bubexel.com)
- **Cobo** - [GitHub](https://github.com/Cobo3) - [WebPage](https://coboantonio.wordpress.com/)
