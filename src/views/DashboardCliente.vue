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
          <li v-for="(pedido, index) in pedidos" :key="pedido.id" class="request-item">
            <div>
              <strong>{{ pedido.quantidade }} kWh</strong><br />
              <span>Data do pedido: {{ pedido.dataPedido }}</span><br />
              <span>Endereço: {{ pedido.endereco }}</span>
            </div>

            <div>
              <button
                v-if="!pedido.contratoFirmado"
                @click="cancelarPedido(index)"
                class="cancelar-pedido-btn"
                type="button"
              >
                Cancelar Pedido
              </button>
              <span v-else class="contrato-ativo">
                ✅ Contrato firmado
              </span>
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
      enderecosUsuario: [], 
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

      const usuario = JSON.parse(localStorage.getItem('usuarioLogado')) || {};

      const novoPedido = {
        id: Date.now(),
        nomeCliente: usuario.nome || 'Cliente',
        emailCliente: usuario.email || '',
        quantidade: this.novaQuantidade,
        dataPedido: new Date().toLocaleDateString('pt-BR'),
        endereco: this.enderecoSelecionado,
        contratoFirmado: false,
      };

      this.pedidos.push(novoPedido);
      localStorage.setItem('pedidosEnergia', JSON.stringify(this.pedidos));

      this.novaQuantidade = null;
      this.enderecoSelecionado = '';
    },
    carregarPedidos() {
      const pedidosSalvos = JSON.parse(localStorage.getItem('pedidosEnergia') || '[]');
      this.pedidos = pedidosSalvos;
    },
    carregarEnderecos() {
      const usuario = JSON.parse(localStorage.getItem('usuarioLogado'));
      this.enderecosUsuario = usuario?.enderecos || [usuario?.endereco].filter(Boolean);
    },
    cancelarPedido(index) {
      if (confirm('Tem certeza que deseja cancelar este pedido?')) {
        this.pedidos.splice(index, 1);
        localStorage.setItem('pedidosEnergia', JSON.stringify(this.pedidos));
      }
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
    },
  },
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

.pedido-form {
  max-width: 400px;
  margin: 1.5rem auto 2rem;
  padding: 1.5rem;
  background: #fff7e6;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(255, 136, 0, 0.25);
  display: flex;
  flex-direction: column;
  gap: 1rem;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.pedido-form label {
  font-weight: 600;
  color: #d35400;
  font-size: 1rem;
}

.pedido-form input[type="number"],
.pedido-form select {
  padding: 0.55rem 1rem;
  font-size: 1rem;
  border: 2px solid #ffbb66;
  border-radius: 8px;
  transition: border-color 0.3s ease, box-shadow 0.3s ease;
  outline-offset: 2px;
}

.pedido-form input[type="number"]:focus,
.pedido-form select:focus {
  border-color: #ff8800;
  box-shadow: 0 0 8px 2px rgba(255, 136, 0, 0.5);
  outline: none;
}

.pedido-form button[type="submit"] {
  background-color: #ff8800;
  color: white;
  font-weight: 700;
  font-size: 1.1rem;
  padding: 0.6rem 1.2rem;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(255, 136, 0, 0.5);
  transition: background-color 0.3s ease, box-shadow 0.3s ease;
}

.pedido-form button[type="submit"]:hover {
  background-color: #e67300;
  box-shadow: 0 6px 16px rgba(230, 115, 0, 0.7);
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

.cancelar-pedido-btn {
  background-color: #e74c3c;
  border: none;
  color: white;
  font-weight: bold;
  padding: 0.4rem 0.8rem;
  border-radius: 8px;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.cancelar-pedido-btn:hover {
  background-color: #c0392b;
}

.contrato-ativo {
  font-weight: bold;
  color: #27ae60;
}
</style>
