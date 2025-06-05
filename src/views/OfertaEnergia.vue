<template>
  <div class="oferta">
    <form @submit.prevent="enviarOferta">
      <h2>Cadastro de Oferta de Energia</h2>

      <div class="form-group">
        <label for="quantidade">Quantidade (kWh):</label>
        <input type="number" id="quantidade" v-model="oferta.quantidade" required />
      </div>

      <div class="form-group">
        <label for="preco">Preço por kWh (R$):</label>
        <input type="number" step="0.01" id="preco" v-model="oferta.preco" required />
      </div>

      <div class="form-group">
        <label for="disponivel">Data de Disponibilidade:</label>
        <input type="date" id="disponivel" v-model="oferta.dataDisponivel" required />
      </div>

      <button type="submit">Enviar Oferta</button>
    </form>
  </div>
</template>

<script>
import { fornecedorService } from '@/services/fornecedor'
import { authService } from '@/services/auth'

export default {
  name: 'OfertaEnergia',
  data() {
    return {
      oferta: {
        quantidade: '',
        preco: '',
        dataDisponivel: ''
      }
    };
  },
  async created() {
    try {
      // Verifica se o usuário está autenticado como fornecedor
      const user = await authService.getCurrentUser()
      if (!user || user.tipo !== 'Fornecedor') {
        this.$router.push('/')
        return
      }

      // Verifica se já existe uma oferta ativa
      const ofertas = await fornecedorService.getMinhasOfertas()
      const temOfertaAtiva = ofertas.some(oferta => 
        oferta.status === 0 || // EM_ANALISE
        oferta.status === 1    // EM_VIGOR
      )

      if (temOfertaAtiva) {
        alert('Você já possui uma oferta ativa. Aguarde sua conclusão para criar uma nova.')
        this.$router.push('/DashboardFornecedor')
        return
      }
    } catch (error) {
      console.error('Erro ao verificar usuário/ofertas:', error)
      this.$router.push('/')
    }
  },
  methods: {
    async enviarOferta() {
      try {
        await fornecedorService.criarOferta({
          quantidade: Number(this.oferta.quantidade),
          preco: Number(this.oferta.preco),
          dataDisponivel: this.oferta.dataDisponivel
        })

        // Redireciona para o dashboard após criar a oferta
        this.$router.push('/DashboardFornecedor')
      } catch (error) {
        console.error('Erro ao criar oferta:', error)
        if (error.response?.data?.message) {
          alert(error.response.data.message)
        } else {
          alert('Erro ao criar oferta. Tente novamente.')
        }
      }
    }
  }
};
</script>

<style scoped>
.oferta {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(to bottom, #fef3c7, #fde68a);
  padding: 2rem;
}

form {
  background: white;
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 400px;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

label {
  margin-bottom: 0.3rem;
  font-weight: bold;
}

input {
  padding: 0.5rem;
  border: 1px solid #ccc;
  border-radius: 8px;
}

button {
  margin-top: 1.5rem;
  padding: 0.75rem;
  background-color: #f59e0b;
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
}

button:hover {
  background-color: #d97706;
}
</style>
