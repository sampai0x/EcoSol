<template>
  <div class="dashboard">
    <nav class="dashboard-nav">
      <ul>
        <li><a href="#" @click.prevent="goToHome">Home</a></li>
        <li><a href="#" @click.prevent="goToPerfil">Meu Perfil</a></li>
        <li><a href="#" @click.prevent="logout">Sair</a></li>
      </ul>
    </nav>

    <section class="dashboard-content">
      <h1>Bem-vindo ao Dashboard do Cliente!</h1>

      <section class="requests">
        <div class="requests-header">
          <h2>Pedidos de Energia</h2>
        </div>

        <div v-if="pedidos.length === 0" class="no-requests">
          Você ainda não tem pedidos de energia registrados.
        </div>

        <ul class="requests-list">
          <li v-for="(pedido, index) in pedidos" :key="index" class="request-item">
            <div>
              <strong>{{ pedido.quantidade }} kWh</strong><br />
              <span>Data do pedido: {{ pedido.data }}</span>
            </div>
          </li>
        </ul>
      </section>
    </section>
  </div>
</template>

<script>
export default {
  name: 'DashboardCliente',
  data() {
    return {
      pedidos: []
    };
  },
  mounted() {
    this.carregarPedidos();
  },
  methods: {
    carregarPedidos() {
      const pedidosSalvos = JSON.parse(localStorage.getItem('pedidosCliente') || '[]');
      this.pedidos = pedidosSalvos;
    },
    logout() {
      localStorage.removeItem('usuarioLogado');
      this.$router.push('/');
    },
    goToPerfil() {
      this.$router.push('/PerfilCliente');
    },
    goToHome() {
      this.$router.push('/');
    }
  }
};
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

.requests {
  background: white;
  padding: 2rem;
  border-radius: 16px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.06);
}

.requests-header {
  margin-bottom: 1.5rem;
}

.requests h2 {
  font-size: 1.4rem;
  color: #ff8800;
  text-align: center;
}

.no-requests {
  text-align: center;
  color: #999;
  font-style: italic;
  padding: 1rem;
}

.requests-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.request-item {
  background: #fdf5e6;
  border: 1px solid #ffddaa;
  border-radius: 12px;
  padding: 1rem 1.5rem;
  margin-bottom: 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.request-item strong {
  font-size: 1.2rem;
  color: #333;
}

.request-item span {
  color: #777;
}
</style>
