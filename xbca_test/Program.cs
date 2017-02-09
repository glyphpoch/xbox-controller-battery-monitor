using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.IO;

namespace xbca_test
{
    class Program
    {
        // [MarshalAs(UnmanagedType.I2)] WORD
        // [MarshalAs(UnmanagedType.I1)] BYTE
        // int DWORD
        private const int XUSER_MAX_COUNT = 4;

#if DEBUG
        [DllImport("D:\\Dropbox\\_Projects\\xbc_proto\\Debug\\dummy.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int dummy();

        [DllImport("D:\\Dropbox\\_Projects\\xbc_proto\\Debug\\dummy.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int dummy2([MarshalAs(UnmanagedType.LPArray)] byte[] arr, int len);

        [DllImport("D:\\Dropbox\\_Projects\\xbc_proto\\Debug\\xbc_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getMaxCount();

        [DllImport("D:\\Dropbox\\_Projects\\xbc_proto\\Debug\\xbc_dll.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool getBatteryInfo([MarshalAs(UnmanagedType.LPArray, SizeConst = XUSER_MAX_COUNT)] byte[] type, 
            [MarshalAs(UnmanagedType.LPArray, SizeConst = XUSER_MAX_COUNT)] byte[] value,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = XUSER_MAX_COUNT)] byte[] note, ref int numOfControllers);

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
        public static extern bool getBatteryInfo([MarshalAs(UnmanagedType.LPArray, SizeConst = XUSER_MAX_COUNT)] byte[] type,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = XUSER_MAX_COUNT)] byte[] value,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = XUSER_MAX_COUNT)] byte[] note, ref int numOfControllers);

        [DllImport("xbc_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int tryVibrate();
#endif
        static void Main(string[] args)
        {
            string cmd = "";

            Console.WriteLine("Commands: help, battery, vibrate, dummy, dummy2, exit");
            Console.WriteLine();

            if (File.Exists("D:\\Dropbox\\_Projects\\xbc_proto\\Debug\\xbc_dll.dll"))
                Console.WriteLine("Dll found.");

            do
            {
                Console.Write("CMD: ");
                cmd = Console.ReadLine();

                if(cmd == "help")
                {
                    Console.WriteLine("Commands: help, battery, vibrate, exit");
                }
                else if(cmd == "battery")
                {
                    byte[] type = new byte[XUSER_MAX_COUNT];
                    byte[] value = new byte[XUSER_MAX_COUNT];
                    byte[] note = new byte[XUSER_MAX_COUNT];
                    int numControllers = 0;

                    bool result = getBatteryInfo(type, value, note, ref numControllers);

                    if(result == false)
                    {
                        Console.WriteLine("No controllers connected.");
                    }
                    else
                    {
                        Console.WriteLine(numControllers.ToString() + " controllers connected");
                        for (int i = 0; i < XUSER_MAX_COUNT; ++i)
                        {
                            //if(type[i] != (byte)BatteryTypes.BATTERY_TYPE_DISCONNECTED)
                            //{
                                Console.WriteLine("Controller: " + (i+1).ToString() + " Type: " + Enum.GetName(typeof(BatteryTypes), type[i])
                                    + " Battery level: " + Enum.GetName(typeof(BatteryLevel), value[i]) );
                            //}
                        }
                    }
                }
                else if(cmd == "vibrate")
                {
                    int res = tryVibrate();
                    Console.WriteLine("Probably won't work for XBone controller.");
                    Console.WriteLine("Result: " + res.ToString());
                }
                else if(cmd == "dummy")
                {
                    Console.WriteLine("Dummy: " + dummy().ToString());
                }
                else if(cmd == "dummy2")
                {
                    byte[] test = new byte[XUSER_MAX_COUNT];
                    test[2] = 1;

                    int n = dummy2(test, XUSER_MAX_COUNT);

                    for(int i = 0; i < test.Length; ++i)
                    {
                        Console.Write(test[i].ToString() + " ");
                    }
                    Console.WriteLine("Result: " + n.ToString());
                }

                Console.WriteLine();


            } while (cmd != "exit");

            return;
        }
    }
}
