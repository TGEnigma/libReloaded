﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SonicHeroes.Overlay
{
    public partial class SonicHeroes_Form_FakeTransparentOverlay : Form
    {
        // Misc
        const string HEROES_WINDOW_NAME = "SONIC HEROES(TM)";
        /// <summary>
        /// Defines a rectangle.
        /// </summary>
        public struct RECT { public int LeftBorder, TopBorder, RightBorder, BottomBorder; }
        public struct Margins { public int LeftBorder, TopBorder, RightBorder, BottomBorder; }
        /// <summary>
        /// Stores the rectangle which represents the Sonic Heroes Game Window.
        /// </summary>
        public RECT Heroes_Window_Rectangle;
        /// <summary>
        /// A handle to access the Window of Sonic Heroes.
        /// </summary>
        public IntPtr Heroes_Window_Handle;

        /// A bit of delegating
        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        public WinEventDelegate Heroes_Window_Move_Hook_Delegate; 

        /// Process delegate to use with the event fired when window gets moved.
        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            // Filter out non-HWND namechanges, e.g. items within a listbox.
            if (idObject != 0 || idChild != 0) { return; }

            // Adjust Size Accordingly
            WINAPI_Components.GetWindowRect(this.Heroes_Window_Handle, out this.Heroes_Window_Rectangle); // Get rectangle

            Set_To_SonicHeroes_Window_Size();
        }

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;

        public SonicHeroes_Form_FakeTransparentOverlay()
        {
            InitializeComponent();
            this.Show();

            // Get the handle for the Sonic_Heroes Window
            Heroes_Window_Handle = WINAPI_Components.FindWindow(null, HEROES_WINDOW_NAME);
            
            // Adjust the Window Style!
            WINAPI_Components.SetWindowLong(this.Handle, GWL_EXSTYLE, (WINAPI_Components.GetWindowLong(this.Handle, GWL_EXSTYLE) ^ WS_EX_LAYERED ^ WS_EX_TRANSPARENT)); // Set window properties | Window is now clickthrough!

            //Set the Alpha on the Whole Window to 255 (solid)
            WINAPI_Components.SetLayeredWindowAttributes(this.Handle, 0, 255, LWA_ALPHA);

            // Get Sanic Heroes Window Rectangle
            WINAPI_Components.GetWindowRect(Heroes_Window_Handle, out Heroes_Window_Rectangle); // Get window rectangle
            Set_To_SonicHeroes_Window_Size(); // Adjust the overlay window to overlap Sonic Heroes.
            Heroes_Window_Move_Hook_Delegate = new WinEventDelegate(WinEventProc);
            IntPtr Heroes_Window_Hook = SetWinEventHook(WINAPI_Components.EVENT_OBJECT_LOCATIONCHANGE, WINAPI_Components.EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, Heroes_Window_Move_Hook_Delegate, 0, 0, WINAPI_Components.WINEVENT_OUTOFCONTEXT);

            // Set to top most such that this overlay always draws above the game.
            this.TopMost = true;


            // Expand the Aero Glass Effect Border to the WHOLE form.
            // since we have already had the border invisible we now
            // have a completely invisible window - apart from the DirectX
            // renders NOT in black.
            // This prevents the form from appearing black as soon as something is rendered to the screen.
            Margins Form_Margins;
            Form_Margins.LeftBorder = 0;
            Form_Margins.TopBorder = 0;
            Form_Margins.RightBorder = this.Width;
            Form_Margins.BottomBorder = this.Height;
            DwmExtendFrameIntoClientArea(this.Handle, ref Form_Margins);
        }

        [DllImport("dwmapi.dll")]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);

        /// <summary>
        /// Sets the Heroes Overlay Window Size and Location
        /// </summary>
        public void Set_To_SonicHeroes_Window_Size()
        {
            // Adjust Size Accordingly
            this.Size = new Size(Heroes_Window_Rectangle.RightBorder - Heroes_Window_Rectangle.LeftBorder, Heroes_Window_Rectangle.BottomBorder - Heroes_Window_Rectangle.TopBorder);
            this.Top = Heroes_Window_Rectangle.TopBorder;
            this.Left = Heroes_Window_Rectangle.LeftBorder;
        }

        /// <summary>
        /// Makes your form a child form of the fake glass game overlay.
        /// </summary>
        /// <param name="Your_Form"></param>
        public void Attach_Windows_Form(Form Your_Form)
        {
            IntPtr User_Form_Handle_ID = Your_Form.Handle;
            IntPtr This_Form_Handle_ID = this.Handle;
            WINAPI_Components.SetParent(User_Form_Handle_ID, This_Form_Handle_ID);
            Your_Form.BringToFront(); // Bring to front
        }

        /// <summary>
        /// Makes your form a child form of the fake glass game overlay.
        /// </summary>
        /// <param name="Your_Form"></param>
        private void Attach_To_Game()
        {
            IntPtr This_Form_Handle_ID = this.Handle;
            WINAPI_Components.SetParent(This_Form_Handle_ID, Heroes_Window_Handle);
        }

        /// <summary>
        /// Do not paint the background normally, just a rectangle!
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e) { /*e.Graphics.FillRectangle(Brushes.LimeGreen, e.ClipRectangle);*/ }

        /// <summary>
        /// P/Invoke. Set the Windows Window event hook.
        /// </summary>
        /// <param name="eventMin"></param>
        /// <param name="eventMax"></param>
        /// <param name="hmodWinEventProc"></param>
        /// <param name="lpfnWinEventProc"></param>
        /// <param name="idProcess"></param>
        /// <param name="idThread"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
    }
}