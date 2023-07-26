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
using dominio;
using negocio;
using System.Configuration;
using System.Runtime.Remoting.Messaging;

namespace Winform_Catalogo
{
    public partial class frmAltaArticulos : Form
    {
        private Articulos articulos = null;
        private OpenFileDialog archivo = null;
        public frmAltaArticulos()
        {
            InitializeComponent();
        }
        public frmAltaArticulos(Articulos articulos)
        {
            InitializeComponent();
            this.articulos = articulos;
            Text = "Modificar artículo";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            ArticulosNegocio negocio = new ArticulosNegocio();
            try
            {
                if (validarAceptar())
                    return;

                if(articulos == null)
                    articulos = new Articulos();

                articulos.Codigo = tbxCodigo.Text;
                articulos.Nombre = tbxNombre.Text;
                articulos.Descripcion = tbxDescripcion.Text;
                articulos.Precio = decimal.Parse(tbxPrecio.Text);
                articulos.ImagenUrl = tbxImagenUrl.Text;
                articulos.Marca = (Marcas)cboMarca.SelectedItem;
                articulos.Categoria = (Categorias)cboCategoria.SelectedItem;

                if (articulos.Id != 0)
                {
                    negocio.modificar(articulos);
                    MessageBox.Show("¡Artículo modificado exitosamente!");

                }
                else
                {
                    negocio.agregar(articulos);
                    MessageBox.Show("¡Artículo agregado exitosamente!");
                }
                Close();
                //Guardo imagen solo si la levantó localmente
                if (archivo != null && !(tbxImagenUrl.Text.ToUpper().Contains("HTTP"))) 
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void frmAltaArticulos_Load(object sender, EventArgs e)
        {
            MarcasNegocio marcaNegocio = new MarcasNegocio();
            CategoriasNegocio categoriasNegocio = new CategoriasNegocio();

            try
            {
                cboMarca.DataSource = marcaNegocio.listar();
                cboMarca.ValueMember = "Id";
                cboMarca.DisplayMember = "Descripcion";
                cboCategoria.DataSource = categoriasNegocio.listar();
                cboCategoria.ValueMember = "Id";
                cboCategoria.DisplayMember = "Descripcion";
                
                if(articulos != null)
                {
                    tbxCodigo.Text = articulos.Codigo;
                    tbxNombre.Text = articulos.Nombre;
                    tbxDescripcion.Text = articulos.Descripcion;
                    tbxImagenUrl.Text = articulos.ImagenUrl;
                    cargarImagen(articulos.ImagenUrl);
                    tbxPrecio.Text = articulos.Precio.ToString();
                    cboMarca.SelectedValue = articulos.Marca.Id;
                    cboCategoria.SelectedValue = articulos.Categoria.Id;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void tbxImagenUrl_Leave(object sender, EventArgs e)
        {
            cargarImagen(tbxImagenUrl.Text);
        }
        private void cargarImagen(string imagen)
        {
            try
            {
                pbxArticulos.Load(imagen);
            }
            catch (Exception ex)
            {
                pbxArticulos.Load("https://uning.es/wp-content/uploads/2016/08/ef3-placeholder-image.jpg");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg;|png|*.png";
            if(archivo.ShowDialog() == DialogResult.OK)
            {
                tbxImagenUrl.Text = archivo.FileName;
                cargarImagen(archivo.FileName);

                //guardo la imagen:
                //File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);
            }
        }
        private bool validarNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter) || (caracter == ',')))
                    return false;
            }
            return true;
        }
        private bool validarAceptar()
        {
            if(string.IsNullOrEmpty(tbxCodigo.Text))
            {
                MessageBox.Show("Ingresa el código");
                return true;
            }
            if (string.IsNullOrEmpty(tbxNombre.Text))
            {
                MessageBox.Show("Ingresa el nombre");
                return true;
            }
            if (string.IsNullOrEmpty(tbxDescripcion.Text))
            {
                MessageBox.Show("Ingresa la descripción");
                return true;
            }
            if (string.IsNullOrEmpty(tbxImagenUrl.Text))
            {
                MessageBox.Show("Ingresa la imagen");
                return true;
            }
            if (string.IsNullOrEmpty(tbxPrecio.Text))
            {
                MessageBox.Show("Por favor, ingresar el precio");
                return true;
            }
            if (!(validarNumeros(tbxPrecio.Text)))
            {
                MessageBox.Show("Por favor, ingresar solo números");
                return true;
            }


            return false;

         
        }
    }
}
