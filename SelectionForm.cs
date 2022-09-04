using System;
using System.Collections.Generic;
using System.Text;

namespace CaptureScreen {
    internal class SelectionForm : Form {
        private Point mouseStart;
        private Point mouseCurrent;

        private bool mouseMoving = true;

        private Pen pen;

        private Panel panel;

        public SelectionForm(Bitmap bitmap) : base() {
            DoubleBuffered = true;

            pen = new Pen(Color.FromArgb(128, Color.Black), 5);

            Left = 0;
            Top = 0;

            Width = bitmap.Width;
            Height = bitmap.Height;

            FormBorderStyle = FormBorderStyle.None;

            StartPosition = FormStartPosition.Manual;
            AutoScaleMode = AutoScaleMode.Dpi;

            PictureBox background = new PictureBox() {
                Image = bitmap,

                Left = 0,
                Top = 0,

                Width = bitmap.Width,
                Height = bitmap.Height
            };

            background.MouseDown += SelectionForm_MouseDown;
            background.MouseMove += SelectionForm_MouseMove;
            background.MouseUp += SelectionForm_MouseUp;

            Controls.Add(background);

            panel = new Panel() {
                BackColor = Color.Black,
                Visible = false
            };

            background.Controls.Add(panel);
        }

        private void SelectionForm_MouseDown(object? sender, MouseEventArgs e) {
            mouseMoving = true;

            mouseStart = e.Location;
            mouseCurrent = e.Location;

            panel.Visible = true;
        }
        private void SelectionForm_MouseMove(object? sender, MouseEventArgs e) {
            if (mouseMoving == false)
                return;

            mouseCurrent = e.Location;

            panel.Location = mouseStart;
            panel.Size = new Size(mouseCurrent.X - mouseStart.X, mouseCurrent.Y - mouseStart.Y);
        }

        private void SelectionForm_MouseUp(object? sender, MouseEventArgs e) {
            panel.Visible = false;

            mouseMoving = false;
        }
    }
}
