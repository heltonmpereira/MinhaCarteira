using System;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Servico.Servico.Base;

namespace MinhaCarteira.Servico.Servico;

public class UsuarioServico(IUsuarioRepositorio repositorio)
    : BaseServico<Usuario, Guid, IUsuarioRepositorio>(repositorio), IUsuarioServico;