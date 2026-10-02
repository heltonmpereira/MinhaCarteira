const evolucaoSaldoVerticalLinePlugin = {
    id: 'verticalLine',
    afterDraw: function(chart, args, pluginOptions) {
        const ctx = chart.ctx;
        const xAxis = chart.scales.x;
        const yAxis = chart.scales.y;
        const tooltip = chart.tooltip;

        let x = null;
        if (tooltip && tooltip._active && tooltip._active.length) {
            const activePoint = tooltip._active[0];
            x = activePoint.element.x;
        } else if (chart.$lastHoverX !== undefined) {
            x = chart.$lastHoverX;
        }

        if (x !== null) {
            ctx.save();
            ctx.beginPath();
            ctx.moveTo(x, yAxis.top);
            ctx.lineTo(x, yAxis.bottom);
            ctx.lineWidth = 1.5;
            ctx.strokeStyle = 'rgba(156, 163, 175, 0.5)';
            ctx.stroke();
            ctx.restore();
        }
    }
};

if (typeof Chart !== 'undefined' && !Chart.registry.plugins.get('verticalLine')) {
    Chart.register(evolucaoSaldoVerticalLinePlugin);
}

const EVOLUCAO_SALDO_PALETA = [
    { border: 'rgb(59, 130, 246)', bg: 'rgba(59, 130, 246, 0.12)' },
    { border: 'rgb(168, 85, 247)', bg: 'rgba(168, 85, 247, 0.12)' },
    { border: 'rgb(249, 115, 22)', bg: 'rgba(249, 115, 22, 0.12)' },
    { border: 'rgb(236, 72, 153)', bg: 'rgba(236, 72, 153, 0.12)' },
    { border: 'rgb(20, 184, 166)', bg: 'rgba(20, 184, 166, 0.12)' },
    { border: 'rgb(234, 179, 8)', bg: 'rgba(234, 179, 8, 0.12)' },
    { border: 'rgb(99, 102, 241)', bg: 'rgba(99, 102, 241, 0.12)' },
    { border: 'rgb(34, 197, 94)', bg: 'rgba(34, 197, 94, 0.12)' },
    { border: 'rgb(239, 68, 68)', bg: 'rgba(239, 68, 68, 0.12)' },
    { border: 'rgb(14, 165, 233)', bg: 'rgba(14, 165, 233, 0.12)' },
    { border: 'rgb(168, 162, 158)', bg: 'rgba(168, 162, 158, 0.12)' },
    { border: 'rgb(192, 132, 252)', bg: 'rgba(192, 132, 252, 0.12)' }
];
const EVOLUCAO_SALDO_PIE_CORES = [
    '#4f46e5', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6',
    '#06b6d4', '#ec4899', '#84cc16', '#f97316', '#6366f1',
    '#14b8a6', '#eab308', '#dc2626', '#a855f7', '#0ea5e9',
    '#f43f5e', '#22c55e', '#d97706', '#7c3aed', '#0891b2'
];

let graficoSaldoInstance;

function evolucaoIsDarkTheme() {
    return document.documentElement.getAttribute('data-bs-theme') === 'dark';
}

