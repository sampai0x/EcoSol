<template>
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
        <label>Nome: <input v-model="perfil.nome" /></label><br />
        <label>Email: <input v-model="perfil.email" /></label><br />
        <label>Endereço: <input v-model="perfil.endereco" /></label><br />
        <p><strong>Tipo de Conta:</strong> {{ perfil.tipo }}</p>
        <p><strong>CPF/CNPJ:</strong> {{ perfil.cpfCnpj }}</p>
      </div>
    </div>

    <div>
      <button v-if="!editando" @click="editarPerfil">Editar Perfil</button>
      <button v-else @click="salvarEdicao">Salvar</button>
      <button v-if="editando" @click="cancelarEdicao">Cancelar</button>
    </div>
  </div>
</template>

<script setup>
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
.perfil-fornecedor {
  max-width: 600px;
  margin: auto;
  padding: 20px;
}

.perfil-card {
  background-color: #f7f7f7;
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 20px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

button {
  background-color: #ff8800;
  color: white;
  padding: 12px 20px;
  border: none;
  border-radius: 8px;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

button:hover {
  background-color: #e67300;
}
</style>
