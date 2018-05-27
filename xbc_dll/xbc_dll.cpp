// xbc_dll.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

extern "C" __declspec(dllexport) bool __stdcall getBatteryInfo(BYTE* type, BYTE* level, BYTE* note, int & numOfControllers)
{
	try {
		std::ofstream log("log.txt", std::ios::app);

		try
		{
			DWORD dwResult;
			numOfControllers = 0;
			for (DWORD i = 0; i< XUSER_MAX_COUNT; i++)
			{
				XINPUT_STATE state;
				SecureZeroMemory(&state, sizeof(XINPUT_STATE));

				// Simply get the state of the controller from XInput.
				dwResult = XInputGetState(i, &state);

				level[i] = static_cast<BYTE>(BATTERY_LEVEL_EMPTY);
				type[i] = static_cast<BYTE>(BATTERY_TYPE_DISCONNECTED);
				note[i] = 0;

				log << "GetState Result: " << dwResult << " Packet number: " << state.dwPacketNumber << std::endl;

				if (dwResult == ERROR_SUCCESS)
				{
					// Controller is connected 
					XINPUT_BATTERY_INFORMATION battery;
					SecureZeroMemory(&battery, sizeof(XINPUT_BATTERY_INFORMATION));

					DWORD info = XInputGetBatteryInformation(i, BATTERY_DEVTYPE_GAMEPAD, &battery);
				
					log << "BatteryInfo Result " << i << ": " << info << std::endl;
					note[i] = 1;

					if (info == ERROR_SUCCESS)
					{
						level[i] = static_cast<BYTE>(battery.BatteryLevel);
						type[i] = static_cast<BYTE>(battery.BatteryType);
					}

					++numOfControllers;
				}
			}

			if (numOfControllers > 0)
			{
				log << "------------------------------------" << std::endl;
				return true;
			}		
		}
		catch (const std::exception& ex)
		{
			log << "Exception: " << ex.what() << std::endl;

			return false;
		}
	}
	catch (const std::exception& fileex)
	{

	}
		
	return false;
}

extern "C" __declspec(dllexport) int __stdcall getMaxCount()
{
	return XUSER_MAX_COUNT;
}
