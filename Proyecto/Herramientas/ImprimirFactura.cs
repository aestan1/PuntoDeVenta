using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Globalization;

namespace Proyecto.Herramientas
{
    class ImprimirFactura
    {
        public class CreaTicket
        {
            public static StringBuilder line = new StringBuilder();
            string ticket = "";
            string parte1, parte2;

            public static int max = 48;
            int cort;
            public static string LineasGuion()
            {
                string LineaGuion   = "------------------------------------------------";   // agrega lineas separadoras -

                return line.AppendLine(LineaGuion).ToString();
            }

            public static string LineasGAsteriscos()
            {
                string Linea        = "************************************************";   // agrega lineas separadoras -

                return line.AppendLine(Linea).ToString();
            }


            public static void EncabezadoVenta()
            {
                string LineEncabezado = "PRODUCTO                 CANTIDAD          VALOR";   // agrega lineas de  encabezados
                line.AppendLine(LineEncabezado);
            }
            public void TextoIzquierda(string par1)                          // agrega texto a la izquierda
            {
                max = par1.Length;
                if (max > 48)                                 // **********
                {
                    cort = max - 48;
                    parte1 = par1.Remove(48, cort);        // si es mayor que 40 caracteres, lo corta
                }
                else { parte1 = par1; }                      // **********
                line.AppendLine(ticket = parte1);

            }
            public void TextoDerecha(string par1)
            {
                ticket = "";
                max = par1.Length;
                if (max > 48)                                 // **********
                {
                    cort = max - 48;
                    parte1 = par1.Remove(48, cort);           // si es mayor que 40 caracteres, lo corta
                }
                else { parte1 = par1; }                      // **********
                max = 40 - par1.Length;                     // obtiene la cantidad de espacios para llegar a 40
                for (int i = 0; i < max; i++)
                {
                    ticket += " ";                          // agrega espacios para alinear a la derecha
                }
                line.AppendLine(ticket += parte1 + "\n");                //Agrega el texto

            }
            public void TextoCentro(string par1)
            {
                ticket = "";
                max = par1.Length;
                if (max > 48)                                 // **********
                {
                    cort = max - 48;
                    parte1 = par1.Remove(48, cort);          // si es mayor que 40 caracteres, lo corta
                }
                else { parte1 = par1; }                      // **********
                max = (int)(48 - parte1.Length) / 2;         // saca la cantidad de espacios libres y divide entre dos
                for (int i = 0; i < max; i++)                // **********
                {
                    ticket += " ";                           // Agrega espacios antes del texto a centrar
                }                                            // **********
                line.AppendLine(ticket += parte1);

            }
            public void TextoExtremos(string par1, string par2)
            {
                max = par1.Length;
                if (max > 18)                                 // **********
                {
                    cort = max - 18;
                    parte1 = par1.Remove(18, cort);          // si par1 es mayor que 18 lo corta
                }
                else { parte1 = par1; }                      // **********
                ticket = parte1;                             // agrega el primer parametro
                max = par2.Length;
                if (max > 18)                                 // **********
                {
                    cort = max - 18;
                    parte2 = par2.Remove(18, cort);          // si par2 es mayor que 18 lo corta
                }
                else { parte2 = par2; }
                max = 40 - (parte1.Length + parte2.Length);
                for (int i = 0; i < max; i++)                 // **********
                {
                    ticket += " ";                            // Agrega espacios para poner par2 al final
                }                                             // **********
                line.AppendLine(ticket += parte2 + "\n");                   // agrega el segundo parametro al final

            }
            public void AgregaTotales(string par1, string total)
            {
               
                double dd = Convert.ToDouble(total);
                total = dd.ToString("N", new CultureInfo("es-CO"));
                total = total.Split(',')[0];
                total = "$ " + total;
                int tamPar1 = par1.Length;
                int tamTotal = total.Length;
                int espacios = (48 - (tamPar1 + tamTotal));
                string spc = "";

                

                for (int i = 0; i < espacios; i++)
                {
                    spc += " ";
                }

                
                line.AppendLine(par1+spc+total);

            }

