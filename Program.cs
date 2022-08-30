using System.Buffers.Text;
using System.Drawing.Imaging;
using System.Net.Http.Headers;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Web.WebView2;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace CaptureScreen
{
    internal static class Program
    {
        public static ResourceManager resource1;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            resource1 = new ResourceManager("CaptureScreen.Resource1", Assembly.GetExecutingAssembly());

            Application.Run(new TrayApplicationContext());
        }

        public static Bitmap GetBitmap()
        {
            Rectangle captureRectangle = Screen.AllScreens[0].Bounds;

            Bitmap captureBitmap = new Bitmap(captureRectangle.Width, captureRectangle.Height, PixelFormat.Format32bppArgb);
            
            Graphics captureGraphics = Graphics.FromImage(captureBitmap);
            
            captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);

            return captureBitmap;
        }

        public static string GetBase64EncodedImage(Image image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Png);

                byte[] imageBytes = memoryStream.ToArray();

                return Convert.ToBase64String(imageBytes);
            }
        }

        public static Image GetBase64DecodedImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                // Convert byte[] to Image
                ms.Write(imageBytes, 0, imageBytes.Length);
                return Image.FromStream(ms, true);
            }
        }
    }

    internal class TrayApplicationContext : ApplicationContext {
        private NotifyIcon notifyIcon;

        public TrayApplicationContext()
        {
            notifyIcon = new NotifyIcon()
            {
                Icon = (Icon)Program.resource1.GetObject("Icon"),
                Text = "Ta en skärmdump",
                Visible = true
            };

            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        }

        private async void NotifyIcon_DoubleClick(object? sender, EventArgs e)
        {
            Bitmap bitmap = Program.GetBitmap();

            Form form = new Form()
            {
                FormBorderStyle = FormBorderStyle.None,

                Width = bitmap.Width,
                Height = bitmap.Height,

                Left = 0,
                Top = 0
            };

            WebView2 webView2 = new WebView2()
            {
                Dock = DockStyle.Fill
            };

            form.Controls.Add(webView2);

            await webView2.EnsureCoreWebView2Async();

            webView2.CoreWebView2.Navigate("file://C:\\Users\\nora.salehson\\Documents\\Innovations Tid\\CaptureScreen\\document\\index.html");

            webView2.NavigationCompleted += async (object? sender, CoreWebView2NavigationCompletedEventArgs e) => {
                await webView2.ExecuteScriptAsync(String.Format("set(\"{0}\")", Program.GetBase64EncodedImage(bitmap)));

                form.Show();

                webView2.NavigationStarting += (object? sender, CoreWebView2NavigationStartingEventArgs e) =>
                {
                    form.Close();

                    string result = e.Uri.Substring("result://".Length);

                    Image image = Program.GetBase64DecodedImage(result);

                    string temp = Path.GetTempFileName();
                    image.Save(temp);

                    Clipboard.SetDataObject(image);

                    new ToastContentBuilder()
                        .AddText("Your capture is copied to your clipboard!")
                        .AddInlineImage(new Uri("file://" + temp))
                        .Show();

                    form.Dispose();
                };
            };
        }
    }

}