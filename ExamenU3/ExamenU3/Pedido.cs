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
        public List<ItemPedido> Items { get; } = new List<ItemPedido>();

        protected Pedido(int id, int mesaId)
        {
            Id = id;
            MesaId = mesaId;
            Fecha = DateTime.Now;
        }

        public abstract string Tipo { get; }
        public void AgregarItem(Producto producto, int cantidad) => Items.Add(new ItemPedido(producto, cantidad));
    }

    // Tipos de pedido
    public class PedidoCocina : Pedido
    {
        public PedidoCocina(int id, int mesaId) : base(id, mesaId) { }
        public override string Tipo => "Cocina";
    }

    public class PedidoBebidas : Pedido
    {
        public PedidoBebidas(int id, int mesaId) : base(id, mesaId) { }
        public override string Tipo => "Bebidas";
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
