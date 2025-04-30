using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenU3
{
    public class Inventario
    {
        private readonly List<Producto> _productos;
        public Inventario()
        {
            _productos = new List<Producto>
            {
            };
        }
        public void AgregarProducto(Producto producto)
        {
            _productos.Add(producto);
        }

        public bool DescontarInventario(int productoId, int cantidad)
        {
            var producto = _productos.FirstOrDefault(p => p.Id == productoId);
            if (producto == null)
            {
                Console.WriteLine($"Producto con ID {productoId} no encontrado.");
                return false;
            }

            if (cantidad <= 0)
            {
                Console.WriteLine("La cantidad a descontar debe ser mayor que cero.");
                return false;
            }

            if (producto.Stock < cantidad)
            {
                Console.WriteLine($"No hay suficiente stock. Solo quedan {producto.Stock} unidades de {producto.Nombre}.");
                return false;
            }

            producto.Stock -= cantidad;
            Console.WriteLine($"Se descontaron {cantidad} unidades de {producto.Nombre}. Stock restante: {producto.Stock}");

            return true;
        }

        public List<Producto> ObtenerProductosParaReabastecer()
        {
            return _productos
                .Where(p => p.NecesitaReposicion())
                .OrderBy(p => p.Stock) 
                .ToList();
        }
    }

    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public string Categoria { get; set; }
        public int Cantidad { get; set; }
        public int Stock { get; set; }          // Cantidad actual en inventario
        public int StockMaximo { get; set; }    // Límite máximo permitido
        public int StockMinimo { get; set; }    // Punto de reorden

        public Producto(int id, string nombre, decimal precio, string categoria,int stockMaximo, int stockInicial = 0, int stockMinimo = 5)
        {
            Id = id;
            Nombre = nombre;
            Precio = precio;
            Categoria = categoria;
            StockMaximo = stockMaximo;
            Stock = stockInicial;
            StockMinimo = stockMinimo;
        }
        public void ReducirStock(int cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero");

            if (cantidad > Stock)
                throw new InvalidOperationException("No hay suficiente stock disponible");

            Stock -= cantidad;
        }

        public bool ReponerStock(int cantidad)
        {
            if (cantidad <= 0 || (Stock + cantidad) > StockMaximo)
                return false;

            Stock += cantidad;
            return true;
        }

        public bool NecesitaReposicion()
        {
            return Stock <= StockMinimo;
        }
    }
}
