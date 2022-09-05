using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Web.WebView2;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace CaptureScreen {
    internal class SelectionForm : Form {
        private Point mouseStart;
        private Point mouseCurrent;

        private bool mouseMoving = true;

        private Pen pen;

        private WebView2 webView2;

        public SelectionForm() {
            DoubleBuffered = true;

            pen = new Pen(Color.FromArgb(128, Color.Black), 5);

            Left = 0;
            Top = 0;

            ShowInTaskbar = false;

            FormBorderStyle = FormBorderStyle.None;

            StartPosition = FormStartPosition.Manual;
            AutoScaleMode = AutoScaleMode.Dpi;

            webView2 = new WebView2() {
                Left = 0,
                Top = 0,

                DefaultBackgroundColor = Color.Transparent
            };

            webView2.CoreWebView2InitializationCompleted += (object? sender, CoreWebView2InitializationCompletedEventArgs e) => {
                webView2.CoreWebView2.SetVirtualHostNameToFolderMapping("selection", Path.Combine(Program.Location, "UI/selection/"), CoreWebView2HostResourceAccessKind.DenyCors);
            };

            Controls.Add(webView2);
        }

        public async Task<Rectangle> GetSelectionAsync(Bitmap image) {
            TaskCompletionSource<Rectangle> task = new TaskCompletionSource<Rectangle>();

            Size = image.Size;
            webView2.Size = image.Size;

            BackgroundImage = image;

            await webView2.EnsureCoreWebView2Async();

            webView2.CoreWebView2.Navigate("https://selection/index.html");

            webView2.NavigationCompleted += (object? sender, CoreWebView2NavigationCompletedEventArgs e) => {
                Show();

                webView2.NavigationStarting += (object? sender, CoreWebView2NavigationStartingEventArgs e) => {
                    e.Cancel = true;

                    Close();

                    string[] result = e.Uri.Substring("result://".Length).Split(',');

                    task.SetResult(new Rectangle(
                        int.Parse(result[0]),
                        int.Parse(result[1]),
                        int.Parse(result[2]),
                        int.Parse(result[3])
                    ));
                };
            };

            return await task.Task;
        }
    }
}
