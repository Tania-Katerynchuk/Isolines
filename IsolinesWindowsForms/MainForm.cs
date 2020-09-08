using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace IsolinesWindowsForms
{
    public partial class MainForm : Form
    {
        private const Int32 DEFAULT_POINTS_COUNT = 30;
        private Vector3[] m_Points;

        public MainForm()
        {
            m_Points = new Vector3[DEFAULT_POINTS_COUNT];

            InitializeComponent();

            Height = 660;
            Width = 640;
        }

        private void MainForm_Paint(Object sender, PaintEventArgs args)
        {
            BuildingIsolines();
        }

        private void MainForm_Click(Object sender, EventArgs args)
        {
            BuildingIsolines();
        }

        private void BuildingIsolines()
        {
            Graphics graphics;
            Pen axisPen;
            SolidBrush pointsBrush;
            Random random;

            axisPen = new Pen(Color.Gray, 1);
            pointsBrush = new SolidBrush(Color.Red);

            graphics = CreateGraphics();
            graphics.Clear(Color.White);

            graphics.TranslateTransform(310, 310);
            graphics.DrawLine(axisPen, 0, 310, 0, -310);
            graphics.DrawLine(axisPen, 310, 0, -310, 0);

            m_Points = new Vector3[DEFAULT_POINTS_COUNT];
            random = new Random();

            for (Int32 i = 0; i < m_Points.Length; i++)
            {
                m_Points[i] = new Vector3(random.Next(-300, 300), random.Next(-300, 300));
                m_Points[i].Z = i == 0 ? 1000 : (Int32)(1000 - LengthTwoVectors(m_Points[0], m_Points[i]));
                graphics.FillEllipse(pointsBrush, m_Points[i].X - 4, m_Points[i].Y - 4, 8, 8);
                //graphics.DrawString(m_Points[i].Z.ToString(), new Font("Helvetica", 9), Brushes.Black, m_Points[i].X, m_Points[i].Y);
            }

            DrawOutLine(graphics);
        }
        private void DrawOutLine(Graphics graphics)
        {
            List<Vector3> pointsOutLine;
            Int32 yMaxId, minId;
            Vector3 vector;
            Double currentAngle, minAngle;
            Pen outLinePen;
            List<Point> outLineDraw;
            List<Triangle> triangleDelaunay;

            outLinePen = new Pen(Color.Black, 1);
            pointsOutLine = new List<Vector3>();
            yMaxId = 0;

            for (Int32 i = 1; i < m_Points.Length; i++)
            {
                if (m_Points[yMaxId].Y > m_Points[i].Y)
                {
                    yMaxId = i;
                }
            }

            vector = new Vector3(10, 10);
            pointsOutLine.Add(m_Points[yMaxId]);

            do
            {
                minId = yMaxId == 0 ? 1 : 0;

                for (Int32 i = 0; i < m_Points.Length; i++)
                {
                    minAngle = AngleBetweenTwoVectors(vector, new Vector3(m_Points[yMaxId].X - m_Points[minId].X, m_Points[yMaxId].Y - m_Points[minId].Y));
                    currentAngle = AngleBetweenTwoVectors(vector, new Vector3(m_Points[yMaxId].X - m_Points[i].X, m_Points[yMaxId].Y - m_Points[i].Y));

                    if (yMaxId != i && minAngle < currentAngle)
                    {
                        minId = i;
                    }
                }

                pointsOutLine.Add(m_Points[minId]);
                vector.X = m_Points[yMaxId].X - m_Points[minId].X;
                vector.Y = m_Points[yMaxId].Y - m_Points[minId].Y;
                yMaxId = minId;
            } while (pointsOutLine[0] != m_Points[yMaxId]);


            outLineDraw = new List<Point>();

            pointsOutLine.ForEach(delegate (Vector3 p)
            {
                outLineDraw.Add(new Point((Int32)p.X, (Int32)p.Y));
            });

            graphics.DrawLines(outLinePen, outLineDraw.ToArray());
            outLineDraw.Clear();

            triangleDelaunay = TriangleDelaunay(pointsOutLine, graphics);

            ConstructionIsolines(triangleDelaunay, graphics);
        }
        private List<Triangle> TriangleDelaunay(List<Vector3> pointOutLine, Graphics graphics)
        {
            Pen trianglePen;
            List<Triangle> triangles;
            Queue<Vector3> pointsQueue;

            trianglePen = new Pen(Color.Purple, 1);
            triangles = new List<Triangle>();
            pointsQueue = new Queue<Vector3>();

            pointOutLine.ForEach(delegate (Vector3 p)
            {
                pointsQueue.Enqueue(p);
            });

            do
            {
                for (Int32 i = 0; i < m_Points.Length; i++)
                {
                    for (Int32 j = 0; j < m_Points.Length; j++)
                    {
                        if (i != j && pointsQueue.Peek() != m_Points[i] && pointsQueue.Peek() != m_Points[j] && (!pointOutLine.Contains(m_Points[i]) || !pointOutLine.Contains(m_Points[j])) && !ContainsTriangle(triangles, new Triangle(pointsQueue.Peek(), m_Points[i], m_Points[j])))
                        {
                            for (Int32 k = 0; k < m_Points.Length; k++)
                            {
                                if (k != i && k != j && pointsQueue.Peek() != m_Points[k] && !ConditionDelaunay(m_Points[k], pointsQueue.Peek(), m_Points[i], m_Points[j]))
                                {
                                    break;
                                }

                                if (k == m_Points.Length - 1)
                                {
                                    triangles.Add(new Triangle(pointsQueue.Peek(), m_Points[i], m_Points[j]));
                                    pointsQueue.Enqueue(m_Points[i]);
                                    pointsQueue.Enqueue(m_Points[j]);

                                    //graphics.DrawPolygon(trianglePen, new Point[] { new Point((Int32) pointsQueue.Peek().X, (Int32) pointsQueue.Peek().Y), new Point((Int32) m_Points[i].X, (Int32) m_Points[i].Y), new Point((Int32) m_Points[j].X, (Int32) m_Points[j].Y) });
                                }
                            }
                        }

                    }
                }

                if (!pointOutLine.Contains(pointsQueue.Peek()))
                {
                    pointOutLine.Add(pointsQueue.Peek());
                }

                pointsQueue.Dequeue();
            } while (pointsQueue.Count != 0);

            return triangles;
        }

        private void ConstructionIsolines(List<Triangle> triangles, Graphics graphics)
        {
            Color[] isolinesColor;
            Pen isolinePen;
            Single[] isolinesValue;
            Single min, max;
            Point[] isoline;
            Int32 minIndex, maxIndex, midIndex;

            isolinesColor = new Color[] {
                Color.FromArgb(255, 0, 0),
                Color.FromArgb(255, 64, 0),
                Color.FromArgb(255, 128, 0),
                Color.FromArgb(255, 191, 0),
                Color.FromArgb(255, 255, 0),
                Color.FromArgb(191, 255, 0),
                Color.FromArgb(128, 255, 0),
                Color.FromArgb(64, 255, 0),
                Color.FromArgb(0, 255, 0),
                Color.FromArgb(0, 255, 64),
                Color.FromArgb(0, 255, 128)};

            isolinePen = new Pen(isolinesColor[0], 1.5f);
            isolinesValue = new Single[] { 950, 900, 850, 800, 750, 700, 650, 600, 550, 500, 450 };

            for (Int32 i = 0; i < isolinesValue.Length; i++)
            {
                isolinePen.Color = isolinesColor[i];
                triangles.ForEach(delegate (Triangle t)
                {
                    min = Math.Min(Math.Min(t.Vectors[0].Z, t.Vectors[1].Z), t.Vectors[2].Z);
                    max = Math.Max(Math.Max(t.Vectors[0].Z, t.Vectors[1].Z), t.Vectors[2].Z);

                    if (min <= isolinesValue[i] && isolinesValue[i] <= max)
                    {
                        isoline = new Point[2];

                        minIndex = t.IndexOfZ(min);
                        maxIndex = t.IndexOfZ(max);
                        midIndex = t.MidIndexZ(minIndex, maxIndex);

                        isoline[0].X = CalculationValue(isolinesValue[i], min, max, t.Vectors[minIndex].X, t.Vectors[maxIndex].X);
                        isoline[0].Y = CalculationValue(isolinesValue[i], min, max, t.Vectors[minIndex].Y, t.Vectors[maxIndex].Y);

                        if (t.Vectors[midIndex].Z <= isolinesValue[i])
                        {
                            min = t.Vectors[midIndex].Z;
                            minIndex = midIndex;
                        }
                        else if (t.Vectors[midIndex].Z >= isolinesValue[i])
                        {
                            max = t.Vectors[midIndex].Z;
                            maxIndex = midIndex;
                        }

                        isoline[1].X = CalculationValue(isolinesValue[i], min, max, t.Vectors[minIndex].X, t.Vectors[maxIndex].X);
                        isoline[1].Y = CalculationValue(isolinesValue[i], min, max, t.Vectors[minIndex].Y, t.Vectors[maxIndex].Y);

                        //graphics.FillEllipse(Brushes.Blue, isoline[0].X - 4, isoline[0].Y - 4, 8, 8);
                        graphics.DrawPolygon(isolinePen, isoline);
                    }
                });
            }
        }

        private Int32 CalculationValue(Single z, Single zMin, Single zMax, Single min, Single max)
        {
            return Convert.ToInt32(((max - min) * (z - zMin)) / (zMax - zMin) + min);
        }

        private Boolean ContainsTriangle(List<Triangle> triangles, Triangle triangle)
        {
            Boolean isContains;
            Int32 pointsCount;

            isContains = false;

            triangles.ForEach(delegate (Triangle t)
            {
                pointsCount = 0;

                for (Int32 i = 0; i < 3; i++)
                {
                    for (Int32 j = 0; j < 3; j++)
                    {
                        if (t.Vectors[i] == triangle.Vectors[j])
                        {
                            pointsCount++;
                            break;
                        }
                    }
                }

                if (pointsCount == 3)
                {
                    isContains = true;
                }

            });

            return isContains;
        }

        private Double AngleBetweenTwoVectors(Vector3 vector1, Vector3 vector2)
        {
            return ((vector1.X * vector2.X + vector1.Y * vector2.Y) / (Math.Sqrt(Math.Pow(vector1.X, 2) + Math.Pow(vector1.Y, 2)) * Math.Sqrt(Math.Pow(vector2.X, 2) + Math.Pow(vector2.Y, 2))));
        }

        private Double LengthTwoVectors(Vector3 vector1, Vector3 vector2)
        {
            return Math.Sqrt(Math.Pow(vector2.X - vector1.X, 2) + Math.Pow(vector2.Y - vector1.Y, 2));
        }

        private Boolean ConditionDelaunay(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Double k, m, n, a, b, c, d;

            k = Math.Pow(v1.X, 2) + Math.Pow(v1.Y, 2);
            m = Math.Pow(v2.X, 2) + Math.Pow(v2.Y, 2);
            n = Math.Pow(v3.X, 2) + Math.Pow(v3.Y, 2);

            a = v1.X * (v2.Y - v3.Y) + v2.X * (v3.Y - v1.Y) + v3.X * (v1.Y - v2.Y);
            b = k * (v2.Y - v3.Y) + m * (v3.Y - v1.Y) + n * (v1.Y - v2.Y);
            c = k * (v2.X - v3.X) + m * (v3.X - v1.X) + n * (v1.X - v2.X);

            d = k * (v2.X * v3.Y - v3.X * v2.Y) + m * (v3.X * v1.Y - v1.X * v3.Y) + n * (v1.X * v2.Y - v2.X * v1.Y);

            return a * (Math.Pow(v0.X, 2) + Math.Pow(v0.Y, 2)) - b * v0.X + c * v0.Y >= d;
        }
    }
}
