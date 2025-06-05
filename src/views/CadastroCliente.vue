<template>
  <div class="cadastro">
    <nav class="navbar">
      <div class="navbar-container">
        <div class="logo">
          <img src="/src/img/EcoSolNavBar.png" alt="Logo" class="logo-img" />
        </div>
        <button class="voltar" @click="goTohome">← Voltar para Home</button>
      </div>
    </nav>
    <form @submit.prevent="enviarFormulario">
      <h2>Cadastro do Cliente</h2>

      <div class="form-group">
        <label for="nome">Nome:</label>
        <input type="text" id="nome" v-model="form.nome" required placeholder="Digite seu nome" />
      </div>

      <div class="form-group">
        <label for="cpfCnpj">CPF/CNPJ:</label>
        <input type="text" id="cpfCnpj" v-model="form.cpfCnpj" required placeholder="Digite o CPF ou CNPJ" />
        <span v-if="cpfCnpjInvalido" class="erro">CPF ou CNPJ inválido</span>
      </div>

      <div class="form-group">
        <label for="email">Email:</label>
        <input type="email" id="email" v-model="form.email" required placeholder="Digite seu email" />
        <span v-if="emailInvalido" class="erro">Email inválido</span>
      </div>

      <div class="form-group">
        <label for="senha">Senha:</label>
        <input type="password" id="senha" v-model="form.senha" required placeholder="Digite sua senha" />
      </div>

      <div class="form-group">
        <label for="confirmarSenha">Confirmar Senha:</label>
        <input type="password" id="confirmarSenha" v-model="form.confirmarSenha" required
          placeholder="Digite sua senha" />
        <span v-if="senhasDiferentes" class="erro">As senhas não coincidem</span>
      </div>

      <div class="form-group">
        <label for="endereco">Endereço:</label>
        <input type="text" id="endereco" v-model="form.endereco" required
          placeholder="Ex: (Rua Tal, 34 - Bairro do Sol, Leme/SP)" />
      </div>

      <div class="form-group">
        <label for="comprovante">Comprovante de Endereço (PDF ou imagem):</label>
        <input type="file" id="comprovante" @change="handleFileUpload" accept=".pdf,image/*" required/>
      </div>

      <button type="submit" :disabled="formInvalido">Cadastrar</button>

      <p v-if="mensagem" class="mensagem">{{ mensagem }}</p>
    </form>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { clienteService } from '@/services/cliente'
import { authService } from '@/services/auth'

const router = useRouter()

const form = ref({
  nome: '',
  email: '',
  senha: '',
  confirmarSenha: '',
  endereco: '',
  cpfCnpj: ''
})

const comprovante = ref(null)
const loading = ref(false)
const erro = ref('')
const sucesso = ref('')

const emailInvalido = computed(() => {
  const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return form.value.email && !regex.test(form.value.email)
})

const senhasDiferentes = computed(() => {
  return form.value.senha && form.value.confirmarSenha && form.value.senha !== form.value.confirmarSenha
})

const cpfCnpjInvalido = computed(() => {
  const val = form.value.cpfCnpj.replace(/\D/g, '')
  if (!form.value.cpfCnpj) return false
  return !(val.length === 11 || val.length === 14)
})

const formInvalido = computed(() => {
  return emailInvalido.value || senhasDiferentes.value || cpfCnpjInvalido.value || !comprovante.value
})

function handleFileUpload(event) {
  const file = event.target.files[0]
  if (file) {
    // Validar tamanho do arquivo (5MB)
    const maxSize = 5 * 1024 * 1024 // 5MB
    if (file.size > maxSize) {
      erro.value = 'Arquivo excede o tamanho máximo permitido (5MB)'
      return
    }

    // Validar extensão
    const extensoesPermitidas = ['.pdf', '.jpg', '.jpeg', '.png']
    const extensao = '.' + file.name.split('.').pop().toLowerCase()
    if (!extensoesPermitidas.includes(extensao)) {
      erro.value = 'Extensão de arquivo não permitida. Use PDF, JPG, JPEG ou PNG'
      return
    }

    comprovante.value = file
    erro.value = ''
  }
}

function goTohome() {
  router.push('/')
}

async function enviarFormulario() {
  if (formInvalido.value || loading.value) return

  loading.value = true
  erro.value = ''
  sucesso.value = ''

  try {
    const dadosCliente = {
      nome: form.value.nome,
      email: form.value.email,
      senha: form.value.senha,
      cpfCnpj: form.value.cpfCnpj,
      endereco: form.value.endereco
    }

    console.log('Enviando dados do cliente:', dadosCliente)

    // Cadastrar cliente
    await clienteService.cadastrar(dadosCliente)
    
    sucesso.value = 'Cliente cadastrado com sucesso! Fazendo login...'
    
    // Fazer login automático após cadastro
    await authService.loginCliente(form.value.email, form.value.senha)
    
    // Upload do comprovante após login (se necessário)
    if (comprovante.value) {
      try {
        await clienteService.uploadComprovante(comprovante.value)
      } catch (comprovanteError) {
        console.warn('Erro ao fazer upload do comprovante:', comprovanteError)
        // Não bloquear o fluxo se o upload do comprovante falhar
      }
    }
    
    sucesso.value = 'Cliente cadastrado com sucesso! Redirecionando...'
    
    // Aguardar um pouco para mostrar a mensagem de sucesso
    setTimeout(() => {
      router.push('/dashboardcliente')
    }, 2000)

  } catch (error) {
    console.error('Erro completo:', error)
    console.error('Response data:', error.response?.data)
    console.error('Response status:', error.response?.status)
    
    erro.value = error.response?.data?.message || 
                 error.response?.data?.title || 
                 error.message || 
                 'Erro ao cadastrar cliente. Tente novamente.'
  } finally {
    loading.value = false
  }
}
</script>


<style scoped>
/* seu CSS permanece igual */
.navbar {
  width: 100%;
  background-color: #ffffff;
  padding: 1rem 2rem;
  position: fixed;
  top: 0;
  left: 0;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  z-index: 10;
}

.navbar-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  max-width: 1080px;
  margin: 0 auto;
}

.logo {
  font-size: 1.5rem;
  font-weight: bold;
  color: white;
  cursor: pointer;
}

.logo {
  display: flex;
  align-items: center;
  cursor: pointer;
}

.logo-img {
  height: 40px;
  margin-right: 10px;
}

.voltar {
  background-color: #ff8800;
  color: white;
  padding: 0.5rem 1rem;
  margin-bottom: 1rem;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.3s;
}

.voltar:hover {
  background-color: #d97706;
}

.cadastro {
  min-height: 100vh;
  margin-top: 100px;
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
  max-width: 420px;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

label {
  font-weight: bold;
  margin-bottom: 0.3rem;
}

input,
select {
  padding: 0.6rem;
  border: 1px solid #ccc;
  border-radius: 8px;
}

button {
  margin-top: 1.2rem;
  padding: 0.75rem;
  background-color: #f59e0b;
  color: white;
  font-weight: bold;
  border: none;
  border-radius: 8px;
  cursor: pointer;
}

button:disabled {
  background-color: #fcd34d;
  cursor: not-allowed;
}

button:hover:enabled {
  background-color: #d97706;
}

.erro {
  color: red;
  font-size: 0.85rem;
}

.mensagem {
  margin-top: 1rem;
  color: green;
  font-weight: bold;
  text-align: center;
}

@media (max-width: 480px) {
  form {
    padding: 1.2rem;
    width: 90%;
  }
}
</style>
