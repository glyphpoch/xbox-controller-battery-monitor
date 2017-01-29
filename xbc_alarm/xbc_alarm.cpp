// This is the main DLL file.

#include "stdafx.h"

#include "xbc_alarm.h"

bool __stdcall xbc_alarm::getBatteryInfo(std::array<BYTE, XUSER_MAX_COUNT> type, std::array<BYTE, XUSER_MAX_COUNT> level, int & numOfControllers)
{
	DWORD dwResult;
	numOfControllers = 0;
	for (DWORD i = 0; i< XUSER_MAX_COUNT; i++)
	{
		XINPUT_STATE state;
		SecureZeroMemory(&state, sizeof(XINPUT_STATE));

		// Simply get the state of the controller from XInput.
		dwResult = XInputGetState(i, &state);

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

bool __stdcall xbc_alarm::tryVibrate(int device)
{
	XINPUT_VIBRATION vibration;
	ZeroMemory(&vibration, sizeof(XINPUT_VIBRATION));
	vibration.wLeftMotorSpeed = 32000; // use any value between 0-65535 here
	vibration.wRightMotorSpeed = 16000; // use any value between 0-65535 here

	DWORD dwResult = XInputSetState(device, &vibration);

	return dwResult;
}
