// xbc_alarm.h

#pragma once

#include <windows.h>
#include <array>
#include <Xinput.h>

using namespace System;

namespace xbc_alarm {

	//public ref class Class1
	//{
	//	// TODO: Add your methods for this class here.
	//};

	__declspec(dllexport) bool __stdcall getBatteryInfo(std::array<BYTE, XUSER_MAX_COUNT> type, std::array<BYTE, XUSER_MAX_COUNT> level, int& numOfControllers);

	__declspec(dllexport) bool __stdcall tryVibrate(int device);
}


