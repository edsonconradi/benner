using Benner.Microondas.Domain.Entities;

namespace Benner.Microondas.Domain.Repositorios
{
    public interface IProgramaPersonalizadoRepositorio
    {
        List<ProgramaAquecimento> ObterTodos();
        void Salvar(ProgramaAquecimento programa);
        bool Excluir(string nome);
    }
}
