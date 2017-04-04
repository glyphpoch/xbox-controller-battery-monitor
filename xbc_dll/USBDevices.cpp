#include "stdafx.h"
#include "USBDevices.h"

bool USBDevices::IsUSB(UINT16 idVendor, UINT16 idProduct)
{
	// Just hardcoded the number for now. The first gen xbox pad doesn't even report battery? Or wasn't wireless. TODO
	for(int i = 0; i < 9; ++i)
	{
		if(xpad_device[i].idVendor == idVendor && xpad_device[i].idProduct == idProduct)
		{
		    return true;
		}
	}

	return false;
}

