<template>
  <div class="container">
    <div class="top-bar">
      <button class="voltar-btn" @click="voltarParaPainel">← Voltar ao Painel da Empresa</button>
    </div>

    <h1 class="titulo">Painel de Usuários</h1>

    <div v-if="loading" class="loading">
      Carregando dados dos usuários...
    </div>

    <div v-else class="cards-container">
      <!-- Clientes -->
      <div class="card">
        <h2 class="card-title">Clientes Cadastrados</h2>
        <table v-if="clientes.length" class="tabela">
          <thead>
            <tr>
              <th>Nome</th>
              <th>Email</th>
              <th>CPF/CNPJ</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="cliente in clientes" :key="cliente.id">
              <td>{{ cliente.nome }}</td>
              <td>{{ cliente.email }}</td>
              <td>{{ cliente.cpfCnpj || 'N/A' }}</td>
              <td>{{ cliente.status || 'Ativo' }}</td>
            </tr>
          </tbody>
        </table>
        <p v-else class="mensagem-vazia">Nenhum cliente cadastrado.</p>
      </div>

      <!-- Fornecedores -->
      <div class="card">
        <h2 class="card-title">Fornecedores Cadastrados</h2>
        <table v-if="fornecedores.length" class="tabela">
          <thead>
            <tr>
              <th>Nome</th>
              <th>Email</th>
              <th>CPF/CNPJ</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="fornecedor in fornecedores" :key="fornecedor.id">
              <td>{{ fornecedor.nome }}</td>
              <td>{{ fornecedor.email }}</td>
              <td>{{ fornecedor.cpfCnpj || 'N/A' }}</td>
              <td>{{ fornecedor.status || 'Ativo' }}</td>
            </tr>
          </tbody>
        </table>
        <p v-else class="mensagem-vazia">Nenhum fornecedor cadastrado.</p>
      </div>

      <!-- Endereços Pendentes -->
      <div class="card">
        <h2 class="card-title">Endereços Pendentes de Aprovação</h2>
        <table v-if="clientesComEnderecosPendentes.length" class="tabela">
          <thead>
            <tr>
              <th>Cliente</th>
              <th>Email</th>
              <th>Endereço</th>
              <th>Data de Envio</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="cliente in clientesComEnderecosPendentes" :key="cliente.id">
              <td>{{ cliente.nome }}</td>
              <td>{{ cliente.email }}</td>
              <td>{{ cliente.enderecoCompleto }}</td>
              <td>{{ formatarData(cliente.dataEnvioComprovante) }}</td>
              <td>
                <button class="aprovar-btn" 
                        @click="aprovarEndereco(cliente.id)" 
                        :disabled="processando">
                  {{ processando ? 'Processando...' : 'Aprovar' }}
                </button>
                <button class="rejeitar-btn" 
                        @click="rejeitarEndereco(cliente.id)" 
                        :disabled="processando">
                  {{ processando ? 'Processando...' : 'Rejeitar' }}
                </button>
              </td>
            </tr>
          </tbody>
        </table>
        <p v-else class="mensagem-vazia">Nenhum endereço pendente.</p>
      </div>
    </div>

    <div v-if="erro" class="erro-message">{{ erro }}</div>
    <div v-if="sucesso" class="sucesso-message">{{ sucesso }}</div>
  </div>
</template>

<script>
import { adminService } from '@/services/admin'

