<template>
  <Toast :message="toastMessage" />

  <div class="dashboard">
    <nav class="dashboard-nav">
      <ul>
        <li><a href="#" @click.prevent="goToHome">Home</a></li>
        <li><a href="#" @click.prevent="goToPerfil">Meu Perfil</a></li>
        <li><a href="#" @click.prevent="logout">Sair</a></li>
      </ul>
    </nav>

    <section class="dashboard-content">
      <h1>Bem-vindo ao Dashboard do Fornecedor!</h1>

      <section class="offers">
        <div class="offers-header">
          <h2>Ofertas de Energia</h2>
          <div class="nova-oferta-container">
            <button @click="goToOfertaEnergia" 
                    :disabled="!podecriarNovaOferta"
                    :class="{ 'disabled': !podecriarNovaOferta }"
                    class="btn-nova-oferta">
              + Nova Oferta
            </button>
          </div>
        </div>

        <div v-if="offers.length === 0" class="no-offers">
          Você ainda não tem ofertas cadastradas.
        </div>

        <ul class="offers-list">
          <li v-for="(offer, index) in offers" :key="offer.id" class="offer-item">
            <div>
              <strong>{{ offer.quantidadeEnergia }} kWh</strong><br />
              <span>R$ {{ offer.valorContrato.toFixed(2) }} por kWh</span><br />
              <span>Disponível em: {{ new Date(offer.dataContrato).toLocaleDateString() }}</span><br />
              <span :class="getStatusClass(offer.status)" class="status-badge">
                {{ getStatusText(offer.status) }}
              </span>
            </div>
            
          </li>
        </ul>
      </section>
    </section>
  </div>
</template>

<script>
import Toast from '../components/Toast.vue'
import { fornecedorService } from '@/services/fornecedor'
import { authService } from '@/services/auth'

const StatusContrato = {
  EM_ANALISE: 0,
  EM_VIGOR: 1,
  CANCELADO: 2,
  CONCLUIDO: 3
}

export default {
  components: { Toast },
  data() {
    return {
      toastMessage: '',
      offers: [],
      StatusContrato
    }
  },
  computed: {
    podecriarNovaOferta() {
      return this.offers.length === 0
    }
  },
  mounted() {
    this.loadOffers()
  },
  methods: {
    async logout() {
      try {
        await authService.logout()
        this.$router.push('/')
      } catch (error) {
        console.error('Erro ao fazer logout:', error)
        this.$router.push('/')
      }
    },
    goToPerfil() {
      this.$router.push('/PerfilFornecedor')
    },
    goToHome() {
      this.$router.push('/')
    },
    goToOfertaEnergia() {
      this.$router.push('/OfertaEnergia')
    },
    async loadOffers() {
      try {
        const ofertas = await fornecedorService.getMinhasOfertas()
        this.offers = ofertas
      } catch (error) {
        console.error('Erro ao carregar ofertas:', error)
        this.offers = []
      }
    },
    
    getStatusText(status) {
      switch (status) {
        case StatusContrato.EM_ANALISE:
          return '⏳ Em Análise'
        case StatusContrato.EM_VIGOR:
          return '✅ Em Vigor'
        case StatusContrato.CANCELADO:
          return '❌ Cancelado'
        case StatusContrato.CONCLUIDO:
          return '🏆 Concluído'
        default:
          return 'Status Desconhecido'
      }
    },
    getStatusClass(status) {
      switch (status) {
        case StatusContrato.EM_ANALISE:
          return 'status-analise'
        case StatusContrato.EM_VIGOR:
          return 'status-vigor'
        case StatusContrato.CANCELADO:
          return 'status-cancelado'
        case StatusContrato.CONCLUIDO:
          return 'status-concluido'
        default:
          return ''
      }
    }
  }
}
</script>

<style scoped>
.dashboard {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.dashboard-nav {
  background-color: #fff;
  padding: 1rem 2rem;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.08);
  position: sticky;
  top: 0;
  z-index: 10;
}

.dashboard-nav ul {
  display: flex;
  justify-content: center;
  gap: 2rem;
  list-style: none;
  margin: 0;
  padding: 0;
}

.dashboard-nav a {
  text-decoration: none;
  color: #ff8800;
  font-weight: 600;
  font-size: 1rem;
  padding: 0.6rem 1rem;
  border-radius: 8px;
  transition: all 0.3s ease;
}

.dashboard-nav a:hover {
  background-color: #ff8800;
  color: white;
  transform: translateY(-2px);
}

.dashboard-content {
  max-width: 900px;
  margin: 2rem auto;
  padding: 0 1rem;
  flex-grow: 1;
}

.dashboard-content h1 {
  font-size: 2rem;
  color: #ff8800;
  text-align: center;
  margin-bottom: 2rem;
}

.offers {
  background: white;
  padding: 2rem;
  border-radius: 16px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.06);
}

.offers-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.offers h2 {
  font-size: 1.4rem;
  color: #ff8800;
}

.offers-header button {
  background-color: #ff8800;
  color: white;
  border: none;
  padding: 0.6rem 1rem;
  border-radius: 8px;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.offers-header button:hover {
  background-color: #e67300;
}

.no-offers {
  text-align: center;
  color: #999;
  font-style: italic;
  padding: 1rem;
}

.offers-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.offer-item {
  background: #fdf5e6;
  border: 1px solid #ffddaa;
  border-radius: 12px;
  padding: 1rem 1.5rem;
  margin-bottom: 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.offer-item strong {
  font-size: 1.2rem;
  color: #333;
}

.offer-item span {
  color: #777;
}

.offer-item button {
  background-color: #e74c3c;
  color: white;
  border: none;
  padding: 0.4rem 0.8rem;
  border-radius: 6px;
  font-weight: bold;
  transition: background-color 0.3s;
}

.offer-item button:hover {
  background-color: #c0392b;
}

.status-badge {
  display: inline-block;
  padding: 0.2rem 0.5rem;
  border-radius: 4px;
  font-weight: bold;
  margin-top: 0.5rem;
}

.status-analise {
  color: #f39c12;
  background-color: #fef3c7;
}

.status-vigor {
  color: #2ecc71;
  background-color: #ecfdf5;
}

.status-cancelado {
  color: #e74c3c;
  background-color: #fef2f2;
}

.status-concluido {
  color: #3498db;
  background-color: #eff6ff;
}

.btn-cancelar {
  background-color: #e74c3c;
  color: white;
  border: none;
  padding: 0.4rem 0.8rem;
  border-radius: 6px;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.3s;
}

.btn-cancelar:hover {
  background-color: #c0392b;
}

.btn-cancelar:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.nova-oferta-container {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.btn-nova-oferta {
  background-color: #ff8800;
  color: white;
  border: none;
  padding: 0.6rem 1rem;
  border-radius: 8px;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.btn-nova-oferta:hover {
  background-color: #e67300;
}

.btn-nova-oferta:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.aviso-oferta {
  color: #777;
  font-size: 0.9rem;
}
</style>
