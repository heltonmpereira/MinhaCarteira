using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MinhaCarteira.AppCliente.Attribute;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;

namespace MinhaCarteira.AppCliente.ViewModel;

public class InstituicaoFinanceiraViewModel : BaseViewModel, IEntidade<Guid>
{
    public Guid Id { get; set; }
    [Required]
    public string Nome { get; set; }
    public string DelimitadorCsv { get; set; }

    [DisplayName("Primeira linha do arquivo possui o nome das colunas?")]
    public bool NomeCamposPrimeiraLinha { get; set; } = true;
    [DisplayName("Mapeamento dos campos do arquivo CSV")]
    public string MapeamentoCamposCsv { get; set; }
    public string CorFundo { get; set; } = "bg-gradient-info";

    [DisplayName("Cadastro")]
    public DateTime DataCriacao { get; set; }
    [DisplayName("Alteração")]
    public DateTime? DataAlteracao { get; set; }

    //[MaxFileSize("UploadTamanhoMaximo")]
    //[AllowedExtensions("UploadExtensaoPermitida")]
    //public IFormFile IconeForm { get; set; }

    public bool Deletado { get; set; }

    public Guid? IconeId { get; set; }
    public IconeViewModel Icone { get; set; }
}
