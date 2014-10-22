using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4
{
    class Program
    {

        #region Enums

        [Flags]
        private enum InterceptionKeyState
        {
            INTERCEPTION_KEY_DOWN = 0x00,
            INTERCEPTION_KEY_UP = 0x01,
            INTERCEPTION_KEY_E0 = 0x02,
            INTERCEPTION_KEY_E1 = 0x04,
            INTERCEPTION_KEY_TERMSRV_SET_LED = 0x08,
            INTERCEPTION_KEY_TERMSRV_SHADOW = 0x10,
            INTERCEPTION_KEY_TERMSRV_VKPACKET = 0x20
        };

        [Flags]
        private enum InterceptionFilterKeyState
        {
            INTERCEPTION_FILTER_KEY_NONE = 0x0000,
            INTERCEPTION_FILTER_KEY_ALL = 0xFFFF,
            INTERCEPTION_FILTER_KEY_DOWN = InterceptionKeyState.INTERCEPTION_KEY_UP,
            INTERCEPTION_FILTER_KEY_UP = InterceptionKeyState.INTERCEPTION_KEY_UP << 1,
            INTERCEPTION_FILTER_KEY_E0 = InterceptionKeyState.INTERCEPTION_KEY_E0 << 1,
            INTERCEPTION_FILTER_KEY_E1 = InterceptionKeyState.INTERCEPTION_KEY_E1 << 1,
            INTERCEPTION_FILTER_KEY_TERMSRV_SET_LED = InterceptionKeyState.INTERCEPTION_KEY_TERMSRV_SET_LED << 1,
            INTERCEPTION_FILTER_KEY_TERMSRV_SHADOW = InterceptionKeyState.INTERCEPTION_KEY_TERMSRV_SHADOW << 1,
            INTERCEPTION_FILTER_KEY_TERMSRV_VKPACKET = InterceptionKeyState.INTERCEPTION_KEY_TERMSRV_VKPACKET << 1
        };

        [Flags]
        private enum InterceptionMouseState
        {
            INTERCEPTION_MOUSE_LEFT_BUTTON_DOWN = 0x001,
            INTERCEPTION_MOUSE_LEFT_BUTTON_UP = 0x002,
            INTERCEPTION_MOUSE_RIGHT_BUTTON_DOWN = 0x004,
            INTERCEPTION_MOUSE_RIGHT_BUTTON_UP = 0x008,
            INTERCEPTION_MOUSE_MIDDLE_BUTTON_DOWN = 0x010,
            INTERCEPTION_MOUSE_MIDDLE_BUTTON_UP = 0x020,

            INTERCEPTION_MOUSE_BUTTON_1_DOWN = INTERCEPTION_MOUSE_LEFT_BUTTON_DOWN,
            INTERCEPTION_MOUSE_BUTTON_1_UP = INTERCEPTION_MOUSE_LEFT_BUTTON_UP,
            INTERCEPTION_MOUSE_BUTTON_2_DOWN = INTERCEPTION_MOUSE_RIGHT_BUTTON_DOWN,
            INTERCEPTION_MOUSE_BUTTON_2_UP = INTERCEPTION_MOUSE_RIGHT_BUTTON_UP,
            INTERCEPTION_MOUSE_BUTTON_3_DOWN = INTERCEPTION_MOUSE_MIDDLE_BUTTON_DOWN,
            INTERCEPTION_MOUSE_BUTTON_3_UP = INTERCEPTION_MOUSE_MIDDLE_BUTTON_UP,

            INTERCEPTION_MOUSE_BUTTON_4_DOWN = 0x040,
            INTERCEPTION_MOUSE_BUTTON_4_UP = 0x080,
            INTERCEPTION_MOUSE_BUTTON_5_DOWN = 0x100,
            INTERCEPTION_MOUSE_BUTTON_5_UP = 0x200,

            INTERCEPTION_MOUSE_WHEEL = 0x400,
            INTERCEPTION_MOUSE_HWHEEL = 0x800
        };

        [Flags]
        private enum InterceptionFilterMouseState
        {
            INTERCEPTION_FILTER_MOUSE_NONE = 0x0000,
            INTERCEPTION_FILTER_MOUSE_ALL = 0xFFFF,

            INTERCEPTION_FILTER_MOUSE_LEFT_BUTTON_DOWN = InterceptionMouseState.INTERCEPTION_MOUSE_LEFT_BUTTON_DOWN,
            INTERCEPTION_FILTER_MOUSE_LEFT_BUTTON_UP = InterceptionMouseState.INTERCEPTION_MOUSE_LEFT_BUTTON_UP,
            INTERCEPTION_FILTER_MOUSE_RIGHT_BUTTON_DOWN = InterceptionMouseState.INTERCEPTION_MOUSE_RIGHT_BUTTON_DOWN,
            INTERCEPTION_FILTER_MOUSE_RIGHT_BUTTON_UP = InterceptionMouseState.INTERCEPTION_MOUSE_RIGHT_BUTTON_UP,
            INTERCEPTION_FILTER_MOUSE_MIDDLE_BUTTON_DOWN = InterceptionMouseState.INTERCEPTION_MOUSE_MIDDLE_BUTTON_DOWN,
            INTERCEPTION_FILTER_MOUSE_MIDDLE_BUTTON_UP = InterceptionMouseState.INTERCEPTION_MOUSE_MIDDLE_BUTTON_UP,

            INTERCEPTION_FILTER_MOUSE_BUTTON_1_DOWN = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_1_DOWN,
            INTERCEPTION_FILTER_MOUSE_BUTTON_1_UP = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_1_UP,
            INTERCEPTION_FILTER_MOUSE_BUTTON_2_DOWN = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_2_DOWN,
            INTERCEPTION_FILTER_MOUSE_BUTTON_2_UP = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_2_UP,
            INTERCEPTION_FILTER_MOUSE_BUTTON_3_DOWN = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_3_DOWN,
            INTERCEPTION_FILTER_MOUSE_BUTTON_3_UP = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_3_UP,

            INTERCEPTION_FILTER_MOUSE_BUTTON_4_DOWN = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_4_DOWN,
            INTERCEPTION_FILTER_MOUSE_BUTTON_4_UP = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_4_UP,
            INTERCEPTION_FILTER_MOUSE_BUTTON_5_DOWN = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_5_DOWN,
            INTERCEPTION_FILTER_MOUSE_BUTTON_5_UP = InterceptionMouseState.INTERCEPTION_MOUSE_BUTTON_5_UP,

            INTERCEPTION_FILTER_MOUSE_WHEEL = InterceptionMouseState.INTERCEPTION_MOUSE_WHEEL,
            INTERCEPTION_FILTER_MOUSE_HWHEEL = InterceptionMouseState.INTERCEPTION_MOUSE_HWHEEL,

            INTERCEPTION_FILTER_MOUSE_MOVE = 0x1000
        };

        [Flags]
        private enum InterceptionMouseFlag
        {
            INTERCEPTION_MOUSE_MOVE_RELATIVE = 0x000,
            INTERCEPTION_MOUSE_MOVE_ABSOLUTE = 0x001,
            INTERCEPTION_MOUSE_VIRTUAL_DESKTOP = 0x002,
            INTERCEPTION_MOUSE_ATTRIBUTES_CHANGED = 0x004,
            INTERCEPTION_MOUSE_MOVE_NOCOALESCE = 0x008,
            INTERCEPTION_MOUSE_TERMSRV_SRC_SHADOW = 0x100
        };
        #endregion

        #region Structs

        private struct InterceptionMouseStroke
        {
            public ushort state;
            public ushort flags;
            public short rolling;
            public int x;
            public int y;
            public uint information;
        }

        private struct InterceptionKeyStroke
        {
            public ushort code;
            public ushort state;
            public uint information;
        };

        private struct KEYBOARD_INPUT_DATA
        {
            public ushort UnitId;
            public ushort MakeCode;
            public ushort Flags;
            public ushort Reserved;
            public ushort ExtraInformation;
        }

        private struct MOUSE_INPUT_DATA
        {
            public ushort UnitId;
            public ushort Flags;
            public ushort ButtonFlags;
            public ushort ButtonData;
            public ulong RawButtons;
            public long LastX;
            public long LastY;
            public ulong ExtraInformation;
        }

        private struct InterceptionDeviceArray
        {
            public IntPtr handle;
            public IntPtr unempty;
        }

        #endregion

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int InterceptionPredicateFunc(int device);

        [DllImport("interception.dll")]
        private static extern IntPtr interception_create_context();

        [DllImport("interception.dll")]
        private static extern void interception_destroy_context(IntPtr context);

        [DllImport("interception.dll")]
        private static extern int interception_get_precedence(IntPtr context, int device);

        [DllImport("interception.dll")]
        private static extern void interception_set_precedence(IntPtr context, int device, int precedence);

        [DllImport("interception.dll")]
        private static extern ushort interception_get_filter(IntPtr context, int device);

        [DllImport("interception.dll")]
        private static extern void interception_set_filter(IntPtr context,[MarshalAs(UnmanagedType.FunctionPtr)]InterceptionPredicateFunc func, ushort filter);

        [DllImport("interception.dll")]
        private static extern int interception_wait(IntPtr context);

        [DllImport("interception.dll")]
        private static extern int interception_wait_with_timeout(IntPtr context, ulong milliseconds);

        [DllImport("interception.dll")]
        private static extern int interception_send(IntPtr context, int device, IntPtr stroke, uint nstroke);

        [DllImport("interception.dll")]
        private static extern int interception_receive(IntPtr context, int device, IntPtr stroke, uint nstroke);

        [DllImport("interception.dll")]
        private static extern uint interception_get_hardware_id(IntPtr context, int device, IntPtr hardware_id_buffer, uint buffer_size);

        [DllImport("interception.dll")]
        private static extern int interception_is_invalid(int device);

        [DllImport("interception.dll")]
        private static extern int interception_is_keyboard(int device);

        [DllImport("interception.dll")]
        private static extern int interception_is_mouse(int device);


        private static InterceptionKeyStroke ctrl_down = new InterceptionKeyStroke { code = 0x1D, state = (ushort)(InterceptionKeyState.INTERCEPTION_KEY_DOWN | InterceptionKeyState.INTERCEPTION_KEY_E0) };
        private static InterceptionKeyStroke alt_down = new InterceptionKeyStroke { code = 0x38, state = (ushort)(InterceptionKeyState.INTERCEPTION_KEY_DOWN | InterceptionKeyState.INTERCEPTION_KEY_E0) };
        private static InterceptionKeyStroke del_down = new InterceptionKeyStroke { code = 0x53, state = (ushort)(InterceptionKeyState.INTERCEPTION_KEY_DOWN | InterceptionKeyState.INTERCEPTION_KEY_E0) };
        private static InterceptionKeyStroke ctrl_up = new InterceptionKeyStroke { code = 0x1D, state = (ushort)(InterceptionKeyState.INTERCEPTION_KEY_UP) };
        private static InterceptionKeyStroke alt_up = new InterceptionKeyStroke { code = 0x38, state = (ushort)(InterceptionKeyState.INTERCEPTION_KEY_UP) };
        private static InterceptionKeyStroke del_up = new InterceptionKeyStroke { code = 0x53, state = (ushort)(InterceptionKeyState.INTERCEPTION_KEY_UP) };

        static void Main(string[] args)
        {
            var context = interception_create_context();
            IntPtr stroke = Marshal.AllocHGlobal(20);
            int device;

            bool actrl_is_down =false, aalt_is_down = false, adel_is_down = false, ctrl_is_down = false, alt_is_down= false, del_is_down = false;

            InterceptionKeyStroke previousStroke = new InterceptionKeyStroke();
            InterceptionKeyStroke newStroke;
            var strokeSequence = new List<InterceptionKeyStroke>();

            context = interception_create_context();

            interception_set_filter(context, interception_is_keyboard, (ushort) InterceptionFilterKeyState.INTERCEPTION_FILTER_KEY_ALL);

            while (interception_receive(context, device = interception_wait(context), stroke, 1) > 0)
            {
                newStroke = (InterceptionKeyStroke) Marshal.PtrToStructure(stroke, typeof(InterceptionKeyStroke));

                if (newStroke.code == ctrl_down.code) 
                {
                    if (newStroke.state == 0)
                        ctrl_is_down = true;
                    else if (newStroke.state == ctrl_down.state)
                        actrl_is_down = true;
                    else if (newStroke.state == ctrl_up.state)
                        ctrl_is_down = false;
                    else if ((newStroke.state & ctrl_up.state) > 0)
                        actrl_is_down = false;
                }

                if (newStroke.code == alt_down.code)
                {
                    if (newStroke.state == 0)
                        alt_is_down = true;
                    else if (newStroke.state == alt_down.state)
                        aalt_is_down = true;
                    else if (newStroke.state == alt_up.state)
                        alt_is_down = false;
                    else if ((newStroke.state & alt_up.state) > 0)
                        aalt_is_down = false;
                }

                if (newStroke.code == del_down.code)
                {
                    if (newStroke.state == 0)
                        del_is_down = true;
                    else if (newStroke.state == del_down.state)
                        adel_is_down = true;
                    else if (newStroke.state == del_up.state)
                        del_is_down = false;
                    else if ((newStroke.state & del_up.state) > 0)
                        adel_is_down = false;
                }

                if ((ctrl_is_down || actrl_is_down) && (alt_is_down||aalt_is_down) && (del_is_down||adel_is_down))
                    Console.WriteLine("combination of ctrl, alt, del pressed");
                else
                    interception_send(context, device, stroke, 1);

                //Console.WriteLine("d {0} c {1} a {2} ad {3} ac {4} aa {5} ", del_is_down, ctrl_is_down, alt_is_down, adel_is_down, actrl_is_down, aalt_is_down);

                if (newStroke.code == 0x01) break;
                
            }

            interception_destroy_context(context);


            //interception_set_filter(context, interception_is_keyboard, (ushort) (InterceptionFilterKeyState.INTERCEPTION_FILTER_KEY_ALL));
            
            //while (interception_receive(context, device = interception_wait(context), stroke, 1) > 0)
            //{
            //    var stroke1 = (InterceptionKeyStroke)Marshal.PtrToStructure(stroke, typeof (InterceptionKeyStroke));

            //    if (stroke)

            //    Console.WriteLine(stroke1.code);
            //    if (stroke1.code == 30)
            //        stroke1.code = 31;
            //    Marshal.StructureToPtr(stroke1, stroke, true);
                
                
            //    interception_send(context, device, stroke, 1);
            //}

            //Console.WriteLine("Finished execution.");

            //interception_destroy_context(context);
        }
    }
}
