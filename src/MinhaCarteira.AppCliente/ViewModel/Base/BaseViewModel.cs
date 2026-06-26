using System.ComponentModel.DataAnnotations;
using Dhani.Utilitarios.Filtro;
using Newtonsoft.Json;

namespace MinhaCarteira.AppCliente.ViewModel.Base;

public class BaseViewModel
{
    public BaseViewModel()
    {
        OpcaoAtual ??= new FiltroOpcao();
    }

    public ICriterio Filtro { get; set; }
    [ScaffoldColumn(false)]
    public string FiltroJson { get => JsonConvert.SerializeObject(Filtro); }
    public FiltroOpcao OpcaoAtual { get; set; }
}