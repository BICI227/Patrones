using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenU3
{
    public abstract class MesaState
    {
        public abstract void Reservar(Mesa mesa);
        public abstract void Ocupar(Mesa mesa);
        public abstract void Liberar(Mesa mesa);
        public abstract string GetEstado();
        public abstract ConsoleColor GetColor();
    }

    public class DisponibleState : MesaState
    {
        public override void Reservar(Mesa mesa) => mesa.CambiarEstado(new ReservadaState());
        public override void Ocupar(Mesa mesa) => mesa.CambiarEstado(new OcupadaState());
        public override void Liberar(Mesa mesa) { }
        public override string GetEstado() => "Disponible";
        public override ConsoleColor GetColor() => ConsoleColor.Green;
    }
    public class ReservadaState : MesaState
    {
        public override void Reservar(Mesa mesa) { } 
        public override void Ocupar(Mesa mesa) => mesa.CambiarEstado(new OcupadaState());
        public override void Liberar(Mesa mesa) => mesa.CambiarEstado(new DisponibleState());
        public override string GetEstado() => "Reservada";
        public override ConsoleColor GetColor() => ConsoleColor.Yellow;
    }
    public class OcupadaState : MesaState
    {
        public override void Reservar(Mesa mesa) { }
        public override void Ocupar(Mesa mesa) { }
        public override void Liberar(Mesa mesa) => mesa.CambiarEstado(new DisponibleState());
        public override string GetEstado() => "Ocupada";
        public override ConsoleColor GetColor() => ConsoleColor.Red;
    }

    public class Mesa
    {
        private MesaState _state;
        public int Numero { get; }
        public List<int> PedidosAsociados { get; } = new List<int>();

        public Mesa(int numero)
        {
            Numero = numero;
            _state = new DisponibleState();
        }

        public void CambiarEstado(MesaState newState) => _state = newState;
        public void Reservar() => _state.Reservar(this);
        public void Ocupar() => _state.Ocupar(this);
        public void Liberar() => _state.Liberar(this);
        public string GetEstado() => _state.GetEstado();
        public ConsoleColor GetColor() => _state.GetColor();

        public void AgregarPedido(int pedidoId)
        {
            PedidosAsociados.Add(pedidoId);
            if (_state is DisponibleState)
            {
                this.Ocupar();
            }
        }

        public void RemoverPedido(int pedidoId)
        {
            PedidosAsociados.Remove(pedidoId);
            if (PedidosAsociados.Count == 0)
            {
                this.Liberar();
            }
        }

        public bool TienePedidosActivos => PedidosAsociados.Count > 0;
    }
}
