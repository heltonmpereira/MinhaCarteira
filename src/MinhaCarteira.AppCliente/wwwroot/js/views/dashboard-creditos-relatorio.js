const verticalLinePluginCreditos = {
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

if (typeof Chart !== 'undefined') {
    Chart.register(verticalLinePluginCreditos);
}

const PALETA_CORES_CREDITOS = [
    { border: 'rgb(16, 185, 129)', bg: 'rgba(16, 185, 129, 0.12)' },
    { border: 'rgb(59, 130, 246)', bg: 'rgba(59, 130, 246, 0.12)' },
    { border: 'rgb(168, 85, 247)', bg: 'rgba(168, 85, 247, 0.12)' },
    { border: 'rgb(249, 115, 22)', bg: 'rgba(249, 115, 22, 0.12)' },
    { border: 'rgb(236, 72, 153)', bg: 'rgba(236, 72, 153, 0.12)' },
    { border: 'rgb(234, 179, 8)', bg: 'rgba(234, 179, 8, 0.12)' },
    { border: 'rgb(99, 102, 241)', bg: 'rgba(99, 102, 241, 0.12)' },
    { border: 'rgb(14, 165, 233)', bg: 'rgba(14, 165, 233, 0.12)' },
    { border: 'rgb(20, 184, 166)', bg: 'rgba(20, 184, 166, 0.12)' },
    { border: 'rgb(192, 132, 252)', bg: 'rgba(192, 132, 252, 0.12)' },
    { border: 'rgb(168, 162, 158)', bg: 'rgba(168, 162, 158, 0.12)' },
    { border: 'rgb(34, 197, 94)', bg: 'rgba(34, 197, 94, 0.12)' }
];
const COR_CINZA_CREDITOS = { border: 'rgb(107, 114, 128)', bg: 'rgba(107, 114, 128, 0.06)' };
const COR_VERDE_CREDITOS = 'rgb(16, 185, 129)';
const COR_VERDE_FUNDO_CREDITOS = 'rgba(16, 185, 129, 0.1)';
const COR_VERMELHO_CREDITOS = 'rgb(248, 113, 113)';
const COR_VERMELHO_FUNDO_CREDITOS = 'rgba(248, 113, 113, 0.1)';

let graficoCreditosInstance;

function isDarkThemeCreditos() {
    return document.documentElement.getAttribute('data-bs-theme') === 'dark';
}

function getColorCreditos(porcentagem) {
    if (porcentagem === 0) {
        return 'transparent';
    }
    const r = 16;
    const g = 185;
    const b = 129;
    return `rgba(${r}, ${g}, ${b}, ${porcentagem})`;
}

function inicializarMultiSelectMesesCreditos() {
    const wrapper = document.getElementById('mesMultiSelectWrapper');
    const display = document.getElementById('mesMultiSelectDisplay');
    const dropdown = document.getElementById('mesMultiSelectDropdown');
    const placeholder = document.getElementById('mesMultiSelectPlaceholder');
    const checkboxes = dropdown ? dropdown.querySelectorAll('input[type="checkbox"][name="meses"]') : [];
    const btnTodos = document.getElementById('selectAllMeses');
    const btnLimpar = document.getElementById('clearAllMeses');

    if (!wrapper || !display || !dropdown) return;

    function atualizarChips() {
        const selecionados = Array.from(checkboxes).filter(cb => cb.checked);
        display.querySelectorAll('.multi-select-chip').forEach(el => el.remove());

        if (selecionados.length === 0) {
            if (placeholder) placeholder.style.display = '';
        } else {
            if (placeholder) placeholder.style.display = 'none';
            selecionados.forEach(cb => {
                const label = dropdown.querySelector(`label[for="${cb.id}"]`);
                const texto = label ? label.textContent : cb.value;
                const chip = document.createElement('span');
                chip.className = 'multi-select-chip';
                chip.innerHTML = `<span>${texto}</span><button type="button" aria-label="Remover ${texto}">&times;</button>`;
                chip.querySelector('button').addEventListener('click', (e) => {
                    e.stopPropagation();
                    cb.checked = false;
                    atualizarChips();
                });
                display.insertBefore(chip, placeholder);
            });
        }
    }

    display.addEventListener('click', (e) => {
        if (e.target.closest('button')) return;
        dropdown.classList.toggle('show');
    });

    document.addEventListener('click', (e) => {
        if (!wrapper.contains(e.target)) {
            dropdown.classList.remove('show');
        }
    });

    checkboxes.forEach(cb => {
        cb.addEventListener('change', atualizarChips);
    });

    if (btnTodos) {
        btnTodos.addEventListener('click', () => {
            checkboxes.forEach(cb => cb.checked = true);
            atualizarChips();
        });
    }
    if (btnLimpar) {
        btnLimpar.addEventListener('click', () => {
            checkboxes.forEach(cb => cb.checked = false);
            atualizarChips();
        });
    }

    atualizarChips();
}

function inicializarMultiSelectContasCreditos() {
    const wrapper = document.getElementById('contasMultiSelectWrapper');
    const display = document.getElementById('contasMultiSelectDisplay');
    const dropdown = document.getElementById('contasMultiSelectDropdown');
    const placeholder = document.getElementById('contasMultiSelectPlaceholder');
    const checkboxes = dropdown ? dropdown.querySelectorAll('input[type="checkbox"][name="contasBancariasIds"]') : [];
    const btnTodos = document.getElementById('selectAllContas');
    const btnLimpar = document.getElementById('clearAllContas');

    if (!wrapper || !display || !dropdown) return;

    function atualizarChips() {
        const selecionados = Array.from(checkboxes).filter(cb => cb.checked);
        display.querySelectorAll('.multi-select-chip').forEach(el => el.remove());

        if (selecionados.length === 0) {
            if (placeholder) placeholder.style.display = '';
        } else {
            if (placeholder) placeholder.style.display = 'none';
            selecionados.forEach(cb => {
                const label = dropdown.querySelector(`label[for="${cb.id}"]`);
                const texto = label ? label.textContent : cb.value.substring(0, 8);
                const chip = document.createElement('span');
                chip.className = 'multi-select-chip';
                chip.innerHTML = `<span>${texto}</span><button type="button" aria-label="Remover ${texto}">&times;</button>`;
                chip.querySelector('button').addEventListener('click', (e) => {
                    e.stopPropagation();
                    cb.checked = false;
                    atualizarChips();
                });
                display.insertBefore(chip, placeholder);
            });
        }
    }

    display.addEventListener('click', (e) => {
        if (e.target.closest('button')) return;
        dropdown.classList.toggle('show');
        const outroDropdown = document.getElementById('mesMultiSelectDropdown');
        if (outroDropdown) outroDropdown.classList.remove('show');
    });

    document.addEventListener('click', (e) => {
        if (!wrapper.contains(e.target)) {
            dropdown.classList.remove('show');
        }
    });

    checkboxes.forEach(cb => {
        cb.addEventListener('change', atualizarChips);
    });

    if (btnTodos) {
        btnTodos.addEventListener('click', () => {
            checkboxes.forEach(cb => cb.checked = true);
            atualizarChips();
        });
    }
    if (btnLimpar) {
        btnLimpar.addEventListener('click', () => {
            checkboxes.forEach(cb => cb.checked = false);
            atualizarChips();
        });
    }

    atualizarChips();
}

function criarHeatmapCreditos(dados) {
    const container = document.getElementById('heatmapCreditos');
    if (!container) return;
    if (!dados || !dados.Itens || dados.Itens.length === 0) {
        container.innerHTML = '';
        const p = document.createElement('p');
        p.className = 'text-muted';
        p.textContent = 'Sem dados para exibir';
        container.appendChild(p);
        return;
    }

    const valores = dados.Itens.map(item => item.CreditosMesAtual);
    const total = valores.reduce((a, b) => a + b, 0);
    const mediaDiaria = total / valores.length;
    let maiorCredito = 0;
    let diaMaiorCredito = 0;
    dados.Itens.forEach(item => {
        if (item.CreditosMesAtual > maiorCredito) {
            maiorCredito = item.CreditosMesAtual;
            diaMaiorCredito = item.Dia;
        }
    });

    const maxVal = Math.max(...valores);
    const diasSemana = ['D', 'S', 'T', 'Q', 'Q', 'S', 'S'];
    const dias = Array(7).fill(null).map(() => Array(5).fill(null));

    dados.Itens.forEach(item => {
        const data = new Date(item.Data);
        const diaSemana = data.getDay();
        const diaMes = data.getDate();
        const primeiroDiaMes = new Date(data.getFullYear(), data.getMonth(), 1);
        const diaSemanaPrimeiroDia = primeiroDiaMes.getDay();
        const semanaIndex = Math.floor((diaMes + diaSemanaPrimeiroDia - 1) / 7);

        if (semanaIndex >= 0 && semanaIndex < 5) {
            dias[diaSemana][semanaIndex] = {
                dia: diaMes,
                valor: item.CreditosMesAtual
            };
        }
    });

    container.innerHTML = '';

    const mainContainer = document.createElement('div');
    mainContainer.className = 'heatmap-creditos-container';
    container.appendChild(mainContainer);

    const header = document.createElement('div');
    header.className = 'heatmap-creditos-header';
    mainContainer.appendChild(header);

    const headerLeft = document.createElement('div');
    header.appendChild(headerLeft);

    const title = document.createElement('div');
    title.className = 'heatmap-creditos-title';
    title.textContent = 'Mapa de calor';
    headerLeft.appendChild(title);

    const totalDiv = document.createElement('div');
    totalDiv.className = 'heatmap-creditos-total';
    totalDiv.textContent = total.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
    headerLeft.appendChild(totalDiv);

    const mediaDiv = document.createElement('div');
    mediaDiv.className = 'heatmap-creditos-media';
    mediaDiv.textContent = `Média diária: ${mediaDiaria.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}`;
    headerLeft.appendChild(mediaDiv);

    const verMais = document.createElement('div');
    verMais.className = 'heatmap-creditos-ver-mais';
    header.appendChild(verMais);

    const table = document.createElement('table');
    table.className = 'heatmap-creditos-table';
    mainContainer.appendChild(table);

    diasSemana.forEach((diaLabel, diaSemana) => {
        const tr = document.createElement('tr');
        table.appendChild(tr);

        const tdLabel = document.createElement('td');
        tdLabel.className = 'heatmap-creditos-day-label';
        tdLabel.textContent = diaLabel;
        tr.appendChild(tdLabel);

        for (let semanaIndex = 0; semanaIndex < 5; semanaIndex++) {
            const dia = dias[diaSemana][semanaIndex];
            const td = document.createElement('td');
            tr.appendChild(td);

            if (dia) {
                const valor = dia.valor;
                const porcentagem = maxVal > 0 ? valor / maxVal : 0;
                const cor = getColorCreditos(porcentagem);
                const textoCor = porcentagem > 0.3 ? 'white' : (isDarkThemeCreditos() ? '#9ca3af' : '#374151');

                const dayDiv = document.createElement('div');
                dayDiv.className = 'heatmap-creditos-day';
                dayDiv.style.backgroundColor = cor;
                dayDiv.style.color = textoCor;
                dayDiv.textContent = dia.dia;
                td.appendChild(dayDiv);

                const tooltip = document.createElement('div');
                tooltip.className = 'heatmap-creditos-tooltip';
                tooltip.innerHTML = `Dia ${dia.dia}<br>Créditos: ${valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}`;
                dayDiv.appendChild(tooltip);
            } else {
                const dayDiv = document.createElement('div');
                dayDiv.className = 'heatmap-creditos-day empty';
                td.appendChild(dayDiv);
            }
        }
    });

    const legend = document.createElement('div');
    legend.className = 'heatmap-creditos-legend';
    mainContainer.appendChild(legend);

    const menosSpan = document.createElement('span');
    menosSpan.className = 'heatmap-creditos-legend-less';
    menosSpan.textContent = 'Menos';
    legend.appendChild(menosSpan);

    const legendColorsDiv = document.createElement('div');
    legendColorsDiv.className = 'heatmap-creditos-legend-colors';
    legend.appendChild(legendColorsDiv);

    const steps = 4;
    for (let i = 0; i <= steps; i++) {
        const porcentagem = i / steps;
        const legendColorDiv = document.createElement('div');
        legendColorDiv.className = 'heatmap-creditos-legend-color';
        legendColorDiv.style.backgroundColor = getColorCreditos(porcentagem);
        legendColorsDiv.appendChild(legendColorDiv);
    }

    const maisSpan = document.createElement('span');
    maisSpan.className = 'heatmap-creditos-legend-more';
    maisSpan.textContent = 'Mais';
    legend.appendChild(maisSpan);

    const footer = document.createElement('div');
    footer.className = 'heatmap-creditos-footer';
    mainContainer.appendChild(footer);

    const maiorCreditoLabel = document.createElement('span');
    maiorCreditoLabel.className = 'heatmap-creditos-footer-label';
    maiorCreditoLabel.textContent = 'Maior crédito';
    footer.appendChild(maiorCreditoLabel);

    const maiorCreditoValue = document.createElement('span');
    maiorCreditoValue.className = 'heatmap-creditos-footer-value';
    maiorCreditoValue.textContent = `${maiorCredito.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })} dia ${diaMaiorCredito}`;
    footer.appendChild(maiorCreditoValue);
}

function criarGraficoCreditosMultiMes(dados) {
    const ctx = document.getElementById('graficoCreditos').getContext('2d');
    if (!ctx || !dados || !dados.Meses || dados.Meses.length === 0) return;
    if (graficoCreditosInstance) {
        graficoCreditosInstance.destroy();
        graficoCreditosInstance = null;
    }

    const meses = dados.Meses.slice().sort((a, b) => a.Ordem - b.Ordem);
    const mesesCount = meses.length;
    const mesMaisRecente = meses.find(m => m.EhMaisRecente) || meses[mesesCount - 1];
    const mesReferencia = mesesCount >= 2 ? meses[mesesCount - 2] : null;

    const labels = dados.Itens.map(item => item.Dia);

    const datasets = meses.map((mesVM, idx) => {
        const ehPrincipal = mesVM.Chave === mesMaisRecente.Chave;
        const cor = ehPrincipal
            ? idx < PALETA_CORES_CREDITOS.length ? PALETA_CORES_CREDITOS[idx] : PALETA_CORES_CREDITOS[PALETA_CORES_CREDITOS.length - 1]
            : (idx < PALETA_CORES_CREDITOS.length ? PALETA_CORES_CREDITOS[idx] : COR_CINZA_CREDITOS);
        const dataArr = dados.Itens.map(item => {
            const v = item.ValoresPorMes.find(vm => vm.ChaveMes === mesVM.Chave);
            return v ? v.CreditosAcumulados : null;
        });

        return {
            label: mesVM.Nome,
            chaveMes: mesVM.Chave,
            data: dataArr,
            borderColor: cor.border,
            backgroundColor: ehPrincipal ? cor.bg : COR_CINZA_CREDITOS.bg,
            pointBackgroundColor: cor.border,
            pointRadius: 0,
            pointHoverRadius: 6,
            pointHoverBackgroundColor: cor.border,
            pointHoverBorderColor: '#fff',
            pointHoverBorderWidth: 2,
            borderWidth: ehPrincipal ? 3.5 : 2.5,
            tension: 0.3,
            fill: ehPrincipal,
            spanGaps: false,
            borderDash: ehPrincipal ? [] : [6, 4],
            order: ehPrincipal ? 0 : (idx + 1)
        };
    });

    let valorPrincipalUltimoDia = null;
    let valorReferenciaUltimoDia = null;
    if (dados.Itens.length > 0) {
        const itensReversos = dados.Itens.slice().reverse();
        for (const item of itensReversos) {
            const vp = item.ValoresPorMes.find(v => v.ChaveMes === mesMaisRecente.Chave);
            if (vp && vp.CreditosAcumulados !== null && vp.CreditosAcumulados !== undefined) {
                valorPrincipalUltimoDia = vp.CreditosAcumulados;
                if (mesReferencia) {
                    const vr = item.ValoresPorMes.find(v => v.ChaveMes === mesReferencia.Chave);
                    if (vr && vr.CreditosAcumulados !== null && vr.CreditosAcumulados !== undefined) {
                        valorReferenciaUltimoDia = vr.CreditosAcumulados;
                    }
                }
                break;
            }
        }
    }

    const temComparacao = mesReferencia &&
        valorPrincipalUltimoDia !== null &&
        valorReferenciaUltimoDia !== null &&
        valorReferenciaUltimoDia !== 0;
    const diferencaGeral = temComparacao ? valorPrincipalUltimoDia - valorReferenciaUltimoDia : 0;
    const principalAbaixo = temComparacao && diferencaGeral < 0;
    const principalAcima = temComparacao && diferencaGeral > 0;

    let classeRitmo;
    let iconeRitmo;
    let textoRitmo;
    if (principalAcima) {
        classeRitmo = 'ritmo-creditos--acima';
        iconeRitmo = '↗';
        textoRitmo = 'acima';
    } else if (principalAbaixo) {
        classeRitmo = 'ritmo-creditos--abaixo';
        iconeRitmo = '↘';
        textoRitmo = 'abaixo';
    } else {
        classeRitmo = 'ritmo-creditos--igual';
        iconeRitmo = '↔';
        textoRitmo = 'igual';
    }
    const porcentagemGeral = temComparacao ? Math.abs((diferencaGeral / valorReferenciaUltimoDia) * 100) : 0;

    graficoCreditosInstance = new Chart(ctx, {
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
                const canvas = chart.canvas;
                const rect = canvas.getBoundingClientRect();
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
                        color: isDarkThemeCreditos() ? '#e5e7eb' : '#374151'
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
                        size: 22,
                        weight: 'bold'
                    },
                    bodyFont: {
                        size: 15
                    },
                    footerFont: {
                        size: 15,
                        weight: '600'
                    },
                    callbacks: {
                        title: function(tooltipItems) {
                            if (!tooltipItems || tooltipItems.length === 0) return '';
                            const dia = tooltipItems[0].label;
                            return 'Dia ' + dia;
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
                            if (!tooltipItems || tooltipItems.length < 2) return '';
                            const linhas = [];
                            const mapa = {};
                            tooltipItems.forEach(ti => {
                                const ds = ti.dataset;
                                const chave = ds.chaveMes || ds.label;
                                mapa[chave] = {
                                    label: ds.label,
                                    valor: ti.raw,
                                    ordem: ds.order
                                };
                            });

                            const ordenado = Object.values(mapa).sort((a, b) => (a.ordem ?? 999) - (b.ordem ?? 999));
                            const principal = ordenado[ordenado.length - 1];

                            for (let i = 0; i < ordenado.length - 1; i++) {
                                const outro = ordenado[i];
                                if (principal.valor === null || principal.valor === undefined ||
                                    outro.valor === null || outro.valor === undefined ||
                                    outro.valor === 0) continue;

                                const diff = principal.valor - outro.valor;
                                const pct = (diff / outro.valor) * 100;
                                const aumento = diff > 0;
                                const sinal = aumento ? '+' : '';
                                const icone = aumento ? '↗' : '↘';
                                const texto = aumento ? 'Aumento' : 'Queda';
                                const diffStr = diff.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
                                linhas.push(`${icone} vs ${outro.label}: ${texto} ${sinal}${diffStr} (${sinal}${pct.toFixed(0)}%)`);
                            }

                            if (linhas.length > 0) {
                                return ['', ...linhas];
                            }
                            return '';
                        }
                    },
                    footerBackgroundColor: function(tooltipItems) {
                        if (!tooltipItems || tooltipItems.length < 2) return 'transparent';
                        const mapa = {};
                        tooltipItems.forEach(ti => {
                            const ds = ti.dataset;
                            const chave = ds.chaveMes || ds.label;
                            mapa[chave] = { valor: ti.raw, ordem: ds.order };
                        });
                        const ordenado = Object.values(mapa).sort((a, b) => (a.ordem ?? 999) - (b.ordem ?? 999));
                        if (ordenado.length < 2) return 'transparent';
                        const principal = ordenado[ordenado.length - 1];
                        const ref = ordenado[ordenado.length - 2];
                        if (principal.valor === null || principal.valor === undefined ||
                            ref.valor === null || ref.valor === undefined) {
                            return 'transparent';
                        }
                        const diff = principal.valor - ref.valor;
                        return diff >= 0
                            ? 'rgba(16, 185, 129, 0.95)'
                            : 'rgba(248, 113, 113, 0.95)';
                    }
                },
                crosshair: false
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: {
                        color: isDarkThemeCreditos() ? 'rgba(75, 85, 99, 0.3)' : 'rgba(229, 231, 235, 0.8)'
                    },
                    ticks: {
                        font: {
                            size: 13,
                            weight: '500'
                        },
                        color: isDarkThemeCreditos() ? '#9ca3af' : '#4b5563',
                        padding: 12,
                        callback: function(value) {
                            return value.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
                        }
                    }
                },
                x: {
                    grid: {
                        display: false
                    },
                    ticks: {
                        font: {
                            size: 13,
                            weight: '500'
                        },
                        color: isDarkThemeCreditos() ? '#9ca3af' : '#4b5563',
                        padding: 12
                    }
                }
            }
        }
    });

    const graficoCardBody = document.querySelector('#graficoCreditos').closest('.card-body');
    const existingRitmo = document.getElementById('ritmoCreditos');
    if (existingRitmo) existingRitmo.remove();

    if (graficoCardBody && temComparacao) {
        const ritmoDiv = document.createElement('div');
        ritmoDiv.id = 'ritmoCreditos';
        ritmoDiv.className = 'ritmo-creditos ' + classeRitmo;

        const ritTitle = document.createElement('div');
        ritTitle.className = 'ritmo-creditos-title';
        ritTitle.textContent = 'Ritmo de créditos';

        const row1 = document.createElement('div');
        row1.className = 'ritmo-creditos-row-1';

        const valorAtual = document.createElement('span');
        valorAtual.className = 'ritmo-creditos-valor-atual';
        valorAtual.textContent = Math.abs(diferencaGeral).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });

        const statusLabel = document.createElement('span');
        statusLabel.className = 'ritmo-creditos-status-label';
        statusLabel.textContent = textoRitmo;

        row1.appendChild(valorAtual);
        row1.appendChild(statusLabel);

        const row2 = document.createElement('div');
        row2.className = 'ritmo-creditos-row-2';

        const badge = document.createElement('span');
        badge.className = 'ritmo-creditos-badge';
        badge.textContent = `${iconeRitmo} ${diferencaGeral >= 0 ? '+' : ''}${porcentagemGeral.toFixed(1)}%`;

        const vsText = document.createElement('span');
        vsText.className = 'ritmo-creditos-vs';
        vsText.innerHTML = `vs ${valorReferenciaUltimoDia.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })} (${mesReferencia.Nome})`;

        row2.appendChild(badge);
        row2.appendChild(vsText);

        ritmoDiv.appendChild(ritTitle);
        ritmoDiv.appendChild(row1);
        ritmoDiv.appendChild(row2);

        graficoCardBody.appendChild(ritmoDiv);
    }
}

function inicializarDashboardCreditosRelatorio(dadosCreditosGlobal, dadosCreditosMultiMesGlobal) {
    $(document).ready(function() {
        inicializarMultiSelectMesesCreditos();
        inicializarMultiSelectContasCreditos();

        if (dadosCreditosGlobal) {
            criarHeatmapCreditos(dadosCreditosGlobal);
        }
        if (dadosCreditosMultiMesGlobal) {
            criarGraficoCreditosMultiMes(dadosCreditosMultiMesGlobal);
        }

        document.addEventListener('themeChanged', function() {
            if (dadosCreditosGlobal) {
                criarHeatmapCreditos(dadosCreditosGlobal);
            }
            if (dadosCreditosMultiMesGlobal && graficoCreditosInstance) {
                graficoCreditosInstance.destroy();
                criarGraficoCreditosMultiMes(dadosCreditosMultiMesGlobal);
            }
        });
    });
}
