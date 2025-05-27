<template>
  <nav class="navbar">
    <div class="navbar-container">
      <div class="logo">
        <img src="/src/img/EcoSolNavBar.png" alt="Logo" class="logo-img" />
      </div>
      <button class="voltar" @click="goTohome">← Voltar para Home</button>
    </div>
  </nav>
  <div class="painel-empresa">
    <div class="header">
      <h1>Painel Administrativo</h1>
      <p class="subtitulo">Bem-vindo, administrador da EcoSol</p>
    </div>

    <div class="cards">
      <div class="card" @click="irParaPedidos">
        <h2>Pedidos de Energia</h2>
        <p>Visualize e gerencie os pedidos dos clientes.</p>
      </div>

      <div class="card" @click="irParaOfertas">
        <h2>Ofertas de Fornecedores</h2>
        <p>Veja as ofertas disponíveis de cada fornecedor.</p>
      </div>

      <div class="card" @click="irParaUsuarios">
        <h2>Usuários Cadastrados</h2>
        <p>Consulte clientes e fornecedores registrados.</p>
      </div>
    </div>
    <div class="graficos">
      <div class="grafico">
        <h2>Histórico de Pedidos de Energia</h2>
        <Line :data="chartData" />
      </div>
      <div class="grafico">
        <h2>Saldo Financeiro da Empresa (R$)</h2>
        <Line :data="chartFinanceiro" />
      </div>
    </div>

  </div>
</template>

<script>
import { Line } from 'vue-chartjs'

import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  LineElement,
  PointElement,
  CategoryScale,
  LinearScale
} from 'chart.js'

ChartJS.register(Title, Tooltip, Legend, LineElement, PointElement, CategoryScale, LinearScale)

export default {
  name: 'PainelEmpresa',
  components: {
    Line
  },
  data() {
    return {
      pedidos: [],
    }
  },
  mounted() {
    this.carregarPedidos();
  },
  methods: {
    carregarPedidos() {
      const pedidosSalvos = JSON.parse(localStorage.getItem('pedidosEnergia') || '[]');
      this.pedidos = pedidosSalvos;
    },
    irParaPedidos() { this.$router.push('/pedidos'); },
    irParaOfertas() { this.$router.push('/ofertas'); },
    irParaUsuarios() { this.$router.push('/usuariosview'); },
    goTohome() { this.$router.push('/') }
  },
  computed: {
    chartData() {
      const energiaClientes = {};
      const energiaFornecedores = {};

      function parseData(dataStr) {
        const partes = dataStr.split('/');
        if (partes.length === 3) {
          return new Date(`${partes[2]}-${partes[1].padStart(2, '0')}-${partes[0].padStart(2, '0')}`);
        }
        return new Date(dataStr);
      }

      // Agrupando pedidos por mês
      this.pedidos.forEach(pedido => {
        const data = parseData(pedido.dataPedido);
        if (isNaN(data.getTime())) return;

        const chave = `${data.getFullYear()}-${String(data.getMonth() + 1).padStart(2, '0')}-01`;
        const qtd = Number(pedido.quantidade) || 0;
        energiaClientes[chave] = (energiaClientes[chave] || 0) + qtd;
      });

      // Agrupando ofertas por mês
      const ofertasRaw = JSON.parse(localStorage.getItem('ofertasEnergia') || '{}');
      const ofertasArray = Object.values(ofertasRaw);

      ofertasArray.forEach(oferta => {
        const data = parseData(oferta.dataDisponivel);
        if (isNaN(data.getTime())) return;

        const chave = `${data.getFullYear()}-${String(data.getMonth() + 1).padStart(2, '0')}-01`;
        const qtd = Number(oferta.quantidade) || 0;
        energiaFornecedores[chave] = (energiaFornecedores[chave] || 0) + qtd;
      });

      // Unificar todas as datas
      const todasDatas = new Set([...Object.keys(energiaClientes), ...Object.keys(energiaFornecedores)]);
      const datasOrdenadas = Array.from(todasDatas).sort();

      const formatador = new Intl.DateTimeFormat('pt-BR', { month: 'short', year: 'numeric' });

      const labels = datasOrdenadas.map(dataStr => {
        const data = new Date(dataStr);
        return formatador.format(data);
      });

      const dadosClientes = datasOrdenadas.map(d => energiaClientes[d] || 0);
      const dadosFornecedores = datasOrdenadas.map(d => energiaFornecedores[d] || 0);

      return {
        labels,
        datasets: [
          {
            label: 'Energia Solicitada pelos Clientes (kWh)',
            borderColor: '#f97316',
            backgroundColor: '#f97316',
            data: dadosClientes,
            fill: false,
            tension: 0.3
          },
          {
            label: 'Energia Oferecida pelos Fornecedores (kWh)',
            borderColor: '#10b981',
            backgroundColor: '#10b981',
            data: dadosFornecedores,
            fill: false,
            tension: 0.3
          }
        ]
      };
    },
    chartFinanceiro() {
      const lucroPorKWh = 2;
      const energiaPorMes = {};

      const ofertasRaw = JSON.parse(localStorage.getItem('ofertasEnergia') || '{}');
      const ofertasArray = Object.values(ofertasRaw);

      const pedidosArray = JSON.parse(localStorage.getItem('pedidosEnergia') || '[]');

      function parseData(dataStr) {
        const partes = dataStr.split('/');
        if (partes.length === 3) {
          return new Date(`${partes[2]}-${partes[1].padStart(2, '0')}-${partes[0].padStart(2, '0')}`);
        }
        return new Date(dataStr);
      }

      // Agrupar por mês
      ofertasArray.forEach(oferta => {
        const data = parseData(oferta.dataDisponivel);
        if (isNaN(data.getTime())) return;
        const chave = `${data.getFullYear()}-${String(data.getMonth() + 1).padStart(2, '0')}-01`;
        energiaPorMes[chave] = energiaPorMes[chave] || { ofertado: 0, pedido: 0 };
        energiaPorMes[chave].ofertado += Number(oferta.quantidade) || 0;
      });

      pedidosArray.forEach(pedido => {
        const data = parseData(pedido.dataPedido);
        if (isNaN(data.getTime())) return;
        const chave = `${data.getFullYear()}-${String(data.getMonth() + 1).padStart(2, '0')}-01`;
        energiaPorMes[chave] = energiaPorMes[chave] || { ofertado: 0, pedido: 0 };
        energiaPorMes[chave].pedido += Number(pedido.quantidade) || 0;
      });

      const datasOrdenadas = Object.keys(energiaPorMes).sort();

      const labels = datasOrdenadas.map(dataStr => {
        const data = new Date(dataStr);
        return new Intl.DateTimeFormat('pt-BR', { month: 'short', year: 'numeric' }).format(data);
      });

      const valoresR$ = [];
      const saldosEnergia = [];
      const energiaDetalhada = [];

      datasOrdenadas.forEach(dataStr => {
        const { ofertado, pedido } = energiaPorMes[dataStr];
        const utilizado = Math.min(ofertado, pedido);
        const saldoEnergia = ofertado - pedido;

        valoresR$.push(utilizado * lucroPorKWh);
        saldosEnergia.push(saldoEnergia);
        energiaDetalhada.push({ ofertado, pedido, utilizado, saldoEnergia });
      });

      return {
        labels,
        datasets: [
          {
            label: 'Saldo (R$)',
            borderColor: '#2563eb',
            backgroundColor: '#93c5fd',
            data: valoresR$,
            fill: true,
            tension: 0.3,
            yAxisID: 'y' // Eixo financeiro
          },
          {
            label: 'Saldo de Energia (kWh)',
            borderColor: '#f97316',
            backgroundColor: '#fdba74',
            data: saldosEnergia,
            fill: false,
            tension: 0.3,
            yAxisID: 'y1' // Eixo de energia
          }
        ],
        options: {
          responsive: true,
          interaction: {
            mode: 'index',
            intersect: false
          },
          scales: {
            y: {
              type: 'linear',
              position: 'left',
              title: {
                display: true,
                text: 'Saldo (R$)'
              }
            },
            y1: {
              type: 'linear',
              position: 'right',
              grid: {
                drawOnChartArea: false
              },
              title: {
                display: true,
                text: 'Energia (kWh)'
              }
            }
          },
          plugins: {
            tooltip: {
              callbacks: {
                afterLabel: function (context) {
                  const index = context.dataIndex;
                  const energia = energiaDetalhada[index];
                  return [
                    `Ofertado: ${energia.ofertado} kWh`,
                    `Pedido: ${energia.pedido} kWh`,
                    `Utilizado: ${energia.utilizado} kWh`,
                    `Saldo de energia: ${energia.saldoEnergia} kWh`
                  ];
                }
              }
            }
          }
        }
      };
    }
  }
}
</script>

