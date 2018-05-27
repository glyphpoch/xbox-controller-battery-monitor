using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

using Windows.Gaming.Input;
using Windows.Devices.Power;

namespace xbca
{
    class XDataW10
    {
        protected Settings m_Settings;
        protected volatile bool m_Run = true;

        public delegate void NotificationDelegate(int device, string type, string value);
        public static event NotificationDelegate NotificationEvent;

        public delegate void DataDelegate(byte[] type, byte[] value, byte[] note, string[] status);
        public static event DataDelegate DataEvent;

        public delegate void ErrorDelegate(int error);
        public static event ErrorDelegate ErrorEvent;

        protected int m_State = -1;

        protected Thread pollInfo;

        public void Start(Settings settings)
        {
            m_Settings = settings;
            pollInfo = new Thread(Poll);
            pollInfo.Start();

            int initCount = Gamepad.Gamepads.Count();

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

        public int State()
        {
            return m_State;
        }

        protected virtual void Poll()
        {
            Thread.Sleep(500);

            byte[] type = new byte[XInputConstants.XUSER_MAX_COUNT];
            byte[] value = new byte[XInputConstants.XUSER_MAX_COUNT];
            byte[] note = new byte[XInputConstants.XUSER_MAX_COUNT];
            string[] status = new string[XInputConstants.XUSER_MAX_COUNT];

            bool[] notified = new bool[XInputConstants.XUSER_MAX_COUNT];
            byte[] last_level = new byte[XInputConstants.XUSER_MAX_COUNT];
            System.Diagnostics.Stopwatch[] notify_timer = new System.Diagnostics.Stopwatch[XInputConstants.XUSER_MAX_COUNT];

            for (int i = 0; i < XInputConstants.XUSER_MAX_COUNT; ++i)
            {
                notify_timer[i] = new System.Diagnostics.Stopwatch();
            }

            for (int i = 0; i < notified.Length; ++i)
            {
                notified[i] = false;
                last_level[i] = 255;
            }

            bool result = false;
            try
            {
                while (m_Run)
                {
                    //result = getBatteryInfo(type, value, note, ref numOfControllers);

                    if(Gamepad.Gamepads.Count > 0)
                    {
                        result = true;

                        for (int i = 0; i < Gamepad.Gamepads.Count; ++i)
                        {
                            BatteryReport batteryReport = Gamepad.Gamepads[i].TryGetBatteryReport();

                            if(batteryReport != null)
                            {
                                if(batteryReport.RemainingCapacityInMilliwattHours != null && batteryReport.FullChargeCapacityInMilliwattHours != null)
                                {
                                    type[i] = (byte)BatteryTypes.BATTERY_TYPE_UNKNOWN;

                                    int chargeLevel = ((int)batteryReport.RemainingCapacityInMilliwattHours * 100) / (int)batteryReport.FullChargeCapacityInMilliwattHours;

                                    value[i] = chargeLevelToValue(chargeLevel);

                                    byte noteLevel = (byte)chargeLevel;

                                    if(noteLevel == 0)
                                    {
                                        noteLevel = 1;
                                    }
                                    note[i] = noteLevel;

                                    status[i] = batteryReport.Status.ToString();
                                } 
                            }
                        }
                    }

                    for (int i = 0; i < XInputConstants.XUSER_MAX_COUNT; ++i)
                    {
                        if (notify_timer[i].IsRunning)
                        {
                            if (m_Settings.NotifyEvery != 0)
                            {
                                notify_timer[i].Stop();
                            }
                            else
                            {
                                if (notify_timer[i].Elapsed.Minutes > m_Settings.NotifyEvery)
                                {
                                    notified[i] = false;
                                }
                            }
                        }
                    }

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
                                    RaiseTheEvent(i, Constants.GetEnumDescription((BatteryTypes)type[i]), Constants.GetEnumDescription((BatteryLevel)value[i]));
                                    notified[i] = true;

                                    if (m_Settings.NotifyEvery != 0)
                                    {
                                        notify_timer[i].Start();
                                    }

                                    last_level[i] = value[i];
                                }

                                if (notified[i] == true && (value[i] > m_Settings.Level || value[i] < last_level[i]))
                                {
                                    notified[i] = false;

                                    if (notify_timer[i].IsRunning)
                                    {
                                        notify_timer[i].Stop();
                                    }
                                }
                            }
                            else
                            {
                                notified[i] = false;
                                last_level[i] = 255;

                                if (notify_timer[i].IsRunning)
                                {
                                    notify_timer[i].Stop();
                                }
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

                        for (int i = 0; i < XInputConstants.XUSER_MAX_COUNT; ++i)
                        {
                            if (notify_timer[i].IsRunning)
                            {
                                notify_timer[i].Stop();
                            }
                        }
                    }
                    //
                    // Send data about the controllers to the main application.
                    //
                    RaiseDataEvent(type, value, note, status);

                    //
                    // Reset arrays to default values.
                    //
                    for (int i = 0; i < XInputConstants.XUSER_MAX_COUNT; ++i)
                    {
                        value[i] = (byte)BatteryLevel.BATTERY_LEVEL_EMPTY;
                        type[i] = (byte)BatteryTypes.BATTERY_TYPE_DISCONNECTED;
                        note[i] = 0;
                    }

                    Console.WriteLine("sleep for a minute");
                    //
                    // Sleep for the defined times unless Stop request is sent by the main application.
                    //
                    for (int s = 0; s < 200 && (m_Run); ++s)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                m_State = -1;
                //RaiseErrorEvent(-1);
            }

            m_State = 0;

            if (m_Run == true)
            {
                RaiseErrorEvent(-1);
            }
        }

        protected byte chargeLevelToValue(int chargeLevel)
        {
            if (chargeLevel >= (int)BatteryChargeThreshold.BATTERY_THRESHOLD_FULL)
            {
                return (byte)BatteryLevel.BATTERY_LEVEL_FULL;
            }
            else if (chargeLevel >= (int)BatteryChargeThreshold.BATTERY_THRESHOLD_MEDIUM)
            {
                return (byte)BatteryLevel.BATTERY_LEVEL_MEDIUM;
            }
            else if (chargeLevel >= (int)BatteryChargeThreshold.BATTERY_THRESHOLD_LOW)
            {
                return (byte)BatteryLevel.BATTERY_LEVEL_LOW;
            }
            else
            {
                return (byte)BatteryLevel.BATTERY_LEVEL_EMPTY;
            }
        }

        protected void RaiseTheEvent(int device, string type, string value)
        {
            Console.WriteLine("raising event in thread");

            //
            // Equals checking if TestEvent == null and then calling TestEvent()
            //
            NotificationEvent?.Invoke(device, type, value);
        }

        protected void RaiseDataEvent(byte[] type, byte[] value, byte[] note, string[] status = null)
        {
            Console.WriteLine("raising data event in thread");

            DataEvent?.Invoke(type, value, note, status);
        }

        protected void RaiseErrorEvent(int error)
        {
            Console.WriteLine("raising error event in thread");

            ErrorEvent?.Invoke(error);
        }
    }
}
