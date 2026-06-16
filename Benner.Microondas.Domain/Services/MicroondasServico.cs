using Benner.Microondas.Domain.Entities;
using Benner.Microondas.Domain.Enums;
using Benner.Microondas.Domain.Repositorios;

namespace Benner.Microondas.Domain.Servicos
{
    public class MicroondasServico
    {
        private const int TempoMinimo = 1;
        private const int TempoMaximoManual = 120;
        private const int TempoMaximoPrograma = 1800;
        private const int PotenciaMinima = 1;
        private const int PotenciaMaxima = 10;
        private const char CaracterePadrao = '.';

        private readonly IProgramaPersonalizadoRepositorio _programaPersonalizadoRepositorio;

        public MicroondasServico() : this(new ProgramaPersonalizadoMemoriaRepositorio())
        {
        }

        public MicroondasServico(IProgramaPersonalizadoRepositorio programaPersonalizadoRepositorio)
        {
            _programaPersonalizadoRepositorio = programaPersonalizadoRepositorio;
        }

        public Entities.Microondas Iniciar(int tempoEmSegundos, int? potencia)
        {
            var potenciaFinal = potencia ?? PotenciaMaxima;

            ValidarTempoManual(tempoEmSegundos);
            ValidarPotencia(potenciaFinal);

            return MontarMicroondasLigado(tempoEmSegundos, potenciaFinal, CaracterePadrao);
        }

        public Entities.Microondas InicioRapido()
        {
            return Iniciar(30, PotenciaMaxima);
        }

        public Entities.Microondas AcrescentarTrintaSegundos(Entities.Microondas microondas)
        {
            ArgumentNullException.ThrowIfNull(microondas);

            var novoTempo = microondas.TempoEmSegundos + 30;
            ValidarTempoManual(novoTempo);

            microondas.TempoEmSegundos = novoTempo;
            microondas.Estado = MicroondasEstado.Aquecendo;
            microondas.ResultadoAquecimento = GerarStringAquecimento(novoTempo, microondas.Potencia, CaracterePadrao);

            return microondas;
        }

        public Entities.Microondas PausarOuCancelar(Entities.Microondas? microondas)
        {
            if (microondas == null || microondas.Estado == MicroondasEstado.Pausado || microondas.Estado == MicroondasEstado.Desligado)
                return new Entities.Microondas();

            microondas.Estado = MicroondasEstado.Pausado;
            return microondas;
        }

        public List<ProgramaAquecimento> ObterProgramasPreDefinidos()
        {
            return new List<ProgramaAquecimento>
            {
                new()
                {
                    Nome = "Pipoca",
                    Alimento = "Pipoca de micro-ondas",
                    TempoEmSegundos = 180,
                    Potencia = 7,
                    CaractereAquecimento = '*',
                    Instrucoes = "Quando o intervalo passar de 4 segundos, stop"
                },
                new()
                {
                    Nome = "Leite",
                    Alimento = "Leite",
                    TempoEmSegundos = 300,
                    Potencia = 5,
                    CaractereAquecimento = '#',
                    Instrucoes = "Cuidado com aquecimento de líquidos"
                },
                new()
                {
                    Nome = "Carnes de boi",
                    Alimento = "Carne em pedaço ou fatias",
                    TempoEmSegundos = 840,
                    Potencia = 4,
                    CaractereAquecimento = '@',
                    Instrucoes = "Pare na metade e vire a carne para descongelar por igual"
                },
                new()
                {
                    Nome = "Frango",
                    Alimento = "Frango",
                    TempoEmSegundos = 480,
                    Potencia = 7,
                    CaractereAquecimento = '&',
                    Instrucoes = "Pare na metade e vire o frango para descongelar por igual"
                },
                new()
                {
                    Nome = "Feijão",
                    Alimento = "Feijão congelado",
                    TempoEmSegundos = 480,
                    Potencia = 9,
                    CaractereAquecimento = '%',
                    Instrucoes = "Mexa ao final do aquecimento"
                }
            };
        }

        public List<ProgramaAquecimento> ObterTodosProgramas()
        {
            return ObterProgramasPreDefinidos()
                .Concat(_programaPersonalizadoRepositorio.ObterTodos())
                .OrderBy(x => x.Personalizado)
                .ThenBy(x => x.Nome)
                .ToList();
        }

