<template>
  <div class="admin-dashboard">
    <nav class="admin-nav">
      <button @click="voltarPainel">← Voltar ao Painel da Empresa</button>
    </nav>

    <section class="admin-content">
      <h1>Área Administrativa - Gerenciar Pedidos de Energia</h1>

      <section class="admin-requests">
        <div class="admin-requests-header">
          <h2>Pedidos de Energia</h2>
        </div>

        <div v-if="pedidos.length === 0" class="no-requests">
          Nenhum pedido registrado.
        </div>

        <ul class="admin-requests-list">
          <li v-for="(pedido, index) in pedidos" :key="index" class="admin-request-item">
            <div>
              <strong>{{ pedido.quantidade }} kWh</strong><br />
              <span>Data do pedido: {{ pedido.data }}</span>
            </div>
            <button @click="removerPedido(index)">Recusar</button>
          </li>
        </ul>
      </section>
    </section>

    <div v-if="mostrarModal" class="modal">
      <div class="modal-content">
        <h3>Novo Pedido de Energia</h3>
        <label for="quantidade">Quantidade (kWh):</label>
        <input id="quantidade" type="number" v-model.number="novoPedido.quantidade" min="1" />

        <div class="modal-actions">
          <button @click="adicionarPedido" :disabled="novoPedido.quantidade <= 0">Adicionar Pedido</button>
          <button @click="fecharModal">Cancelar</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'AdminPedidos',
  data() {
    return {
      pedidos: [],
      mostrarModal: false,
      novoPedido: {
        quantidade: 0
      }
    }
  },
  mounted() {
    this.verificarAcesso()
    this.carregarPedidos()
  },
  methods: {
    voltarPainel() {
      this.$router.push('/painelempresa');
    },
    verificarAcesso() {
      const usuarioLogado = JSON.parse(localStorage.getItem('usuarioLogado'))
      if (!usuarioLogado || !usuarioLogado.isAdmin) {
        alert('Acesso negado: Área administrativa apenas para administradores.')
        this.$router.push('/')
      }
    },
    carregarPedidos() {
      const pedidosSalvos = JSON.parse(localStorage.getItem('pedidosCliente') || '[]')
      this.pedidos = pedidosSalvos
    },
    salvarPedidos() {
      localStorage.setItem('pedidosCliente', JSON.stringify(this.pedidos))
    },
    abrirModalNovoPedido() {
      this.mostrarModal = true
      this.novoPedido.quantidade = 0
    },
    fecharModal() {
      this.mostrarModal = false
    },
    adicionarPedido() {
      const pedido = {
        quantidade: this.novoPedido.quantidade,
        data: new Date().toLocaleDateString()
      }
      this.pedidos.push(pedido)
      this.salvarPedidos()
      this.fecharModal()
    },
    removerPedido(index) {
      if (confirm('Deseja remover este pedido?')) {
        this.pedidos.splice(index, 1)
        this.salvarPedidos()
      }
    },
    logout() {
      localStorage.removeItem('usuarioLogado')
      this.$router.push('/')
    },
    goToHome() {
      this.$router.push('/')
    }
  }
}
</script>

<style scoped>
.admin-dashboard {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  background-color: #fff8f0;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

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


.admin-content {
  max-width: 900px;
  margin: 2rem auto;
  padding: 0 1rem;
  flex-grow: 1;
}

.admin-content h1 {
  font-size: 2rem;
  color: #cc6600;
  text-align: center;
  margin-bottom: 2rem;
}

.admin-requests {
  background: white;
  padding: 2rem;
  border-radius: 16px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.06);
}

.admin-requests-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.admin-requests-header h2 {
  font-size: 1.4rem;
  color: #ff8800;
}

.admin-requests-header button {
  background-color: #ff8800;
  color: white;
  border: none;
  padding: 0.6rem 1rem;
  border-radius: 8px;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.admin-requests-header button:hover {
  background-color: #e67300;
}

.no-requests {
  text-align: center;
  color: #999;
  font-style: italic;
  padding: 1rem;
}

.admin-requests-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.admin-request-item {
  background: #fff3e6;
  border: 1px solid #ffddaa;
  border-radius: 12px;
  padding: 1rem 1.5rem;
  margin-bottom: 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.admin-request-item strong {
  font-size: 1.2rem;
  color: #cc6600;
}

.admin-request-item span {
  color: #777;
}

.admin-request-item button {
  background-color: #e74c3c;
  color: white;
  border: none;
  padding: 0.4rem 0.8rem;
  border-radius: 6px;
  font-weight: bold;
  transition: background-color 0.3s;
  cursor: pointer;
}

.admin-request-item button:hover {
  background-color: #c0392b;
}

/* Modal */
.modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.4);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 100;
}

.modal-content {
  background: white;
  padding: 2rem;
  border-radius: 12px;
  max-width: 320px;
  width: 90%;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
}
</style>
