// dummy.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

extern "C" __declspec(dllexport) int dummy()
{
	//XINPUT_STATE state;
	//SecureZeroMemory(&state, sizeof(XINPUT_STATE));

	// Simply get the state of the controller from XInput.
	//XInputGetState(0, &state);

	return XUSER_MAX_COUNT;
}

extern "C" __declspec(dllexport) int dummy2(BYTE* arr, const int len)
{
	for (int i = 0; i < len; ++i)
	{
		arr[i] = i;
	}

	return -1;
}
