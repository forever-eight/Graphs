using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystAnalys_lr1
{
    public class Condition
    {
        private List<Vertex> _v;
        private List<Line> _l;
        public List<Condition> ThisDeleteCondition = new List<Condition>();

        public List<Vertex> V
        {
            get => new List<Vertex>(_v);
            private set
            {
                _v = value;
            }
        }

        public List<Line> L
        {
            get => new List<Line>(_l);
            private set
            {
                _l = value;
            }
        }


        public Condition(List<Vertex> v, List<Line> l)
        {
            _v = new List<Vertex>(v);
            _l = new List<Line>(l);
        }
    }

    public class ListCondition
    {
        private List<Condition> ConditionArray;

        public List<Condition> Array
        {
            get => new List<Condition>(ConditionArray);
        }

        //коллекция удаленных состояний
        private List<Condition> DeleteCollection = new List<Condition>();

        public List<Condition> DeletedCollection
        {
            get => new List<Condition>(DeleteCollection);
        }

        public ListCondition()
        {
            ConditionArray = new List<Condition>();
        }

        public void RemoveFromEnd()
        {
            if (ConditionArray.Count != 0)
            {
                var elem = ConditionArray.Last().ThisDeleteCondition;
                if (elem.Count != 0)
                {
                    ConditionArray.Add(elem.Last());
                    elem.RemoveAt(elem.Count - 1);
                    ConditionArray.Last().ThisDeleteCondition = new List<Condition>(ConditionArray[ConditionArray.Count - 2].ThisDeleteCondition);
                    elem.Clear();
                }
                else
                {
                    DeleteCollection.Add(ConditionArray.Last());
                    ConditionArray.RemoveAt(ConditionArray.Count - 1);
                }
            }
        }

        public void Add(Condition condition)
        {
            if (DeleteCollection.Count != 0)
            {
                if (ConditionArray.Count == 0)
                {
                    ConditionArray.Add(new Condition(new List<Vertex>(), new List<Line>()));
                }

                ConditionArray.Last().ThisDeleteCondition = new List<Condition>(DeleteCollection);
                DeleteCollection.Clear();
            }

            ConditionArray.Add(condition);
        }

        public void Redo()
        {
            if (DeleteCollection.Count != 0)
            {
                ConditionArray.Add(DeleteCollection.Last());
                DeleteCollection.RemoveAt(DeleteCollection.Count - 1);
            }
        }
    }

    public class Vertex
    {
        public int x, y, Number;
        
        public Vertex(int x, int y, int number)
        {
            this.x = x;
            this.y = y;
            Number = number;            
        }
    }

    public class Line
    {
        public int v1, v2;
        public string Name;
        public int weight;
        public string direction;
        public Line(int v1, int v2, string name)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.Name = name;
        }
    }

    class DrawGraph
    {
        Bitmap bitmap;
        Pen blackPen;
        Pen redPen;
        Pen GreenPen;
        Graphics gr;
        Font fo;
        Brush br;
        PointF point;
        public int R = 14;

        public DrawGraph(int width, int height)
        {
            bitmap = new Bitmap(width, height);
            gr = Graphics.FromImage(bitmap);
            clearSheet();
            blackPen = new Pen(Color.Black);
            blackPen.Width = 2;
            redPen = new Pen(Color.Blue);
            redPen.Width = 2;
            GreenPen = new Pen(Color.Green);
            GreenPen.Width = 2;
            fo = new Font("Arial", 15);
            br = Brushes.Black;
        }

        public Bitmap GetBitmap()
        {
            return bitmap;
        }

        public void clearSheet()
        {
            gr.Clear(Color.White);
        }

        public void drawVertex(int x, int y, string number)
        {
            gr.FillEllipse(Brushes.White, (x - R), (y - R), 2 * R, 2 * R);
            gr.DrawEllipse(blackPen, (x - R), (y - R), 2 * R, 2 * R);
            point = new PointF(x - 9, y - 9);
            gr.DrawString(number, fo, br, point);
        }

        public void drawVertex(int x, int y, string number, Color color)
        {
            gr.DrawEllipse(blackPen, (x - R), (y - R), 2 * R, 2 * R);
            point = new PointF(x - 9, y - 9);
            gr.DrawString(number, fo, br, point);
        }

        public void drawSelectedVertex(int x, int y)
        {
            gr.DrawEllipse(redPen, (x - R), (y - R), 2 * R, 2 * R);
        }

        public void drawSelectedVertex(Pen color, int x, int y)
        {
            gr.DrawEllipse(color, (x - R), (y - R), 2 * R, 2 * R);
            gr.FillEllipse(color.Brush, (x - R), (y - R), 2 * R, 2 * R);
        }

        public void drawEdge(Vertex V1, Vertex V2, Line E, int numberE)
        {
            ///TODO:падает при закрытии!
                GreenPen.CustomEndCap = new AdjustableArrowCap(10, 10);

            if (E.v1 == E.v2)
            {
                gr.DrawArc(GreenPen, (V1.x - 2 * R), (V1.y - 2 * R), 2 * R, 2 * R, 90, 270);
                point = new PointF(V1.x - (int)(2.75 * R), V1.y - (int)(2.75 * R));
                gr.DrawString(((char)('a' + numberE)).ToString()+":"+E.weight.ToString(), fo, br, point);
                drawVertex(V1.x, V1.y, (E.v1 + 1).ToString());
            }
            else
            {
                if (E.direction == null)
                {
                    gr.DrawLine(new Pen(Color.Green,2f), V1.x, V1.y, V2.x, V2.y);
                }
                else
                {
                    string[] words = E.direction.Split(new char[] { ' ' });
                    int FVertexNumber = Convert.ToInt32(words[0]);
                    if (FVertexNumber == V1.Number)
                        gr.DrawLine(GreenPen, V1.x, V1.y, V2.x, V2.y);
                    else if (FVertexNumber == V2.Number)
                        gr.DrawLine(GreenPen, V2.x, V2.y, V1.x, V1.y);

                }

                point = new PointF((V1.x + V2.x) / 2, (V1.y + V2.y) / 2);
                gr.DrawString(((char)('a' + numberE)).ToString() + ":" + E.weight.ToString(), fo, br, point);
                drawVertex(V1.x, V1.y, (E.v1 + 1).ToString());
                drawVertex(V2.x, V2.y, (E.v2 + 1).ToString());
                
            }
        }

        public void DrawRedEdge(Vertex V1, Vertex V2)
        {
            Pen RedPen = new Pen(Color.Red);

            gr.DrawLine(RedPen, V1.x, V1.y, V2.x, V2.y);

            drawVertex(V1.x, V1.y, V1.Number.ToString());
            drawVertex(V2.x, V2.y, V2.Number.ToString());
        }

        public void drawALLGraph(List<Vertex> V, List<Line> E)
        {
            GreenPen.CustomEndCap = new AdjustableArrowCap(10, 10);
            for (int i = 0; i < E.Count; i++)
            {


                if (E[i].v1 == E[i].v2)
                {
                    gr.DrawArc(GreenPen, (V[E[i].v1].x - 2 * R), (V[E[i].v1].y - 2 * R), 2 * R, 2 * R, 90, 270);
                    point = new PointF(V[E[i].v1].x - (int)(2.75 * R), V[E[i].v1].y - (int)(2.75 * R));
                    gr.DrawString(((char)('a' + i)).ToString() + ":" + E[i].weight.ToString(), fo, br, point);
                }
                else
                {

                    if (E[i].direction == null)
                    {
                        gr.DrawLine(new Pen(Color.Green, 2f), V[E[i].v1].x, V[E[i].v1].y, V[E[i].v2].x, V[E[i].v2].y);
                        point = new PointF((V[E[i].v1].x + V[E[i].v2].x) / 2, (V[E[i].v1].y + V[E[i].v2].y) / 2);
                        gr.DrawString(((char)('a' + i)).ToString() + ":" + E[i].weight.ToString(), fo, br, point);
                    }
                    else
                    {
                        string[] words = E[i].direction.Split(new char[] { ' ' });
                        int FVertexNumber = Convert.ToInt32(words[0]);
                        if (FVertexNumber == V[E[i].v1].Number)
                            gr.DrawLine(GreenPen, V[E[i].v1].x, V[E[i].v1].y, V[E[i].v2].x, V[E[i].v2].y);
                        else if (FVertexNumber == V[E[i].v2].Number)
                            gr.DrawLine(GreenPen, V[E[i].v2].x, V[E[i].v2].y, V[E[i].v1].x, V[E[i].v1].y);

                    point = new PointF((V[E[i].v1].x + V[E[i].v2].x) / 2, (V[E[i].v1].y + V[E[i].v2].y) / 2);
                    gr.DrawString(((char)('a' + i)).ToString() + ":" + E[i].weight.ToString(), fo, br, point);
                    }
                }
            }
            for (int i = 0; i < V.Count; i++)
            {
                drawVertex(V[i].x, V[i].y, (i + 1).ToString());
            }
        }

        public void fillAdjacencyMatrix(int numberV, List<Line> E, int[,] matrix)
        {
            for (int i = 0; i < numberV; i++)
                for (int j = 0; j < numberV; j++)
                    matrix[i, j] = 0;
            for (int i = 0; i < E.Count; i++)
            {
                matrix[E[i].v1, E[i].v2] = 1;
                matrix[E[i].v2, E[i].v1] = 1;
            }
        }

        //заполняет матрицу инцидентности
        public void fillIncidenceMatrix(int numberV, List<Line> E, int[,] matrix)
        {
            for (int i = 0; i < numberV; i++)
                for (int j = 0; j < E.Count; j++)
                    matrix[i, j] = 0;
            for (int i = 0; i < E.Count; i++)
            {
                matrix[E[i].v1, i] = 1;
                matrix[E[i].v2, i] = 1;
            }
        }

        
    }
}