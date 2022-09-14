using DocumentFormat.OpenXml.Bibliography;
using Proyecto.Modelo;
using ProyectoVenta.Logica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto.Formularios.Modales
{
    public partial class mdVentaExitosa : Form
    {
        public string _numerodocumento { get; set; }
        public Venta _venta;
        public mdVentaExitosa(Venta osalida)
        {
            InitializeComponent();
            _venta= osalida;
            
        }

        private void mdVentaExitosa_Load(object sender, EventArgs e)
        {
            txtnumerodocumento.Text = _numerodocumento;
            txtnumerodocumento.Focus();
        }

        private void btnaceptar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            VentaLogica.Instancia.imprimirTicket(_venta);
        }
    }
}
