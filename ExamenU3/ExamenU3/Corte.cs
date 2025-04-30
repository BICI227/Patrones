using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenU3
{
    public class Corte
    {
        private readonly SistemaVentas _sistemaVentas;
        private readonly Personal _personalService;
        private readonly Inventario _inventarioService;

        public Corte()
        {
            _sistemaVentas = SistemaVentas.Instance;
            _personalService = new Personal();
            _inventarioService = new Inventario();
        }

        public void GenerarReporte()
        {
            Console.WriteLine("=== REPORTE DE CIERRE ===");
            GenerarReporteVentas();
            GenerarReportePropinas();
            GenerarReportePersonal();
            GenerarReporteInventario();
        }

        private void GenerarReporteVentas()
        {
            Console.WriteLine($"\nTotal de ventas: {_sistemaVentas.VentasAcumuladas:C}");
        }

        private void GenerarReportePropinas()
        {
            Console.WriteLine($"\nTotal de propinas: {_sistemaVentas.PropinasAcumuladas:C}");
        }

        private void GenerarReportePersonal()
        {
            var personal = _personalService.ObtenerPersonalEnTurno();
            Console.WriteLine("\nPersonal en turno:");

            foreach (var empleado in personal)
            {
                Console.WriteLine($"- {empleado.Nombre} ({empleado.Puesto})");
            }
        }

        private void GenerarReporteInventario()
        {
            var productos = _inventarioService.ObtenerProductosParaReabastecer();
            Console.WriteLine("\nProductos para reabastecer:");

            foreach (var producto in productos)
            {
                Console.WriteLine($"- {producto.Nombre} (Stock: {producto.Cantidad})");
            }
        }
    }

    public class Personal
    {
        public List<Empleado> ObtenerPersonalEnTurno()
        {
            
            return new List<Empleado>
        {
            new Empleado { Id = 1, Nombre = "Juan Pérez", Puesto = "Mesero" },
            new Empleado { Id = 2, Nombre = "María García", Puesto = "Cocinero" },
            new Empleado { Id = 3, Nombre = "Carlos López", Puesto = "Bartender" }
        };
        }
    }

    public class Empleado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Puesto { get; set; }
    }
}
