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

        <!-- Formulário de novo pedido -->
        <form @submit.prevent="fazerPedido" class="pedido-form">
          <label for="quantidade">Quantidade de energia (kWh):</label>
          <input type="number" id="quantidade" v-model.number="novaQuantidade" min="1" max="50" required />

          <label for="endereco">Selecione o endereço para envio da energia:</label>
          <select id="endereco" v-model="enderecoSelecionado" required>
            <option disabled value="">Escolha um endereço</option>
            <option v-for="(end, idx) in enderecosUsuario" :key="idx" :value="end">
              {{ end }}
            </option>
          </select>



          <button type="submit">Fazer Pedido</button>
        </form>

        <!-- Lista de pedidos -->
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
      pedidos: [],
      novaQuantidade: null,
      enderecoSelecionado: '',
      enderecosUsuario: [] // Assumindo que você tem isso vindo de algum lugar
    };
  },
  mounted() {
    this.carregarPedidos();
    this.carregarEnderecos();
  },
  methods: {
    fazerPedido() {
      const limiteMaximo = 50;
      const totalAtual = this.pedidos.reduce((soma, p) => soma + p.quantidade, 0);

      if (!this.enderecoSelecionado) {
        alert('Por favor, selecione um endereço.');
        return;
      }

      if (this.novaQuantidade + totalAtual > limiteMaximo) {
        const restante = limiteMaximo - totalAtual;
        alert(`Você só pode pedir mais ${restante} kWh.`);
        return;
      }

      const novoPedido = {
        quantidade: this.novaQuantidade,
        data: new Date().toLocaleDateString('pt-BR'),
        endereco: this.enderecoSelecionado
      };

      this.pedidos.push(novoPedido);
      localStorage.setItem('pedidosCliente', JSON.stringify(this.pedidos));

      this.novaQuantidade = null;
      this.enderecoSelecionado = '';
    },
    carregarPedidos() {
      const pedidosSalvos = JSON.parse(localStorage.getItem('pedidosCliente') || '[]');
      this.pedidos = pedidosSalvos;
    },
    carregarEnderecos() {
      const usuario = JSON.parse(localStorage.getItem('usuarioLogado'));
      this.enderecosUsuario = usuario?.enderecos || [usuario?.endereco].filter(Boolean);
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

.pedido-form {
  display: flex;
  flex-direction: column;
  margin-bottom: 2rem;
  gap: 0.5rem;
}

.pedido-form label {
  font-weight: bold;
  color: #555;
}

.pedido-form input {
  padding: 0.6rem;
  border: 1px solid #ccc;
  border-radius: 8px;
  font-size: 1rem;
}

.pedido-form button {
  align-self: flex-start;
  background-color: #ff8800;
  color: white;
  padding: 0.6rem 1rem;
  border: none;
  border-radius: 8px;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.pedido-form button:hover {
  background-color: #e67300;
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