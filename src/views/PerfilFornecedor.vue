<template>
  <div class="barra-voltar">
    <div class="logo-titulo">
      <img src="/src/img/EcoSolNavBar.png" alt="Logo" class="logo" />
      
    </div>
    <button @click="voltarDashboard" class="btn-voltar">
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" stroke="currentColor" stroke-width="2"
        stroke-linecap="round" stroke-linejoin="round" class="feather feather-arrow-left" viewBox="0 0 24 24">
        <line x1="19" y1="12" x2="5" y2="12" />
        <polyline points="12 19 5 12 12 5" />
      </svg>
      Voltar ao Dashboard
    </button>
  </div>
  <div class="perfil-fornecedor">

    <h1>Meu Perfil</h1>

    <div class="perfil-card">
      <div v-if="!editando">
        <p><strong>Nome:</strong> {{ perfil.nome }}</p>
        <p><strong>E-mail:</strong> {{ perfil.email }}</p>
        <p><strong>Tipo de Conta:</strong> {{ perfil.tipo }}</p>
        <p><strong>CPF/CNPJ:</strong> {{ perfil.cpfCnpj }}</p>
        <p><strong>Endereço:</strong> {{ perfil.endereco }}</p>
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
          <label>Endereço:</label>
          <input type="text" v-model="perfil.endereco" />
        </div>

        <div class="form-group readonly">
          <label>Tipo de Conta:</label>
          <span>{{ perfil.tipo }}</span>
        </div>

        <div class="form-group readonly">
          <label>CPF/CNPJ:</label>
          <span>{{ perfil.cpfCnpj }}</span>
        </div>
      </div>
    </div>

    <div class="botoes">
      <button v-if="!editando" @click="editarPerfil">Editar Perfil</button>
      <button v-else @click="salvarEdicao">Salvar</button>
      <button v-if="editando" @click="cancelarEdicao">Cancelar</button>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { ref, onMounted } from 'vue'

const perfil = ref({
  nome: '',
  email: '',
  tipo: '',
  cpfCnpj: '',
  endereco: ''
})

const originalPerfil = ref({})
const editando = ref(false)
const router = useRouter()

const voltarDashboard = () => {
  router.push('/DashboardFornecedor')
}

const carregarPerfil = () => {
  const usuario = JSON.parse(localStorage.getItem('usuarioLogado'))
  if (usuario) {
    perfil.value = {
      nome: usuario.nome,
      email: usuario.email,
      tipo: usuario.tipo,
      cpfCnpj: usuario.cpfCnpj,
      endereco: usuario.endereco
    }
  }
}

const editarPerfil = () => {
  originalPerfil.value = { ...perfil.value };
  editando.value = true
}

const salvarEdicao = () => {
  localStorage.setItem('usuarioLogado', JSON.stringify(perfil.value))


  const usuarios = JSON.parse(localStorage.getItem('usuarios') || '[]')
  const index = usuarios.findIndex(u => u.email === originalPerfil.value.email)

  if (index !== -1) {
    usuarios[index] = { ...perfil.value }
    localStorage.setItem('usuarios', JSON.stringify(usuarios))
  }

  editando.value = false
}

const cancelarEdicao = () => {
  perfil.value = { ...originalPerfil.value }
  editando.value = false
}


onMounted(() => {
  carregarPerfil()
})
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

.titulo {
  font-size: 1.25rem;
  font-weight: 600;
  color: #333;
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
  max-width: 700px;
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

button:hover {
  background-color: #e67300;
}
</style>
