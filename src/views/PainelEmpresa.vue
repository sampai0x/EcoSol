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

    <div v-if="loading" class="loading">
      Carregando dados...
    </div>

    <div v-else>
      <div class="stats-cards" v-if="stats">
        <div class="stat-card">
          <h3>{{ stats.totalClientes || 0 }}</h3>
          <p>Clientes</p>
        </div>
        <div class="stat-card">
          <h3>{{ stats.totalFornecedores || 0 }}</h3>
          <p>Fornecedores</p>
        </div>
        <div class="stat-card">
          <h3>{{ stats.totalContratos || 0 }}</h3>
          <p>Contratos</p>
        </div>
        <div class="stat-card saldo-card">
          <h3>{{ formatSaldoResumido(stats.saldo || 0) }}</h3>
          <p>Saldo</p>
        </div>
      </div>

      <div class="cards">
        <div class="card" @click="irParaPedidos">
          <h2>Pedidos de Energia</h2>
          <p>Visualize e gerencie os pedidos dos clientes.</p>
          <span class="card-count" v-if="stats?.totalPedidos">{{ stats.totalPedidos }} pedidos</span>
        </div>

        <div class="card" @click="irParaOfertas">
          <h2>Ofertas de Fornecedores</h2>
          <p>Veja as ofertas disponíveis de cada fornecedor.</p>
          <span class="card-count" v-if="stats?.totalContratos">{{ stats.totalContratos }} contratos</span>
        </div>

        <div class="card" @click="irParaUsuarios">
          <h2>Usuários Cadastrados</h2>
          <p>Consulte clientes e fornecedores registrados.</p>
          <span class="card-count" v-if="stats">{{ stats.totalClientes + stats.totalFornecedores }} usuários</span>
        </div>
      </div>

      <div class="graficos">
        <div class="grafico">
          <Bar 
            :data="energiaChartData" 
            :options="energiaChartOptions"
            style="height: 300px;"
          />
        </div>
        <div class="grafico">
          <Bar 
            :data="saldoChartData" 
            :options="saldoChartOptions"
            style="height: 300px;"
          />
        </div>
      </div>
    </div>

    <div v-if="erro" class="erro-message">{{ erro }}</div>
  </div>
</template>

<script>
import { Line, Bar } from 'vue-chartjs'
import { adminService } from '@/services/admin'
import { authService } from '@/services/auth'

import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  LineElement,
  BarElement,
  PointElement,
  CategoryScale,
  LinearScale
} from 'chart.js'

ChartJS.register(
  Title, 
  Tooltip, 
  Legend, 
  LineElement, 
  BarElement,
  PointElement, 
  CategoryScale, 
  LinearScale
)

