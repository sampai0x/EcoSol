<template>
  <div class="barra-voltar">
    <div class="logo-titulo">
      <img src="/src/img/EcoSolNavBar.png" alt="Logo" class="logo" />
    </div>
    <button @click="voltarDashboard" class="btn-voltar">
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"
        stroke-linejoin="round" class="feather feather-arrow-left" viewBox="0 0 24 24">
        <line x1="19" y1="12" x2="5" y2="12" />
        <polyline points="12 19 5 12 12 5" />
      </svg>
      Voltar ao Dashboard
    </button>
  </div>

  <div class="perfil-fornecedor">
    <h1>Meu Perfil</h1>

    <div v-if="loading" class="loading">
      Carregando dados do perfil...
    </div>

    <div v-else class="perfil-card">
      <div v-if="!editando">
        <p><strong>Nome:</strong> {{ perfil.nome }}</p>
        <p><strong>E-mail:</strong> {{ perfil.email }}</p>
        <p><strong>Tipo de Conta:</strong> Cliente</p>
        <p><strong>CPF/CNPJ:</strong> {{ perfil.cpfCnpj }}</p>
        <p><strong>Endereço:</strong> {{ perfil.enderecoCompleto || 'Não informado' }}</p>

        <div class="status-aprovacao">
          <p><strong>Status do Comprovante:</strong></p>
          <span v-if="perfil.statusAprovacao === 1" class="status pendente">
            ⏳ Aguardando aprovação
          </span>
          <span v-else-if="perfil.statusAprovacao === 2" class="status aprovado">
            ✅ Aprovado
          </span>
          <span v-else-if="perfil.statusAprovacao === 3" class="status rejeitado">
            ❌ Rejeitado
          </span>
          <span v-else class="status sem-comprovante">
            📄 Comprovante não enviado
          </span>

          <p v-if="perfil.dataEnvioComprovante" class="data-info">
            <small>Enviado em: {{ formatarData(perfil.dataEnvioComprovante) }}</small>
          </p>

          <p v-if="perfil.statusAprovacao === 2" class="data-info">
            <small>Aprovado em: {{ formatarData(perfil.dataAprovacaoComprovante) }}</small>
          </p>

          <p v-if="perfil.observacaoRejeicao" class="observacao-rejeicao">
            <strong>Motivo da rejeição:</strong> {{ perfil.observacaoRejeicao }}
          </p>
        </div>
      </div>

      <div v-else>
        <div class="form-group">
          <label>Nome:</label>
          <input type="text" v-model="perfil.nome" />
        </div>

        <div class="form-group">
          <label>Email:</label>
          <input type="email" v-model="perfil.email" />
        </div>

        <div class="form-group">
          <label>Endereço Completo:</label>
          <input type="text" v-model="perfil.enderecoCompleto" @input="enderecoAlterado = true"
            placeholder="Digite seu endereço completo" class="input-endereco" />

          <div v-if="enderecoAlterado" class="aviso-comprovante">
            ⚠️ Você alterou o endereço. É necessário enviar um novo comprovante.
          </div>

          <input v-if="enderecoAlterado" type="file" @change="handleFileUpload" accept=".pdf,image/*"
            class="input-comprovante" required />

          <p v-if="comprovante">
            Arquivo selecionado: {{ comprovante.name }}
          </p>
        </div>

        <div class="form-group readonly">
          <label>CPF/CNPJ:</label>
          <span>{{ perfil.cpfCnpj }}</span>
        </div>

        <div class="form-group readonly">
          <label>Tipo de Conta:</label>
          <span>Cliente</span>
        </div>
      </div>
    </div>

    <div class="botoes" v-if="!loading">
      <button v-if="!editando" @click="editarPerfil">Editar Perfil</button>
      <button v-else @click="salvarEdicao" :disabled="salvando || (enderecoAlterado && !comprovante)">
        {{ salvando ? 'Salvando...' : 'Salvar' }}
      </button>
      <button v-if="editando" @click="cancelarEdicao">Cancelar</button>
    </div>

    <div v-if="erro" class="erro-message">{{ erro }}</div>
    <div v-if="sucesso" class="sucesso-message">{{ sucesso }}</div>
  </div>
</template>

<script>
import { clienteService } from '@/services/cliente'

