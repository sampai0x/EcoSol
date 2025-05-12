<template>
  <div class="dashboard">
    <nav class="dashboard-nav">
      <ul>
        <li><a href="#" @click="goToPerfil">Meu Perfil</a></li>
        <li><a href="#" @click="logout">Sair</a></li>
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
export default {
    data() {
        return {
            offers: []
        };
    },
    mounted() {
        this.loadOffers();
    },
    methods: {
        logout() {
            localStorage.removeItem('usuarioLogado');
            this.$router.push('/');
        },
        goToPerfil() {
            this.$router.push('/perfil');
        },
        goToOfertaEnergia() {
            this.$router.push('/OfertaEnergia');
        },
        loadOffers() {
            const savedOffers = JSON.parse(localStorage.getItem('userOffers') || '[]');
            const validOffers = savedOffers.filter(
                offer => offer && offer.quantidade && offer.preco
            );
            console.log('Ofertas carregadas:', validOffers);
            this.offers = validOffers;
        },
        removerOferta(index) {
            this.offers.splice(index, 1);
            localStorage.setItem('userOffers', JSON.stringify(this.offers));
        }
    }
};
</script>

<style scoped>
.dashboard {
    display: flex;
    flex-direction: column;
    height: 100vh;  
}

.dashboard-nav {
    width: 100%;  
    background-color: #ffffff; 
    padding: 20px;
    border-bottom: 1px solid #ddd; 
    
}

.dashboard-nav ul {
    list-style: none;
    padding: 0;
    display: flex;
    justify-content: space-around; 
}

.dashboard-nav li {
    margin-bottom: 0;
}

.dashboard-nav a {
    text-decoration: none;
    color: #ff8800; 
    font-weight: bold;
    display: block;
    padding: 12px 18px;  
    border-radius: 8px;  
    transition: all 0.3s ease;  
    text-transform: uppercase;
}

.dashboard-nav a:hover {
    background-color: #ff8800;  
    color: #fff;  
    transform: scale(1.05); 
}

.dashboard-nav a:active {
    background-color: #ff8800;  
    color: #fff;  
    transform: scale(1.05);
}

.dashboard-content {
    padding: 20px;
    margin-top: 20px;  
    flex-grow: 1;
    overflow-y: auto;  
}
.offers {
  margin-top: 2rem;
  background: #fff;
  padding: 1.5rem;
  border-radius: 12px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.05);
}

.offers h2 {
  margin-bottom: 1rem;
  font-size: 1.5rem;
}

button {
  background-color: #ff8800;
  color: white;
  padding: 0.5rem 1rem;
  margin-bottom: 1rem;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.3s;
}

button:hover {
  background-color: #d97706;
}

.offer-item {
  display: flex;
  justify-content: space-between;
  padding: 1rem;
  margin: 0.5rem 0;
  background: #f1f1f1;
  border-radius: 8px;
}

</style>