export default {
  name: 'PainelEmpresa',
  components: {
    Line,
    Bar
  },
  data() {
    return {
      loading: false,
      erro: '',
      stats: null,
    }
  },
  async mounted() {
    await this.verificarAutenticacao()
    await this.carregarDados()
  },
  computed: {
    energiaChartData() {
      if (!this.stats) return {
        labels: [],
        datasets: []
      }

      return {
        labels: ['Energia'],
        datasets: [
          {
            label: 'Energia Contratada (kWh)',
            backgroundColor: '#10b981',
            data: [this.stats.energiaContratada || 0],
            barPercentage: 0.5
          },
          {
            label: 'Energia Solicitada (kWh)',
            backgroundColor: '#f97316',
            data: [this.stats.energiaSolicitada || 0],
            barPercentage: 0.5
          }
        ]
      }
    },
    energiaChartOptions() {
      return {
        responsive: true,
        maintainAspectRatio: false,
        indexAxis: 'y',
        scales: {
          x: {
            beginAtZero: true,
            title: {
              display: true,
              text: 'Quantidade (kWh)'
            }
          }
        },
        plugins: {
          legend: {
            position: 'top',
            labels: {
              font: {
                size: 12
              }
            }
          },
          title: {
            display: true,
            text: 'Comparação de Energia',
            font: {
              size: 16
            }
          }
        }
      }
    },
    saldoChartData() {
      if (!this.stats) return {
        labels: [],
        datasets: []
      }

      return {
        labels: ['Valores Atuais'],
        datasets: [
          {
            label: 'Saldo (R$)',
            backgroundColor: '#2563eb',
            data: [this.stats.saldo || 0],
            barPercentage: 0.5,
            yAxisID: 'y'
          },
          {
            label: 'Banco de Energia (kWh)',
            backgroundColor: '#f97316',
            data: [this.stats.bancoEnergia || 0],
            barPercentage: 0.5,
            yAxisID: 'y1'
          }
        ]
      }
    },
    saldoChartOptions() {
      return {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          y: {
            type: 'linear',
            position: 'left',
            title: {
              display: true,
              text: 'Saldo (R$)',
              font: {
                weight: 'bold',
                size: 14
              }
            },
            ticks: {
              callback: function(value) {
                if (value >= 1000000) {
                  return 'R$ ' + (value / 1000000).toLocaleString('pt-BR', { 
                    minimumFractionDigits: 1, 
                    maximumFractionDigits: 1 
                  }) + ' M'
                } else if (value >= 1000) {
                  return 'R$ ' + (value / 1000).toLocaleString('pt-BR', { 
                    minimumFractionDigits: 1, 
                    maximumFractionDigits: 1 
                  }) + ' K'
                }
                return 'R$ ' + value.toLocaleString('pt-BR')
              },
              font: {
                size: 12
              }
            }
          },
          y1: {
            type: 'linear',
            position: 'right',
            title: {
              display: true,
              text: 'Energia (kWh)',
              font: {
                weight: 'bold',
                size: 14
              }
            },
            ticks: {
              callback: function(value) {
                if (value >= 1000000) {
                  return (value / 1000000).toLocaleString('pt-BR', { 
                    minimumFractionDigits: 1, 
                    maximumFractionDigits: 1 
                  }) + ' M kWh'
                } else if (value >= 1000) {
                  return (value / 1000).toLocaleString('pt-BR', { 
                    minimumFractionDigits: 1, 
                    maximumFractionDigits: 1 
                  }) + ' K kWh'
                }
                return value.toLocaleString('pt-BR') + ' kWh'
              },
              font: {
                size: 12
              }
            },
            grid: {
              drawOnChartArea: false
            }
          }
        },
        plugins: {
          legend: {
            position: 'top',
            labels: {
              font: {
                size: 12,
                weight: 'bold'
              },
              padding: 20
            }
          },
          title: {
            display: true,
            text: 'Saldo e Banco de Energia',
            font: {
              size: 16,
              weight: 'bold'
            },
            padding: {
              top: 10,
              bottom: 30
            }
          },
          tooltip: {
            callbacks: {
              label: function(context) {
                let value = context.raw;
                if (context.dataset.yAxisID === 'y') {
                  return 'Saldo: R$ ' + value.toLocaleString('pt-BR', {
                    minimumFractionDigits: 2,
                    maximumFractionDigits: 2
                  });
                } else {
                  return 'Energia: ' + value.toLocaleString('pt-BR') + ' kWh';
                }
              }
            }
          }
        }
      }
    }
  },
  methods: {
    async verificarAutenticacao() {
      try {
        const usuario = await authService.getCurrentUser()
        if (!usuario || usuario.tipo !== 'Empresa') {
          this.$router.push('/')
          return
        }
      } catch (error) {
        console.error('Erro ao verificar autenticação:', error)
        this.$router.push('/')
      }
    },

    async carregarDados() {
      this.loading = true
      this.erro = ''
      
      try {
        // Carregar estatísticas da empresa
        this.stats = await adminService.getEmpresaStats()
      } catch (error) {
        console.error('Erro ao carregar dados:', error)
        this.erro = 'Erro ao carregar dados do painel'
      } finally {
        this.loading = false
      }
    },

    formatSaldoResumido(value) {
      if (value >= 1000000) {
        return 'R$ ' + (value / 1000000).toLocaleString('pt-BR', {
          minimumFractionDigits: 1,
          maximumFractionDigits: 1
        }) + ' M'
      } else if (value >= 1000) {
        return 'R$ ' + (value / 1000).toLocaleString('pt-BR', {
          minimumFractionDigits: 1,
          maximumFractionDigits: 1
        }) + ' K'
      }
      return 'R$ ' + value.toLocaleString('pt-BR')
    },

    formatCurrency(value) {
      return new Intl.NumberFormat('pt-BR', { 
        minimumFractionDigits: 2, 
        maximumFractionDigits: 2 
      }).format(value)
    },

    irParaPedidos() { this.$router.push('/pedidos') },
    irParaOfertas() { this.$router.push('/ofertas') },
    irParaUsuarios() { this.$router.push('/usuariosview') },
    goTohome() { this.$router.push('/') }
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

.loading {
  text-align: center;
  padding: 2rem;
  color: #6b7280;
  font-style: italic;
}

.erro-message {
  color: #dc2626;
  font-size: 0.875rem;
  margin-top: 1rem;
  padding: 0.75rem;
  background-color: #fef2f2;
  border-radius: 8px;
  border: 1px solid #fecaca;
  text-align: center;
}

.stats-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.stat-card {
  background: white;
  padding: 1.5rem;
  border-radius: 12px;
  text-align: center;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.06);
  border: 1px solid #fcd34d;
}

.stat-card h3 {
  font-size: 1.8rem;
  font-weight: bold;
  color: #92400e;
  margin-bottom: 0.5rem;
}

.stat-card p {
  color: #6b7280;
  font-size: 0.9rem;
  margin: 0;
}

.cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.8rem;
  margin-bottom: 2rem;
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
  position: relative;
}

.card h2 {
  color: #92400e;
  font-size: 1.4rem;
  margin-bottom: 0.8rem;
}

.card p {
  color: #78350f;
  font-size: 0.95rem;
  margin-bottom: 1rem;
}

.card-count {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: #fcd34d;
  color: #92400e;
  padding: 0.25rem 0.5rem;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: bold;
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
  padding: 1.5rem;
  border-radius: 1rem;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.08);
  height: 350px;
}

.grafico h2 {
  text-align: center;
  color: #92400e;
  margin-bottom: 1rem;
}

.saldo-card h3 {
  font-size: 1.6rem;
  line-height: 1.2;
  color: #047857;
}

.saldo-card {
  background: #ecfdf5;
  border-color: #6ee7b7;
}
</style>
