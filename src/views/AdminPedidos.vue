<template>
  <nav class="admin-nav">
    <button @click="voltarPainel">← Voltar ao Painel da Empresa</button>
  </nav>
  <div class="admin-pedidos">
    <h1>Gestão de Pedidos de Energia</h1>

    <div v-if="loading" class="loading">
      Carregando pedidos...
    </div>

    <div v-else>
      <!-- Pedidos Pendentes -->
      <section class="pedidos-bloco">
        <h2>Pedidos Pendentes</h2>
        <div v-if="pedidosPendentes.length" class="card-container">
          <div v-for="(pedido, index) in pedidosPendentes" :key="index" class="card pedido">
            <div class="card-header">
              <h3>{{ pedido.nomeCliente || pedido.cliente?.nome || 'Cliente' }}</h3>
              <span class="email">{{ pedido.emailCliente || pedido.cliente?.email || 'N/A' }}</span>
            </div>
            <p><strong>{{ pedido.quantidadeEnergia }} kWh</strong> solicitados</p>
            <p><span class="label">Data do pedido:</span> {{ formatDate(pedido.dataContrato || pedido.createdAt) }}</p>
            <p><span class="label">Endereço:</span> {{ pedido.enderecoEntrega || 'N/A' }}</p>
            <div class="card-actions">
              <button class="confirmar" @click="firmarContrato(pedido.id)" :disabled="processando">
                {{ processando ? 'Processando...' : 'Confirmar Contrato' }}
              </button>
              <button class="cancelar" @click="cancelarPedido(pedido.id)" :disabled="processando">
                {{ processando ? 'Processando...' : 'Cancelar Pedido' }}
              </button>
            </div>
          </div>
        </div>
        <p v-else class="mensagem-vazia">Nenhum pedido pendente.</p>
      </section>

      <!-- Contratos Firmados -->
      <section class="pedidos-bloco">
        <h2>Contratos Firmados</h2>
        <div v-if="contratosFirmados.length" class="card-container">
          <div v-for="(pedido, index) in contratosFirmados" :key="'c' + index" class="card contrato">
            <div class="card-header">
              <h3>{{ pedido.nomeCliente || pedido.cliente?.nome || 'Cliente' }}</h3>
              <span class="email">{{ pedido.emailCliente || pedido.cliente?.email || 'N/A' }}</span>
            </div>
            <p><strong>{{ pedido.quantidadeEnergia }} kWh</strong> contratados</p>
            <p><span class="label">Data do pedido:</span> {{ formatDate(pedido.dataContrato || pedido.createdAt) }}</p>
            <p><span class="label">Endereço:</span> {{ pedido.enderecoEntrega || 'N/A' }}</p>
            <p class="status">✅ Contrato firmado</p>
          </div>
        </div>
        <p v-else class="mensagem-vazia">Nenhum contrato firmado.</p>
      </section>
    </div>

    <div v-if="erro" class="erro-message">{{ erro }}</div>
  </div>
</template>

<script>
import { adminService } from '@/services/admin'


const StatusPedido = {
  EM_ANALISE: 0,
  EM_VIGOR: 1,
  CANCELADO: 2,
  CONFIRMADO: 3
}

export default {
  data() {
    return {
      pedidos: [],
      loading: false,
      processando: false,
      erro: ''
    };
  },
  computed: {
    pedidosPendentes() {
      return this.pedidos.filter(p =>
        p.status === StatusPedido.EM_ANALISE ||
        p.status === StatusPedido.EM_VIGOR
      );
    },
    contratosFirmados() {
      return this.pedidos.filter(p =>
        p.status === StatusPedido.CONFIRMADO
      );
    }
  },
  async mounted() {
    await this.carregarPedidos();
  },
  methods: {
    voltarPainel() {
      this.$router.push('/painelempresa');
    },

    async carregarPedidos() {
      this.loading = true;
      this.erro = '';

      try {
        this.pedidos = await adminService.getPedidos();
        console.log('Pedidos carregados:', this.pedidos);
      } catch (error) {
        console.error('Erro ao carregar pedidos:', error);
        this.erro = 'Erro ao carregar pedidos. Tente novamente.';
        this.pedidos = [];
      } finally {
        this.loading = false;
      }
    },

    async firmarContrato(id) {
      console.log('Firmando contrato para pedido ID:', id);
      console.log('Status que será enviado:', StatusPedido.CONFIRMADO);
      if (!confirm('Tem certeza que deseja firmar este contrato?')) return;

      this.processando = true;
      this.erro = '';

      try {
        await adminService.atualizarStatusPedido(id, StatusPedido.CONFIRMADO);

        // Atualizar localmente
        const pedido = this.pedidos.find(p => p.id === id);
        if (pedido) {
          pedido.status = StatusPedido.CONFIRMADO;
        }
      
        const response = await adminService.atualizarStatusPedido(id, StatusPedido.CONFIRMADO);
        
        console.log('Resposta da API:', response);

        const pedidoIndex = this.pedidos.findIndex(p => p.id === id);
        if (pedidoIndex !== -1) {
          // Atualizar o status localmente
          this.pedidos[pedidoIndex].status = StatusPedido.CONFIRMADO;
          
          // Forçar reatividade do Vue
          this.$forceUpdate();
        }
        
        // Mostrar mensagem de sucesso
        alert('Contrato firmado com sucesso! As alterações serão processadas no próximo ciclo mensal.');

        // Recarregar para garantir dados atualizados
        await this.carregarPedidos();
      } catch (error) {
        console.error('Erro ao firmar contrato:', error);
        this.erro = 'Erro ao firmar contrato. Tente novamente.';
      } finally {
        this.processando = false;
      }
    },

    async cancelarPedido(id) {
      if (!confirm('Tem certeza que deseja cancelar este pedido?')) return;

      this.processando = true;
      this.erro = '';

      try {
        await adminService.atualizarStatusPedido(id, StatusPedido.CANCELADO);

        // Atualizar localmente
        const pedido = this.pedidos.find(p => p.id === id);
        if (pedido) {
          pedido.status = StatusPedido.CANCELADO;
        }

        // Recarregar para garantir dados atualizados
        await this.carregarPedidos();
      } catch (error) {
        console.error('Erro ao cancelar pedido:', error);
        this.erro = 'Erro ao cancelar pedido. Tente novamente.';
      } finally {
        this.processando = false;
      }
    },

    formatDate(dateString) {
      if (!dateString) return 'N/A';

      try {
        const date = new Date(dateString);
        return date.toLocaleDateString('pt-BR');
      } catch (error) {
        return dateString;
      }
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

button:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}

.confirmar {
  background-color: #27ae60;
  color: white;
}

.confirmar:hover:not(:disabled) {
  background-color: #1e8449;
}

.cancelar {
  background-color: #e74c3c;
  color: white;
}

.cancelar:hover:not(:disabled) {
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
