# Xbox 360/One Controller Battery Monitor

## Building

* Open the solution in Visual Studio
* Build > Build solution
* On successful build a folder called BuildDir will be created in Solution directory and should contain the exe and dll for the app.

## Installation

* Place it any folder on your computer.
* When you first run it, it will create a default XML settings file in AppData/XBCA
* Customize the settings and that's it!

## Usage

* Only works on Windows 8 and higher - **requires xinput1_4.dll**!

### GUI

All connected controllers are listed in the table. For now it only supports 4 controllers. New Windows 10 should be able to support more than that but haven't got around to it yet.

First column is the ID that is assigned to the controller by the monitoring app (completely useless).

Second column is the ID that the controller is assigned to by the XInput API.

That is followed by the reported battery type and charge level.

![Connected controller example](https://github.com/matt-345/xbox-controller-battery-monitor/blob/master/images/connected_controller.png)

### Settings

Has two possible notification levels - low and medium.

The app can make a beep sound when it notifies you about the charge level.

You can also make it run on Windows startup (minimized to tray or not).

Mostly self-explanatory, except the *notify limit* which will alert you every X amount of time but that timer will reset if the controller goes to sleep or is turned off during that time.

![Available settings](https://github.com/matt-345/xbox-controller-battery-monitor/blob/master/images/settings.png)


**Most of these (especially startup) settings are disabled by default so make sure to change them if you want to use the app for longer periods of time, which is what it's designed for.**

## To-do

* Create a github release.
* Detect when the controller is charging.
* Implement Close-to-tray setting.
* Clean and comment the code.
