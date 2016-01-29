using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace SimpleEntry.ViewModels.Windows
{
    /// <summary>
    /// Defines the reposition behavior of a <see cref="Popup"/> control when the window to which it is attached is moved or resized.
    /// </summary>
    /// <remarks>
    /// This solution was influenced by the answers provided by <see href="http://stackoverflow.com/users/262204/nathanaw">NathanAW</see> and
    /// <see href="http://stackoverflow.com/users/718325/jason">Jason</see> to
    /// <see href="http://stackoverflow.com/questions/1600218/how-can-i-move-a-wpf-popup-when-its-anchor-element-moves">this</see> question.
    /// </remarks>
    public class RepositionPopupBehavior : Behavior<Popup>
    {
        #region Protected Methods

        /// <summary>
        /// Called after the behavior is attached to an <see cref="Behavior.AssociatedObject"/>.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            var window = Window.GetWindow(AssociatedObject.PlacementTarget);
            if (window == null) { return; }
            //位置移动
            window.LocationChanged += OnLocationChanged;
            //大小改变
            window.SizeChanged += OnSizeChanged;
            //窗口windowstate改变
            AssociatedObject.LayoutUpdated += OnLocationChanged;
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            //AssociatedObject.HorizontalOffset = 7;
            //AssociatedObject.VerticalOffset = -AssociatedObject.Height;
        }

        /// <summary>
        /// Called when the behavior is being detached from its <see cref="Behavior.AssociatedObject"/>, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            var window = Window.GetWindow(AssociatedObject.PlacementTarget);
            if (window == null) { return; }
            window.LocationChanged -= OnLocationChanged;
            window.SizeChanged -= OnSizeChanged;
            AssociatedObject.LayoutUpdated -= OnLocationChanged;
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Handles the <see cref="Window.LocationChanged"/> routed event which occurs when the window's location changes.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// An object that contains the event data.
        /// </param>
        private void OnLocationChanged(object sender, EventArgs e)
        {
            var offset = AssociatedObject.HorizontalOffset;
            AssociatedObject.HorizontalOffset = offset + 1;
            AssociatedObject.HorizontalOffset = offset;
        }

        /// <summary>
        /// Handles the <see cref="Window.SizeChanged"/> routed event which occurs when either then <see cref="Window.ActualHeight"/> or the
        /// <see cref="Window.ActualWidth"/> properties change value.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// An object that contains the event data.
        /// </param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var offset = AssociatedObject.HorizontalOffset;
            AssociatedObject.HorizontalOffset = offset + 1;
            AssociatedObject.HorizontalOffset = offset;
        }

        #endregion Private Methods
    }
}

    /// <summary>
    /// 上面那个类在窗口最大化时，Popup不能更新自身的位置，而这个类可以，但会导致闪烁
    /// Popup有阴影效果时，鼠标悬停在Popup.PlacementTarget会导致闪烁（Popup无法判断悬停的触发控件）
    /// 解决办法是把阴影效果去掉，或者把VerticalOffSet/HorizontalOffSet调大，使Popup与其PlacementTarget不重叠
    /// </summary>
//    public class RepositionPopupBehavior : Behavior<Popup>
//    {
//        private const int WM_MOVING = 0x0216;

//        // should be moved to a helper class
//        private DependencyObject GetTopmostParent(DependencyObject element)
//        {
//            var current = element;
//            var result = element;

//            while (current != null)
//            {
//                result = current;
//                current = (current is Visual || current is Visual3D) ?
//                   VisualTreeHelper.GetParent(current) :
//                   LogicalTreeHelper.GetParent(current);
//            }
//            return result;
//        }

//        protected override void OnAttached()
//        {
//            base.OnAttached();
//            AssociatedObject.LayoutUpdated += (sender, e) => Update();
//            AssociatedObject.Loaded += (sender, e) =>
//            {
//                var root = GetTopmostParent(AssociatedObject.PlacementTarget) as Window;
//                if (root != null)
//                {
//                    var helper = new WindowInteropHelper(root);
//                    var hwndSource = HwndSource.FromHwnd(helper.Handle);
//                    if (hwndSource != null)
//                    {
//                        hwndSource.AddHook(HwndMessageHook);
//                    }
//                }
//            };
//        }

//        private IntPtr HwndMessageHook(IntPtr hWnd,
//                int msg, IntPtr wParam,
//                IntPtr lParam, ref bool bHandled)
//        {
//            if (msg == WM_MOVING)
//            {
//                Update();
//            }
//            return IntPtr.Zero;
//        }

//        public void Update()
//        {
//            // force the popup to update it's position
//            var mode = AssociatedObject.Placement;
//            AssociatedObject.Placement = PlacementMode.Relative;
//            AssociatedObject.Placement = mode;
//        }
//    }
//}
