using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenU3
{
    // Comanda base
    public class ItemPedido
    {
        public Producto Producto { get; }
        public int Cantidad { get; }

        public ItemPedido(Producto producto, int cantidad)
        {
            Producto = producto;
            Cantidad = cantidad;
        }
    }

    // Clase base Pedido (como en tu código original)
    public abstract class Pedido
    {
        public int Id { get; }
        public DateTime Fecha { get; }
        public int MesaId { get; }
        public abstract string Tipo { get; }
        public List<ItemPedido> Items { get; } = new List<ItemPedido>();

        protected Pedido(int id, int mesaId)
        {
            Id = id;
            MesaId = mesaId;
        }
        public virtual decimal CalcularTotal()
        {
            return Items.Sum(item => item.Producto.Precio * item.Cantidad);
        }
        public virtual string MostrarDetalle()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Pedido {Id} - Mesa {MesaId} - {Tipo}");
            sb.AppendLine("Ítems:");
            foreach (var item in Items)
            {
                sb.AppendLine($"- {item.Cantidad}x {item.Producto.Nombre} (${item.Producto.Precio})");
            }
            sb.AppendLine($"Total: ${CalcularTotal()}");
            return sb.ToString();
        }
        public void AgregarItem(Producto producto, int cantidad)
        {
            Items.Add(new ItemPedido(producto, cantidad));
        }
    }

    public class PedidoCocina : Pedido
    {
        public override string Tipo => "Cocina";

        public PedidoCocina(int id, int mesaId) : base(id, mesaId) { }
    }

    public class PedidoBebidas : Pedido
    {
        public override string Tipo => "Bebidas";

        public PedidoBebidas(int id, int mesaId) : base(id, mesaId) { }
    }

    // Factory de pedidos
    public class PedidoFactory
    {
        private static readonly Dictionary<string, Type> _tiposPedido =
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        static PedidoFactory()
        {
            _tiposPedido.Add("Cocina", typeof(PedidoCocina));
            _tiposPedido.Add("Bebidas", typeof(PedidoBebidas));
        }

        public Pedido CrearPedido(string tipo, int id, int mesaId)
        {
            if (_tiposPedido.TryGetValue(tipo, out var tipoPedido))
            {
                return (Pedido)Activator.CreateInstance(tipoPedido, id, mesaId);
            }
            throw new ArgumentException($"Tipo de pedido no válido: {tipo}");
        }
    }
}
