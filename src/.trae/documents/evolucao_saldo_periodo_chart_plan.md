# Plan: Atualizar Visual Gráfico EvolucaoSaldoPeriodo para igualar Dashboard

## 1. Pesquisa do Repositório (arquitetura e diferenças)

### Gráfico Atual (EvolucaoSaldoPeriodo.cshtml)
Arquivo: [EvolucaoSaldoPeriodo.cshtml](file:///c:/dev/github/MinhaCarteira/src/MinhaCarteira.AppCliente/Views/Relatorio/EvolucaoSaldoPeriodo.cshtml#L66-L115)

- **Lib:** Chart.js `4.4.0` (CDN)
- **Tipo:** `line`
- **Séries exibidas:** 1 única série (`SaldoFinal`) com:
  - `borderColor: 'rgb(153, 102, 255)'`
  - `backgroundColor: 'rgba(153, 102, 255, 0.1)'`
  - `tension: 0.3` e `fill: true`
- **Legend:** `position: 'top'` — sem usePointStyle, sem font customizada
- **Tooltip:** BÁSICO — só callback `label` currency BRL
- **Interaction/Hover:** NENHUM (padrão Chart.js `point`)
- **Scales Y:** `beginAtZero: false` (correto para saldos), ticks com currency BRL
- **Plugins custom:** NENHUM (sem linha vertical ao hover)
- **Sem listener de troca de tema**

### Gráfico Referência (Dashboard.cshtml — GraficoGastosMultiMes)
Arquivo: [Dashboard.cshtml](file:///c:/dev/github/MinhaCarteira/src/MinhaCarteira.AppCliente/Views/Relatorio/Dashboard.cshtml#L309-L950)

- **Lib:** Chart.js `4.4.1` (CDN — patch mais novo)
- **Plugin custom `verticalLine`:** desenha linha vertical cinza no eixo X ao passar o mouse (combina com tooltip "por dia")
- **Paleta de CORES:** 12 cores padronizadas + `COR_VERDE`, `COR_VERMELHO`, `COR_CINZA`
- **Options principais:**
  - `interaction: { mode:'index', intersect:false, axis:'x' }` — mostra TODAS séries no mesmo índice ao passar perto
  - `hover: { mode:'index', intersect:false }`
  - **Legend:** position `bottom`, `usePointStyle:true`, `pointStyle:'line'`, `padding:20`, `font size 14/weight 500`
  - **Tooltip:** tema DARK profissional com:
    - `backgroundColor: rgba(31,41,55,0.98)`, titleColor white, bodyColor light
    - `cornerRadius: 16`, `borderWidth:1`, `padding 16/20`
    - `titleFont: 22/bold`, `bodyFont: 15`, `footerFont: 15/600`
    - `usePointStyle:true`, `boxPadding:8`
    - Callbacks: `title`, `label` (currency + tratamento null com '—'), `labelColor` (swatch), `afterBody` (comparação %)
  - **Scales:**
    - `grid.color` adaptativo `isDarkTheme()` (tema claro/escuro)
    - `ticks.color` adaptativo
    - `ticks.font.size:13`
- **isDarkTheme():** detecta `data-bs-theme` no `<html>`
- **Listener `themeChanged`:** destrói e recria instância para atualizar grid/ticks
- **Datasets:**
  - `pointRadius: 0` (nenhum ponto desenhado por padrão)
  - `pointHoverRadius: 6` (só aparece no hover)
  - `pointHoverBorderWidth: 2`, `pointHoverBorderColor: '#fff'`
  - `borderWidth: 3.5` (linhas grossas)
  - `tension: 0.3`
  - `onHover` armazena `chart.$lastHoverX` para o plugin de linha vertical usar

### Dados disponíveis no EvolucaoSaldoPeriodo ViewModel
O `Model.EvolucaoSaldoPeriodo.Itens[]` (já serializado em `relatorioData`) tem OS 4 CAMPOS abaixo, mas HOJE só usamos `SaldoFinal`:
- `SaldoInicial` — saldo do dia antes dos movimentos
- `MovimentosRealizados` — total realizado (movimentos bancários pagos)
- `MovimentosPlanejados` — total planejado (parcelas agendadas)
- `SaldoFinal` — saldo após os movimentos

## 2. Arquivos e módulos a alterar (ESCOPO)
**SOMENTE 1 arquivo** — a view de relatório:
- `c:\dev\github\MinhaCarteira\src\MinhaCarteira.AppCliente\Views\Relatorio\EvolucaoSaldoPeriodo.cshtml`

NÃO alterar: gráfico de pizza "GastosPorCategoria", controllers, viewmodels, repositórios, refit, etc.

## 3. Passos de Implementação (ordem de dependência)
Tudo DENTRO do bloco `@section Scripts { ... }` de EvolucaoSaldoPeriodo.cshtml.

**Passo 1 — Atualizar versão Chart.js no CDN**
- Trocar `https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js` por `chart.js@4.4.1` (igual ao Dashboard, mesma versão evita divergências).

**Passo 2 — Injetar constantes e plugin** (entre o `<script>` de CDN e a função `criarGraficoEvolucaoSaldoPeriodo`):
- Copiar `PALETA_CORES[]` (12 cores), `COR_VERDE`, `COR_VERMELHO` do Dashboard
- Copiar helper `function isDarkTheme()` que lê `document.documentElement.getAttribute('data-bs-theme')`
- Copiar `verticalLinePlugin` (id: 'verticalLine', afterDraw) + `Chart.register(verticalLinePlugin)`
- Criar variável `let graficoSaldoInstance;` (para destruir em re-render troca de tema / reprocessamento)

**Passo 3 — Rewrite da função `criarGraficoEvolucaoSaldoPeriodo(dados)`**
Antes de criar: **destrói instância anterior** se existir (`if (graficoSaldoInstance) graficoSaldoInstance.destroy();`).

Construir `labels` com datas pt-BR (manter igual).

Construir 4 datasets (usando 4 campos do ViewModel):
1. **Saldo Inicial** — paleta[1] (roxo), fill: false, borderDash opcional
2. **Movimentos Realizados** — paleta[7] (verde) fill false
3. **Movimentos Planejados** — paleta[2] (laranja) fill false, borderDash [6,4] (pontilhado)
4. **Saldo Final** — paleta[0] (azul) **fill: true** (linha principal destacada — igual "principal" do Dashboard)

Propriedades COMUNS para todos datasets:
- `pointRadius: 0`
- `pointHoverRadius: 6`
- `pointHoverBorderWidth: 2`
- `pointHoverBorderColor: '#fff'`
- `borderWidth: SaldoFinal=3.5, outros=2.5`
- `tension: 0.3` (curvas suaves)
- `spanGaps: false`

**Passo 4 — Aplicar `options` completas (igual Dashboard):**
- `responsive:true`, `maintainAspectRatio:false`
- **onHover:** armazenar `chart.$lastHoverX = x` quando dentro do eixo X
- **interaction:** mode='index', intersect=false, axis='x'
- **hover:** mode='index', intersect=false
- **plugins.legend:** position='bottom', usePointStyle=true, pointStyle='line', padding=20, font size 14 / weight 500
- **plugins.tooltip:**
  - Tema dark (rgba(31,41,55,0.98), border 1 gray600, cornerRadius 16, padding 16/20)
  - titleFont size 22 bold, bodyFont 15, footerFont 15/600
  - usePointStyle true, boxPadding 8, displayColors true
  - **callbacks:**
    - `title` — formatar data dd/MM/yyyy (pegar tooltipItems[0].label)
    - `label` — currency BRL, se raw for null/undefined → `'—'`
    - `labelColor` — swatch com a borderColor do dataset
    - **afterBody (novo):** quando 4 séries presentes, calcular: `Saldo Final vs Saldo Inicial` (diferença + %, com ↗ ou ↘). Manter apenas uma comparação útil para esta tela (não precisa múltiplas comparações como o Dashboard)
  - `footerBackgroundColor` adaptativo vermelho/verde baseado na diferença SaldoFinal vs SaldoInicial para o dia corrente
- **scales:**
  - `x.grid.color` adaptativo tema, `ticks.color` adaptativo, `ticks.font.size:13`
  - `y.beginAtZero: false` (MANTER igual ao gráfico atual — saldos podem ser negativos), `ticks.callback` currency BRL, grid e ticks cor adaptativa
  - `y.grid.color` adaptativo `isDarkTheme()`

**Passo 5 — listener `themeChanged`**
No `$(document).ready` (após a chamada criarGrafico inicial), adicionar:
```js
document.addEventListener('themeChanged', function() {
    if (graficoSaldoInstance) {
        graficoSaldoInstance.destroy();
        graficoSaldoInstance = null;
    }
    if (window.relatorioData && window.relatorioData.Itens && window.relatorioData.Itens.length > 0) {
        criarGraficoEvolucaoSaldoPeriodo(window.relatorioData);
    }
});
```
E atribuir `window.relatorioData = relatorioData` no início do script para ficar acessível ao handler.

**Passo 6 — Gráfico de Pizza (gastos por categoria): NÃO ALTERAR** por enquanto. Se quiser depois, aplicar tooltip igual ao de linha.

## 4. Dependências e Considerações
- **Nenhuma nova dependência / pacote npm / NuGet.** Só muda versão CDN.
- **CSP (Content Security Policy):** todas `<style>` e `<script>` que já existem na View já usam `nws-csp-add-nonce="true"`. O código novo fica DENTRO da tag `<script>` que já tem nonce.
- **Exportações que capturam canvas (toDataURL):**
  - `btnExportarHTML` (ln 241-246 usa `canvas.toDataURL('image/png')` em `graficoEvolucaoSaldoPeriodo` e `graficoGastosPorCategoria`): **mantém funcionando**, porque o `id="graficoEvolucaoSaldoPeriodo"` do canvas não muda. Instância Chart.js continua renderizando pro mesmo canvas.
  - `btnImprimir` (printThis): imprime o DOM, canvas renderizado normal.
  - `btnExportarCSV`: lê `relatorioData.Itens` JS normal, não usa canvas.
- **Cultura pt-BR:** callbacks `toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })` — mantidos.
- **Saldo negativo:** como `beginAtZero: false`, o gráfico de linha mostra valores abaixo de zero corretamente.
- **Quantidade de dias:** o período pode ter muitos dias (ex: 1 ano = 365 pontos). Chart.js 4.4 aguenta. Para manter a legibilidade de labels no eixo X, Chart.js já auto-omite ticks em datasets densos. Podemos usar `scales.x.ticks.maxTicksLimit: 10` para limitar.

## 5. Validação
1. **`dotnet build MinhaCarteira.AppCliente.csproj --no-restore`** → esperado 0 erros / 0 warnings.
2. **Diagnósticos IDE (GetDiagnostics):** esperado 0.
3. **Verificação manual (opcional rodar local):**
   - Carregar tela `https://localhost:44352/Relatorio/EvolucaoSaldoPeriodo` com período válido
   - Confirmar: 4 linhas (Saldo Inicial, Realizados, Planejados, Saldo Final)
   - Confirmar: legend embaixo, com estilos (usePointStyle line)
   - Passar mouse perto de qualquer dia → aparece tooltip dark com data, todas séries, comparação vs inicial, linha vertical cinza
   - Trocar tema (se botão existir no layout) → gráfico recria, grid/ticks atualizam de claro/escuro
   - Clicar em "Exportar HTML" → imagem PNG do gráfico aparece dentro do HTML salvo
   - Clicar em "Imprimir" → renderiza sem erros
   - Clicar em "CSV" → arquivo CSV com campos corretos

## 6. Riscos e Contorno
| Risco | Mitigação |
|---|---|
| **4 linhas visualmente poluídas** (muitas séries) | Diferenciação clara: Saldo Final com `fill:true` + `borderWidth:3.5` (destacado azul); Planejados com `borderDash` (pontilhado). Resto sem fill. |
| **Muitos pontos (365 dias)** → ticks X lotados | Em `scales.x.ticks` adicionar `maxTicksLimit: 10` e `autoSkip: true`. |
| **Valores em escalas diferentes** (ex: Saldo 10.000, movimentos de ± 500) → linhas de movimento achatadas | Manter 1 eixo Y único. Se usuário reclamar depois, podemos separar em 2 eixos Y (segundo eixo à direita). |
| **CDN Chart.js 4.4.1 com cache antigo** do navegador | Asp-append-version não se aplica a CDN; sem ação necessária (patch vers. é improvável cachear por muito tempo). |
| **Gráfico "GastosPorCategoria" (pie)** fica "feio" em comparação ao novo gráfico de linha | **Fora do escopo atual**, usuário não pediu. Aplicar depois em separado. |
