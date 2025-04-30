using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenU3
{
    public class GestorMesas
    {
        private readonly List<Mesa> _mesas;

        public GestorMesas()
        {
            _mesas = new List<Mesa>
            {
                new Mesa(1),
                new Mesa(2),
                new Mesa(3),
                new Mesa(4),
                new Mesa(5)
            };
        }

        public void MostrarMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\nGESTIÓN DE MESAS");
                MostrarEstadoMesas();

                Console.WriteLine("\n1. Ocupar mesa");
                Console.WriteLine("2. Liberar mesa");
                Console.WriteLine("3. Reservar mesa");
                Console.WriteLine("4. Ver detalles de mesa");
                Console.WriteLine("5. Volver al menú principal");
                Console.Write("Seleccione una opción: ");

                var opcion = Console.ReadLine();
                if (opcion == "5") break;

                Console.Write("Ingrese número de mesa: ");
                if (!int.TryParse(Console.ReadLine(), out int numeroMesa) || !MesaExiste(numeroMesa))
                {
                    Console.WriteLine("Número de mesa no válido.");
                    Console.ReadKey();
                    continue;
                }

                ProcesarOpcionMesa(opcion, numeroMesa);
            }
        }

        public void MostrarEstadoMesas()
        {
            Console.WriteLine("Estado de las Mesas:");
            foreach (var mesa in _mesas.OrderBy(m => m.Numero))
            {
                ConsoleColor originalColor = Console.ForegroundColor;
                Console.ForegroundColor = mesa.GetColor();
                Console.WriteLine($"Mesa {mesa.Numero}: {mesa.GetEstado()}"); 
                Console.ForegroundColor = originalColor;
            }
        }
        public void MostrarEstadoMesasDetallado()
        {
            Console.WriteLine("\nESTADO DETALLADO DE MESAS:");
            Console.WriteLine("--------------------------");
            foreach (var mesa in _mesas.OrderBy(m => m.Numero))
            {
                ConsoleColor originalColor = Console.ForegroundColor;
                Console.ForegroundColor = mesa.GetColor();

                string infoPedidos = mesa.TienePedidosActivos
                    ? $" (Pedidos: {string.Join(", ", mesa.PedidosAsociados)})"
                    : "";

                Console.WriteLine($"Mesa {mesa.Numero}: {mesa.GetEstado()}{infoPedidos}");
                Console.ForegroundColor = originalColor;
            }
        }
        private bool MesaExiste(int numeroMesa) => _mesas.Any(m => m.Numero == numeroMesa);

        private void ProcesarOpcionMesa(string opcion, int numeroMesa)
        {
            var mesa = ObtenerMesa(numeroMesa);

            switch (opcion)
            {
                case "1":
                    mesa.Ocupar();
                    Console.WriteLine($"Mesa {numeroMesa} ocupada.");
                    break;
                case "2":
                    if (mesa.TienePedidosActivos)
                    {
                        Console.WriteLine($"Advertencia: La mesa tiene pedidos activos ({string.Join(", ", mesa.PedidosAsociados)})");
                        Console.Write("¿Liberar de todos modos? (S/N): ");
                        if (Console.ReadLine()?.ToUpper() != "S") return;
                    }
                    mesa.Liberar();
                    Console.WriteLine($"Mesa {numeroMesa} liberada.");
                    break;
                case "3":
                    mesa.Reservar();
                    Console.WriteLine($"Mesa {numeroMesa} reservada.");
                    break;
                case "4":
                    MostrarDetallesMesa(mesa);
                    return;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
        private void MostrarDetallesMesa(Mesa mesa)
        {
            Console.Clear();
            Console.WriteLine($"\nDETALLES MESA {mesa.Numero}");
            Console.WriteLine("---------------------");
            Console.WriteLine($"Estado actual: {mesa.GetEstado()}");
            Console.WriteLine($"Pedidos: {(mesa.TienePedidosActivos ? string.Join(", ", mesa.PedidosAsociados) : "Ninguno")}");
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
        public Mesa ObtenerMesa(int numeroMesa) => _mesas.FirstOrDefault(m => m.Numero == numeroMesa);

        public List<Mesa> ObtenerMesasOcupadas() => _mesas.Where(m => m.GetEstado() == "Ocupada").ToList();

        public List<Mesa> ObtenerMesasDisponibles() => _mesas.Where(m => m.GetEstado() == "Disponible").ToList();
        public void AgregarPedidoAMesa(int mesaNumero, int pedidoId)
        {
            var mesa = ObtenerMesa(mesaNumero);
            if (mesa != null)
            {
                mesa.AgregarPedido(pedidoId);
                if (mesa.GetEstado() != "Ocupada")
                {
                    mesa.Ocupar();
                }
            }
        }

        public void RemoverPedidoDeMesa(int mesaNumero, int pedidoId)
        {
            var mesa = ObtenerMesa(mesaNumero);
            if (mesa != null)
            {
                mesa.RemoverPedido(pedidoId);
                if (!mesa.TienePedidosActivos && mesa.GetEstado() == "Ocupada")
                {
                    mesa.Liberar();
                }
            }
        }
    }

}
