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

          <label for="endereco">Endereço para envio da energia:</label>
          <select id="endereco" v-model="enderecoSelecionado" required>
            <option disabled value="">Escolha um endereço</option>
            <option v-if="enderecoAprovado" :value="enderecoAprovado">
              {{ enderecoAprovado }}
            </option>
          </select>

          <button type="submit" :disabled="loading || !enderecoAprovado">
            {{ loading ? 'Enviando...' : 'Fazer Pedido' }}
          </button>
        </form>


        <!-- Lista de pedidos -->
        <div v-if="loading" class="loading">
          Carregando pedidos...
        </div>

        <div v-else-if="pedidos.length === 0" class="no-requests">
          Você ainda não tem pedidos de energia registrados.
        </div>

        <ul v-else class="requests-list">
          <li v-for="pedido in pedidos" :key="pedido.id" class="request-item">
            <div>
              <strong>{{ pedido.quantidadeEnergia || pedido.quantidade }} kWh</strong><br />
              <span>Data do pedido: {{ formatDate(pedido.dataContrato || pedido.dataPedido) }}</span><br />
              <span>Endereço: {{ pedido.enderecoEntrega || pedido.endereco }}</span>
            </div>

            <div>
              <span v-if="pedido.status === 'Concluido'" class="contrato-ativo">
                ✅ Contrato firmado
              </span>
              <span v-else-if="pedido.status === 'EmAnalise'" class="pedido-emAnalise">
                ⏳ Em Análise
              </span>
              <span v-else-if="pedido.status === 'Cancelado'" class="pedido-cancelado">
                ❌ Pedido cancelado
              </span>
            </div>
          </li>

        </ul>
      </section>
    </section>

    <!-- Toast para mensagens -->
    <div v-if="toastMessage" class="toast-message" :class="toastType">
      {{ toastMessage }}
    </div>
  </div>
</template>

<script>
import { clienteService } from '@/services/cliente'

