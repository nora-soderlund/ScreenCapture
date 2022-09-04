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
    internal static class Program {
        public static ResourceManager Resources;

        public static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main() {
            SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Resources = new ResourceManager("CaptureScreen.Resources", Assembly.GetExecutingAssembly());

            Application.Run(new TrayApplicationContext());
        }

        public static Rectangle GetScreenBounds() {
            Rectangle r = new Rectangle();

            foreach (Screen s in Screen.AllScreens)
                r = Rectangle.Union(r, s.Bounds);

            return r;
        }

        public static Rectangle GetVirtualScreenBounds() {
            return new Rectangle(
                SystemInformation.VirtualScreen.Left,
                SystemInformation.VirtualScreen.Top,
                SystemInformation.VirtualScreen.Width,
                SystemInformation.VirtualScreen.Height
            );
        }

        public static Bitmap GetScreenAsBitmap() {
            Rectangle bounds = GetVirtualScreenBounds();

            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);

            using(Graphics graphics = Graphics.FromImage(bitmap))
                graphics.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size);

            return bitmap;
        }

        public static Bitmap GetVirtualBitmap(Bitmap bitmap) {
            Rectangle bounds = GetVirtualScreenBounds();

            return new Bitmap(bitmap, new Size(bounds.Width, bounds.Height));
        }

        public static async Task<Rectangle> GetSelectionAsync(Bitmap bitmap) {
            var task = new TaskCompletionSource<Rectangle>();

            SelectionForm form = new SelectionForm(bitmap);

            form.Show();

            /*WebView2 webView2 = new WebView2() {
                Width = bitmap.Width,
                Height = bitmap.Height,

                DefaultBackgroundColor = Color.Transparent
            };

            form.Controls.Add(webView2);

            await webView2.EnsureCoreWebView2Async();

            webView2.CoreWebView2.SetVirtualHostNameToFolderMapping("selection", Path.Combine(Location, "UI/selection/"), CoreWebView2HostResourceAccessKind.DenyCors);
            webView2.CoreWebView2.Navigate("https://selection/index.html");

            webView2.NavigationCompleted += async (object? sender, CoreWebView2NavigationCompletedEventArgs e) => {
                form.Show();

                webView2.NavigationStarting += (object? sender, CoreWebView2NavigationStartingEventArgs e) => {
                    form.Close();

                    string[] result = e.Uri.Substring("result://".Length).Split(',');

                    form.Dispose();

                    task.SetResult(new Rectangle(
                        int.Parse(result[0]),
                        int.Parse(result[1]),
                        int.Parse(result[2]),
                        int.Parse(result[3])
                    ));
                };
            };*/

            return await task.Task;
        }
    }
}
