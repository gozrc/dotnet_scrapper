using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Scrapper.Web;
using Scrapper.Web.Scrappers;
using Scrapper.Web.Entities;
using Scrapper.Web.Scrappers.Pelispedia;


namespace Scrapper
{
    public partial class FrmMain : Form
    {
        //Channel canal_pelispedia = null;

        public FrmMain ()
        {
            InitializeComponent();
        }

        void CmdBuscar_Click (object sender, EventArgs e)
        {
            IWebScrap scrapper = new ScrapPelispedia();
            Movies peliculas = new Movies();
            Series series = new Series();
            string error = string.Empty;

            //scrapper.scrapMovies(ref peliculas, ref error);
            scrapper.scrapSeries(ref series, ref error);


            //string    error    = "";
            //IWebScrap scrapper = new ScrapPelispedia();

            //Elements elementos = new Elements();

            //int peliculas = 0;
            //int series = 0;

            //scrapper.onError        += delegate (string texto) { MessageBox.Show(texto); };
            //scrapper.onMoviesLoad   += delegate (int count) { MessageBox.Show("Cantidad de peliculas indexadas: " + count.ToString()); peliculas = count; };
            //scrapper.onSeriesLoad   += delegate (int count) { MessageBox.Show("Cantidad de episodios indexados: " + count.ToString()); series = count; };
            //scrapper.onMovieLoad    += delegate (string nombre, int count) { MessageBox.Show(string.Format("{0}/{1} Pelicula: {2}", count, peliculas, nombre)); };
            //scrapper.onSerieLoad    += delegate (string nombre, int count) { MessageBox.Show(string.Format("{0}/{1} Episodio: {2}", count, series, nombre)); };

            //scrapper.screappear (ref elementos, ref error);

            //if (scrapper.dameCanal(ref canal_pelispedia, ref error))
            //{
            //    listBox1.Items.Clear();

            //    foreach (Channel canal in canal_pelispedia.canales)
            //        listBox1.Items.Add(canal);
            //}
        }
        void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Channel canal = null;

            //if (listBox1.SelectedIndex >= 0)
            //{
            //    canal = (Channel)listBox1.Items[listBox1.SelectedIndex];

            //    foreach (Control control in flowLayoutPanel1.Controls)
            //        control.Dispose();

            //    flowLayoutPanel1.Controls.Clear();

            //    int index = 0;

            //    foreach (Element element in canal.elements)
            //    {
            //        PictureBox picturebox = new PictureBox();

            //        picturebox.Name         = element.id_element;
            //        picturebox.Height       = 210;
            //        picturebox.Width        = 140;
            //        picturebox.BorderStyle  = BorderStyle.FixedSingle;
            //        picturebox.SizeMode     = PictureBoxSizeMode.StretchImage;

            //        picturebox.Tag = element;

            //        picturebox.Click += Picturebox_Click;

            //        picturebox.MouseEnter += Picturebox_MouseEnter;
            //        picturebox.MouseLeave += Picturebox_MouseLeave;


            //        try
            //        {
            //            picturebox.Load(element.imagen);
            //        }
            //        catch
            //        {
            //            //
            //        }

            //        flowLayoutPanel1.Controls.Add(picturebox);

            //        index++;

            //        if (index > 500)
            //            break;
            //    }
            //}
        }
        void Picturebox_MouseLeave (object sender, EventArgs e)
        {
            PictureBox picture = (PictureBox)sender;
            picture.Cursor = Cursors.Default;
        }
        void Picturebox_MouseEnter (object sender, EventArgs e)
        {
            PictureBox picture = (PictureBox)sender;
            picture.Cursor = Cursors.Hand;
        }
        void Picturebox_Click (object sender, EventArgs e)
        {
            //Element element = (Element)((PictureBox)sender).Tag;

            //FrmElemento.mostrar(element);
        }
    }
}
