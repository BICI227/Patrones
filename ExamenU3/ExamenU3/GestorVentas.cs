using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenU3
{
    public class GestorVentas
    {
        private readonly List<Venta> _ventas = new List<Venta>();
        private readonly GestorPedidos _gestorPedidos;
        private readonly GestorMesas _gestorMesas;

        public GestorVentas(GestorPedidos gestorPedidos, GestorMesas gestorMesas)
        {
            _gestorPedidos = gestorPedidos;
            _gestorMesas = gestorMesas;
        }

        public void MostrarMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\nSISTEMA DE VENTAS");
                Console.WriteLine("1. Registrar venta");
                Console.WriteLine("2. Ver ventas registradas");
                Console.WriteLine("3. Reporte de ventas");
                Console.WriteLine("4. Volver al menú principal");
                Console.Write("Seleccione una opción: ");

                var opcion = Console.ReadLine();
                if (opcion == "4") break;

                switch (opcion)
                {
                    case "1":
                        RegistrarVenta();
                        break;
                    case "2":
                        MostrarVentasRegistradas();
                        break;
                    case "3":
                        GenerarReporteVentas();
                        break;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }
            }
        }

        public void RegistrarVenta()
        {
            try
            {
                Console.Clear();
                var mesasOcupadas = _gestorMesas.ObtenerMesasOcupadas();
                if (mesasOcupadas.Count == 0)
                {
                    Console.WriteLine("No hay mesas ocupadas.");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine("\nMesas ocupadas:");
                foreach (var mesa in mesasOcupadas)
                {
                    Console.WriteLine($"Mesa {mesa.Numero}");
                }

                Console.Write("\nIngrese número de mesa: ");
                if (!int.TryParse(Console.ReadLine(), out int mesaId))
                {
                    Console.WriteLine("Número de mesa no válido.");
                    return;
                }

                var pedidos = _gestorPedidos.ObtenerPedidosPorMesa(mesaId);
                if (pedidos.Count == 0)
                {
                    Console.WriteLine("No hay pedidos para esta mesa.");
                    return;
                }

                decimal total = CalcularTotal(pedidos);
                Console.WriteLine($"\nDetalle de consumo:");
                MostrarDetallePedidos(pedidos);
                Console.WriteLine($"\nTotal a pagar: {total:C}");

                Console.Write("\nIngrese monto recibido: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal montoRecibido) || montoRecibido < total)
                {
                    Console.WriteLine("Monto insuficiente o no válido.");
                    return;
                }

                decimal cambio = montoRecibido - total;
                decimal propina = CalcularPropina(total);

                var venta = new Venta(
                    id: _ventas.Count + 1,
                    mesaId: mesaId,
                    total: total,
                    propina: propina,
                    fecha: DateTime.Now,
                    pedidos: pedidos
                );

                _ventas.Add(venta);
                _gestorMesas.ObtenerMesa(mesaId)?.Liberar();

                Console.WriteLine("\nVenta registrada exitosamente:");
                Console.WriteLine($"Total: {total:C}");
                Console.WriteLine($"Propina: {propina:C}");
                Console.WriteLine($"Cambio: {cambio:C}");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar venta: {ex.Message}");
            }
        }

        private decimal CalcularTotal(List<Pedido> pedidos)
        {
            return pedidos.Sum(p => p.Items.Sum(i => i.Producto.Precio * i.Cantidad));
        }

        private decimal CalcularPropina(decimal total)
        {
            Console.Write($"\nIngrese propina (10% sugerido: {total * 0.1m:C}): ");
            if (decimal.TryParse(Console.ReadLine(), out decimal propina))
            {
                return propina >= 0 ? propina : total * 0.1m;
            }
            return total * 0.1m;
        }

        private void MostrarDetallePedidos(List<Pedido> pedidos)
        {
            foreach (var pedido in pedidos)
            {
                Console.WriteLine($"\nPedido {pedido.Tipo}:");
                foreach (var item in pedido.Items)
                {
                    Console.WriteLine($"- {item.Cantidad}x {item.Producto.Nombre} ({item.Producto.Precio:C} c/u) = {item.Cantidad * item.Producto.Precio:C}");
                }
            }
        }

        public void MostrarVentasRegistradas()
        {
            if (_ventas.Count == 0)
            {
                Console.WriteLine("No hay ventas registradas.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nVENTAS REGISTRADAS:");
            Console.WriteLine("ID  | Mesa | Fecha            | Total    | Propina");
            Console.WriteLine("----|------|------------------|----------|----------");

            foreach (var venta in _ventas)
            {
                Console.WriteLine($"{venta.Id,-3} | {venta.MesaId,-4} | {venta.Fecha:dd/MM/yy HH:mm} | {venta.Total,8:C} | {venta.Propina,8:C}");
            }
            Console.ReadKey();
        }

        public void GenerarReporteVentas()
        {
            if (_ventas.Count == 0)
            {
                Console.WriteLine("No hay ventas para generar reporte.");
                Console.ReadKey();
                return;
            }

            var totalVentas = _ventas.Sum(v => v.Total);
            var totalPropinas = _ventas.Sum(v => v.Propina);

            Console.WriteLine("\nREPORTE DE VENTAS");
            Console.WriteLine("-----------------");
            Console.WriteLine($"Total ventas: {totalVentas:C}");
            Console.WriteLine($"Total propinas: {totalPropinas:C}");
            Console.WriteLine($"Promedio por venta: {totalVentas / _ventas.Count:C}");

            Console.WriteLine("\nVentas por tipo:");
            var ventasPorTipo = _ventas
                .SelectMany(v => v.Pedidos)
                .GroupBy(p => p.Tipo)
                .Select(g => new
                {
                    Tipo = g.Key,
                    Total = g.Sum(p => p.Items.Sum(i => i.Producto.Precio * i.Cantidad))
                });

            foreach (var item in ventasPorTipo)
            {
                Console.WriteLine($"- {item.Tipo}: {item.Total:C}");
            }
            Console.ReadKey();
        }

        public List<Venta> ObtenerVentasDelDia()
        {
            return _ventas.Where(v => v.Fecha.Date == DateTime.Today).ToList();
        }
    }
}