function criarGraficoEvolucaoSaldoPeriodo(dados) {
    const canvas = document.getElementById('graficoEvolucaoSaldoPeriodo');
    if (!canvas) return;
    const ctx = canvas.getContext('2d');

    if (graficoSaldoInstance) {
        graficoSaldoInstance.destroy();
        graficoSaldoInstance = null;
    }

    const labels = dados.Itens.map(item => new Date(item.Data).toLocaleDateString('pt-BR'));
    const saldoInicialArr = dados.Itens.map(item => item.SaldoInicial);
    const movimentosRealizadosArr = dados.Itens.map(item => item.MovimentosRealizados);
    const movimentosPlanejadosArr = dados.Itens.map(item => item.MovimentosPlanejados);
    const saldoFinalArr = dados.Itens.map(item => item.SaldoFinal);

    const datasets = [
        {
            label: 'Saldo Inicial',
            data: saldoInicialArr,
            borderColor: EVOLUCAO_SALDO_PALETA[1].border,
            backgroundColor: EVOLUCAO_SALDO_PALETA[1].bg,
            pointBackgroundColor: EVOLUCAO_SALDO_PALETA[1].border,
            pointRadius: 0,
            pointHoverRadius: 6,
            pointHoverBackgroundColor: EVOLUCAO_SALDO_PALETA[1].border,
            pointHoverBorderColor: '#fff',
            pointHoverBorderWidth: 2,
            borderWidth: 2.5,
            tension: 0.3,
            fill: false,
            spanGaps: false
        },
        {
            label: 'Movimentos Realizados',
            data: movimentosRealizadosArr,
            borderColor: EVOLUCAO_SALDO_PALETA[7].border,
            backgroundColor: EVOLUCAO_SALDO_PALETA[7].bg,
            pointBackgroundColor: EVOLUCAO_SALDO_PALETA[7].border,
            pointRadius: 0,
            pointHoverRadius: 6,
            pointHoverBackgroundColor: EVOLUCAO_SALDO_PALETA[7].border,
            pointHoverBorderColor: '#fff',
            pointHoverBorderWidth: 2,
            borderWidth: 2.5,
            tension: 0.3,
            fill: false,
            spanGaps: false
        },
        {
            label: 'Movimentos Planejados',
            data: movimentosPlanejadosArr,
            borderColor: EVOLUCAO_SALDO_PALETA[2].border,
            backgroundColor: EVOLUCAO_SALDO_PALETA[2].bg,
            pointBackgroundColor: EVOLUCAO_SALDO_PALETA[2].border,
            pointRadius: 0,
            pointHoverRadius: 6,
            pointHoverBackgroundColor: EVOLUCAO_SALDO_PALETA[2].border,
            pointHoverBorderColor: '#fff',
            pointHoverBorderWidth: 2,
            borderWidth: 2.5,
            tension: 0.3,
            fill: false,
            spanGaps: false,
            borderDash: [6, 4]
        },
        {
            label: 'Saldo Final',
            data: saldoFinalArr,
            borderColor: EVOLUCAO_SALDO_PALETA[0].border,
            backgroundColor: EVOLUCAO_SALDO_PALETA[0].bg,
            pointBackgroundColor: EVOLUCAO_SALDO_PALETA[0].border,
            pointRadius: 0,
            pointHoverRadius: 6,
            pointHoverBackgroundColor: EVOLUCAO_SALDO_PALETA[0].border,
            pointHoverBorderColor: '#fff',
            pointHoverBorderWidth: 2,
            borderWidth: 3.5,
            tension: 0.3,
            fill: true,
            spanGaps: false
        }
    ];

    const gridColor = evolucaoIsDarkTheme() ? 'rgba(75, 85, 99, 0.3)' : 'rgba(229, 231, 235, 0.8)';
    const ticksColor = evolucaoIsDarkTheme() ? '#e5e7eb' : '#374151';

    graficoSaldoInstance = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: datasets
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            onHover: (event, elements, chart) => {
                const xAxis = chart.scales.x;
                const canvasEl = chart.canvas;
                const rect = canvasEl.getBoundingClientRect();
                const x = event.x - rect.left;
                if (x >= xAxis.left && x <= xAxis.right) {
                    chart.$lastHoverX = x;
                } else {
                    chart.$lastHoverX = undefined;
                }
            },
            interaction: {
                mode: 'index',
                intersect: false,
                axis: 'x'
            },
            hover: {
                mode: 'index',
                intersect: false
            },
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        usePointStyle: true,
                        pointStyle: 'line',
                        padding: 20,
                        font: {
                            size: 14,
                            weight: '500'
                        },
                        color: ticksColor
                    }
                },
                tooltip: {
                    enabled: true,
                    backgroundColor: 'rgba(31, 41, 55, 0.98)',
                    titleColor: '#ffffff',
                    bodyColor: '#e5e7eb',
                    footerColor: '#ffffff',
                    borderColor: 'rgba(75, 85, 99, 0.6)',
                    borderWidth: 1,
                    cornerRadius: 16,
                    padding: {
                        top: 16,
                        bottom: 16,
                        left: 20,
                        right: 20
                    },
                    displayColors: true,
                    boxPadding: 8,
                    usePointStyle: true,
                    titleFont: {
                        size: 18,
                        weight: 'bold'
                    },
                    bodyFont: {
                        size: 14
                    },
                    footerFont: {
                        size: 14,
                        weight: '600'
                    },
                    callbacks: {
                        title: function(tooltipItems) {
                            if (!tooltipItems || tooltipItems.length === 0) return '';
                            return tooltipItems[0].label;
                        },
                        label: function(context) {
                            const valor = context.raw;
                            const label = context.dataset.label || '';
                            if (valor === null || valor === undefined) {
                                return label + ': —';
                            }
                            return label + ': ' + valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
                        },
                        labelColor: function(context) {
                            const border = context.dataset.borderColor;
                            return {
                                backgroundColor: border,
                                borderColor: border,
                                borderWidth: 2,
                                borderRadius: 4,
                                pointStyle: 'circle'
                            };
                        },
                        afterBody: function(tooltipItems) {
                            if (!tooltipItems || tooltipItems.length === 0) return '';
                            const mapa = {};
                            tooltipItems.forEach(ti => {
                                mapa[ti.dataset.label] = ti.raw;
                            });
                            const saldoInicial = mapa['Saldo Inicial'];
                            const saldoFinal = mapa['Saldo Final'];
                            if (saldoInicial === null || saldoInicial === undefined ||
                                saldoFinal === null || saldoFinal === undefined ||
                                saldoInicial === 0) return '';
                            const diff = saldoFinal - saldoInicial;
                            const pct = (diff / saldoInicial) * 100;
                            const aumento = diff >= 0;
                            const sinal = aumento ? '+' : '';
                            const icone = aumento ? '↗' : '↘';
                            const texto = aumento ? 'Aumento' : 'Queda';
                            const diffStr = diff.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
                            return ['', `${icone} Saldo Final vs Inicial: ${texto} ${sinal}${diffStr} (${sinal}${pct.toFixed(2)}%)`];
                        }
                    },
                    footerBackgroundColor: function(tooltipItems) {
                        if (!tooltipItems || tooltipItems.length === 0) return 'transparent';
                        const mapa = {};
                        tooltipItems.forEach(ti => {
                            mapa[ti.dataset.label] = ti.raw;
                        });
                        const saldoInicial = mapa['Saldo Inicial'];
                        const saldoFinal = mapa['Saldo Final'];
                        if (saldoInicial === null || saldoInicial === undefined ||
                            saldoFinal === null || saldoFinal === undefined) return 'transparent';
                        const diff = saldoFinal - saldoInicial;
                        return diff >= 0
                            ? 'rgba(16, 185, 129, 0.95)'
                            : 'rgba(220, 38, 38, 0.95)';
                    }
                }
            },
            scales: {
                x: {
                    grid: {
                        color: gridColor
                    },
                    ticks: {
                        color: ticksColor,
                        font: {
                            size: 13
                        },
                        autoSkip: true,
                        maxTicksLimit: 12
                    }
                },
                y: {
                    beginAtZero: false,
                    grid: {
                        color: gridColor
                    },
                    ticks: {
                        color: ticksColor,
                        font: {
                            size: 13
                        },
                        callback: function(value) {
                            return value.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
                        }
                    }
                }
            }
        }
    });
}

