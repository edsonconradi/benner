namespace Benner.Microondas.Domain.Entities
{
    public class ProgramaAquecimento
    {
        public string Nome { get; set; } = string.Empty;
        public string Alimento { get; set; } = string.Empty;
        public int TempoEmSegundos { get; set; }
        public int Potencia { get; set; }
        public string Instrucoes { get; set; } = string.Empty;
        public char CaractereAquecimento { get; set; }

        public bool Personalizado { get; set; }
    }
}