<style scoped>
.navbar {
  width: 100%;
  background-color: #ffffff;
  padding: 1rem 2rem;
  position: fixed;
  top: 0;
  left: 0;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  z-index: 10;
}

.navbar-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  max-width: 1080px;
  margin: 0 auto;
}

.logo {
  font-size: 1.5rem;
  font-weight: bold;
  color: white;
  cursor: pointer;
}

.logo {
  display: flex;
  align-items: center;
  cursor: pointer;
}

.logo-img {
  height: 40px;
  margin-right: 10px;
}

.voltar {
  background-color: #ff8800;
  color: white;
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.3s;
}

.voltar:hover {
  background-color: #d97706;
}

.painel-empresa {
  padding: 2rem;
  max-width: 1000px;
  margin: auto;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  background: linear-gradient(to right, #fff7ed, #ffedd5);
  min-height: 100vh;
}

.header {
  margin-top: 80px;
  text-align: center;
  margin-bottom: 2rem;
}

.header h1 {
  font-size: 2.5rem;
  color: #b45309;
}

.subtitulo {
  color: #78350f;
  font-size: 1.1rem;
  margin-top: 0.5rem;
}

.cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.8rem;
}

.card {
  background: #ffffff;
  border-radius: 1rem;
  padding: 2rem;
  text-align: center;
  cursor: pointer;
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
  border: 1px solid #fcd34d;
}

.card h2 {
  color: #92400e;
  font-size: 1.4rem;
  margin-bottom: 0.8rem;
}

.card p {
  color: #78350f;
  font-size: 0.95rem;
}

.card:hover {
  background-color: #fef3c7;
  transform: translateY(-4px);
  box-shadow: 0 10px 16px rgba(0, 0, 0, 0.12);
}

.graficos {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 2rem;
  margin-top: 2rem;
}

.grafico {
  flex: 1 1 400px;
  max-width: 600px;
  margin-top: 50px;
  background: white;
  padding: 1rem;
  border-radius: 1rem;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.08);
}

.grafico h2 {
  text-align: center;
  color: #92400e;
  margin-bottom: 1rem;
}
</style>