export default {
  name: 'DashboardCliente',
  data() {
    return {
      pedidos: [],
      novaQuantidade: null,
      valor: 0,
      enderecoAprovado: '', // Apenas um endereço
      enderecoSelecionado: '',
      statusAprovacao: null, // Status de aprovação do endereço
      loading: false,
      processando: false,
      toastMessage: '',
      toastType: 'success',
      usuario: null
    };
  },
  async mounted() {
    this.loading = true
    try {
      // Carregar perfil do cliente para obter endereço
      await this.carregarPerfil()

      // Carregar pedidos do cliente
      await this.carregarPedidos()
    } catch (error) {
      console.error('Erro ao inicializar dashboard:', error)
      this.mostrarToast('Erro ao carregar dados. Tente novamente mais tarde.', 'error')
    } finally {
      this.loading = false
    }
  },
  methods: {
    async carregarPerfil() {
      try {
        console.log('🔍 Carregando perfil do cliente...')

        // Tentar carregar do backend primeiro
        try {
          const perfil = await clienteService.getPerfil()
          console.log('✅ Perfil carregado do backend:', perfil)

          this.usuario = perfil
          this.statusAprovacao = perfil.statusAprovacao

          // Verificar se o endereço está aprovado
          if (perfil.statusAprovacao === 2 && perfil.enderecoCompleto) {
            this.enderecoAprovado = perfil.enderecoCompleto
            this.enderecoSelecionado = perfil.enderecoCompleto
            console.log('📍 Endereço aprovado encontrado:', this.enderecoAprovado)
          } else {
            this.enderecoAprovado = ''
            this.enderecoSelecionado = ''
            console.log('❌ Endereço não aprovado ou não existe')
          }

          return
        } catch (apiError) {
          console.warn('⚠️ Erro ao carregar do backend, tentando localStorage:', apiError)
        }

        // Fallback para localStorage
        const usuarioLogado = JSON.parse(localStorage.getItem('usuarioLogado'))
        if (!usuarioLogado) {
          throw new Error('Usuário não encontrado')
        }

        console.log('📱 Carregando do localStorage:', usuarioLogado)

        this.usuario = usuarioLogado
        this.statusAprovacao = usuarioLogado.statusAprovacao || usuarioLogado.statusAprovacaoComprovante

        // Verificar endereço no localStorage
        if (usuarioLogado.statusAprovacao === 2 || usuarioLogado.statusAprovacaoComprovante === 'Aprovado') {
          const endereco = usuarioLogado.enderecoCompleto || usuarioLogado.endereco
          if (endereco) {
            this.enderecoAprovado = endereco
            this.enderecoSelecionado = endereco
            console.log('📍 Endereço aprovado do localStorage:', this.enderecoAprovado)
          }
        } else {
          this.enderecoAprovado = ''
          this.enderecoSelecionado = ''
          console.log('❌ Endereço não aprovado no localStorage')
        }

      } catch (error) {
        console.error('❌ Erro ao carregar perfil:', error)
        throw error
      }
    },

    async fazerPedido() {
      const limiteMaximo = 50;
      const totalAtual = this.pedidos.reduce((soma, p) => soma + p.quantidadeEnergia, 0);


      if (!this.enderecoSelecionado) {
        this.mostrarToast('Por favor, selecione um endereço.', 'error')
        return
      }

      if (!this.enderecoAprovado) {
        this.mostrarToast('Seu endereço precisa ser aprovado antes de fazer pedidos.', 'error')
        return
      }

      if (this.novaQuantidade + totalAtual > limiteMaximo) {
        const restante = limiteMaximo - totalAtual;
        alert(`Você só pode pedir mais ${restante} kWh.`);
        return
      }
      

      this.loading = true

      
      try {
        // Criar objeto de pedido para enviar ao backend
        const novoPedido = {
          quantidadeEnergia: this.novaQuantidade,
          valorContrato: this.valor,
          enderecoEntrega: this.enderecoSelecionado
        }

        console.log('📤 Criando pedido:', novoPedido)

        // Tentar chamar API para criar pedido
        try {
          await clienteService.criarPedido(novoPedido)
          console.log('✅ Pedido criado via API')
        } catch (apiError) {
          console.warn('⚠️ Erro na API, salvando no localStorage:', apiError)

          // Fallback para localStorage
          const pedidosExistentes = JSON.parse(localStorage.getItem('pedidosCliente')) || []
          const novoPedidoLocal = {
            id: Date.now(),
            quantidade: this.novaQuantidade,
            valor: 0,
            endereco: this.enderecoSelecionado,
            dataPedido: new Date().toISOString(),
            nomeCliente: this.usuario?.nome || 'Cliente',
            emailCliente: this.usuario?.email || '',
            status: 'EmAnalise', // Em análise
          }

          pedidosExistentes.push(novoPedidoLocal)
          localStorage.setItem('pedidosCliente', JSON.stringify(pedidosExistentes))
        }

        // Recarregar pedidos após criar um novo
        await this.carregarPedidos()

        // Limpar formulário
        this.novaQuantidade = null
        this.enderecoSelecionado = this.enderecoAprovado // Manter o endereço selecionado

        this.mostrarToast('Pedido criado com sucesso!', 'success')
      } catch (error) {
        console.error('Erro ao criar pedido:', error)
        this.mostrarToast('Erro ao criar pedido. Tente novamente.', 'error')
      } finally {
        this.loading = false
      }

    },

    async carregarPedidos() {
      this.loading = true
      try {
        // Tentar usar o endpoint /api/Pedidos/me para obter apenas os pedidos do cliente logado
        try {
          const pedidos = await clienteService.getMeusPedidos()
          this.pedidos = pedidos
          console.log('✅ Pedidos carregados via API:', pedidos)
        } catch (apiError) {
          console.warn('⚠️ Erro na API, carregando do localStorage:', apiError)

          // Fallback para localStorage
          const todosPedidos = JSON.parse(localStorage.getItem('pedidosCliente')) || []
          const emailUsuario = this.usuario?.email

          if (emailUsuario) {
            this.pedidos = todosPedidos.filter(p => p.emailCliente === emailUsuario)
          } else {
            this.pedidos = []
          }

          console.log('📱 Pedidos carregados do localStorage:', this.pedidos)
        }
      } catch (error) {
        console.error('Erro ao carregar pedidos:', error)
        this.mostrarToast('Erro ao carregar pedidos.', 'error')
        this.pedidos = []
      } finally {
        this.loading = false
      }
    },


    async logout() {
      try {
        // Chamar o serviço de autenticação para fazer logout
        await import('@/services/auth').then(({ authService }) => {
          return authService.logout()
        })
        this.$router.push('/')
      } catch (error) {
        console.error('Erro ao fazer logout:', error)
        this.$router.push('/')
      }
    },

    goToPerfil() {
      this.$router.push('/perfilcliente')
    },

    goToHome() {
      this.$router.push('/')
    },

    formatDate(dateString) {
      if (!dateString) return 'N/A'

      try {
        const date = new Date(dateString)
        return date.toLocaleDateString('pt-BR')
      } catch (error) {
        return dateString
      }
    },

    mostrarToast(mensagem, tipo = 'success') {
      this.toastMessage = mensagem
      this.toastType = tipo

      setTimeout(() => {
        this.toastMessage = ''
      }, 3000)
    },

    async recarregarPerfil() {
      console.log('🔄 Recarregando perfil...')
      await this.carregarPerfil()

      if (this.enderecoAprovado) {
        this.mostrarToast('Endereço aprovado e disponível para pedidos!', 'success')
      } else if (this.statusAprovacao === 2) {
        this.mostrarToast('Endereço ainda aguardando aprovação do administrador.', 'warning')
      } else if (this.statusAprovacao === 1) {
        this.mostrarToast('Endereço foi rejeitado. Atualize seu perfil com um novo comprovante.', 'error')
      } else {
        this.mostrarToast('Nenhum endereço encontrado. Atualize seu perfil.', 'warning')
      }
    }
  }
};
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

