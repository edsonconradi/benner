using Benner.Microondas.Domain.Enums;

namespace Benner.Microondas.Domain.Entities
{
    public class Microondas
    {
        public int TempoEmSegundos { get; set; }
        public int Potencia { get; set; }

        public MicroondasEstado Estado { get; set; } = MicroondasEstado.Desligado;
        public string ResultadoAquecimento { get; set; } = string.Empty;

        public bool EstaAquecendo => Estado == MicroondasEstado.Aquecendo;
        public bool EstaPausado => Estado == MicroondasEstado.Pausado;
    }
}