            // se le pasan los Aticulos  con sus detalles (CREADO DESDE CERO POR ANDRES ESTAN PARA REDUCIR COMPLEGIDAD)
            public void AgregaArticulo(string Articulo, int cant, string subtotal)
            {
                double dd = Convert.ToDouble(subtotal);
                subtotal = dd.ToString("N", new CultureInfo("es-CO"));
                subtotal = subtotal.Split(',')[0];
                
                //subtotal = subtotal.Split('.')[0]; //se quitan los centavos, por ser innecesarios
                int tamArticulo = Articulo.Length; //se guardan los tamaños de los parametros recibidos
                int tamCant = cant.ToString().Length;
                int tamSubtotal = subtotal.Length;
                string spc = "";

                if (tamArticulo > 24) //se trunca el nombre del producto hasta 24 caracteres
                {
                    Articulo = Articulo.Remove(24);
                    tamArticulo = Articulo.Length;
                }

                for (int i = 0; i < 5; i++) //se agregan 5 espacios vacios que se utilizaran adelante
                {
                    spc += " ";
                }

                int spcAdic1 = 24 - tamArticulo; //se calculan los espacios adicionales cuando el articulo sea mas pequeño que 24
                int spcAdic2 = 4 - tamCant;//lo mismo con la cantidad y el subtotal
                int spcAdic3 = 10 - tamSubtotal;

                string spc1 = spc;
                string spc2 = "";
                string spc3 = "";

                for (int i = 0; i < spcAdic1; i++) //se agregan los espacios adicionales
                {
                    spc1 += " ";
                }

                for (int i = 0; i < spcAdic2; i++)
                {
                    spc2 += " ";
                }

                for (int i = 0; i < spcAdic3; i++)
                {
                    spc3 += " ";
                }

                Articulo = Articulo.ToLowerInvariant(); //se cambia el nombre del producto a minusculas para mejor legibilidad
                String linea = Articulo + spc1 +spc2+ cant + spc +spc3+ subtotal+"\n"; //se contatena el contenido y los espacios
                line.Append(linea);
            }

            public void ImprimirTiket(string stringimpresora)
            {
                RawPrinterHelper.SendStringToPrinter(stringimpresora, line.ToString());
                line = new StringBuilder();
                
            }

            public void CortaTicket(string impresora)
            {
                //    string CUT_PAPPER = "\x1Bm";                                  // caracteres de corte
                //RawPrinterHelper.SendStringToPrinter(impresora, CUT_PAPPER); 		// corta
                string corte = "\x1B" + "m";                  					// caracteres de corte
                string avance = "\x1B" + "d" + "\x03";        					// avanza 9 renglones
                RawPrinterHelper.SendStringToPrinter(impresora, avance); 		// avanza
                RawPrinterHelper.SendStringToPrinter(impresora, corte);
            }

            public void AbreCajon(string impresora)
            {
                string cajon0 = "\x1B" + "p" + "\x00" + "\x0F" + "\x96";   		// caracteres de apertura cajon 0
                RawPrinterHelper.SendStringToPrinter(impresora, cajon0); 		// abre cajon0
            }
        }

        #region Clase para enviar a imprsora texto plano
        public class RawPrinterHelper
        {
            // Structure and API declarions:
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public class DOCINFOA
            {
                [MarshalAs(UnmanagedType.LPStr)]
                public string pDocName;
                [MarshalAs(UnmanagedType.LPStr)]
                public string pOutputFile;
                [MarshalAs(UnmanagedType.LPStr)]
                public string pDataType;
            }
            [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

            [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool ClosePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

            [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool EndDocPrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool StartPagePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool EndPagePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

            // SendBytesToPrinter()
            // When the function is given a printer name and an unmanaged array
            // of bytes, the function sends those bytes to the print queue.
            // Returns true on success, false on failure.
            public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
            {
                Int32 dwError = 0, dwWritten = 0;
                IntPtr hPrinter = new IntPtr(0);
                DOCINFOA di = new DOCINFOA();
                bool bSuccess = false; // Assume failure unless you specifically succeed.

                di.pDocName = "My C#.NET RAW Document";
                di.pDataType = "RAW";
                // di.pOutputFile = @"C:\Users\Roland\Documents\Visual Studio 2015\Projects\pjtVentas\Ventas";

                // Open the printer.
                if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
                {
                    // Start a document.
                    if (StartDocPrinter(hPrinter, 1, di))
                    {
                        // Start a page.
                        if (StartPagePrinter(hPrinter))
                        {
                            // Write your bytes.
                            bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                            EndPagePrinter(hPrinter);
                        }
                        EndDocPrinter(hPrinter);
                    }
                    ClosePrinter(hPrinter);
                }
                // If you did not succeed, GetLastError may give more information
                // about why not.
                if (bSuccess == false)
                {
                    dwError = Marshal.GetLastWin32Error();
                }
                return bSuccess;
            }

            public static bool SendStringToPrinter(string szPrinterName, string szString)
            {
                IntPtr pBytes;
                Int32 dwCount;
                // How many characters are in the string?
                dwCount = szString.Length;
                // Assume that the printer is expecting ANSI text, and then convert
                // the string to ANSI text.
                pBytes = Marshal.StringToCoTaskMemAnsi(szString);
                // Send the converted ANSI string to the printer.
                SendBytesToPrinter(szPrinterName, pBytes, dwCount);
                Marshal.FreeCoTaskMem(pBytes);
                return true;
            }
        }
        #endregion

    }
}
