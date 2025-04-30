using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenU3
{
    class Program
       {
            static void Main(string[] args)
            {
                var gestorMesas = new GestorMesas();
                var gestorPedidos = new GestorPedidos(gestorMesas);
                var gestorVentas = new GestorVentas(gestorPedidos, gestorMesas);

                var sistema = new SistemaRestaurante(gestorMesas, gestorPedidos, gestorVentas);
                sistema.Iniciar();
            }
        }

        public class SistemaRestaurante
        {
            private readonly GestorMesas _gestorMesas;
              private readonly GestorPedidos _gestorPedidos;
              private readonly GestorVentas _gestorVentas;

        public SistemaRestaurante(GestorMesas gestorMesas, GestorPedidos gestorPedidos, GestorVentas gestorVentas)
        {
            _gestorMesas = gestorMesas;
            _gestorPedidos = gestorPedidos;
            _gestorVentas = gestorVentas;
        }

        public void Iniciar()
            {
                Console.WriteLine("Sistema de Gestión de Restaurante");
                Console.WriteLine("=================================");

                MostrarMenuPrincipal();
            }

            private void MostrarMenuPrincipal()
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("\nMENÚ PRINCIPAL");
                    Console.WriteLine("1. Gestión de Mesas");
                    Console.WriteLine("2. Gestión de Pedidos");
                    Console.WriteLine("3. Sistema de Ventas");
                    Console.WriteLine("4. Corte de Caja");
                    Console.WriteLine("5. Salir");
                    Console.Write("Seleccione una opción: ");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            _gestorMesas.MostrarMenu();
                            break;
                        case "2":
                            _gestorPedidos.MostrarMenu();
                            break;
                        case "3":
                            _gestorVentas.MostrarMenu();
                            break;
                        case "4":
                            RealizarCorteCaja();
                            break;
                        case "5":
                            return;
                        default:
                            Console.WriteLine("Opción no válida. Intente nuevamente.");
                            break;
                    }
                }
            }

            private void RealizarCorteCaja()
            {
                Console.Clear();
                Console.WriteLine("\nREPORTE DE CIERRE DIARIO");
                Console.WriteLine("========================");

                _gestorVentas.GenerarReporteVentas();
                _gestorPedidos.GenerarReporteProductosVendidos();

            }
        }
    }

