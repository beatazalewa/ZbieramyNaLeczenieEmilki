using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notatnik.NET
{
    public partial class Form1 : Form
    {
        bool tekstZmieniony = false;
        private StringReader sr = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void zamknijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Czy  zapisać zmiany  w  edytowanym  dokumencie?", this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
            switch (dr)
            {
                case DialogResult.Yes: zapiszJakoToolStripMenuItem_Click(null, null); break;
                case DialogResult.No: break;
                case DialogResult.Cancel: e.Cancel = true; break;
                default: e.Cancel = true; break;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            tekstZmieniony = true;
        }

        private void pasekstanuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip1.Visible = pasekstanuToolStripMenuItem.Checked;
        }

        public static string[] CzytajPlikTekstowy(string nazwaPliku)
        {
            List<string> tekst = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(nazwaPliku))
                {
                    string wiersz;
                    while ((wiersz = sr.ReadLine()) != null) tekst.Add(wiersz);
                }
                return tekst.ToArray();
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd  odczytu  pliku  " + nazwaPliku + "  (" + e.Message + ")"); return null;
            }
        }

        private void otwórzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string nazwaPliku = openFileDialog1.FileName; 
                textBox1.Lines = CzytajPlikTekstowy(nazwaPliku); 
                int ostatniSlash = nazwaPliku.LastIndexOf('\\');
                toolStripStatusLabel1.Text = nazwaPliku.Substring(ostatniSlash + 1,
                nazwaPliku.Length - ostatniSlash - 1);
            }
        }

        public static void ZapiszDoPlikuTekstowego(string nazwaPliku, string[] tekst)
        {
            using (StreamWriter sw = new StreamWriter(nazwaPliku))
            {
                foreach (string wiersz in tekst)
                    sw.WriteLine(wiersz);
            }
        }

        private void zapiszJakoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string nazwaPliku = openFileDialog1.FileName;
            if (nazwaPliku.Length > 0) saveFileDialog1.FileName = nazwaPliku; 
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                nazwaPliku = saveFileDialog1.FileName; 
                ZapiszDoPlikuTekstowego(nazwaPliku, textBox1.Lines); 
                int ostatniSlash = nazwaPliku.LastIndexOf('\\');
                toolStripStatusLabel1.Text = nazwaPliku.Substring(ostatniSlash + 1,
                nazwaPliku.Length - ostatniSlash - 1);
            }
        }

        private void tłoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = textBox1.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK) textBox1.BackColor = colorDialog1.Color;
        }

        private void czcionkaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = textBox1.Font; fontDialog1.Color = textBox1.ForeColor;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Font = fontDialog1.Font; 
                textBox1.ForeColor = fontDialog1.Color;
            }

        }

        private void cofnijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Undo();
        }

        private void wytnijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Cut();
        }

        private void kopiujToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Copy();
        }

        private void wklejToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Paste();
        }

        private void usuńToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.SelectedText = "";
        }

        private void zaznaczWszystkoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Font czcionka = textBox1.Font;
            int wysokoscWiersza = (int)czcionka.GetHeight(e.Graphics); int iloscLinii = e.MarginBounds.Height / wysokoscWiersza;

            if (sr == null) sr = new StringReader(textBox1.Text);  // czy pierwsza strona?
            e.HasMorePages = true;

            for (int i = 0; i < iloscLinii; i++)
            {
                string wiersz = sr.ReadLine(); if (wiersz == null)
                {
                    e.HasMorePages = false; sr = null;
                    break;
                }
                e.Graphics.DrawString(wiersz,
                czcionka, Brushes.Black,
                e.MarginBounds.Left,    // x
                e.MarginBounds.Top + i * wysokoscWiersza);  // y
            }
        }

        /* wersja z drukowaniem długich linii 
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Font czcionka = textBox1.Font;
            int wysokoscWiersza = (int)czcionka.GetHeight(e.Graphics); 
            int iloscLinii = e.MarginBounds.Height / wysokoscWiersza;

            // przy dzieleniu linii nie dbamy o spacje
            if (sr == null)
            {
                string tekst = "";
                foreach (string wiersz in textBox1.Lines)
                {
                    float szerokosc = e.Graphics.MeasureString(wiersz, czcionka).Width; 
                    if (szerokosc < e.MarginBounds.Width)
                    {
                        tekst += wiersz + "\n";
                    }
                    else
                    {
                        float sredniaSzerokoscLitery = szerokosc / wiersz.Length; 
                        int ileLiterWWierszu = (int)(e.MarginBounds.Width /
                        sredniaSzerokoscLitery);
                        int ileRazy = wiersz.Length / ileLiterWWierszu;  // ile pełnych wierszy
                        for (int i = 0; i < ileRazy; i++)
                        {
                            tekst += wiersz.Substring(i * ileLiterWWierszu,
                            ileLiterWWierszu) + "\n";
                        }
                        // kopiowanie reszty wiersza
                        tekst += wiersz.Substring(ileRazy * ileLiterWWierszu) + "\n";
                    } // if-else
                } // foreach
                sr = new StringReader(tekst);
            }  // if (sr==null)

            e.HasMorePages = true;


            for (int i = 0; i < iloscLinii; i++)
            {
                string wiersz = sr.ReadLine(); if (wiersz == null)
                {
                    e.HasMorePages = false; sr = null;
                    break;
                }
                e.Graphics.DrawString(wiersz,
                czcionka, Brushes.Black,
                e.MarginBounds.Left,    // x
                e.MarginBounds.Top + i * wysokoscWiersza);  // y
            }
        }
        */
        private void drukujToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.DocumentName = "Notatnik.NET - " + toolStripStatusLabel1.Text; printDocument1.Print();
            }
        }

        private void ustawieniaStronyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pageSetupDialog1.ShowDialog();
        }

        private void podgladWydrukuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.ShowDialog();
        }
    }
}
