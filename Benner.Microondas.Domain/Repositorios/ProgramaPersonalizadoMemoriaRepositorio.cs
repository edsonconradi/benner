using Benner.Microondas.Domain.Entities;

namespace Benner.Microondas.Domain.Repositorios
{
    public class ProgramaPersonalizadoMemoriaRepositorio : IProgramaPersonalizadoRepositorio
    {
        private readonly List<ProgramaAquecimento> _programas = new();

        public List<ProgramaAquecimento> ObterTodos()
        {
            return _programas.ToList();
        }

        public void Salvar(ProgramaAquecimento programa)
        {
            _programas.Add(programa);
        }

        public bool Excluir(string nome)
        {
            var programa = _programas.FirstOrDefault(x => x.Nome.Equals(nome ?? string.Empty, StringComparison.OrdinalIgnoreCase));

            if (programa == null)
                return false;

            _programas.Remove(programa);
            return true;
        }
    }
}
