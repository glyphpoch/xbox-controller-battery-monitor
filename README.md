# Xbox 360/One Controller Battery Monitor

Simple app that alerts you when your controller's battery is low with a Windows system tray notification.

## Building

* Open the solution in Visual Studio (Windows 10 SDK v10.0.15063.0 required because of RawGameController)
* Build > Build solution
* If the build succeeds, a folder called BuildDir will be created in Solution directory and should contain the exe and dll for the app.

## Installation

* Place it in any folder on your computer.
* When you first run it, it will create a default XML settings file in AppData\Roaming\XBCA.
* Customize the settings and that's it!

## Usage

* Only works on Windows 10 Creators Update and higher.

### GUI

All connected controllers are listed in the table. For now it only supports 4 controllers. Windows 10 should be able to support more than that but haven't got around to it yet.

First column is the ID that is assigned to the controller by the monitoring app (completely useless).

Second column is the Name of the controller - provided by the hardware.

That is followed by the reported battery level, battery type and battery status.

![Connected controller example](https://github.com/matt-345/xbox-controller-battery-monitor/blob/master/images/connected_controller.png)

### Settings

Has two possible notification levels - low and medium.

You can also make it run on Windows startup (minimized to tray or not).

Mostly self-explanatory, except the *notify limit* which will alert you every X amount of time, but the timer will reset if the controller goes to sleep or is turned off during that time.

![Available settings](https://github.com/matt-345/xbox-controller-battery-monitor/blob/master/images/settings.png)


**Most of these (especially startup) settings are disabled by default so make sure to change them if you want to use the app for longer periods of time, which is what it's designed for.**

## To-do

* Detect when the controller is charging.
* Clean and comment the code.