function criarGraficoGastosPorCategoria(dados) {
    const canvas = document.getElementById('graficoGastosPorCategoria');
    if (!canvas || !dados || !dados.Itens || dados.Itens.length === 0) return;

    const ctx = canvas.getContext('2d');
    const labels = dados.Itens.map(item => item.CaminhoCompleto || item.CategoriaNome);
    const valores = dados.Itens.map(item => item.Valor);
    const backgroundColors = dados.Itens.map((_, i) => EVOLUCAO_SALDO_PIE_CORES[i % EVOLUCAO_SALDO_PIE_CORES.length]);

    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: valores,
                backgroundColor: backgroundColors,
                borderColor: '#ffffff',
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'right',
                    labels: {
                        boxWidth: 15,
                        padding: 15,
                        font: {
                            size: 12
                        }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const valor = context.raw;
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentual = ((valor / total) * 100).toFixed(2);
                            return context.label + ': ' +
                                valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' }) +
                                ' (' + percentual + '%)';
                        }
                    }
                }
            }
        }
    });
}

function formatCurrency(value) {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
}

function formatDate(dateString) {
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
}

function gerarHTMLExportacao(relatorioData, gastosPorCategoriaData, dataInicialFormatada, dataFinalFormatada) {
    const canvas = document.getElementById('graficoEvolucaoSaldoPeriodo');
    const chartImage = canvas ? canvas.toDataURL('image/png') : null;

    const canvasPie = document.getElementById('graficoGastosPorCategoria');
    const pieChartImage = canvasPie ? canvasPie.toDataURL('image/png') : null;

    let htmlContent = '';
    if (relatorioData && relatorioData.Itens && relatorioData.Itens.length > 0) {
        let chartHtml = '';
        if (chartImage) {
            chartHtml = `
                                <h2 style="color: #1e293b; margin-bottom: 16px; margin-top: 32px; border-bottom: 1px solid #e2e8f0; padding-bottom: 8px;">Gráfico de Evolução do Saldo</h2>
                                <img src="${chartImage}" style="width: 100%; max-width: 100%; border-radius: 12px; box-shadow: 0 1px 3px 0 rgba(0,0,0,0.1), 0 1px 2px -1px rgba(0,0,0,0.1); margin-bottom: 24px;" />
                                `;
        }

        let pieChartHtml = '';
        if (pieChartImage && gastosPorCategoriaData && gastosPorCategoriaData.Itens && gastosPorCategoriaData.Itens.length > 0) {
            pieChartHtml = `
                                <h2 style="color: #1e293b; margin-bottom: 16px; margin-top: 32px; border-bottom: 1px solid #e2e8f0; padding-bottom: 8px;">Gastos por Categoria</h2>
                                <div style="margin-bottom: 24px; padding: 16px; background-color: #fef2f2; border-radius: 12px; border-left: 4px solid #ef4444;">
                                    <p style="color: #1e293b; font-size: 1.1rem; font-weight: 500;">Total de Gastos no Período: <strong>${formatCurrency(gastosPorCategoriaData.TotalGastos)}</strong></p>
                                </div>
                                <img src="${pieChartImage}" style="max-width: 600px; width: 100%; border-radius: 12px; box-shadow: 0 1px 3px 0 rgba(0,0,0,0.1), 0 1px 2px -1px rgba(0,0,0,0.1); margin-bottom: 24px; display: block; margin-left: auto; margin-right: auto;" />
                                `;

            let gastosTableRows = '';
            for (const item of gastosPorCategoriaData.Itens) {
                gastosTableRows += `
                                        <tr>
                                            <td>${item.CaminhoCompleto || item.CategoriaNome}</td>
                                            <td>${formatCurrency(item.Valor)}</td>
                                            <td>${item.QuantidadeMovimentos}</td>
                                            <td>${item.Percentual.toFixed(2)}%</td>
                                        </tr>`;
            }

            pieChartHtml += `
                                <h3 style="color: #1e293b; margin-bottom: 16px; border-bottom: 1px solid #e2e8f0; padding-bottom: 8px;">Detalhamento por Categoria</h3>
                                <table style="width:100%; border-collapse: collapse; border-radius: 12px; overflow: hidden; box-shadow: 0 1px 3px 0 rgba(0,0,0,0.1), 0 1px 2px -1px rgba(0,0,0,0.1);">
                                    <thead style="background: linear-gradient(to right, #ef4444, #f97316);">
                                        <tr>
                                            <th style="color: white; padding: 16px; font-weight: 600; text-align: left;">Categoria</th>
                                            <th style="color: white; padding: 16px; font-weight: 600; text-align: left;">Valor</th>
                                            <th style="color: white; padding: 16px; font-weight: 600; text-align: left;">Qtd. Movimentos</th>
                                            <th style="color: white; padding: 16px; font-weight: 600; text-align: left;">Percentual</th>
                                        </tr>
                                    </thead>
                                    <tbody>${gastosTableRows}</tbody>
                                </table>
                                `;
        }

        let tableRows = '';
        for (const item of relatorioData.Itens) {
            const movRealizadosClass = item.MovimentosRealizados >= 0 ? 'text-success' : 'text-danger';
            const movPlanejadosClass = item.MovimentosPlanejados >= 0 ? 'text-success' : 'text-danger';

            tableRows += `
                                    <tr>
                                        <td>${formatDate(item.Data)}</td>
                                        <td>${formatCurrency(item.SaldoInicial)}</td>
                                        <td class="${movRealizadosClass}">${formatCurrency(item.MovimentosRealizados)}</td>
                                        <td class="${movPlanejadosClass}">${formatCurrency(item.MovimentosPlanejados)}</td>
                                        <td>${formatCurrency(item.SaldoFinal)}</td>
                                    </tr>`;
        }

        htmlContent = `<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Evolução do Saldo</title>
    <style>
        * { box-sizing: border-box; margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Helvetica Neue', sans-serif; }
        body { background-color: #f8fafc; padding: 24px; }
        .container { max-width: 1200px; margin: 0 auto; background-color: white; border-radius: 16px; box-shadow: 0 1px 3px 0 rgba(0,0,0,0.1), 0 1px 2px -1px rgba(0,0,0,0.1); padding: 24px; }
        h1 { color: #1e293b; margin-bottom: 24px; border-bottom: 2px solid #4f46e5; padding-bottom: 12px; }
        .summary-card { background-color: #eef2ff; border-radius: 12px; padding: 16px; margin-bottom: 24px; border-left:4px solid #4f46e5; }
        .summary-card p { color: #475569; font-size: 1.1rem; font-weight: 500; }
        .summary-card strong { color: #1e293b; }
        table { width:100%; border-collapse: collapse; border-radius: 12px; overflow: hidden; box-shadow: 0 1px 3px 0 rgba(0,0,0,0.1), 0 1px 2px -1px rgba(0,0,0,0.1); }
        thead { background: linear-gradient(to right, #4f46e5, #6366f1); }
        th { color: white; padding: 16px; font-weight: 600; text-align: left; }
        tbody tr { transition: background-color 0.2s ease; }
        tbody tr:hover { background-color: #f8fafc; }
        td { padding:16px; border-bottom: 1px solid #e2e8f0; color: #1e293b; }
        .text-success { color: #10b981; font-weight: 500; }
        .text-danger { color: #ef4444; font-weight: 500; }
    </style>
</head>
<body>
    <div class="container">
        <h1>Evolução do Saldo - ${dataInicialFormatada} a ${dataFinalFormatada}</h1>
        <div class="summary-card">
            <p><strong>Saldo Inicial Total:</strong> ${formatCurrency(relatorioData.SaldoInicialTotal)}</p>
        </div>
        ${chartHtml}
        <h2 style="color: #1e293b; margin-bottom: 16px; margin-top: 32px; border-bottom: 1px solid #e2e8f0; padding-bottom: 8px;">Tabela de Saldos Diários</h2>
        <table>
            <thead>
                <tr>
                    <th>Data</th>
                    <th>Saldo Inicial</th>
                    <th>Movimentos Realizados</th>
                    <th>Movimentos Planejados</th>
                    <th>Saldo Final</th>
                </tr>
            </thead>
            <tbody>${tableRows}</tbody>
        </table>
        ${pieChartHtml}
    </div>
</body>
</html>`;
    }
    return htmlContent;
}

