using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenU3
{
    public abstract class PedidoDecorator : Pedido
    {
        protected readonly Pedido _pedidoDecorado;
        public override string Tipo => _pedidoDecorado.Tipo;

        protected PedidoDecorator(Pedido pedido) : base(pedido.Id, pedido.MesaId)
        {
            _pedidoDecorado = pedido;
            // Copiar items del pedido original
            foreach (var item in pedido.Items)
            {
                base.AgregarItem(item.Producto, item.Cantidad);
            }
        }

        // Delegamos las operaciones al pedido decorado
        public override decimal CalcularTotal()
        {
            return _pedidoDecorado.CalcularTotal();
        }

        public override string MostrarDetalle()
        {
            return _pedidoDecorado.MostrarDetalle();
        }
    }
    public class DescuentoDecorator : PedidoDecorator
    {
        private readonly decimal _porcentajeDescuento;

        public DescuentoDecorator(Pedido pedido, decimal porcentajeDescuento) : base(pedido)
        {
            _porcentajeDescuento = porcentajeDescuento;
        }

        public override decimal CalcularTotal()
        {
            return base.CalcularTotal() * (1 - _porcentajeDescuento / 100);
        }

        public override string MostrarDetalle()
        {
            return $"{base.MostrarDetalle()}\n * Descuento aplicado: {_porcentajeDescuento}%";
        }
    }
}
