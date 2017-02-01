// xbc_dll.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

extern "C" __declspec(dllexport) bool getBatteryInfo(BYTE* type, BYTE* level, int & numOfControllers)
{
	DWORD dwResult;
	numOfControllers = 0;
	for (DWORD i = 0; i< XUSER_MAX_COUNT; i++)
	{
		XINPUT_STATE state;
		SecureZeroMemory(&state, sizeof(XINPUT_STATE));

		// Simply get the state of the controller from XInput.
		dwResult = XInputGetState(i, &state);

		level[i] = static_cast<BYTE>(BATTERY_LEVEL_FULL);
		type[i] = static_cast<BYTE>(BATTERY_TYPE_DISCONNECTED);

		if (dwResult == ERROR_SUCCESS)
		{
			// Controller is connected 
			XINPUT_BATTERY_INFORMATION battery;
			memset(&battery, 0, sizeof(XINPUT_BATTERY_INFORMATION));

			if (XInputGetBatteryInformation(0, BATTERY_DEVTYPE_GAMEPAD, &battery) == ERROR_SUCCESS)
			{
				level[i] = static_cast<BYTE>(battery.BatteryLevel);
				type[i] = static_cast<BYTE>(battery.BatteryType);
			}

			++numOfControllers;
		}
	}

	if (numOfControllers > 0)
		return true;

	return false;
}

extern "C" __declspec(dllexport) DWORD __stdcall tryVibrate(int device)
{
	XINPUT_VIBRATION vibration;
	ZeroMemory(&vibration, sizeof(XINPUT_VIBRATION));
	vibration.wLeftMotorSpeed = 32000; // use any value between 0-65535 here
	vibration.wRightMotorSpeed = 16000; // use any value between 0-65535 here

	DWORD dwResult = XInputSetState(device, &vibration);

	return dwResult;
}

extern "C" __declspec(dllexport) int getMaxCount()
{
	return XUSER_MAX_COUNT;
}
