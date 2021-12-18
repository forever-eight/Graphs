using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace SystAnalys_lr1
{
    public partial class Form1 : Form
    {
        DrawGraph G;
        List<Vertex> V;
        List<Line> L;
        ListCondition ArrayCondition;
        int[,] AMatrix; //матрица смежности
        int[,] IMatrix; //матрица инцидентности
        int selected1; //выбранные вершины, для соединения линиями
        int selected2;

        public Form1()
        {
            InitializeComponent();
            V = new List<Vertex>();
            G = new DrawGraph(sheet.Width, sheet.Height);
            L = new List<Line>();
            ArrayCondition = new ListCondition();
            sheet.Image = G.GetBitmap();
        }

        //кнопка - выбрать вершину
        private void selectButton_Click(object sender, EventArgs e)
        {
            selectButton.Enabled = false;
            drawVertexButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            drag.Enabled = false;
            deleteButton.Enabled = true;
            G.clearSheet();
            G.drawALLGraph(V, L);
            sheet.Image = G.GetBitmap();
            selected1 = -1;
        }

        //кнопка - рисовать вершину
        private void drawVertexButton_Click(object sender, EventArgs e)
        {
            drawVertexButton.Enabled = false;
            selectButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            deleteButton.Enabled = true;
            G.clearSheet();
            G.drawALLGraph(V, L);
            sheet.Image = G.GetBitmap();
        }

        //кнопка - рисовать ребро
        private void drawEdgeButton_Click(object sender, EventArgs e)
        {
            drawEdgeButton.Enabled = false;
            selectButton.Enabled = true;
            drawVertexButton.Enabled = true;
            deleteButton.Enabled = true;
            G.clearSheet();
            G.drawALLGraph(V, L);
            sheet.Image = G.GetBitmap();
            selected1 = -1;
            selected2 = -1;

        }

        //кнопка - удалить элемент
        private void deleteButton_Click(object sender, EventArgs e)
        {
            deleteButton.Enabled = false;
            selectButton.Enabled = true;
            drawVertexButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            G.clearSheet();
            G.drawALLGraph(V, L);
            sheet.Image = G.GetBitmap();
        }

        //кнопка - удалить граф
        private void deleteALLButton_Click(object sender, EventArgs e)
        {
            selectButton.Enabled = true;
            drawVertexButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            deleteButton.Enabled = true;
            const string message = "Вы действительно хотите полностью удалить граф?";
            const string caption = "Удаление";
            var MBSave = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (MBSave == DialogResult.Yes)
            {
                V.Clear();
                L.Clear();
                G.clearSheet();
                sheet.Image = G.GetBitmap();
            }
        }

        //кнопка - матрица смежности
        private void buttonAdj_Click(object sender, EventArgs e)
        {
            createAdjAndOut();
        }

        private void buttonInc_Click(object sender, EventArgs e)
        {
            createIncAndOut();
        }

        private void sheet_MouseClick(object sender, MouseEventArgs e)
        {
            if (selectButton.Enabled == false)
            {
                for (int i = 0; i < V.Count; i++)
                {
                    if (Math.Pow((V[i].x - e.X), 2) + Math.Pow((V[i].y - e.Y), 2) <= G.R * G.R)
                    {
                        if (selected1 != -1)
                        {
                            selected1 = -1;
                            G.clearSheet();
                            G.drawALLGraph(V, L);
                            sheet.Image = G.GetBitmap();
                        }
                        if (selected1 == -1)
                        {
                            G.drawSelectedVertex(V[i].x, V[i].y);
                            selected1 = i;
                            sheet.Image = G.GetBitmap();
                            createAdjAndOut();
                            listBoxMatrix.Items.Clear();
                            int degree = 0;
                            for (int j = 0; j < V.Count; j++)
                                degree += AMatrix[selected1, j];
                            listBoxMatrix.Items.Add("Степень вершины №" + (selected1 + 1) + " равна " + degree);
                            break;
                        }
                    }
                }
            }
            if (drawVertexButton.Enabled == false)
            {
                V.Add(new Vertex(e.X, e.Y, V.Count + 1));
                G.drawVertex(e.X, e.Y, V.Count.ToString());
                ArrayCondition.Add(new Condition(V, L));
                sheet.Image = G.GetBitmap();
            }
            if (drawEdgeButton.Enabled == false)
            {
                if (e.Button == MouseButtons.Left)
                {
                    for (int i = 0; i < V.Count; i++)
                    {
                        if (Math.Pow((V[i].x - e.X), 2) + Math.Pow((V[i].y - e.Y), 2) <= G.R * G.R)
                        {
                            if (selected1 == -1)
                            {
                                G.drawSelectedVertex(V[i].x, V[i].y);
                                selected1 = i;
                                sheet.Image = G.GetBitmap();
                                break;
                            }
                            if (selected2 == -1)
                            {
                                G.drawSelectedVertex(V[i].x, V[i].y);
                                selected2 = i;
                                Line l = new Line(selected1, selected2, Convert.ToString((char)('a' + L.Count())));
                                L.Add(l);
                                SelectEdgeForm newForm = new SelectEdgeForm(l);
                                newForm.ShowDialog();

                                G.drawEdge(V[selected1], V[selected2], L[L.Count - 1], L.Count - 1);
                                selected1 = -1;
                                selected2 = -1;
                                ArrayCondition.Add(new Condition(V, L));
                                sheet.Image = G.GetBitmap();
                                break;
                            }
                        }
                    }
                    createAdjAndOut();
                }
                if (e.Button == MouseButtons.Right)
                {
                    if ((selected1 != -1) &&
                        (Math.Pow((V[selected1].x - e.X), 2) + Math.Pow((V[selected1].y - e.Y), 2) <= G.R * G.R))
                    {
                        G.drawVertex(V[selected1].x, V[selected1].y, (selected1 + 1).ToString());
                        selected1 = -1;
                        sheet.Image = G.GetBitmap();
                    }
                }
            }
            if (deleteButton.Enabled == false)
            {
                bool flag = false;
                for (int i = 0; i < V.Count; i++)
                {
                    if (Math.Pow((V[i].x - e.X), 2) + Math.Pow((V[i].y - e.Y), 2) <= G.R * G.R)
                    {
                        for (int j = 0; j < L.Count; j++)
                        {
                            if ((L[j].v1 == i) || (L[j].v2 == i))
                            {
                                L.RemoveAt(j);
                                j--;
                            }
                            else
                            {
                                if (L[j].v1 > i) L[j].v1--;
                                if (L[j].v2 > i) L[j].v2--;
                            }
                        }
                        V.RemoveAt(i);
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    for (int i = 0; i < L.Count; i++)
                    {
                        if (L[i].v1 == L[i].v2)
                        {
                            if ((Math.Pow((V[L[i].v1].x - G.R - e.X), 2) + Math.Pow((V[L[i].v1].y - G.R - e.Y), 2) <= ((G.R + 2) * (G.R + 2))) &&
                                (Math.Pow((V[L[i].v1].x - G.R - e.X), 2) + Math.Pow((V[L[i].v1].y - G.R - e.Y), 2) >= ((G.R - 2) * (G.R - 2))))
                            {
                                L.RemoveAt(i);
                                flag = true;
                                break;
                            }
                        }
                        else
                        {
                            if (((e.X - V[L[i].v1].x) * (V[L[i].v2].y - V[L[i].v1].y) / (V[L[i].v2].x - V[L[i].v1].x) + V[L[i].v1].y) <= (e.Y + 4) &&
                                ((e.X - V[L[i].v1].x) * (V[L[i].v2].y - V[L[i].v1].y) / (V[L[i].v2].x - V[L[i].v1].x) + V[L[i].v1].y) >= (e.Y - 4))
                            {
                                if ((V[L[i].v1].x <= V[L[i].v2].x && V[L[i].v1].x <= e.X && e.X <= V[L[i].v2].x) ||
                                    (V[L[i].v1].x >= V[L[i].v2].x && V[L[i].v1].x >= e.X && e.X >= V[L[i].v2].x))
                                {
                                    L.RemoveAt(i);
                                    flag = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (flag)
                {
                    G.clearSheet();
                    G.drawALLGraph(V, L);
                    ArrayCondition.Add(new Condition(V, L));
                    sheet.Image = G.GetBitmap();
                }
            }
        }
        private void createAdjAndOut()
        {
            AMatrix = new int[V.Count, V.Count];
            G.fillAdjacencyMatrix(V.Count, L, AMatrix);
            listBoxMatrix.Items.Clear();
            string sOut = "    ";
            for (int i = 0; i < V.Count; i++)
                sOut += (i + 1) + "   ";
            listBoxMatrix.Items.Add(sOut);
            for (int i = 0; i < V.Count; i++)
            {
                sOut = (i + 1) + " | ";
                for (int j = 0; j < V.Count; j++)
                    sOut += AMatrix[i, j] + "   ";
                listBoxMatrix.Items.Add(sOut);
            }
        }
        private void createIncAndOut()
        {
            if (L.Count > 0)
            {
                IMatrix = new int[V.Count, L.Count];
                G.fillIncidenceMatrix(V.Count, L, IMatrix);
                listBoxMatrix.Items.Clear();
                string sOut = "    ";
                for (int i = 0; i < L.Count; i++)
                    sOut += (char)('a' + i) + "   ";
                listBoxMatrix.Items.Add(sOut);
                for (int i = 0; i < V.Count; i++)
                {
                    sOut = (i + 1) + " | ";
                    for (int j = 0; j < L.Count; j++)
                        sOut += IMatrix[i, j] + "   ";
                    listBoxMatrix.Items.Add(sOut);
                }
            }
            else
                listBoxMatrix.Items.Clear();
        }

        //поиск элементарных цепей
        private void chainButton_Click(object sender, EventArgs e)
        {
            listBoxMatrix.Items.Clear();
            //1-white 2-black
            int[] color = new int[V.Count];
            for (int i = 0; i < V.Count - 1; i++)
                for (int j = i + 1; j < V.Count; j++)
                {
                    for (int k = 0; k < V.Count; k++)
                        color[k] = 1;
                    DFSchain(i, j, L, color, (i + 1).ToString());
                }
        }

        //обход в глубину. поиск элементарных цепей. (1-white 2-black)
        private void DFSchain(int u, int endV, List<Line> E, int[] color, string s)
        {
            //вершину не следует перекрашивать, если u == endV (возможно в нее есть несколько путей)
            if (u != endV)
                color[u] = 2;
            else
            {
                listBoxMatrix.Items.Add(s);
                return;
            }
            for (int w = 0; w < E.Count; w++)
            {
                if (color[E[w].v2] == 1 && E[w].v1 == u)
                {
                    DFSchain(E[w].v2, endV, E, color, s + "-" + (E[w].v2 + 1).ToString());
                    color[E[w].v2] = 1;
                }
                else if (color[E[w].v1] == 1 && E[w].v2 == u)
                {
                    DFSchain(E[w].v1, endV, E, color, s + "-" + (E[w].v1 + 1).ToString());
                    color[E[w].v1] = 1;
                }
            }
        }

        //поиск элементарных циклов
        private void cycleButton_Click(object sender, EventArgs e)
        {
            listBoxMatrix.Items.Clear();
            //1-white 2-black
            int[] color = new int[V.Count];
            for (int i = 0; i < V.Count; i++)
            {
                for (int k = 0; k < V.Count; k++)
                    color[k] = 1;
                List<int> cycle = new List<int>();
                cycle.Add(i + 1);
                DFScycle(i, i, L, color, -1, cycle);
            }
        }

        //обход в глубину. поиск элементарных циклов. (1-white 2-black)
        //Вершину, для которой ищем цикл, перекрашивать в черный не будем. Поэтому, для избежания неправильной
        //работы программы, введем переменную unavailableEdge, в которой будет хранится номер ребра, исключаемый
        //из рассмотрения при обходе графа. В действительности это необходимо только на первом уровне рекурсии,
        //чтобы избежать вывода некорректных циклов вида: 1-2-1, при наличии, например, всего двух вершин.

        private void DFScycle(int u, int endV, List<Line> E, int[] color, int unavailableEdge, List<int> cycle)
        {
            //если u == endV, то эту вершину перекрашивать не нужно, иначе мы в нее не вернемся, а вернуться необходимо
            if (u != endV)
                color[u] = 2;
            else
            {
                if (cycle.Count >= 2)
                {
                    cycle.Reverse();
                    string s = cycle[0].ToString();
                    for (int i = 1; i < cycle.Count; i++)
                        s += "-" + cycle[i].ToString();
                    bool flag = false; //есть ли палиндром для этого цикла графа в листбоксе?
                    for (int i = 0; i < listBoxMatrix.Items.Count; i++)
                        if (listBoxMatrix.Items[i].ToString() == s)
                        {
                            flag = true;
                            break;
                        }
                    if (!flag)
                    {
                        cycle.Reverse();
                        s = cycle[0].ToString();
                        for (int i = 1; i < cycle.Count; i++)
                            s += "-" + cycle[i].ToString();
                        listBoxMatrix.Items.Add(s);
                    }
                    return;
                }
            }
            for (int w = 0; w < E.Count; w++)
            {
                if (w == unavailableEdge)
                    continue;
                if (color[E[w].v2] == 1 && E[w].v1 == u)
                {
                    List<int> cycleNEW = new List<int>(cycle);
                    cycleNEW.Add(E[w].v2 + 1);
                    DFScycle(E[w].v2, endV, E, color, w, cycleNEW);
                    color[E[w].v2] = 1;
                }
                else if (color[E[w].v1] == 1 && E[w].v2 == u)
                {
                    List<int> cycleNEW = new List<int>(cycle);
                    cycleNEW.Add(E[w].v1 + 1);
                    DFScycle(E[w].v1, endV, E, color, w, cycleNEW);
                    color[E[w].v1] = 1;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBoxMatrix.ItemHeight = 25;
        }

        private void картинкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sheet.Image != null)
            {
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить картинку как...";
                savedialog.OverwritePrompt = true;
                savedialog.CheckPathExists = true;
                savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                savedialog.ShowHelp = true;
                if (savedialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        sheet.Image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }

        private void выходИзПрограммыToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DialogResult result = DialogResult.No;
            if (L.Count != 0)
            {
                result = MessageBox.Show(
                    "Сохранить проект?",
                    "Внимание",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
            }


            if (result == DialogResult.Yes)
            {

                картинкуToolStripMenuItem_Click(sender, e);
                this.Close();
            }
            else
                this.Close();


        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                    "version 1.0.0 \n" + Convert.ToString(DateTime.Now),
                    " О Программе",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
        }

        private void оАвтореToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                   "Муханов Дмитрий, Лебедева Анна, Цыкина Мария, Семешкин Данила, Жаворонков Артем \nМ8О-310Б \n8 факультет Маи ",
                   "О Авторах",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Information);
        }

        private void матрицуСмежностиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createAdjAndOut();
            if (sheet.Image != null)
            {
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить картинку как...";
                savedialog.OverwritePrompt = true;
                savedialog.CheckPathExists = true;
                savedialog.Filter = "Text file|*.txt";
                savedialog.ShowHelp = true;
                if (savedialog.ShowDialog() == DialogResult.OK)
                {
                    TextWriter writer = null;
                    try
                    {
                        writer = new StreamWriter(savedialog.FileName);
                        foreach (var item in listBoxMatrix.Items)
                        {
                            writer.WriteLine(item.ToString());
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить матрицу смежности", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    writer.Close();
                }
            }

        }

        private void матрицуИнциденстностиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createIncAndOut();
            if (sheet.Image != null)
            {
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить картинку как...";
                savedialog.OverwritePrompt = true;
                savedialog.CheckPathExists = true;
                savedialog.Filter = "Text file|*.txt";
                savedialog.ShowHelp = true;
                if (savedialog.ShowDialog() == DialogResult.OK)
                {
                    TextWriter writer = null;
                    try
                    {
                        writer = new StreamWriter(savedialog.FileName);
                        foreach (var item in listBoxMatrix.Items)
                        {
                            writer.WriteLine(item.ToString());
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить матрицу инцидентности", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    writer.Close();
                }
            }
            createAdjAndOut();
        }


        private void списокРеберToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sheet.Image != null)
            {
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить список ребер как...";
                savedialog.OverwritePrompt = true;
                savedialog.CheckPathExists = true;
                savedialog.Filter = "Text file|*.txt";
                savedialog.ShowHelp = true;
                if (savedialog.ShowDialog() == DialogResult.OK)
                {
                    TextWriter writer = null;
                    try
                    {
                        writer = new StreamWriter(savedialog.FileName);
                        foreach (var line in L)
                        {
                            writer.WriteLine("Edge{" + line.Name + '(' + line.weight + ',' + line.v1 + ',' + line.v2 + ")}");
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить список ребер", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    writer.Close();
                }
            }
        }

        private void спискокВершинToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sheet.Image != null)
            {
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить список вершин как...";
                savedialog.OverwritePrompt = true;
                savedialog.CheckPathExists = true;
                savedialog.Filter = "Text file|*.txt";
                savedialog.ShowHelp = true;
                if (savedialog.ShowDialog() == DialogResult.OK)
                {
                    TextWriter writer = null;
                    try
                    {
                        writer = new StreamWriter(savedialog.FileName);
                        foreach (var vertex in V)
                        {
                            writer.WriteLine("Vertex{" + vertex.Number + '(' + vertex.x + ',' + vertex.y + ")}");
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить список вершин", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    writer.Close();
                }
            }
        }

        private void undoredo_Draw()
        {
            G.clearSheet();
            if (ArrayCondition.Array.Count != 0)
            {
                V = ArrayCondition.Array.Last().V;
                L = ArrayCondition.Array.Last().L;
                G.drawALLGraph(V, L);
                sheet.Image = G.GetBitmap();
            }
            else
            {
                V = new List<Vertex>();
                L = new List<Line>();
                G.drawALLGraph(V, L);
                sheet.Image = G.GetBitmap();
            }
        }

        private void undo_Click(object sender, EventArgs e)
        {
            ArrayCondition.RemoveFromEnd();
            undoredo_Draw();
        }

        private void redo_Click(object sender, EventArgs e)
        {
            ArrayCondition.Redo();
            undoredo_Draw();
        }

        private void матрицейСмежностиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sheet.Image != null)
            {
                OpenFileDialog opendialog = new OpenFileDialog();
                opendialog.CheckPathExists = true;
                opendialog.Filter = "Text file|*.txt";
                if (opendialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        TextReader reader = null;
                        reader = new StreamReader(opendialog.FileName);
                        V = new List<Vertex>();
                        L = new List<Line>();
                        string line;
                        line = reader.ReadLine();
                        string[] numbers = Regex.Split(line, @"\D+");
                        Random r = new Random();
                        for (int i = 1; i < numbers.Length - 1; i++)
                        {
                            Vertex ve = new Vertex(
                                r.Next(this.Size.Width - 300), 
                                r.Next(this.Size.Height - 200),
                                Convert.ToInt32(numbers[i])
                            );

                            V.Add(ve);
                        }
                        while ((line = reader.ReadLine()) != null)
                        {
                            numbers = Regex.Split(line, @"\D+");
                            for (int i = Convert.ToInt32(numbers[0]); i < numbers.Length - 1; i++)
                            {
                                if (Convert.ToInt32(numbers[i]) == 1)
                                {
                                    Line li = new Line(
                                        Convert.ToInt32(numbers[0]),
                                        i, 
                                        Convert.ToString((char)('a' + L.Count()))
                                    );

                                    L.Add(li);
                                }
                            }
                        }
                        reader.Close();
                        G.clearSheet();
                        for(int i = 0; i < L.Count; i++)
                        {
                            L[i].direction = (L[i].v1 + " -> " + L[i].v2).ToString();
                            G.drawEdge(V[L[i].v1 - 1], V[L[i].v2 - 1], L[i], i);
                        }
                        sheet.Image = G.GetBitmap();
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно открыть граф в виде матрицы смежности", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void матрицейИнцидентностиToolStripMenuItem_Click(object sender, EventArgs e)
        {
           if (sheet.Image != null)
            {
                OpenFileDialog opendialog = new OpenFileDialog();
                opendialog.CheckPathExists = true;
                opendialog.Filter = "Text file|*.txt";
                if (opendialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        TextReader reader = null;
                        reader = new StreamReader(opendialog.FileName);

                        V = new List<Vertex>();
                        L = new List<Line>();

                        string line = reader.ReadLine();

                        string[] names = Regex.Split(line, @"\s+");

                        Random r = new Random();
                        
                        List<List<int>> N = new List<List<int>>();
                        List<int> row = null;

                        while ((line = reader.ReadLine()) != null)
                        {
                            row = new List<int>();
                            string[] numbers = Regex.Split(line, @"\D+");   
                            
                            for (int i = 0; i < numbers.Length - 1; i++)
                            {
                                row.Add(Convert.ToInt32(numbers[i]));
                            }
                            
                            N.Add(row);
                        }

                        reader.Close();

                        
                        for (int j = 0; j < N.Count; j++)
                        {
                            N[j][0]--;

                            Vertex ve = new Vertex(
                                r.Next(this.Size.Width - 300), 
                                r.Next(this.Size.Height - 200),
                                N[j][0]
                            );

                            V.Add(ve);
                        }
                        

                        for (int i = 1; i < names.Length - 1; i++)
                        {
                            int From = -1;
                            int To = -1;

                            Line li = null;

                            for (int j = 0; j < N.Count; j++)
                            {
                                if (N[j][i] == 0) {
                                    continue;
                                }
                                
                                if (From == -1) {
                                    From = N[j][0];

                                    continue;
                                }

                                To = N[j][0];

                                break;
                            }

                            if (From == -1) {
                                continue;
                            }

                            if (To == -1) {
                                To = From;
                            }

                            li = new Line(
                                From,
                                To, 
                                names[i]
                            );

                            
                            L.Add(li);
                        }

                        G.clearSheet();

                        for(int i = 0; i < L.Count; i++)
                        {
                            L[i].direction = (L[i].v1 + " -> " + L[i].v2).ToString();
                            G.drawEdge(V[L[i].v1], V[L[i].v2], L[i], i);
                        }
                       
                        sheet.Image = G.GetBitmap();
                    }
                      catch
                    {
                        MessageBox.Show("Невозможно открыть граф в виде матрицы смежности", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                 }
            }
        }

        private void спискомРеберToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sheet.Image != null)
            {
                OpenFileDialog opendialog = new OpenFileDialog();
                opendialog.CheckPathExists = true;
                opendialog.Filter = "Text file|*.txt";
                if (opendialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        TextReader reader = null;
                        reader = new StreamReader(opendialog.FileName);
                        L = new List<Line>();
                        V = new List<Vertex>();
                        List<int> vertex = new List<int>();
                        string line;
                        Random r = new Random();
                        for (int i = 0; (line = reader.ReadLine()) != null; i++)
                        {
                            string[] numbers = Regex.Split(line, @"\W+");
                            L.Add(new Line(Convert.ToInt32(numbers[3]), Convert.ToInt32(numbers[4]), numbers[1]));
                            L[i].weight = Convert.ToInt32(numbers[2]);
                            L[i].direction = (L[i].v1 + " -> " + L[i].v2).ToString();
                            vertex.Add(Convert.ToInt32(numbers[3]));
                            vertex.Add(Convert.ToInt32(numbers[4]));
                        }
                        reader.Close();
                        IEnumerable<int> distinct_vertex = vertex.Distinct();
                        foreach (int item in distinct_vertex)
                        {
                            V.Add(new Vertex(r.Next(this.Size.Width - 300), r.Next(this.Size.Height - 200), item));
                        }
                        G.clearSheet();
                        G.drawALLGraph(V, L);
                        sheet.Image = G.GetBitmap();
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно открыть граф, как список рёбер", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
            }
        }

        private void спискомВершинToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sheet.Image != null)
            {
                OpenFileDialog opendialog = new OpenFileDialog();
                opendialog.CheckPathExists = true;
                opendialog.Filter = "Text file|*.txt";
                if (opendialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        TextReader reader = null;
                        reader = new StreamReader(opendialog.FileName);
                        V = new List<Vertex>();
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] numbers = Regex.Split(line, @"\D+");
                            V.Add(new Vertex(Convert.ToInt32(numbers[2]), Convert.ToInt32(numbers[3]), Convert.ToInt32(numbers[1])));
                        }
                        reader.Close();
                        G.clearSheet();
                        foreach (Vertex vertex in V)
                        {
                            G.drawVertex(vertex.x, vertex.y, vertex.Number.ToString());
                            sheet.Image = G.GetBitmap();
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно открыть граф, как список вершин", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
            }
        }
    }
}
