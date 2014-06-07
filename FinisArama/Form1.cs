using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinisArama
{
	public partial class Form1 : Form
	{
		private Dictionary<string, List<Ayrinti>> kayitlar;
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			string[] strings = Directory.GetFiles(Application.StartupPath, "*.txt");
			kayitlar = new Dictionary<string, List<Ayrinti>>();

			foreach (string s in strings)
			{
				string dosyaAdi = Path.GetFileName(s);
				int adet = 0;
				StreamReader sr = new StreamReader(s);
				int tip = 0;
				while (sr.Peek() >= 0)
				{
					string satir = sr.ReadLine().Replace("\"","");
					
					//if (satir.Contains("Fins") && satir.Contains("MLI") && satir.Contains("Offer"))
					if (satir.Contains("Fins;MLI;Offer"))
					{
						tip = 1;
						continue;
					} 
					if (satir.Contains("Part Number	Unit Sale (Pieces per package)	 Price  "))
					{
						tip = 2;
						continue;
					}


					string[] tokens = satir.Split(new char[] { ';', '\t' }, StringSplitOptions.RemoveEmptyEntries);
					if (tokens.Length == 2 && tip == 0)
					{
						Ayrinti yeni = new Ayrinti {DosyaAdi = dosyaAdi, Kod = tokens[0], Fiyat = tokens[1]};
						if (!kayitlar.ContainsKey(yeni.Kod))
							kayitlar.Add(yeni.Kod, new List<Ayrinti> {yeni});
						else
							kayitlar[yeni.Kod].Add(yeni);
						adet ++;
					}
					else if (tokens.Length == 3 && tip == 1)
					{
						Ayrinti yeni = new Ayrinti { DosyaAdi = dosyaAdi, Kod = tokens[0], Fiyat = tokens[2] };
						if (!kayitlar.ContainsKey(yeni.Kod))
							kayitlar.Add(yeni.Kod, new List<Ayrinti> { yeni });
						else
							kayitlar[yeni.Kod].Add(yeni);
						adet++;
					}
					else if (tokens.Length == 3 && tip == 2)
					{
						Ayrinti yeni = new Ayrinti { DosyaAdi = dosyaAdi, Kod = tokens[0], Fiyat = tokens[2] };
						if (!kayitlar.ContainsKey(yeni.Kod))
							kayitlar.Add(yeni.Kod, new List<Ayrinti> { yeni });
						else
							kayitlar[yeni.Kod].Add(yeni);
						adet++;
					}
				}
				sr.Close();
				listBox1.Items.Add(dosyaAdi + "(" + adet + ")");
			}
		}

		private void Form1_DoubleClick(object sender, EventArgs e)
		{
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
		}

		private void button1_Click(object sender, EventArgs e)
		{
			BindingList<Ayrinti> satirlar = new BindingList<Ayrinti>();
			string[] aranan = textBox1.Text.Split(new char[] {',', ';', '-', ','});
			foreach (string s in aranan)
			{
				if (kayitlar.ContainsKey(s))
				{
					foreach (var ayrinti in kayitlar[s])
					{
						satirlar.Add(ayrinti);
					}
					
				}
			}

			dataGridView1.DataSource = null;
			dataGridView1.DataSource = satirlar;
			
			dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
			dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
			dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			dataGridView1.Refresh();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			//printDocument1.Print();
			printPreviewDialog1.Document = printDocument1;
			printPreviewDialog1.PrintPreviewControl.Zoom = 1;
			printPreviewDialog1.ShowDialog();
		}

		private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			Bitmap bm = new Bitmap(this.dataGridView1.Width, this.dataGridView1.Height);
			dataGridView1.DrawToBitmap(bm, new Rectangle(0, 0, this.dataGridView1.Width, this.dataGridView1.Height));
			e.Graphics.DrawImage(bm, 0, 0);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			BindingList<Ayrinti> satirlar = new BindingList<Ayrinti>();
			
			foreach (KeyValuePair<string, List<Ayrinti>> s in kayitlar)
			{
				if (s.Value.Count > 1)
				{
					foreach (var ayrinti in kayitlar[s.Key])
					{
						satirlar.Add(ayrinti);
					}
				}
				
			}

			dataGridView1.DataSource = null;
			dataGridView1.DataSource = satirlar;

			dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
			dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
			dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

			dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
			dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
			dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

			dataGridView1.Refresh();

		}
	}

	internal class Ayrinti
	{
		public string DosyaAdi { get;set; }
		public string Kod { get; set; }
		public string Fiyat { get; set; }
	}
}
