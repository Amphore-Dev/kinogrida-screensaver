using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Kinogrida
{
    public class PreviewForm : ScreensaverForm
    {
        [DllImport("user32.dll")] static extern bool   GetClientRect(IntPtr hwnd, out RECT rect);
        [DllImport("user32.dll")] static extern IntPtr SetParent(IntPtr child, IntPtr parent);
        [DllImport("user32.dll")] static extern int    SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")] static extern int    GetWindowLong(IntPtr hwnd, int nIndex);
        [DllImport("user32.dll")] static extern bool   MoveWindow(IntPtr hwnd, int x, int y, int w, int h, bool repaint);

        [StructLayout(LayoutKind.Sequential)]
        struct RECT { public int Left, Top, Right, Bottom; }

        const int GWL_STYLE = -16;
        const int WS_CHILD  = 0x40000000;
        const int WS_POPUP  = unchecked((int)0x80000000);

        private readonly IntPtr _previewHwnd;

        public PreviewForm(IntPtr previewHwnd) : base(Rectangle.Empty, isPreview: true)
        {
            _previewHwnd = previewHwnd;
            GetClientRect(previewHwnd, out RECT r);
            Size = new Size(Math.Max(r.Right, 100), Math.Max(r.Bottom, 100));
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Parent  = _previewHwnd;
                cp.Style  |= WS_CHILD;
                cp.Style  &= ~WS_POPUP;
                return cp;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            int style = GetWindowLong(Handle, GWL_STYLE);
            SetWindowLong(Handle, GWL_STYLE, (style | WS_CHILD) & ~WS_POPUP);
            SetParent(Handle, _previewHwnd);
            GetClientRect(_previewHwnd, out RECT r);
            MoveWindow(Handle, 0, 0, r.Right, r.Bottom, true);
        }
    }
}