.pedido-form button[type="submit"]:hover:not(:disabled) {
  background-color: #e67300;
  box-shadow: 0 6px 16px rgba(230, 115, 0, 0.7);
}

.pedido-form button[type="submit"]:disabled {
  background-color: #ffbb66;
  cursor: not-allowed;
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

.loading {
  text-align: center;
  padding: 2rem;
  color: #6b7280;
  font-style: italic;
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

.cancelar-pedido-btn:hover:not(:disabled) {
  background-color: #c0392b;
}

.cancelar-pedido-btn:disabled {
  background-color: #e74c3c;
  opacity: 0.6;
  cursor: not-allowed;
}

.contrato-ativo {
  font-weight: bold;
  color: #27ae60;
}

.pedido-cancelado {
  font-weight: bold;
  color: #dc2626;
}

.toast-message {
  position: fixed;
  bottom: 20px;
  right: 20px;
  padding: 12px 20px;
  border-radius: 8px;
  color: white;
  font-weight: bold;
  z-index: 1000;
  animation: fadeIn 0.3s, fadeOut 0.3s 2.7s;
}

.toast-message.success {
  background-color: #10b981;
}

.toast-message.error {
  background-color: #ef4444;
}

.toast-message.warning {
  background-color: #f59e0b;
}

.endereco-info {
  margin: 1rem 0;
  padding: 1rem;
  background: #f8f9fa;
  border-radius: 8px;
  text-align: center;
}

.sem-enderecos {
  color: #dc3545;
  font-weight: bold;
  margin: 0;
}

.enderecos-disponiveis {
  color: #28a745;
  font-weight: bold;
  margin: 0;
}

.status-endereco {
  margin-top: 0.5rem;
}

.status {
  display: inline-block;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-weight: bold;
  font-size: 0.9rem;
}

.status.pendente {
  background-color: #fff3cd;
  color: #856404;
  border: 1px solid #ffeaa7;
}

.status.aprovado {
  background-color: #d4edda;
  color: #155724;
  border: 1px solid #c3e6cb;
}

.status.rejeitado {
  background-color: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
}

.btn-recarregar {
  background-color: #17a2b8;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 6px;
  font-weight: bold;
  cursor: pointer;
  margin-left: 0.5rem;
  transition: background-color 0.3s ease;
}

.btn-recarregar:hover {
  background-color: #138496;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes fadeOut {
  from {
    opacity: 1;
    transform: translateY(0);
  }

  to {
    opacity: 0;
    transform: translateY(20px);
  }
}
</style>
