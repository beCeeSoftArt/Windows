using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ThreadSpecificMouseHook
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nCode">The n code.</param>
        /// <param name="wParam">The w parameter.</param>
        /// <param name="lParam">The l parameter.</param>
        /// <returns></returns>
        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Declare the hook handle as an int.
        /// </summary>
        static int hHook = 0;

        /// <summary>
        /// Declare the mouse hook constant.
        /// For other hook types, you can obtain these values from Winuser.h in the Microsoft SDK.
        /// </summary>
        public const int WH_MOUSE = 7;

        /// <summary>
        /// Declare MouseHookProcedure as a HookProc type.
        /// </summary>
        HookProc MouseHookProcedure;

        /// <summary>
        /// Declare the wrapper managed POINT class.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            /// <summary>
            /// The x
            /// </summary>
            public int x;
            /// <summary>
            /// The y
            /// </summary>
            public int y;
        }

        /// <summary>
        /// Declare the wrapper managed MouseHookStruct class.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            /// <summary>
            /// The pt
            /// </summary>
            public POINT pt;
            /// <summary>
            /// The HWND
            /// </summary>
            public int hwnd;
            /// <summary>
            /// The w hit test code
            /// </summary>
            public int wHitTestCode;
            /// <summary>
            /// The dw extra information
            /// </summary>
            public int dwExtraInfo;
        }

        /// <summary>
        /// This is the Import for the SetWindowsHookEx function.
        /// Use this function to install a thread-specific hook.
        /// </summary>
        /// <param name="idHook">The identifier hook.</param>
        /// <param name="lpfn">The LPFN.</param>
        /// <param name="hInstance">The h instance.</param>
        /// <param name="threadId">The thread identifier.</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        /// <summary>
        /// This is the Import for the UnhookWindowsHookEx function.
        /// Call this function to uninstall the hook.
        /// </summary>
        /// <param name="idHook">The identifier hook.</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        /// <summary>
        /// This is the Import for the CallNextHookEx function.
        /// Use this function to pass the hook information to the next hook procedure in chain.
        /// </summary>
        /// <param name="idHook">The identifier hook.</param>
        /// <param name="nCode">The n code.</param>
        /// <param name="wParam">The w parameter.</param>
        /// <param name="lParam">The l parameter.</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Mouses the hook proc.
        /// </summary>
        /// <param name="nCode">The n code.</param>
        /// <param name="wParam">The w parameter.</param>
        /// <param name="lParam">The l parameter.</param>
        /// <returns></returns>
        public static int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //Marshall the data from the callback.
            MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));

            if (nCode < 0)
            {
                return CallNextHookEx(hHook, nCode, wParam, lParam);
            }
            else
            {
                //Create a string variable that shows the current mouse coordinates.
                String strCaption = "x = " +
                        MyMouseHookStruct.pt.x.ToString("d") +
                            "  y = " +
                MyMouseHookStruct.pt.y.ToString("d");
                //You must get the active form because it is a static function.
                Form tempForm = Form.ActiveForm;

                //Set the caption of the form.
                tempForm.Text = strCaption;
                return CallNextHookEx(hHook, nCode, wParam, lParam);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the button1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (hHook == 0)
            {
                // Create an instance of HookProc.
                MouseHookProcedure = new HookProc(Form1.MouseHookProc);

                hHook = SetWindowsHookEx(WH_MOUSE,
                            MouseHookProcedure,
                            (IntPtr)0,
                            AppDomain.GetCurrentThreadId());
                //If the SetWindowsHookEx function fails.
                if (hHook == 0)
                {
                    MessageBox.Show(@"SetWindowsHookEx Failed");
                    return;
                }
                button1.Text = @"UnHook Windows Hook";
            }
            else
            {
                bool ret = UnhookWindowsHookEx(hHook);
                //If the UnhookWindowsHookEx function fails.
                if (ret == false)
                {
                    MessageBox.Show(@"UnhookWindowsHookEx Failed");
                    return;
                }
                hHook = 0;
                button1.Text = @"Set Windows Hook";
                this.Text = @"Mouse Hook";
            }

        }
    }
}