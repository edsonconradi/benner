using System.Text.Json;
using Benner.Microondas.Domain.Entities;
using Benner.Microondas.Domain.Repositorios;

namespace Benner.Microondas.Web.Infra
{
    public class ProgramaPersonalizadoArquivoRepositorio : IProgramaPersonalizadoRepositorio
    {
        private readonly string _caminhoArquivo;

        public ProgramaPersonalizadoArquivoRepositorio(IWebHostEnvironment ambiente)
        {
            var pastaDados = Path.Combine(ambiente.ContentRootPath, "App_Data");
            Directory.CreateDirectory(pastaDados);

            _caminhoArquivo = Path.Combine(pastaDados, "programas-personalizados.json");
        }

        public List<ProgramaAquecimento> ObterTodos()
        {
            if (!File.Exists(_caminhoArquivo))
                return new List<ProgramaAquecimento>();

            var conteudo = File.ReadAllText(_caminhoArquivo);

            if (string.IsNullOrWhiteSpace(conteudo))
                return new List<ProgramaAquecimento>();

            return JsonSerializer.Deserialize<List<ProgramaAquecimento>>(conteudo) ?? new List<ProgramaAquecimento>();
        }

        public void Salvar(ProgramaAquecimento programa)
        {
            var programas = ObterTodos();
            programas.Add(programa);
            Gravar(programas);

        }


        public bool Excluir(string nome)
        {
            var programas = ObterTodos();
            var removidos = programas.RemoveAll(x => x.Nome.Equals(nome ?? string.Empty, StringComparison.OrdinalIgnoreCase));

            if (removidos == 0)
                return false;

            Gravar(programas);
            return true;

        }

        private void Gravar(List<ProgramaAquecimento> programas)
        {
            var opcoes = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(_caminhoArquivo, JsonSerializer.Serialize(programas, opcoes));
        }
    }
}
