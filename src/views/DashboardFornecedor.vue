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
          <button @click="goToOfertaEnergia">+ Nova Oferta</button>
        </div>

        <div v-if="offers.length === 0" class="no-offers">
          Você ainda não tem ofertas cadastradas.
        </div>

        <ul class="offers-list">
          <li v-for="(offer, index) in offers" :key="index" class="offer-item">
            <div>
              <strong>{{ offer.quantidade }} kWh</strong> <br />
              <span>R$ {{ offer.preco }} por kWh</span>
            </div>
            <button @click="removerOferta(index)">Remover</button>
          </li>
        </ul>
      </section>
    </section>
  </div>
</template>

<script>
import Toast from '../components/Toast.vue'

export default {
  components: { Toast },
  data() {
    return {
      toastMessage: '',
      offers: []
    }
  },
  mounted() {
    this.loadOffers()
  },
  methods: {
    logout() {
      localStorage.removeItem('usuarioLogado')
      this.$router.push('/')
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
    loadOffers() {
      const savedOffers = JSON.parse(localStorage.getItem('userOffers') || '[]')
      this.offers = savedOffers.filter(
        offer => offer && offer.quantidade && offer.preco
      )
    },
    removerOferta(index) {
      this.offers.splice(index, 1)
      localStorage.setItem('userOffers', JSON.stringify(this.offers))

    
      this.toastMessage = ''
      setTimeout(() => {
        this.toastMessage = 'Oferta removida com sucesso!'
      }, 50)
    }
  }
}
</script>

<style scoped>
.dashboard {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  background-color: #f9f9f9;
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
  color: #333;
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
</style>
