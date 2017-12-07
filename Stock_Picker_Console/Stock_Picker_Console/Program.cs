using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;

namespace Stock_Picker_Console
{
    internal class Program
    {
        public class Field : IComparable
        {
            public string sku;

            public int qty;

            public string dsc;

            public Field(string sku, int qty, string dsc)
            {
                this.sku = sku;
                this.qty = qty;
                this.dsc = dsc;
            }

            public override string ToString()
            {
                return sku + " " + qty + " " + dsc;
            }

            public int CompareTo(object obj)
            {
                Field field = (Field)obj;
                return sku.CompareTo(field.sku);
            }
        }

        private static Font f1;

        private static Font f2;

        private static List<Field> data;

        private static void Main(string[] args)
        {
            Console.WriteLine("Printing picking list");
            data = new List<Field>();
            StreamReader pathReader = new StreamReader("path.txt");
            string path = pathReader.ReadLine();
            pathReader.Close();
            StreamReader streamReader = new StreamReader(path);
            streamReader.ReadLine();
            while (!streamReader.EndOfStream)
            {
                Field field = splitLine(streamReader.ReadLine());
                bool flag = true;
                for (int i = 0; i < data.Count; i++)
                {
                    if (data[i].sku == field.sku)
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    data.Add(field);
                }
                else
                {
                    for (int j = 0; j < data.Count; j++)
                    {
                        if (data[j].sku == field.sku)
                        {
                            data[j].qty += field.qty;
                        }
                    }
                }
            }
            data.Sort();
            streamReader.Close();
            f1 = new Font("Arial", 10f);
            f2 = new Font("Arial", 8f);
            PrintDocument printDocument = new PrintDocument();
            printDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
           
            printDocument.DocumentName = "Picking List";
            printDocument.PrinterSettings.Duplex = Duplex.Vertical;
            printDocument.PrintPage += new PrintPageEventHandler(Pd_PrintPage);
            printDocument.Print();
        }

        private static Field splitLine(string line)
        {
            string[] array = line.Split(',');
            int start = 23;
            return new Field(array[start + 2].Trim('"'), int.Parse(array[start].Trim('"')), array[start + 5].Trim('"'));
        }

        private static void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            int num = 0;
            float num2 = e.MarginBounds.Height / 20 - 6;
            num += DrawField(new string[]
            {
                "Qty",
                "SKU",
                "Description",
                "Notes"
            }, e, num);
            while (num < num2 && data.Count != 0)
            {
                Field field = data[0];
                num += DrawField(new string[]
                {
                    field.qty.ToString(),
                    field.sku,
                    field.dsc,
                    ""
                }, e, num);
                data.Remove(field);
            }
            if (data.Count != 0)
            {
                e.HasMorePages = true;
                return;
            }
            e.HasMorePages = false;
        }

        private static int DrawField(string[] t, PrintPageEventArgs e, int c)
        {
            Pen pen = new Pen(Brushes.Black);
            int[] array = new int[]
            {
                40,
                240,
                380,
                140
            };
            int num = (int)Math.Ceiling(e.Graphics.MeasureString(t[1], f1).Width / array[1]);
            if (num == 0)
            {
                num++;
            }
            int num2 = c * 20 + 80;
            e.Graphics.DrawRectangle(pen, 0, num2, array[0], num * 20);
            e.Graphics.DrawRectangle(pen, array[0], num2, array[1], num * 20);
            e.Graphics.DrawRectangle(pen, array[0] + array[1], num2, array[2], num * 20);
            e.Graphics.DrawRectangle(pen, array[0] + array[1] + array[2], num2, array[3], num * 20);
            SizeF sizeF = e.Graphics.MeasureString(t[0], f1);
            e.Graphics.DrawString(t[0], f1, Brushes.Black, 20f - sizeF.Width / 2f, 10f - sizeF.Height / 2f + num2);
            string[] array2 = BreakStrings(t[1], array[1], num, e, f1);
            for (int i = 0; i < num; i++)
            {
                e.Graphics.DrawString(array2[i], f1, Brushes.Black, array[0] + 2, 2 + num2 + i * 20);
            }
            array2 = BreakStrings(t[2], array[2], num, e, f2);
            for (int j = 0; j < num; j++)
            {
                e.Graphics.DrawString(array2[j], f2, Brushes.Black, array[0] + array[1] + 2, 2 + num2 + j * 20);
            }
            e.Graphics.DrawString(t[3], f1, Brushes.Black, array[0] + array[1] + array[2] + 2, 2 + num2);
            return num;
        }

        private static string[] BreakStrings(string s, int m, int r, PrintPageEventArgs e, Font f)
        {
            string[] array = s.Split(new char[]
            {
                ' '
            });
            string[] array2 = new string[r + 1];
            int num = 0;
            array2[num] = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                string text = array2[num] + " " + array[i];
                if (e.Graphics.MeasureString(text, f).Width <= m)
                {
                    string[] varA = array2;
                    int varB = num;
                    varA[varB] = varA[varB] + " " + array[i];
                }
                else
                {
                    num++;
                    if (num == array2.Length)
                    {
                        break;
                    }
                    array2[num] = array[i];
                }
            }
            return array2;
        }
    }
}