export default {
  data() {
    return {
      perfil: {
        id: null,
        nome: '',
        email: '',
        senha: '',
        cpfCnpj: '',
        enderecoCompleto: '',
        comprovanteUrl: null,
        statusAprovacao: null,
        dataEnvioComprovante: null,
        dataAprovacaoComprovante: null,
        observacaoRejeicao: null
      },
      originalPerfil: {},
      editando: false,
      enderecoAlterado: false,
      comprovante: null,
      loading: false,
      salvando: false,
      erro: '',
      sucesso: ''
    }
  },
  methods: {
    handleFileUpload(event) {
      const file = event.target.files[0]
      if (file) {
        // Validar tamanho do arquivo (5MB)
        const maxSize = 5 * 1024 * 1024
        if (file.size > maxSize) {
          this.erro = 'Arquivo excede o tamanho máximo permitido (5MB)'
          return
        }

        // Validar extensão
        const extensoesPermitidas = ['.pdf', '.jpg', '.jpeg', '.png']
        const extensao = '.' + file.name.split('.').pop().toLowerCase()
        if (!extensoesPermitidas.includes(extensao)) {
          this.erro = 'Extensão de arquivo não permitida. Use PDF, JPG, JPEG ou PNG'
          return
        }

        this.comprovante = file
        this.erro = ''
      }
    },

    voltarDashboard() {
      this.$router.push('/dashboardcliente')
    },

    async carregarPerfil() {
      this.loading = true
      this.erro = ''

      try {
        console.log('🔍 Carregando perfil do cliente...')

        // Tentar carregar do backend primeiro
        try {
          const perfil = await clienteService.getPerfil()
          console.log('✅ Perfil carregado do backend:', perfil)

          this.perfil = {
            id: perfil.id,
            nome: perfil.nome || '',
            email: perfil.email || '',
            senha: perfil.senha || '',
            cpfCnpj: perfil.cpfCnpj || '',
            enderecoCompleto: perfil.enderecoCompleto || '',
            comprovanteUrl: perfil.comprovanteUrl,
            statusAprovacao: perfil.statusAprovacao,
            dataEnvioComprovante: perfil.dataEnvioComprovante,
            dataAprovacaoComprovante: perfil.dataAprovacaoComprovante,
            observacaoRejeicao: perfil.observacaoRejeicao
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

        this.perfil = {
          id: usuarioLogado.id || null,
          nome: usuarioLogado.nome || '',
          email: usuarioLogado.email || '',
          senha: perfil.senha || '',
          cpfCnpj: usuarioLogado.cpfCnpj || '',
          enderecoCompleto: usuarioLogado.endereco || usuarioLogado.enderecoCompleto || '',
          comprovanteUrl: usuarioLogado.comprovanteUrl || null,
          statusAprovacao: usuarioLogado.statusAprovacao || 0,
          dataEnvioComprovante: usuarioLogado.dataEnvioComprovante || null,
          dataAprovacaoComprovante: usuarioLogado.dataAprovacaoComprovante || null,
          observacaoRejeicao: usuarioLogado.observacaoRejeicao || null
        }

      } catch (error) {
        console.error('❌ Erro ao carregar perfil:', error)
        this.erro = 'Erro ao carregar dados do perfil'
      } finally {
        this.loading = false
      }
    },

    editarPerfil() {
      this.originalPerfil = JSON.parse(JSON.stringify(this.perfil))
      this.editando = true
      this.enderecoAlterado = false
    },

    async salvarEdicao() {
      if (this.enderecoAlterado && !this.comprovante) {
        this.erro = 'Você alterou o endereço. É necessário enviar um comprovante.'
        return
      }

      this.salvando = true
      this.erro = ''

      try {
        // Preparar dados para atualização (apenas campos editáveis)
        const dadosAtualizados = {
          nome: this.perfil.nome,
          email: this.perfil.email,
          senha: this.perfil.senha || '',
          enderecoCompleto: this.perfil.enderecoCompleto,
          cpfCnpj: this.perfil.cpfCnpj
        }

        console.log('📤 Atualizando perfil:', dadosAtualizados)

        // Chamar API para atualizar perfil
        await clienteService.atualizarPerfil(dadosAtualizados)

        // Se houver comprovante, enviar separadamente
        if (this.comprovante) {
          console.log('📎 Enviando comprovante de endereço...')
          await clienteService.uploadComprovante(this.comprovante)
        }

        this.editando = false
        this.enderecoAlterado = false
        this.comprovante = null

        this.sucesso = 'Perfil atualizado com sucesso!'
        setTimeout(() => {
          this.sucesso = ''
        }, 3000)

        // Recarregar perfil para obter dados atualizados do backend
        await this.carregarPerfil()

      } catch (error) {
        console.error('❌ Erro ao salvar perfil:', error)

        // Mostrar detalhes do erro se disponível
        if (error.response?.data?.errors) {
          const erros = Object.values(error.response.data.errors).flat()
          this.erro = `Erro de validação: ${erros.join(', ')}`
        } else if (error.response?.data?.title) {
          this.erro = `Erro: ${error.response.data.title}`
        } else {
          this.erro = 'Erro ao salvar perfil. Verifique os dados e tente novamente.'
        }
      } finally {
        this.salvando = false
      }
    },

    cancelarEdicao() {
      this.perfil = JSON.parse(JSON.stringify(this.originalPerfil))
      this.editando = false
      this.enderecoAlterado = false
      this.comprovante = null
      this.erro = ''
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
    }
  },
  mounted() {
    this.carregarPerfil()
  }
}
</script>

<style scoped>
.barra-voltar {
  background-color: white;
  padding: 1rem 2rem;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid #eee;
}

.logo-titulo {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.logo {
  height: 36px;
  width: 36px;
}

.btn-voltar {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  background-color: #ff8800;
  color: white;
  padding: 10px 18px;
  border: none;
  border-radius: 8px;
  font-size: 0.95rem;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.3s ease, transform 0.1s ease;
}

.btn-voltar:hover {
  background-color: #e67300;
  transform: translateY(-1px);
}

.btn-voltar svg {
  width: 20px;
  height: 20px;
  stroke: white;
  flex-shrink: 0;
}

.perfil-fornecedor {
  max-width: 1000px;
  margin: 40px auto;
  padding: 20px;
  background-color: #fff8e7;
  border-radius: 16px;
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.08);
}

h1 {
  text-align: center;
  margin-bottom: 20px;
  color: #ff8800;
  font-size: 2rem;
  font-weight: bold;
}

.perfil-card {
  background-color: #fff;
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.06);
}

.perfil-card p {
  margin: 12px 0;
  font-size: 1.05rem;
}

.perfil-card strong {
  color: #e67300;
  font-weight: 600;
}

.status-aprovacao {
  margin-top: 1.5rem;
  padding: 1rem;
  background-color: #f8f9fa;
  border-radius: 8px;
  border-left: 4px solid #ff8800;
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

.status.sem-comprovante {
  background-color: #e2e3e5;
  color: #383d41;
  border: 1px solid #d6d8db;
}

.data-info {
  margin-top: 0.5rem;
  color: #6c757d;
}

.observacao-rejeicao {
  margin-top: 1rem;
  padding: 0.75rem;
  background-color: #f8d7da;
  border: 1px solid #f5c6cb;
  border-radius: 6px;
  color: #721c24;
}

.form-group {
  display: flex;
  flex-direction: column;
  margin-bottom: 1rem;
}

.form-group label {
  font-weight: bold;
  margin-bottom: 0.3rem;
}

.form-group input {
  padding: 0.6rem;
  border: 1px solid #ccc;
  border-radius: 8px;
}

.form-group.readonly span {
  background-color: #eee;
  padding: 0.6rem;
  border-radius: 8px;
  color: #555;
  font-weight: bold;
}

.aviso-comprovante {
  margin-top: 0.5rem;
  padding: 0.75rem;
  background-color: #fff3cd;
  border: 1px solid #ffeaa7;
  border-radius: 6px;
  color: #856404;
  font-weight: bold;
}

.input-comprovante {
  margin-top: 0.5rem;
  padding: 0.6rem;
  border: 1px solid #ccc;
  border-radius: 8px;
  background-color: #fff;
  font-size: 0.95rem;
  font-family: inherit;
}

.input-comprovante::file-selector-button {
  background-color: #ff8800;
  border: none;
  color: white;
  padding: 8px 16px;
  border-radius: 8px;
  font-weight: bold;
  cursor: pointer;
  margin-right: 1rem;
  transition: background-color 0.3s ease;
}

.input-comprovante::file-selector-button:hover {
  background-color: #e67300;
}

.input-endereco {
  width: 100%;
  padding: 0.6rem;
  border: 1px solid #ccc;
  border-radius: 8px;
  margin-bottom: 0.5rem;
}

.botoes {
  display: flex;
  justify-content: center;
  gap: 1rem;
}

button {
  background-color: #ff8800;
  color: white;
  padding: 12px 0;
  min-width: 140px;
  margin-top: 15px;
  border: none;
  border-radius: 8px;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.3s ease;
  text-align: center;
}

button:hover:not(:disabled) {
  background-color: #e67300;
}

button:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
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
</style>
