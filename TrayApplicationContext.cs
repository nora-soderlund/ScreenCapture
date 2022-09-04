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

        public TrayApplicationContext() {
            notifyIcon = new NotifyIcon() {
                Icon = (Icon)Program.Resources.GetObject("Icon"),
                Text = "Ta en skärmdump",
                Visible = true
            };

            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        }

        private async void NotifyIcon_DoubleClick(object? sender, EventArgs e) {
            Bitmap bitmap = Program.GetScreenAsBitmap();

            Rectangle bounds = Program.GetVirtualScreenBounds();
            Rectangle selection = await Program.GetSelectionAsync(new Bitmap(bitmap, bounds.Size));

            float aspectRatioWidth = (float)bitmap.Width / (float)bounds.Width;
            float aspectRatioHeight = (float)bitmap.Height / (float)bounds.Height;

            selection.X = (int)(selection.X * aspectRatioWidth);
            selection.Y = (int)(selection.Y * aspectRatioHeight);

            selection.Width = (int)(selection.Width * aspectRatioWidth);
            selection.Height = (int)(selection.Height * aspectRatioHeight);

            Bitmap result = bitmap.Clone(selection, PixelFormat.DontCare);

            Clipboard.SetImage(result);

            string path = Path.GetTempFileName() + ".png";

            result.Save(path, ImageFormat.Png);

            new ToastContentBuilder()
                .AddText("Selection: " + selection.Left + "," + selection.Top + "," + selection.Width + "," + selection.Height)
                .AddText("Image size: " + bitmap.Width + "," + bitmap.Height + "; result size: " + result.Width + ", " + result.Height)
                .AddInlineImage(new Uri("file://" + path))
                .Show();
        }
    }
}
