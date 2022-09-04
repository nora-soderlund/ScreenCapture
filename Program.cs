using System.Buffers.Text;
using System.Drawing.Imaging;
using System.Net.Http.Headers;
using System.Reflection;
using System.Resources;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Web.WebView2;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace CaptureScreen {
    internal static class Program {
        public static ResourceManager Resources;

        public static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static SelectionForm SelectionForm;

        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Resources = new ResourceManager("CaptureScreen.Resources", Assembly.GetExecutingAssembly());

            SelectionForm = new SelectionForm();

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
            // Initialize the virtual screen to dummy values
            int screenLeft = int.MaxValue;
            int screenTop = int.MaxValue;
            int screenRight = int.MinValue;
            int screenBottom = int.MinValue;

            // Enumerate system display devices
            int deviceIndex = 0;
            while (true) {
                NativeUtilities.DisplayDevice deviceData = new NativeUtilities.DisplayDevice { cb = Marshal.SizeOf(typeof(NativeUtilities.DisplayDevice)) };
                if (NativeUtilities.EnumDisplayDevices(null, deviceIndex, ref deviceData, 0) != 0) {
                    // Get the position and size of this particular display device
                    NativeUtilities.DEVMODE devMode = new NativeUtilities.DEVMODE();
                    if (NativeUtilities.EnumDisplaySettings(deviceData.DeviceName, NativeUtilities.ENUM_CURRENT_SETTINGS, ref devMode)) {
                        // Update the virtual screen dimensions
                        screenLeft = Math.Min(screenLeft, devMode.dmPositionX);
                        screenTop = Math.Min(screenTop, devMode.dmPositionY);
                        screenRight = Math.Max(screenRight, devMode.dmPositionX + devMode.dmPelsWidth);
                        screenBottom = Math.Max(screenBottom, devMode.dmPositionY + devMode.dmPelsHeight);
                    }
                    deviceIndex++;
                }
                else
                    break;
            }

            // Create a bitmap of the appropriate size to receive the screen-shot.
            Bitmap bmp = new Bitmap(screenRight - screenLeft, screenBottom - screenTop);
               
            // Draw the screen-shot into our bitmap.
            using (Graphics g = Graphics.FromImage(bmp))
                g.CopyFromScreen(screenLeft, screenTop, 0, 0, bmp.Size);

            return bmp;
        }

        public static Bitmap GetVirtualBitmap(Bitmap bitmap) {
            Rectangle bounds = GetVirtualScreenBounds();

            return new Bitmap(bitmap, new Size(bounds.Width, bounds.Height));
        }

        public static async Task<Rectangle> GetSelectionAsync(Bitmap bitmap) {
            return await Program.SelectionForm.GetRectangleAsync(bitmap);
        }
    }
}
