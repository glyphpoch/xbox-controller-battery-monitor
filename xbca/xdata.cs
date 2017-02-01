using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xbca
{
    class xdata
    {
#if DEBUG
        [DllImport("D:\\Dropbox\\_Projects\\xbc_proto\\Debug\\dummy.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int dummy();

        [DllImport("D:\\Dropbox\\_Projects\\xbc_proto\\Debug\\dummy.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int dummy2([MarshalAs(UnmanagedType.LPArray)] byte[] arr, int len);

        [DllImport("D:\\Dropbox\\_Projects\\xbc_proto\\Debug\\xbc_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getMaxCount();

        [DllImport("D:\\Dropbox\\_Projects\\xbc_proto\\Debug\\xbc_dll.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool getBatteryInfo([MarshalAs(UnmanagedType.LPArray, SizeConst = XInputConstants.XUSER_MAX_COUNT)] byte[] type,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = XInputConstants.XUSER_MAX_COUNT)] byte[] value, ref int numOfControllers);

        [DllImport("D:\\Dropbox\\_Projects\\xbc_proto\\Debug\\xbc_dll.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int tryVibrate();
#else
        [DllImport("dummy.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int dummy();

        [DllImport("dummy.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int dummy2([MarshalAs(UnmanagedType.LPArray)] byte[] arr, int len);

        [DllImport("xbc_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getMaxCount();

        [DllImport("xbc_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getBatteryInfo([MarshalAs(UnmanagedType.LPArray, SizeConst = XInputConstants.XUSER_MAX_COUNT)] byte[] type,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = XInputConstants.XUSER_MAX_COUNT)] byte[] value, ref int numOfControllers);

        [DllImport("xbc_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int tryVibrate();
#endif
        private Settings m_Settings;
        private volatile bool m_Run = true;

        public delegate void NotificationDelegate(bool display, int device, string type, string value);
        public static event NotificationDelegate NotificationEvent;

        public delegate void DataDelegate(byte[] type, byte[] value);
        public static event DataDelegate DataEvent;

        private int m_State = -1;

        private Thread pollInfo;

        public void Start(Settings settings)
        {
            m_Settings = settings;
            pollInfo = new Thread(Poll);
            pollInfo.Start();

            m_State = 1;
        }

        public void Stop()
        {
            m_Run = false;

            if (m_State == 1 && pollInfo.ThreadState == ThreadState.Running)
            {
                pollInfo.Join();
            }
        }

        public void UpdateSettings(Settings settings)
        {
            lock (m_Settings)
            {
                m_Settings = null;
                m_Settings = settings;
            }
        }

        public void Vibrate()
        {
            try
            {
                tryVibrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public int State()
        {
            return m_State;
        }

        private void Poll()
        {
            byte[] type = new byte[XInputConstants.XUSER_MAX_COUNT];
            byte[] value = new byte[XInputConstants.XUSER_MAX_COUNT];
            bool[] notified = new bool[XInputConstants.XUSER_MAX_COUNT];
            byte[] last_level = new byte[XInputConstants.XUSER_MAX_COUNT];

            for (int i = 0; i < notified.Length; ++i)
            {
                notified[i] = false;
                last_level[i] = 255;
            }

            int numOfControllers = 0;
            bool result = false;
            try
            {
                while (m_Run)
                {
#if DEBUG
                    Console.WriteLine(dummy() + " " + Constants.GetEnumDescription((BatteryTypes)1));

                    RaiseTheEvent(false, 0, "", "");
#else
                    result = getBatteryInfo(type, value, ref numOfControllers);

                    if (result == true)
                    {
                        //
                        // Check for all possible controllers. XUSER_MAX_COUNT is 4. Defined in Xinput.h and copied to Constants.cs.
                        //
                        for (int i = 0; i < XInputConstants.XUSER_MAX_COUNT; ++i)
                        {
                            if (type[i] != (byte)BatteryTypes.BATTERY_TYPE_DISCONNECTED)
                            {
                                //
                                // Notify the user only once, unless otherwise specified.
                                // Values will be 0,1,2,3 - More in Constants.
                                //
                                if (notified[i] == false && value[i] <= m_Settings.Level)
                                {
                                    //
                                    // Raise an event which reports the status of the battery to the main app.
                                    // Alternative Enum name: Enum.GetName(typeof(BatteryTypes), type[i])
                                    //
                                    RaiseTheEvent(true, i, Constants.GetEnumDescription((BatteryTypes)type[i]), Constants.GetEnumDescription((BatteryLevel)value[i]));
                                    notified[i] = true;
                                    last_level[i] = value[i];
                                }

                                if (notified[i] == true && (value[i] > m_Settings.Level || value[i] < last_level[i]))
                                {
                                    notified[i] = false;
                                }
                            }
                            else
                            {
                                notified[i] = false;
                                last_level[i] = 255;
                            }
                        }
                    }
                    else
                    {
                        //
                        // If no controllers are connected just reset the notified array. It's initialized to false by default.
                        //
                        notified = new bool[XInputConstants.XUSER_MAX_COUNT];
                        last_level = Enumerable.Repeat((byte)255, XInputConstants.XUSER_MAX_COUNT).ToArray();
                    }
#endif
                    //
                    // Send data about the controllers to the main application.
                    //
                    RaiseDataEvent(type, value);

                    Console.WriteLine("sleep for a minute");
                    //
                    // Sleep for the defined times unless Stop request is sent by the main application.
                    //
                    for (int s = 0; s < 600 && (m_Run); ++s)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                m_State = -1;
            }

            m_State = 0;
        }

        private void RaiseTheEvent(bool display, int device, string type, string value)
        {
            Console.WriteLine("raising event in thread");

            //
            // Equals checking if TestEvent == null and then calling TestEvent()
            //
            NotificationEvent?.Invoke(display, device, type, value);
        }

        private void RaiseDataEvent(byte[] type, byte[] value)
        {
            Console.WriteLine("raising data event in thread");

            DataEvent?.Invoke(type, value);
        }
    }
}
