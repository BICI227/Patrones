using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenU3
{
    public class SistemaVentas
    {
        private static SistemaVentas _instance;
        private static readonly object _lock = new object();
        private List<Venta> _ventas = new List<Venta>();

        public static SistemaVentas Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SistemaVentas();
                        }
                    }
                }
                return _instance;
            }
        }

        private SistemaVentas() { }

        public void RegistrarVenta(Venta venta)
        {
            _ventas.Add(venta);
        }

        public decimal VentasAcumuladas
        {
            get { return _ventas.Sum(v => v.Total); }
        }

        public decimal PropinasAcumuladas
        {
            get { return _ventas.Sum(v => v.Propina); }
        }
    }
    public class Venta
    {
        public int Id { get; }
        public int MesaId { get; }
        public decimal Total { get; }
        public decimal Propina { get; }
        public DateTime Fecha { get; }
        public List<Pedido> Pedidos { get; }

        public Venta(int id, int mesaId, decimal total, decimal propina, DateTime fecha, List<Pedido> pedidos)
        {
            Id = id;
            MesaId = mesaId;
            Total = total;
            Propina = propina;
            Fecha = fecha;
            Pedidos = pedidos;
        }
    }
}
