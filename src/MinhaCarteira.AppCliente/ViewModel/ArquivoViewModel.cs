using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using MinhaCarteira.AppCliente.Attribute;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;
using System;

namespace MinhaCarteira.AppCliente.ViewModel
{
    public class ArquivoViewModel : BaseViewModel, IEntidade<Guid>
    {
        public Guid Id { get; set; }
        public bool Deletado { get; set; }
        public Guid? ProprietarioId { get; set; }
        public DateTime DataCadastro { get; set; }

        public string Nome { get; set; }
        public string Conteudo { get; set; }
        public string Extensao { get; set; }

        [MaxFileSize("UploadTamanhoMaximo")]
        [AllowedExtensions("UploadExtensaoPermitida")]
        public IFormFile IconeForm { get; set; }

        public string MimeType
        {
            get
            {
                new FileExtensionContentTypeProvider()
                    .TryGetContentType(Nome, out string contentType);

                return contentType ?? "image/png";
            }
        }

    }
}