function inicializarEvolucaoSaldoPeriodo(ctx) {
    const {
        relatorioData,
        gastosPorCategoriaData,
        evolucaoSaldoItensRaw,
        gastosPorCategoriaItensRaw
    } = ctx;

    $(document).ready(function() {
        document.querySelectorAll('.progress-bar-width[data-width]').forEach(function(el) {
            var largura = parseFloat(el.dataset.width);
            if (!isNaN(largura)) {
                el.style.width = largura + '%';
            }
        });

        if (relatorioData && relatorioData.Itens && relatorioData.Itens.length > 0) {
            criarGraficoEvolucaoSaldoPeriodo(relatorioData);
        }

        document.addEventListener('themeChanged', function() {
            if (graficoSaldoInstance) {
                graficoSaldoInstance.destroy();
                graficoSaldoInstance = null;
            }
            if (window.relatorioData && window.relatorioData.Itens && window.relatorioData.Itens.length > 0) {
                criarGraficoEvolucaoSaldoPeriodo(window.relatorioData);
            }
        });

        if (gastosPorCategoriaData && gastosPorCategoriaData.Itens && gastosPorCategoriaData.Itens.length > 0) {
            criarGraficoGastosPorCategoria(gastosPorCategoriaData);
        }

        $('#btnImprimir').on('click', function() {
            $('#tabela-tab').tab('show');

            setTimeout(() => {
                $('#divConteudoImpressao').printThis({
                    importCSS: true,
                    importStyle: true,
                    printContainer: true,
                    pageTitle: 'Evolução do Saldo'
                });
            }, 100);
        });

        $('#btnExportarCSV').on('click', function() {
            let csv = 'Data,Saldo Inicial,Movimentos Realizados,Movimentos Planejados,Saldo Final\n';
            const dados = evolucaoSaldoItensRaw;
            if (dados && Array.isArray(dados)) {
                dados.forEach(function(item) {
                    const data = new Date(item.Data).toLocaleDateString('pt-BR');
                    const linha = `${data},${item.SaldoInicial},${item.MovimentosRealizados},${item.MovimentosPlanejados},${item.SaldoFinal}`;
                    csv += linha + '\n';
                });
            }
            const blob = new Blob(['\ufeff' + csv], { type: 'text/csv;charset=utf-8;' });
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = 'EvolucaoSaldo.csv';
            link.click();
            window.URL.revokeObjectURL(url);
        });

        $('#btnExportarGastosCSV').on('click', function() {
            let csv = 'Categoria,Categoria Pai,Valor,Quantidade Movimentos,Percentual\n';
            const dados = gastosPorCategoriaItensRaw;
            if (dados && Array.isArray(dados)) {
                dados.forEach(function(item) {
                    const linha = `"${(item.CategoriaNome || '').replace(/"/g, '""')}","${(item.CategoriaPaiNome || '').replace(/"/g, '""')}",${item.Valor},${item.QuantidadeMovimentos},${item.Percentual}`;
                    csv += linha + '\n';
                });
            }
            const blob = new Blob(['\ufeff' + csv], { type: 'text/csv;charset=utf-8;' });
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = 'GastosPorCategoria.csv';
            link.click();
            window.URL.revokeObjectURL(url);
        });

        $('#btnExportarHTML').on('click', function() {
            $('#grafico-tab').tab('show');

            setTimeout(() => {
                const htmlContent = gerarHTMLExportacao(relatorioData, gastosPorCategoriaData, ctx.dataInicialFormatada, ctx.dataFinalFormatada);
                if (!htmlContent) return;

                const blob = new Blob([htmlContent], { type: 'text/html;charset=utf-8;' });
                const url = window.URL.createObjectURL(blob);
                const link = document.createElement('a');
                link.href = url;
                link.download = 'EvolucaoSaldo.html';
                link.click();
                window.URL.revokeObjectURL(url);
            }, 300);
        });
    });
}