        public ProgramaAquecimento CadastrarProgramaPersonalizado(
            string nome,
            string alimento,
            int tempoEmSegundos,
            int potencia,
            char caractereAquecimento,
            string instrucoes)
        {
            nome = (nome ?? string.Empty).Trim();
            alimento = (alimento ?? string.Empty).Trim();
            instrucoes = (instrucoes ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(nome))
                throw new Exception("Informe o nome do progama");

            if (string.IsNullOrWhiteSpace(alimento))
                throw new Exception("Informe o alimento");

            ValidarTempoPrograma(tempoEmSegundos);
            ValidarPotencia(potencia);
            ValidarCaracterePrograma(caractereAquecimento);

            var programas = ObterTodosProgramas();

            if (programas.Any(x => x.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("Já existe um programa com esse nome");

            if (programas.Any(x => x.CaractereAquecimento == caractereAquecimento))
                throw new Exception("Esse caractere de aquecimento já está em uso por outro programa");

            var programa = new ProgramaAquecimento
            {
                Nome = nome,
                Alimento = alimento,
                TempoEmSegundos = tempoEmSegundos,
                Potencia = potencia,
                CaractereAquecimento = caractereAquecimento,
                Instrucoes = instrucoes,
                Personalizado = true
            };

            _programaPersonalizadoRepositorio.Salvar(programa);
            return programa;
        }


        public void ExcluirProgramaPersonalizado(string nome)
        {
            var programa = _programaPersonalizadoRepositorio.ObterTodos()
                .FirstOrDefault(x => x.Nome.Equals(nome ?? string.Empty, StringComparison.OrdinalIgnoreCase));

            if (programa == null)
                throw new Exception("Programa personalizado não encontrado.");

            _programaPersonalizadoRepositorio.Excluir(programa.Nome);
        }

        public Entities.Microondas IniciarPrograma(string nomePrograma)
        {
            var programa = ObterTodosProgramas()
                .FirstOrDefault(x => x.Nome.Equals(nomePrograma ?? string.Empty, StringComparison.OrdinalIgnoreCase));

            if (programa == null)
                throw new Exception("Programa de aquecimento não encontrado.");

            return MontarMicroondasLigado(programa.TempoEmSegundos, programa.Potencia, programa.CaractereAquecimento);
        }

        private static Entities.Microondas MontarMicroondasLigado(int tempoEmSegundos, int potencia, char caractere)
        {
            return new Entities.Microondas
            {
                TempoEmSegundos = tempoEmSegundos,
                Potencia = potencia,
                Estado = MicroondasEstado.Aquecendo,
                ResultadoAquecimento = GerarStringAquecimento(tempoEmSegundos, potencia, caractere)
            };
        }

        private static void ValidarTempoManual(int tempoEmSegundos)
        {
            if (tempoEmSegundos < TempoMinimo || tempoEmSegundos > TempoMaximoManual)
                throw new Exception("Informe um tempo entre 1 segundo e 2 minutos.");
        }

        private static void ValidarTempoPrograma(int tempoEmSegundos)
        {
            if (tempoEmSegundos < TempoMinimo || tempoEmSegundos > TempoMaximoPrograma)
                throw new Exception("Informe um tempo entre 1 segundo e 30 minutos.");
        }

        private static void ValidarPotencia(int potencia)
        {
            if (potencia < PotenciaMinima || potencia > PotenciaMaxima)
                throw new Exception("Informe uma potência entre 1 e 10.");
        }


        private static void ValidarCaracterePrograma(char caractere)
        {
            if (char.IsWhiteSpace(caractere))
                throw new Exception("Informe um caractere de aquecimento");

            if (char.IsLetterOrDigit(caractere))
                throw new Exception("Use um simbolo como caractere de aquecimento, sem letras ou números.");

            if (caractere == CaracterePadrao)
                throw new Exception("O ponto é reservado para aquecimento manual");
        }

        private static string GerarStringAquecimento(int tempoEmSegundos, int potencia, char caractere)
        {
            var resultado = new System.Text.StringBuilder();

            for (var segundo = 1; segundo <= tempoEmSegundos; segundo++)
            {
                resultado.Append('[').Append(segundo).Append("s] ");
                resultado.Append(new string(caractere, potencia));
                resultado.AppendLine();
            }

            resultado.AppendLine();
            resultado.Append("Aquecimento concluirdo");

            return resultado.ToString();
        }
    }

}
