using System.Buffers.Text;
using System.Drawing.Imaging;
using System.Net.Http.Headers;
using System.Reflection;
using System.Resources;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Web.WebView2;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace CaptureScreen {
    internal class TrayApplicationContext : ApplicationContext {
        private NotifyIcon notifyIcon;


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        public TrayApplicationContext() {
            SetProcessDPIAware();

            notifyIcon = new NotifyIcon() {
                Icon = (Icon)Program.Resources.GetObject("Icon"),
                Text = "Ta en skärmdump",
                Visible = true
            };

            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        }

        private async void NotifyIcon_DoubleClick(object? sender, EventArgs e) {
            Bitmap bitmap = Program.GetVirtualBitmap(Program.GetScreenAsBitmap());

            Rectangle bounds = Program.GetVirtualScreenBounds();
            Rectangle selection = await Program.GetSelectionAsync(bitmap);

            Bitmap result = bitmap.Clone(selection, PixelFormat.DontCare);

            string path = Path.GetTempFileName() + ".png";

            result.Save(path, ImageFormat.Png);

            Clipboard.SetImage(result);

            new ToastContentBuilder()
                .AddText(selection.Left + "," + selection.Top + "," + selection.Width + "," + selection.Height)
                .AddText(bounds.Left + "," + bounds.Top + "," + bounds.Width + "," + bounds.Height)
                .AddText(bitmap.Width + "," + bitmap.Height)
                .AddInlineImage(new Uri("file://" + path))
                .Show();
        }
    }
}
