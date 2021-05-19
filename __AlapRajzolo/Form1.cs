using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace __AlapRajzolo
{
    public partial class Form1 : Form
    {
        Graphics g;
        Color colorControl = Color.Black;
        Color colorCurve = Color.Blue;
        Color colorCurve2 = Color.Red;
        List<PointF> P = new List<PointF>();
        int grab = -1;
        float width;
        float heigth;
        float dx=0, dy=0;
        bool gotcha1 = false;
        bool gotcha2 = false;

        bool doubleCLick = false;

        public Form1()
        {
            InitializeComponent();
            
            width = canvas.Width;
            heigth = canvas.Height;
            
            
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
            
            if (gotcha1)
            {
                g.DrawLine(new Pen(Color.Red,3f), P[grab - 1], Add(Subs(P[grab - 2], P[grab - 1]), P[grab - 1]));
            }
            if (gotcha2)
            {
                g.DrawLine(new Pen(Color.Red, 3f), P[grab +1], Add(Subs(P[grab + 2], P[grab + 1]), P[grab + 1]));
            }
            for (int i = 0; i < P.Count-3; i+=3)
            {
                DrawBezier3Arc(new Pen(colorCurve,3f),P[i],P[i+1],P[i+2],P[i+3]);
            }

            for (int i = 0; i < P.Count; i++)
            {
                
                if  (i > 1 && (i) % 3 == 0)
                    g.FillEllipse(new SolidBrush(Color.Yellow), P[i].X - 5, P[i].Y - 5, 10, 10);
                else if (i > 1 && (i+1) % 3 == 0)
                    g.FillEllipse(new SolidBrush(Color.LightBlue), P[i].X - 5, P[i].Y - 5, 10, 10);
                else if(i>1 &&(i-1)%3==0)
                    g.FillEllipse(new SolidBrush(Color.Red), P[i].X - 5, P[i].Y - 5, 10, 10);
                
                else g.FillRectangle(new SolidBrush(colorControl),P[i].X-5,P[i].Y-5,10,10);
            }

            for (int i = 0; i < P.Count-1; i++)
            {
                g.DrawLine(new Pen(colorControl),P[i],P[i+1]);
            }
        }

        #region Mouse Handling
        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            
            for (int i = 0; i < P.Count; i++)
            {
                if (IsGrab(P[i],e.Location))
                {
                    grab = i;
                    
                }
            }
            if (grab==-1)
            {
                PointF point;
                PointF location = e.Location;
                
                

                //A harmadik pont megfogása
                 if (P.Count >1 && (P.Count-1) % 3 == 0 && P.Count != 0)
                {
                    //
                    
                    float m = (P[P.Count - 1].Y - P[P.Count - 2].Y) / (P[P.Count - 1].X - P[P.Count - 2].X);
                    float b = P[P.Count - 1].Y - m * P[P.Count - 1].X;

                    if (P[P.Count - 1].X < P[P.Count - 2].X && (location.Y - b) / m < 0)
                        point = new PointF(0, (location.Y));
                    else if (P[P.Count - 1].X > P[P.Count - 2].X && (location.Y - b) / m > width)
                        point = new PointF(width, (location.Y));

                    else {
                        if (P[P.Count - 2].Y > P[P.Count - 1].Y && location.Y > P[P.Count - 1].Y)
                        {
                            point = new PointF((P[P.Count-1].Y-20 - b) / m, (P[P.Count - 1].Y - 20));
                        }
                        else if (P[P.Count - 2].Y < P[P.Count - 1].Y && location.Y < P[P.Count - 1].Y)
                            point = new PointF((P[P.Count - 1].Y + 20 - b) / m, (P[P.Count - 1].Y + 20));

                        else point = new PointF((location.Y - b) / m, (location.Y));
                    }
                    P.Add(point);
                }
                    else
                    {
                        P.Add(e.Location);
                    }

               
                
               

                
                grab = P.Count - 1;
                canvas.Refresh();
                
            }
        }
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (grab!=-1)
            {
                if (grab != P.Count - 1 && grab >= 3 && grab != 1 && (grab) % 3 == 0)
                {
                    //A köztes pont megfogása
                    dx = e.X - P[grab].X;
                    dy = e.Y - P[grab].Y;

                    P[grab] = e.Location;

                    PointF tmp = new PointF(P[grab + 1].X + (dx), P[grab + 1].Y + (dy));

                    P[grab + 1] = tmp;
                    tmp = new PointF(P[grab - 1].X + (dx), P[grab - 1].Y + (dy));
                    P[grab - 1] = tmp;
                }

                //Az i-edik görbe harmadik pontja 
                else if ( grab <= P.Count - 3 && (grab + 1) % 3 == 0)
                {
                   

                    if (e.Button == MouseButtons.Right)
                    {
                        //A segédszakasz kirajzolásához egy változó
                        gotcha2 = true;

                        //Az i+1-edik görbe második pontja
                        float m = (P[grab + 1].Y - P[grab + 2].Y) / (P[grab + 1].X - P[grab + 2].X);
                        float b = P[grab + 1].Y - m * P[grab + 1].X;

                        //Elkerüljük a pontnak való nekiütközést
                        if ((P[grab + 1].Y > P[grab + 2].Y && e.Location.Y > P[grab + 1].Y + 5)||
                            (P[grab+1].Y<P[grab+2].Y&&e.Location.Y<P[grab+1].Y-5))
                        {
                            PointF point = new PointF((e.Y - b) / m, (e.Y));
                            P[grab] = point;
                        }
                    }
                    if (e.Button == MouseButtons.Left)
                    {
                        
                        //Arány kiszámítása
                        float arany = Math.Abs((float)Math.Sqrt(Math.Pow(P[grab + 1].X - P[grab + 2].X, 2) + Math.Pow(P[grab + 1].Y - P[grab + 2].Y, 2)) /
                            (float)Math.Sqrt(Math.Pow(P[grab].X - P[grab + 1].X, 2) + Math.Pow(P[grab].Y - P[grab + 1].Y, 2)));
                        ;

                        dx = P[grab].X - P[grab + 1].X;
                        dy = P[grab].Y - P[grab + 1].Y;


                        //Az aránnyal meg kell szoroznunk a távolságot, hiszen nem ugyan azon a köríven mozognak

                        PointF tmp = new PointF(P[grab + 1].X - (dx * arany), P[grab + 1].Y - (dy * arany));
                        P[grab] = e.Location;


                        P[grab + 2] = tmp;
                    }


                }

                //Az i+1 görbe második pontja 
                else if (  grab !=1&&(grab - 1) % 3 == 0)
                {
                    if (e.Button == MouseButtons.Right) {
                        //A segédszakasz kirajzolásához egy változó
                        gotcha1 = true;

                        //Az i+1-edik görbe második pontja
                        float m = (P[grab - 1].Y - P[grab - 2].Y) / (P[grab - 1].X - P[grab - 2].X);
                        float b = P[grab - 1].Y - m * P[grab - 1].X;
                        if ((P[grab - 1].Y > P[grab - 2].Y && e.Location.Y > P[grab - 1].Y + 5) ||
                            (P[grab - 1].Y < P[grab - 2].Y && e.Location.Y < P[grab - 1].Y - 5))
                        {
                            PointF point = new PointF((e.Y - b) / m, (e.Y));
                            P[grab] = point;
                        }
                    }
                    if (e.Button == MouseButtons.Left)
                    {
                        float arany = Math.Abs((float)Math.Sqrt(Math.Pow(P[grab - 1].X - P[grab - 2].X, 2) + Math.Pow(P[grab - 1].Y - P[grab - 2].Y, 2)) /
                        (float)Math.Sqrt(Math.Pow(P[grab].X - P[grab - 1].X, 2) + Math.Pow(P[grab].Y - P[grab - 1].Y, 2)));
                        ;

                        dx = P[grab].X - P[grab - 1].X;
                        dy = P[grab].Y - P[grab - 1].Y;


                        //Az aránnyal meg kell szoroznunk a távolságot, hiszen nem ugyan azon a köríven mozognak
                        float m = (P[P.Count - 1].Y - P[P.Count - 2].Y) / (P[P.Count - 1].X - P[P.Count - 2].X);
                        float b = P[P.Count - 1].Y - m * P[P.Count - 1].X;
                        PointF tmp = new PointF(P[grab - 1].X - (dx * arany), P[grab - 1].Y - (dy * arany));

                        P[grab] = e.Location;
                        P[grab - 2] = tmp;
                    }
                    
                }
                
                
                else
                    P[grab] = e.Location;

                canvas.Refresh();
            }
        }
        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            grab = -1;
            gotcha1 = false;
            gotcha2 = false;
            doubleCLick = false;
        }
        private void canvas_MouseWheel(object sender, MouseEventArgs e)
        {

        }
        #endregion

        private bool IsGrab(PointF p, PointF mouseLocation)
        {
            return p.X - 5 <= mouseLocation.X && mouseLocation.X <= p.X + 5 &&
                   p.Y - 5 <= mouseLocation.Y && mouseLocation.Y <= p.Y + 5;
        }

        private float Binom(int n,int k)
        {
            if (k == 0) return 1;
            if (k == n) return 1;
            if (n == 0) return 0;
            return Binom(n-1,k-1)+Binom(n-1,k);
        }



        private double B0(double t) { return (1 - t) * (1 - t) * (1 - t); }
        private double B1(double t) { return 3 * t * (1 - t) * (1 - t); }
        private double B2(double t) { return 3 * t * t * (1 - t); }
        private double B3(double t) { return t * t * t; }

        private void DrawBezier3Arc(Pen pen,
            PointF p0, PointF p1, PointF p2, PointF p3)
        {
            double a = 0;
            double t = a;
            double h = 1.0 / 500.0;
            PointF d0, d1;
            d0 = new PointF((float)(B0(t) * p0.X + B1(t) * p1.X + B2(t) * p2.X + B3(t) * p3.X),
                            (float)(B0(t) * p0.Y + B1(t) * p1.Y + B2(t) * p2.Y + B3(t) * p3.Y));
            while (t < 1)
            {
                t += h;
                d1 = new PointF((float)(B0(t) * p0.X + B1(t) * p1.X + B2(t) * p2.X + B3(t) * p3.X),
                                (float)(B0(t) * p0.Y + B1(t) * p1.Y + B2(t) * p2.Y + B3(t) * p3.Y));
                g.DrawLine(pen, d0, d1);
                d0 = d1;
            }
        }

        private PointF Add(PointF a, PointF b)
        {
            return new PointF(b.X + a.X, b.Y + a.Y);
        }
        private PointF Subs(PointF a, PointF b)
        {
            return new PointF(b.X - a.X, b.Y - a.Y);
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            P = new List<PointF>();
            dx = 0;
            dy = 0;
            grab = -1;
            gotcha1 = false;
            gotcha2 = false;
            canvas.Refresh();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void canvas_DoubleClick(object sender, EventArgs e)
        {
            doubleCLick = true;
        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private PointF Mult(PointF a, float l)
        {
            return new PointF(a.X * l, a.Y * l);
        }






    }
}
