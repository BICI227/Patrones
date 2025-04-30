using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenU3
{
    public class GestorPedidos
    {
        private readonly List<Pedido> _pedidos = new List<Pedido>();
        private readonly PedidoFactory _factory = new PedidoFactory();
        private readonly List<Producto> _productos;
        private readonly GestorMesas _gestorMesas;

        public GestorPedidos(GestorMesas gestorMesas)
        {
            _gestorMesas = gestorMesas;
            _productos = new List<Producto>
            {
             new Producto(1, "Hamburguesa", 8.99m, "Comida", stockMaximo: 100, stockInicial: 50),
            new Producto(2, "Pizza", 10.50m, "Comida", stockMaximo: 50, stockInicial: 30),
            new Producto(3, "Refresco", 2.50m, "Bebida", stockMaximo: 200, stockInicial: 100),
            new Producto(4, "Cerveza", 3.75m, "Bebida", stockMaximo: 150, stockInicial: 80)
            };
        }

        public void MostrarMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\nGESTIÓN DE PEDIDOS");
                Console.WriteLine("1. Crear nuevo pedido");
                Console.WriteLine("2. Ver pedidos existentes");;
                Console.WriteLine("3. Volver al menú principal");
                Console.Write("Seleccione una opción: ");

                var opcion = Console.ReadLine();
                if (opcion == "3") break;

                switch (opcion)
                {
                    case "1":
                        CrearNuevoPedido();
                        break;
                    case "2":
                        MostrarPedidosExistentes();
                        break;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }
            }
        }

        public void CrearNuevoPedido()
        {
            Console.Clear();
            Console.WriteLine("CREACIÓN DE NUEVO PEDIDO");
            Console.WriteLine("-----------------------");

            _gestorMesas.MostrarEstadoMesas();

            int mesaId = SeleccionarMesa();
            if (mesaId == 0) return;

            string tipo = SeleccionarTipoPedido();
            if (tipo == null) return;

            try
            {
                var pedido = _factory.CrearPedido(tipo, _pedidos.Count + 1, mesaId);
                AgregarItemsAPedido(pedido);

                if (pedido.Items.Count == 0)
                {
                    Console.WriteLine("\nPedido cancelado (no se agregaron ítems).");
                    Console.ReadKey();
                    return;
                }

                _pedidos.Add(pedido);
                _gestorMesas.AgregarPedidoAMesa(mesaId, pedido.Id);

                MostrarResumenPedido(pedido);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError al crear pedido: {ex.Message}");
                Console.ReadKey();
            }
        }



        /* if (!int.TryParse(Console.ReadLine(), out int mesaId))
         {
             Console.WriteLine("Número de mesa no válido.");
             return;
         }

         Console.Write("Tipo de pedido (Cocina/Bebidas): ");
         var tipo = Console.ReadLine();

         try
         {
             var pedido = _factory.CrearPedido(tipo, _pedidos.Count + 1, mesaId);
             AgregarItemsAPedido(pedido);
             _pedidos.Add(pedido);
             Console.WriteLine($"Pedido {pedido.Id} creado para mesa {mesaId}");
         }
         catch (Exception ex)
         {
             Console.WriteLine($"Error: {ex.Message}");
         }*/

        private int SeleccionarMesa()
        {
            while (true)
            {
                Console.Write("\nIngrese número de mesa (0 para cancelar): ");
                if (!int.TryParse(Console.ReadLine(), out int mesaId))
                {
                    Console.WriteLine("Número no válido. Intente nuevamente.");
                    continue;
                }

                if (mesaId == 0) return 0;

                var mesa = _gestorMesas.ObtenerMesa(mesaId);
                if (mesa == null)
                {
                    Console.WriteLine($"La mesa {mesaId} no existe.");
                    continue;
                }

                if (mesa.GetEstado() == "Ocupada")
                {
                    Console.Write($"La mesa {mesaId} está ocupada. ¿Agregar a este pedido? (S/N): ");
                    if (Console.ReadLine()?.ToUpper() != "S") continue;
                }
                else if (mesa.GetEstado() == "Reservada")
                {
                    Console.Write($"La mesa {mesaId} está reservada. ¿Ocupar ahora? (S/N): ");
                    if (Console.ReadLine()?.ToUpper() == "S")
                    {
                        mesa.Ocupar();
                    }
                }

                return mesaId;
            }
        }

        private string SeleccionarTipoPedido()
        {
            while (true)
            {
                Console.Write("\nTipo de pedido (Cocina/Bebidas) (0 para cancelar): ");
                var input = Console.ReadLine()?.Trim();

                if (input == "0") return null;

                if (string.Equals(input, "Cocina", StringComparison.OrdinalIgnoreCase))
                    return "Cocina";

                if (string.Equals(input, "Bebidas", StringComparison.OrdinalIgnoreCase))
                    return "Bebidas";

                Console.WriteLine("Tipo no válido. Debe ser 'Cocina' o 'Bebidas'.");
            }
        }

        private void MostrarResumenPedido(Pedido pedido)
        {
            Console.Clear();
            Console.WriteLine("PEDIDO CREADO EXITOSAMENTE");
            Console.WriteLine("--------------------------");
            Console.WriteLine($"Número: {pedido.Id}");
            Console.WriteLine($"Mesa: {pedido.MesaId}");
            Console.WriteLine($"Tipo: {pedido.Tipo}");
            Console.WriteLine($"Fecha: {pedido.Fecha:g}");
            Console.WriteLine("\nÍtems:");

            foreach (var item in pedido.Items)
            {
                Console.WriteLine($"- {item.Cantidad}x {item.Producto.Nombre}");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private void AgregarItemsAPedido(Pedido pedido)
        {
            while (true)
            {
                Console.WriteLine("\nProductos disponibles:");
                MostrarProductosDisponibles();

                Console.Write("Seleccione producto (0 para terminar): ");
                if (!int.TryParse(Console.ReadLine(), out int prodId) || !ProductoExiste(prodId))
                {
                    if (prodId == 0) break;
                    Console.WriteLine("ID de producto no válido.");
                    continue;
                }

                var producto = _productos.First(p => p.Id == prodId);

                Console.Write("Cantidad: ");
                if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
                {
                    Console.WriteLine("Cantidad no válida.");
                    continue;
                }

                if (producto.Stock < cantidad)
                {
                    Console.WriteLine($"No hay suficiente stock. Stock disponible: {producto.Stock}");
                    continue;
                }

                producto.ReducirStock(cantidad);
                pedido.AgregarItem(producto, cantidad);
                Console.WriteLine($"Añadido {cantidad}x {producto.Nombre} al pedido.");
            }
        }

        private void MostrarProductosDisponibles()
        {
            foreach (var prod in _productos)
                Console.WriteLine($"{prod.Id}. {prod.Nombre} - {prod.Precio:C} (Stock: {prod.Stock})");
        }

        private bool ProductoExiste(int prodId)
        {
            return _productos.Any(p => p.Id == prodId);
        }

        private void MostrarPedidosExistentes()
        {
            Console.WriteLine("\nPedidos registrados:");
            foreach (var pedido in _pedidos)
            {
                Console.WriteLine($"\nPedido {pedido.Id} - Mesa {pedido.MesaId} - {pedido.Tipo}");
                Console.WriteLine($"Fecha: {pedido.Fecha}");
                Console.WriteLine("Items:");
                foreach (var item in pedido.Items)
                {
                    Console.WriteLine($"- {item.Cantidad}x {item.Producto.Nombre}");
                }
            }
            Console.ReadLine();
        }

        public List<Pedido> ObtenerPedidosPorMesa(int mesaId)
        {
            return _pedidos.Where(p => p.MesaId == mesaId).ToList();
        }

        public void GenerarReporteProductosVendidos()
        {
            var productosVendidos = _pedidos
                .SelectMany(p => p.Items)
                .GroupBy(i => i.Producto.Nombre)
                .Select(g => new { Nombre = g.Key, Total = g.Sum(i => i.Cantidad) });

            Console.WriteLine("\nREPORTE DE PRODUCTOS VENDIDOS:");
            foreach (var item in productosVendidos)
            {
                Console.WriteLine($"{item.Nombre}: {item.Total} unidades");
            }
        }
    }
}
