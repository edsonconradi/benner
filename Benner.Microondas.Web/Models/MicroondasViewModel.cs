using Benner.Microondas.Domain.Entities;

namespace Benner.Microondas.Web.Models
{
    public class MicroondasViewModel
    {
        public int? TempoEmSegundos { get; set; }
        public int? Potencia { get; set; }
        public string ProgramaSelecionado { get; set; } = string.Empty;
        public string ResultadoAquecimento { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public string Instrucoes { get; set; } = string.Empty;
        public List<ProgramaAquecimento> Programas { get; set; } = new();

        public string NovoNome { get; set; } = string.Empty;
        public string NovoAlimento { get; set; } = string.Empty;
        public int? NovoTempoEmSegundos { get; set; }
        public int? NovaPotencia { get; set; }
        public string NovoCaractere { get; set; } = string.Empty;
        public string NovasInstrucoes { get; set; } = string.Empty;
    }
}
