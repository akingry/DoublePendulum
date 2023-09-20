using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace DoublePendulum
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.BackColor = Color.Black;
        }

        int fulcrumX;  
        int fulcrumY; 
        
        static double g = 9.81;
        static double l1 = 3.0, l2 = l1 / 2;
        static double m1 = 2.0, m2 = 3.0;
        double theta1 = Math.PI / 1.12, omega1 = 0.0;
        double theta2 = Math.PI / 2, omega2 = 0.0;
        double dt = 0.01;
        
        List<Point> tipPoints = new List<Point>();
        float hue = 0.0f;
        List<Color> colors = new List<Color>();
        int counter = 1;        

        public Color ColorFromHue(float hue)
        {
            float Clamp(float value, float min, float max)
            {
                if (value < min) return min;
                if (value > max) return max;
                return value;
            }

            float R = Math.Abs(hue * 6 - 3) - 1;
            float G = 2 - Math.Abs(hue * 6 - 2);
            float B = 2 - Math.Abs(hue * 6 - 4);

            R = Clamp(R, 0, 1);
            G = Clamp(G, 0, 1);
            B = Clamp(B, 0, 1);

            return Color.FromArgb(
                (int)(R * 255),
                (int)(G * 255),
                (int)(B * 255)
            );
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            fulcrumX = this.Width / 2;
            fulcrumY = this.Height / 2;

            Timer timer = new Timer();
            timer.Interval = (int)(dt * 200);
            timer.Tick += (s, ee) =>
            {
                RungeKutta(ref theta1, ref omega1, ref theta2, ref omega2, dt);
                counter++;
                this.Refresh();
            };
            timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int x_offset = fulcrumX;
            int y_offset = fulcrumY;

            int x1 = x_offset + (int)(l1 * Math.Sin(theta1) * 100);
            int y1 = y_offset + (int)(l1 * Math.Cos(theta1) * 100);

            int x2 = x1 + (int)(l2 * Math.Sin(theta2) * 100);
            int y2 = y1 + (int)(l2 * Math.Cos(theta2) * 100);
                        
            g.DrawLine(Pens.White, x_offset, y_offset, x1, y1);
            g.DrawLine(Pens.White, x1, y1, x2, y2);

            if (counter % 5 == 0)
            {
                hue += 0.01f;
                if (hue > 1.0f) hue = 0.0f;
                colors.Add(ColorFromHue(hue));
                tipPoints.Add(new Point(x2, y2));
            }
                        
            while (tipPoints.Count > 300)
            {
                tipPoints.RemoveAt(0);
                colors.RemoveAt(0);
            }
            
            for (int i = 0; i < tipPoints.Count; i++)
            {
                Brush brush = new SolidBrush(colors[i]);
                g.FillEllipse(brush, tipPoints[i].X - 5, tipPoints[i].Y - 5, 10, 10);
            }
        }

        public (double, double, double, double) Derivatives(double theta1, double omega1, double theta2, double omega2)
        {
            double deltaTheta = theta2 - theta1;
            double denom1 = (m1 + m2) * l1 - m2 * l1 * Math.Cos(deltaTheta) * Math.Cos(deltaTheta);
            double denom2 = (l1 / l2) * denom1;

            double theta1DotDot = ((m2 * l2 * omega2 * omega2 * Math.Sin(deltaTheta) * Math.Cos(deltaTheta)) +
                                   (m2 * g * Math.Sin(theta2) * Math.Cos(deltaTheta)) +
                                   (m2 * l2 * omega2 * omega2 * Math.Sin(deltaTheta)) -
                                   ((m1 + m2) * g * Math.Sin(theta1))) / denom1;

            double theta2DotDot = (-l1 / l2 * omega1 * omega1 * Math.Sin(deltaTheta) * Math.Cos(deltaTheta) +
                                   (m1 + m2) * g * Math.Sin(theta1) * Math.Cos(deltaTheta) -
                                   (m1 + m2) * l1 * omega1 * omega1 * Math.Sin(deltaTheta) -
                                   (m1 + m2) * g * Math.Sin(theta2)) / denom2;
            
            //double dampingConstant = 0.05;

            //theta1DotDot -= dampingConstant * omega1;
            //theta2DotDot -= dampingConstant * omega2;

            return (theta1DotDot, theta2DotDot, omega1, omega2);
        }

        public void RungeKutta(ref double theta1, ref double omega1, ref double theta2, ref double omega2, double dt)
        {
            var (k1_1, k1_2, _, _) = Derivatives(theta1, omega1, theta2, omega2);

            var (k2_1, k2_2, _, _) = Derivatives(theta1 + dt / 2.0 * omega1, omega1 + dt / 2.0 * k1_1, theta2 + dt / 2.0 * omega2, omega2 + dt / 2.0 * k1_2);

            var (k3_1, k3_2, _, _) = Derivatives(theta1 + dt / 2.0 * omega1, omega1 + dt / 2.0 * k2_1, theta2 + dt / 2.0 * omega2, omega2 + dt / 2.0 * k2_2);

            var (k4_1, k4_2, _, _) = Derivatives(theta1 + dt * omega1, omega1 + dt * k3_1, theta2 + dt * omega2, omega2 + dt * k3_2);

            theta1 += dt / 6.0 * (omega1 + 2 * (omega1 + dt / 2.0 * k1_1 + omega1 + dt / 2.0 * k2_1) + omega1 + dt * k3_1);
            omega1 += dt / 6.0 * (k1_1 + 2 * (k2_1 + k3_1) + k4_1);
            theta2 += dt / 6.0 * (omega2 + 2 * (omega2 + dt / 2.0 * k1_2 + omega2 + dt / 2.0 * k2_2) + omega2 + dt * k3_2);
            omega2 += dt / 6.0 * (k1_2 + 2 * (k2_2 + k3_2) + k4_2);
        }
    }
}
