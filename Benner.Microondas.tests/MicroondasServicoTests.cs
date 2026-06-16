using Benner.Microondas.Domain.Servicos;


namespace Benner.Microondas.Tests
{
    public class MicroondasServicoTests
    {
        [Test]
        public void Iniciar_DeveUsarPotenciaDez_QuandoPotenciaNaoForInformada()
        {
            var servico = new MicroondasServico();

            var microondas = servico.Iniciar(5, null);

            Assert.That(microondas.Potencia, Is.EqualTo(10));
            Assert.That(microondas.ResultadoAquecimento, Does.Contain("[1s] .........."));
        }

        [Test]
        public void Iniciar_DeveBarrarTempoMaiorQueDoisMinutos_NoModoManual()
        {
            var servico = new MicroondasServico();

            Assert.Throws<Exception>(() => servico.Iniciar(121, 5));
        }

        [Test]
        public void ProgramaPreDefinido_DeveUsarCaractereProprio()
        {
            var servico = new MicroondasServico();

            var microondas = servico.IniciarPrograma("Pipoca");

            Assert.That(microondas.Potencia, Is.EqualTo(7));
            Assert.That(microondas.ResultadoAquecimento, Does.Contain("[1s] *******"));
        }

        [Test]
        public void ProgramaPersonalizado_DeveEntrarNaListaEExecutarComSimboloEscolhido()
        {
            var servico = new MicroondasServico();
            var nome = "Lasanha Teste " + Guid.NewGuid().ToString("N")[..6];

            servico.CadastrarProgramaPersonalizado(nome, "Lasanha congelada", 90, 8, '!', "Tirar o plastico");
            var microondas = servico.IniciarPrograma(nome);

            Assert.That(microondas.ResultadoAquecimento, Does.Contain("[1s] !!!!!!!!"));
        }

        [Test]
        public void ProgramaPersonalizado_DevePermitirExcluirProgramaCriado()
        {
            var servico = new MicroondasServico();
            var nome = "Sopa Teste " + Guid.NewGuid().ToString("N")[..6];

            servico.CadastrarProgramaPersonalizado(nome, "Sopa", 60, 6, '$', "Mexer antes");
            servico.ExcluirProgramaPersonalizado(nome);

            Assert.Throws<Exception>(() => servico.IniciarPrograma(nome));
        }
    }
}
