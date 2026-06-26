using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.AppCliente.ViewModel;

public class DashboardMonitorViewModel : BaseViewModel, IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public string CorFundo { get; set; }
    public string CriterioBuscaMovimentosJson { get; set; }

    [Display(Name = "Agendamento")]
    public Guid AgendamentoId { get; set; }
    public AgendamentoViewModel Agendamento { get; set; }
    private string agendamentoDescricao;
    public string AgendamentoDescricao
    {
        get => Agendamento?.Descricao ?? agendamentoDescricao;
        set => agendamentoDescricao = value;
    }

    public ICollection<MovimentoBancarioViewModel> Movimentos { get; set; }

    public Guid ProprietarioId { get; set; }
    public CriterioViewModel Criterio { get; set; }

    public bool Deletado { get; set; }
}
