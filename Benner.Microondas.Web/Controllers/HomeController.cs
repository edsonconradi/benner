using Benner.Microondas.Domain.Servicos;
using Benner.Microondas.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Benner.Microondas.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly MicroondasServico _microondasServico;

        public HomeController(MicroondasServico microondasServico)
        {
            _microondasServico = microondasServico;
        }

        public IActionResult Index()
        {
            return View(MontarTela());
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Iniciar(MicroondasViewModel model)
        {
            try
            {
                var microondas = _microondasServico.Iniciar(model.TempoEmSegundos ?? 0, model.Potencia);

                model.ResultadoAquecimento = microondas.ResultadoAquecimento;
                model.Mensagem = "Aquecimento iniciado.";
            }
            catch (Exception ex)
            {
                model.Mensagem = ex.Message;
            }

            return View("Index", AtualizarProgramas(model));
        }

        [HttpPost]
        public IActionResult InicioRapido()
        {
            var model = MontarTela();
            var microondas = _microondasServico.InicioRapido();

            model.TempoEmSegundos = microondas.TempoEmSegundos;
            model.Potencia = microondas.Potencia;
            model.ResultadoAquecimento = microondas.ResultadoAquecimento;
            model.Mensagem = "Início rápido acionado.";

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult IniciarPrograma(MicroondasViewModel model)
        {
            try
            {
                var microondas = _microondasServico.IniciarPrograma(model.ProgramaSelecionado);
                var programa = _microondasServico.ObterTodosProgramas()
                    .First(x => x.Nome.Equals(model.ProgramaSelecionado, StringComparison.OrdinalIgnoreCase));

                model.TempoEmSegundos = microondas.TempoEmSegundos;
                model.Potencia = microondas.Potencia;
                model.ResultadoAquecimento = microondas.ResultadoAquecimento;
                model.Instrucoes = programa.Instrucoes;
                model.Mensagem = $"Programa {programa.Nome} iniciado.";
            }
            catch (Exception ex)
            {
                model.Mensagem = ex.Message;
            }

            return View("Index", AtualizarProgramas(model));
        }

        [HttpPost]
        public IActionResult CadastrarPrograma(MicroondasViewModel model)
        {
            try
            {
                var caractere = string.IsNullOrWhiteSpace(model.NovoCaractere)
                    ? ' '
                    : model.NovoCaractere.Trim()[0];

                var programa = _microondasServico.CadastrarProgramaPersonalizado(
                    model.NovoNome,
                    model.NovoAlimento,
                    model.NovoTempoEmSegundos ?? 0,
                    model.NovaPotencia ?? 0,
                    caractere,
                    model.NovasInstrucoes);

                model.Mensagem = $"Programa {programa.Nome} cadastrado com sucesso.";
                LimparCamposNovoPrograma(model);
            }
            catch (Exception ex)
            {
                model.Mensagem = ex.Message;
            }

            return View("Index", AtualizarProgramas(model));
        }


        [HttpPost]
        public IActionResult ExcluirPrograma(string nome)
        {
            var model = MontarTela();

            try
            {
                _microondasServico.ExcluirProgramaPersonalizado(nome);
                model.Mensagem = "Programa personalizado excluído.";
            }
            catch (Exception ex)
            {
                model.Mensagem = ex.Message;
            }

            return View("Index", AtualizarProgramas(model));
        }

        [HttpPost]
        public IActionResult PausarOuCancelar(MicroondasViewModel model)
        {
            model.TempoEmSegundos = null;
            model.Potencia = null;
            model.ResultadoAquecimento = string.Empty;
            model.Instrucoes = string.Empty;
            model.Mensagem = "Aquecimento pausado/cancelado. Tela limpa.";

            return View("Index", AtualizarProgramas(model));
        }

        private MicroondasViewModel MontarTela()
        {
            return AtualizarProgramas(new MicroondasViewModel());
        }

        private MicroondasViewModel AtualizarProgramas(MicroondasViewModel model)
        {
            model.Programas = _microondasServico.ObterTodosProgramas();
            return model;
        }

        private static void LimparCamposNovoPrograma(MicroondasViewModel model)
        {
            model.NovoNome = string.Empty;
            model.NovoAlimento = string.Empty;
            model.NovoTempoEmSegundos = null;
            model.NovaPotencia = null;
            model.NovoCaractere = string.Empty;
            model.NovasInstrucoes = string.Empty;
        }
    }
}
