<template>
  <nav class="admin-nav">
    <button @click="voltarPainel">← Voltar ao Painel da Empresa</button>
  </nav>
  <div class="admin-pedidos">
    <h1>Gestão de Pedidos de Energia</h1>

    <!-- Pedidos Pendentes -->
    <section class="pedidos-bloco">
      <h2>Pedidos Pendentes</h2>
      <div v-if="pedidosPendentes.length" class="card-container">
        <div v-for="(pedido, index) in pedidosPendentes" :key="index" class="card pedido">
          <div class="card-header">
            <h3>{{ pedido.nomeCliente }}</h3>
            <span class="email">{{ pedido.emailCliente }}</span>
          </div>
          <p><strong>{{ pedido.quantidade }} kWh</strong> solicitados</p>
          <p><span class="label">Data do pedido:</span> {{ pedido.dataPedido }}</p>
          <div class="card-actions">
            <button class="confirmar" @click="firmarContrato(pedido.id)">Confirmar Contrato</button>
            <button class="cancelar" @click="cancelarPedido(pedido.id)">Cancelar Pedido</button>
          </div>
        </div>
      </div>
      <p v-else class="mensagem-vazia">Nenhum pedido pendente.</p>
    </section>

    <!-- Contratos Firmados -->
    <section class="pedidos-bloco">
      <h2>Contratos Firmados</h2>
      <div v-if="contratosFirmados.length" class="card-container">
        <div v-for="(pedido, index) in contratosFirmados" :key="'c'+index" class="card contrato">
          <div class="card-header">
            <h3>{{ pedido.nomeCliente }}</h3>
            <span class="email">{{ pedido.emailCliente }}</span>
          </div>
          <p><strong>{{ pedido.quantidade }} kWh</strong> contratados</p>
          <p><span class="label">Data do pedido:</span> {{ pedido.dataPedido }}</p>
          <p class="status">✅ Contrato firmado</p>
        </div>
      </div>
      <p v-else class="mensagem-vazia">Nenhum contrato firmado.</p>
    </section>
  </div>
</template>

<script>
export default {
  data() {
    return {
      pedidos: []
    };
  },
  computed: {
    pedidosPendentes() {
      return this.pedidos.filter(p => !p.contratoFirmado);
    },
    contratosFirmados() {
      return this.pedidos.filter(p => p.contratoFirmado);
    }
  },
  mounted() {
    this.carregarPedidos();
  },
  methods: {
    voltarPainel() {
      this.$router.push('/painelempresa');
    },
    carregarPedidos() {
      // Simulação: pede dados do localStorage
      const dados = JSON.parse(localStorage.getItem('pedidosEnergia')) || [];
      this.pedidos = dados;
    },
    salvarPedidos() {
      localStorage.setItem('pedidosEnergia', JSON.stringify(this.pedidos));
    },
    firmarContrato(id) {
      const pedido = this.pedidos.find(p => p.id === id);
      if (pedido) {
        pedido.contratoFirmado = true;
        this.salvarPedidos();
        this.carregarPedidos();
      }
    },
    cancelarPedido(id) {
      this.pedidos = this.pedidos.filter(p => p.id !== id);
      this.salvarPedidos();
      this.carregarPedidos();
    }
  }
};
</script>

<style scoped>
.admin-nav {
  background-color: #fff8e1;
  padding: 1rem 2rem;
  border-bottom: 1px solid #ffd95c;
  display: flex;
  justify-content: flex-start;
}

.admin-nav button {
  background-color: #ff9800;
  color: white;
  font-weight: bold;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.admin-nav button:hover {
  background-color: #e68900;
}

.admin-pedidos {
  max-width: 900px;
  margin: 2rem auto;
  background-color: white;
  font-family: 'Segoe UI', sans-serif;
  padding: 2rem;
  color: #333;
  border-radius: 10px;
}

h1 {
  text-align: center;
  font-size: 2rem;
  margin-bottom: 2rem;
  color: #2c3e50;
}

h2 {
  font-size: 1.5rem;
  color: #ff8800;
  margin-bottom: 1rem;
}

.pedidos-bloco {
  margin-bottom: 3rem;
}

.card-container {
  display: grid;
  gap: 1.5rem;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
}

.card {
  background: #fff;
  border-radius: 16px;
  padding: 1.2rem 1.5rem;
  box-shadow: 0 4px 14px rgba(0, 0, 0, 0.08);
  transition: transform 0.2s ease;
}

.card:hover {
  transform: translateY(-4px);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.card-header h3 {
  font-size: 1.2rem;
  margin: 0;
  color: #333;
}

.email {
  font-size: 0.9rem;
  color: #888;
}

.label {
  font-weight: bold;
  color: #555;
}

.card-actions {
  margin-top: 1rem;
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
}

button {
  padding: 0.5rem 0.9rem;
  border-radius: 8px;
  font-weight: bold;
  border: none;
  cursor: pointer;
  font-size: 0.9rem;
  transition: background-color 0.2s ease;
}

.confirmar {
  background-color: #27ae60;
  color: white;
}

.confirmar:hover {
  background-color: #1e8449;
}

.cancelar {
  background-color: #e74c3c;
  color: white;
}

.cancelar:hover {
  background-color: #c0392b;
}

.contrato {
  background-color: #ecfdf5;
  border: 1px solid #b2f2bb;
}

.status {
  margin-top: 0.8rem;
  font-weight: bold;
  color: #27ae60;
}

.mensagem-vazia {
  color: #999;
  font-style: italic;
  margin-top: 1rem;
}
</style>
