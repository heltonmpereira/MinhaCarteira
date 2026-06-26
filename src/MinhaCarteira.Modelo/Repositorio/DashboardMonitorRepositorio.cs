using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Repositorio.Base;

namespace MinhaCarteira.Modelo.Repositorio;

public class DashboardMonitorRepositorio(IDbContext contexto)
    : BaseRepositorio<DashboardMonitor, Guid>(contexto), IDashboardMonitorRepositorio
{
    protected override IQueryable<DashboardMonitor> AdicionarIncludes(IQueryable<DashboardMonitor> source, params object[] args)
    {
        string mesAno = string.Empty;
        try
        {
            mesAno = args != null ? args[0].ToString() : DateTime.Now.ToString("yyyyMM");
        }
        catch (Exception)
        {
            mesAno = DateTime.Now.ToString("yyyyMM");
        }
        var mes = int.Parse(mesAno.Substring(4, 2));
        var ano = int.Parse(mesAno[..4]);
        return source
            .Include(i => i.Agendamento)
                .ThenInclude(ti => ti.Categoria)
                    .ThenInclude(ti => ti.Icone)
            .Include(i => i.Agendamento)
                .ThenInclude(ti => ti.Parcelas
                    .Where(prc => prc.Data.Year == ano && prc.Data.Month == mes));
    }
}