export default {
  data() {
    return {
      clientes: [],
      fornecedores: [],
      loading: false,
      processando: false,
      erro: '',
      sucesso: ''
    }
  },
  computed: {
    clientesComEnderecosPendentes() {
      return this.clientes.filter(cliente => 
        cliente.statusAprovacaoComprovante === 'Pendente'
      )
    }
  },
  methods: {
    voltarParaPainel() {
      this.$router.push('/painelempresa')
    },

    formatarData(dataString) {
      if (!dataString) return 'N/A'
      
      try {
        const data = new Date(dataString)
        return data.toLocaleDateString('pt-BR') + ' ' + data.toLocaleTimeString('pt-BR', {
          hour: '2-digit',
          minute: '2-digit'
        })
      } catch (error) {
        return dataString
      }
    },

    async aprovarEndereco(clienteId) {
      if (!confirm('Tem certeza que deseja aprovar o endereço deste cliente?')) return
      
      this.processando = true
      this.erro = ''
      this.sucesso = ''
      
      try {
        console.log('🔄 Aprovando endereço do cliente ID:', clienteId)
        
        // Chamar API para aprovar endereço
        await adminService.aprovarEndereco(clienteId)
        
        this.sucesso = 'Endereço aprovado com sucesso! O cliente já pode fazer pedidos.'
        
        // Recarregar dados para atualizar a lista
        await this.carregarDados()
        
        // Mostrar mensagem adicional
        setTimeout(() => {
          this.sucesso = ''
        }, 5000)
        
      } catch (error) {
        console.error('❌ Erro ao aprovar endereço:', error)
        this.erro = 'Erro ao aprovar endereço. Tente novamente.'
      } finally {
        this.processando = false
      }
    },

    async rejeitarEndereco(clienteId) {
      const motivo = prompt('Por favor, informe o motivo da rejeição:')
      if (motivo === null) return // Usuário cancelou
      
      if (!motivo.trim()) {
        alert('É necessário informar um motivo para rejeitar o endereço.')
        return
      }
      
      this.processando = true
      this.erro = ''
      this.sucesso = ''
      
      try {
        // Chamar API para rejeitar endereço
        await adminService.rejeitarEndereco(clienteId, motivo)
        
        this.sucesso = 'Endereço rejeitado com sucesso!'
        
        // Recarregar dados para atualizar a lista
        await this.carregarDados()
      } catch (error) {
        console.error('Erro ao rejeitar endereço:', error)
        this.erro = 'Erro ao rejeitar endereço. Tente novamente.'
      } finally {
        this.processando = false
      }
    },

    async carregarDados() {
      this.loading = true
      this.erro = ''
      
      try {
        // Carregar clientes do backend
        this.clientes = await adminService.getClientes()
        
        // Carregar fornecedores do backend
        this.fornecedores = await adminService.getFornecedores()
      } catch (error) {
        console.error('Erro ao carregar dados:', error)
        this.erro = 'Erro ao carregar dados dos usuários. Tente novamente.'
        this.clientes = []
        this.fornecedores = []
      } finally {
        this.loading = false
      }
    }
  },
  mounted() {
    this.carregarDados()
  }
}
</script>

<style scoped>
body {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.top-bar {
  display: flex;
  justify-content: flex-start;
  margin-bottom: 20px;
}

.voltar-btn {
  background-color: white;
  color: #ff8800;
  padding: 10px 18px;
  font-size: 14px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.voltar-btn:hover {
  background-color: #ff8800;
  color: white;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 40px 20px;
  background-color: #f4f6f8;
  min-height: 100vh;
}

.titulo {
  text-align: center;
  font-size: 36px;
  margin-bottom: 50px;
  color: #2c3e50;
  font-weight: 600;
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

.sucesso-message {
  color: #059669;
  font-size: 0.875rem;
  margin-top: 1rem;
  padding: 0.75rem;
  background-color: #ecfdf5;
  border-radius: 8px;
  border: 1px solid #a7f3d0;
  text-align: center;
}

.cards-container {
  display: flex;
  flex-wrap: wrap;
  gap: 40px;
  justify-content: center;
}

.card {
  flex: 1 1 450px;
  background-color: #fff;
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.06);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.12);
}

.card-title {
  margin-bottom: 20px;
  font-size: 22px;
  color: #34495e;
  border-bottom: 2px solid #e0e0e0;
  padding-bottom: 8px;
}

/* Tabela */
.tabela {
  width: 100%;
  border-collapse: collapse;
  font-size: 15px;
}

.tabela th,
.tabela td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #e6e6e6;
}

.tabela th {
  background-color: #f1f1f1;
  font-weight: 600;
  color: #555;
}

.tabela tr:hover {
  background-color: #f9f9f9;
}

.mensagem-vazia {
  color: #999;
  font-style: italic;
  margin-top: 10px;
}

.aprovar-btn {
  background-color: #4CAF50;
  color: white;
  padding: 8px 16px;
  border: none;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.3s ease;
  margin-right: 8px;
}

.aprovar-btn:hover:not(:disabled) {
  background-color: #45a049;
}

.aprovar-btn:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}

.rejeitar-btn {
  background-color: #f44336;
  color: white;
  padding: 8px 16px;
  border: none;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.rejeitar-btn:hover:not(:disabled) {
  background-color: #da190b;
}

.rejeitar-btn:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}
</style>
