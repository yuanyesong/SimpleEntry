using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace SimpleEntry.ViewModels.Windows
{
    public static class FormHelper
    {
        /// <summary>
        /// 子窗体自动显示在父窗体中心位置
        /// </summary>
        /// <param name="owner">要中心化子窗体的窗体</param>
        /// <remarks>扩展方法</remarks>
        public static void CenterChild(this Window owner)
        {
            CenterChildHelper helper = new CenterChildHelper();
            helper.Run(owner);
        }

        /// <summary>
        /// 基于Hook实现子窗体自动显示在父窗体中心位置
        /// </summary>
        private class CenterChildHelper
        {
            private const Int32 WH_CBT = 5;
            private const Int32 HCBT_ACTIVATE = 5;
            private const Int32 GWL_HINSTANCE = -6;

            private IntPtr _hhk;    // 钩子句柄
            private IntPtr _parent; // 父窗体句柄
            private GCHandle _gch;

            public void Run(Window owner)
            {
                NativeMethods.CBTProc CenterChildHookProc = new NativeMethods.CBTProc(CenterChildCallBack);

                // 分配新的GCHandle，保护对象不被垃圾回收
                _gch = GCHandle.Alloc(CenterChildHookProc);

                _parent = new WindowInteropHelper(owner).Handle; // 父窗体句柄

                // 注意：dwThreadId为System.Threading.Thread.CurrentThread.ManagedThreadId不起作用
                _hhk = NativeMethods.SetWindowsHookEx(WH_CBT, CenterChildHookProc, IntPtr.Zero, NativeMethods.GetCurrentThreadId());
            }

            private IntPtr CenterChildCallBack(Int32 nCode, IntPtr wParam, IntPtr lParam)
            {
                if (nCode == HCBT_ACTIVATE)
                {   // 父窗体
                    NativeMethods.RECT formRect;
                    NativeMethods.GetWindowRect(_parent, out formRect);

                    // 子窗体
                    NativeMethods.RECT messageBoxRect;
                    NativeMethods.GetWindowRect(wParam, out messageBoxRect);

                    Int32 width = messageBoxRect.right - messageBoxRect.left;       // 消息窗体宽度
                    Int32 height = messageBoxRect.bottom - messageBoxRect.top;      // 消息窗体高度
                    Int32 xPos = (formRect.left + formRect.right - width) >> 1;     // 消息窗体位于父窗体中心时左上角X坐标
                    Int32 yPos = (formRect.top + formRect.bottom - height) >> 1;    // 消息窗体位于父窗体中心时左上角Y坐标

                    // 将消息窗体移到父窗体中心
                    NativeMethods.MoveWindow(wParam, xPos, yPos, width, height, false);

                    // 卸载钩子
                    NativeMethods.UnhookWindowsHookEx(_hhk);

                    // 释放已分配的GCHandle
                    _gch.Free();
                }

                // 允许操作
                return IntPtr.Zero;
            }
        }

        private static class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            internal struct RECT
            {
                public Int32 left;
                public Int32 top;
                public Int32 right;
                public Int32 bottom;
            }

            /// <summary>
            /// CBTProc委托声明
            /// </summary>
            /// <param name="nCode">HCBT_ACTIVATE：系统将要激活一个窗口</param>
            /// <param name="wParam">要激活的窗口句柄</param>
            /// <param name="lParam">指向CBTACTIVATESTRUCT结构</param>
            /// <returns>
            ///     0：执行这个操作
            ///     1：阻止这个操作
            /// </returns>
            internal delegate IntPtr CBTProc(Int32 nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr SetWindowsHookEx(Int32 idHook, CBTProc lpfn, IntPtr hMod, Int32 dwThreadId);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern Boolean UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern Boolean GetWindowRect(IntPtr hWnd, out RECT lpRect);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern Boolean MoveWindow(IntPtr hWnd, Int32 X, Int32 Y, Int32 nWidth, Int32 nHeight, [MarshalAs(UnmanagedType.Bool)]Boolean bRepaint);

            [DllImport("kernel32.dll")]
            internal static extern Int32 GetCurrentThreadId();
        }
    }
}
