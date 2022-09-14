using Proyecto.Modelo;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proyecto.Herramientas;

namespace ProyectoVenta.Logica
{
    public class VentaLogica
    {

        private static VentaLogica _instancia = null;

        public VentaLogica()
        {

        }

        public static VentaLogica Instancia
        {

            get
            {
                if (_instancia == null) _instancia = new VentaLogica();
                return _instancia;
            }
        }


        public int reducirStock(int idproducto,int cantidad, out string mensaje)
        {
            mensaje = string.Empty;
            int respuesta = 0;
            try
            {
                using (SQLiteConnection conexion = new SQLiteConnection(Conexion.cadena))
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("update PRODUCTO set Stock = Stock - @pcantidad where IdProducto = @pidproducto");
                    SQLiteCommand cmd = new SQLiteCommand(query.ToString(), conexion);
                    cmd.Parameters.Add(new SQLiteParameter("@pcantidad", cantidad));
                    cmd.Parameters.Add(new SQLiteParameter("@pidproducto", idproducto));
                    cmd.CommandType = System.Data.CommandType.Text;
                    respuesta = cmd.ExecuteNonQuery();
                    if (respuesta < 1) {
                        mensaje = "no se puede reducir stock";
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = 0;
                mensaje = ex.Message;
            }

            return respuesta;
        }

        public int aumentarStock(int idproducto, int cantidad, out string mensaje)
        {
            mensaje = string.Empty;
            int respuesta = 0;
            try
            {
                using (SQLiteConnection conexion = new SQLiteConnection(Conexion.cadena))
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("update PRODUCTO set Stock = Stock + @pcantidad where IdProducto = @pidproducto");
                    SQLiteCommand cmd = new SQLiteCommand(query.ToString(), conexion);
                    cmd.Parameters.Add(new SQLiteParameter("@pcantidad", cantidad));
                    cmd.Parameters.Add(new SQLiteParameter("@pidproducto", idproducto));
                    cmd.CommandType = System.Data.CommandType.Text;
                    respuesta = cmd.ExecuteNonQuery();
                    if (respuesta < 1)
                    {
                        mensaje = "no se puede aumentar stock";
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = 0;
                mensaje = ex.Message;
            }

            return respuesta;
        }


        public int ObtenerCorrelativo(out string mensaje)
        {
            mensaje = string.Empty;
            int respuesta = 0;
            try
            {
                using (SQLiteConnection conexion = new SQLiteConnection(Conexion.cadena))
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select count(*) + 1 from Venta");
                    SQLiteCommand cmd = new SQLiteCommand(query.ToString(), conexion);
                    cmd.CommandType = System.Data.CommandType.Text;
                    respuesta = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                    if (respuesta < 1) {
                        mensaje = "No se pudo generar el correlativo";
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = 0;
                mensaje = "No se pudo generar el correlativo\nMayor Detalle:\n" + ex.Message;
            }

            return respuesta;
        }


        public int Registrar(Venta obj, out string mensaje)
        {

            mensaje = string.Empty;
            int respuesta = 0;
            SQLiteTransaction objTransaccion = null;

            using (SQLiteConnection conexion = new SQLiteConnection(Conexion.cadena))
            {
                try
                {
                    conexion.Open();
                    objTransaccion = conexion.BeginTransaction();
                    StringBuilder query = new StringBuilder();

                    query.AppendLine("CREATE TEMP TABLE _TEMP(id INTEGER);");
                    query.AppendLine(string.Format("Insert into Venta(NumeroDocumento,FechaRegistro,UsuarioRegistro,DocumentoCliente,NombreCliente,CantidadProductos,MontoTotal,PagoCon,Cambio,MedioPago) values('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{7}','{8}','{9}');",
                        obj.NumeroDocumento,
                        obj.FechaRegistro,
                        obj.UsuarioRegistro,
                        obj.DocumentoCliente,
                        obj.NombreCliente,
                        obj.CantidadProductos,
                        obj.MontoTotal,
                        obj.PagoCon,
                        obj.Cambio,
                        obj.MedioPago));

                    query.AppendLine("INSERT INTO _TEMP (id) VALUES (last_insert_rowid());");

                    foreach (DetalleVenta de in obj.olistaDetalle)
                    {
                        query.AppendLine(string.Format("insert into DETALLE_VENTA(IdVenta,IdProducto,CodigoProducto,DescripcionProducto,CategoriaProducto,MedidaProducto,PrecioVenta,Cantidad,SubTotal) values({0},{1},'{2}','{3}','{4}','{5}','{6}',{7},'{8}');",
                            "(select id from _TEMP)",
                            de.IdProducto,
                            de.CodigoProducto,
                            de.DescripcionProducto,
                            de.CategoriaProducto,
                            de.MedidaProducto,
                            de.PrecioVenta,
                            de.Cantidad,
                            de.SubTotal
                            ));

                    }

                    query.AppendLine("DROP TABLE _TEMP;");

                    SQLiteCommand cmd = new SQLiteCommand(query.ToString(), conexion);
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Transaction = objTransaccion;
                    respuesta = cmd.ExecuteNonQuery();


                    if (respuesta < 1)
                    {
                        objTransaccion.Rollback();
                        mensaje = "No se pudo registrar la Venta de los productos";
                    }

                    //imprimirTicket(obj);
                    objTransaccion.Commit();

                }
                catch (Exception ex)
                {
                    objTransaccion.Rollback();
                    respuesta = 0;
                    mensaje = ex.Message;
                }
            }


            return respuesta;
        }


        public void imprimirTicket(Venta venta)
        {
            ImprimirFactura.CreaTicket Ticket1 = new ImprimirFactura.CreaTicket();

            Ticket1.TextoCentro("FERREMAS DE LA COSTA S.A.S."); //imprime una linea de descripcion
            Ticket1.TextoCentro("NIT:    900959210"); //imprime una linea de descripcion
            ImprimirFactura.CreaTicket.LineasGuion();

            Ticket1.TextoIzquierda("Dir: Calle 82A #17-54, Los almendros, Soledad");
            Ticket1.TextoIzquierda("Tel: 301-459-7643 ");            
            Ticket1.TextoIzquierda("Fecha:" + venta.FechaRegistro);
            Ticket1.TextoIzquierda("Le Atendio: "+venta.UsuarioRegistro);

            if (venta.DocumentoCliente != "0")
            {
                Ticket1.TextoIzquierda("Cliente: " + venta.NombreCliente);
                Ticket1.TextoIzquierda("Doc Cliente: " + venta.DocumentoCliente);
            }
            
            Ticket1.TextoIzquierda("");
            Ticket1.TextoCentro("Factura de Venta POS No. " + venta.NumeroDocumento); //imprime una linea de descripcion

            Ticket1.TextoIzquierda("");
            
            ImprimirFactura.CreaTicket.EncabezadoVenta();
            ImprimirFactura.CreaTicket.LineasGuion();
            foreach (DetalleVenta de in venta.olistaDetalle)
            {
                // PROD                                                      CANT                         TOTAL
                Ticket1.AgregaArticulo(de.DescripcionProducto, de.Cantidad,  de.SubTotal); //imprime una linea de descripcion             
            }


            ImprimirFactura.CreaTicket.LineasGuion();
            //Ticket1.AgregaTotales("Sub-Total", double.Parse("000")); // imprime linea con Subtotal
            //Ticket1.AgregaTotales("Menos Descuento", double.Parse("000")); // imprime linea con decuento total
            //Ticket1.AgregaTotales("Mas ITBIS", double.Parse("000")); // imprime linea con ITBis total
            //Ticket1.TextoIzquierda(" ");
            Ticket1.AgregaTotales("TOTAL:", venta.MontoTotal); // imprime linea con total
            double dd = Convert.ToDouble(venta.MontoTotal);
            double iva = (dd-(dd / 1.19));        
            Ticket1.AgregaTotales("IVA INCLUIDO:", iva.ToString());
            Ticket1.TextoIzquierda(" ");
            Ticket1.TextoIzquierda("Medio de pago: " + venta.MedioPago);

            if (venta.MedioPago == "Efectivo")
            {
                Ticket1.AgregaTotales("Efectivo Recibido:", venta.PagoCon);
                Ticket1.AgregaTotales("Cambio:", venta.Cambio);
            }

            


            // Ticket1.LineasTotales(); // imprime linea 

            Ticket1.TextoIzquierda(" ");
            ImprimirFactura.CreaTicket.LineasGAsteriscos();
            Ticket1.TextoCentro("Trabajamos para brindarle el mejor servicio");
            Ticket1.TextoCentro("Gracias por su compra");
            ImprimirFactura.CreaTicket.LineasGAsteriscos();
            Ticket1.TextoIzquierda(" ");
            //string impresora = "Microsoft XPS Document Writer";
            string impresora = "EPSON TM-T20II Receipt";
            Ticket1.ImprimirTiket(impresora);
            Ticket1.CortaTicket(impresora);
            

        }

        public List<VistaVenta> Resumen(string fechainicio = "", string fechafin = "")
        {
            List<VistaVenta> oLista = new List<VistaVenta>();
            try
            {
                using (SQLiteConnection conexion = new SQLiteConnection(Conexion.cadena))
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();
               
                    query.AppendLine("select e.NumeroDocumento,strftime('%d/%m/%Y', date(e.FechaRegistro))[FechaRegistro],e.UsuarioRegistro,");
                    query.AppendLine("e.DocumentoCliente,e.NombreCliente,e.MontoTotal,e.PagoCon,e.Cambio,");
                    query.AppendLine("de.CodigoProducto,de.DescripcionProducto,de.CategoriaProducto,de.MedidaProducto,");
                    query.AppendLine("de.PrecioVenta,de.Cantidad,de.SubTotal");
                    query.AppendLine("from Venta e");
                    query.AppendLine("inner join DETALLE_Venta de on e.IdVenta = de.IdVenta");
                    query.AppendLine("where DATE(e.FechaRegistro) BETWEEN DATE(@pfechainicio) AND DATE(@pfechafin)");

                    SQLiteCommand cmd = new SQLiteCommand(query.ToString(), conexion);
                    cmd.Parameters.Add(new SQLiteParameter("@pfechainicio", fechainicio));
                    cmd.Parameters.Add(new SQLiteParameter("@pfechafin", fechafin));
                    cmd.CommandType = System.Data.CommandType.Text;

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            oLista.Add(new VistaVenta()
                            {
                                NumeroDocumento = dr["NumeroDocumento"].ToString(),
                                FechaRegistro = dr["FechaRegistro"].ToString(),
                                UsuarioRegistro = dr["UsuarioRegistro"].ToString(),
                                DocumentoCliente = dr["DocumentoCliente"].ToString(),
                                NombreCliente = dr["NombreCliente"].ToString(),
                                MontoTotal = dr["MontoTotal"].ToString(),
                                PagoCon = dr["PagoCon"].ToString(),
                                Cambio = dr["Cambio"].ToString(),
                                CodigoProducto = dr["CodigoProducto"].ToString(),
                                DescripcionProducto = dr["DescripcionProducto"].ToString(),
                                CategoriaProducto = dr["CategoriaProducto"].ToString(),
                                MediadProducto = dr["MedidaProducto"].ToString(),
                                PrecioVenta = dr["PrecioVenta"].ToString(),
                                Cantidad = dr["Cantidad"].ToString(),
                                SubTotal = dr["SubTotal"].ToString()
                                
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oLista = new List<VistaVenta>();
            }
            return oLista;
        }


        public Venta Obtener(string numerodocumento)
        {
            Venta objeto = null;

            try
            {
                using (SQLiteConnection conexion = new SQLiteConnection(Conexion.cadena))
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select IdVenta,NumeroDocumento, strftime('%d/%m/%Y', date(FechaRegistro))[FechaRegistro],UsuarioRegistro,DocumentoCliente,");
                    query.AppendLine("NombreCliente,CantidadProductos,MontoTotal,PagoCon,Cambio from Venta");
                    query.AppendLine("where NumeroDocumento = @pnumero");

                    SQLiteCommand cmd = new SQLiteCommand(query.ToString(), conexion);
                    cmd.Parameters.Add(new SQLiteParameter("@pnumero", numerodocumento));
                    cmd.CommandType = System.Data.CommandType.Text;

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            objeto = new Venta()
                            {
                                IdVenta = Convert.ToInt32(dr["IdVenta"].ToString()),
                                NumeroDocumento = dr["NumeroDocumento"].ToString(),
                                FechaRegistro = dr["FechaRegistro"].ToString(),
                                UsuarioRegistro = dr["UsuarioRegistro"].ToString(),
                                DocumentoCliente = dr["DocumentoCliente"].ToString(),
                                NombreCliente = dr["NombreCliente"].ToString(),
                                CantidadProductos = Convert.ToInt32(dr["CantidadProductos"].ToString()),
                                MontoTotal = dr["MontoTotal"].ToString(),
                                PagoCon = dr["PagoCon"].ToString(),
                                Cambio = dr["Cambio"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objeto = null;
            }
            return objeto;
        }

        public List<DetalleVenta> ListarDetalle(int idVenta)
        {
            List<DetalleVenta> oLista = new List<DetalleVenta>();

            try
            {

                using (SQLiteConnection conexion = new SQLiteConnection(Conexion.cadena))
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select CodigoProducto, DescripcionProducto, CategoriaProducto,");
                    query.AppendLine("MedidaProducto, PrecioVenta, Cantidad, SubTotal");
                    query.AppendLine("from DETALLE_Venta where IdVenta = @pidVenta");

                    SQLiteCommand cmd = new SQLiteCommand(query.ToString(), conexion);
                    cmd.Parameters.Add(new SQLiteParameter("@pidVenta", idVenta));
                    cmd.CommandType = System.Data.CommandType.Text;

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            oLista.Add(new DetalleVenta()
                            {
                                CodigoProducto = dr["CodigoProducto"].ToString(),
                                DescripcionProducto = dr["DescripcionProducto"].ToString(),
                                CategoriaProducto = dr["CategoriaProducto"].ToString(),
                                MedidaProducto = dr["MedidaProducto"].ToString(),
                                PrecioVenta = dr["PrecioVenta"].ToString(),
                                Cantidad = Convert.ToInt32(dr["Cantidad"].ToString()),
                                SubTotal = dr["SubTotal"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oLista = new List<DetalleVenta>();
            }


            return oLista;
        }






    }
}
