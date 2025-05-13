<template>
  <div class="perfil-fornecedor">
    <h1>Meu Perfil</h1>

    <div class="perfil-card">
      <p><strong>Nome:</strong> {{ perfil.nome }}</p>
      <p><strong>E-mail:</strong> {{ perfil.email }}</p>
      <p><strong>Tipo de Cliente:</strong> {{ perfil.tipoCliente }}</p>
      <p><strong>Data de Cadastro:</strong> {{ perfil.dataCadastro }}</p>
    </div>

    <button @click="editarPerfil">Editar Perfil</button>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const perfil = ref({
  nome: '',
  email: '',
  tipoCliente: '',
  dataCadastro: ''
})

const carregarPerfil = () => {
  const usuario = JSON.parse(localStorage.getItem('usuarioLogado'))
  if (usuario) {
    perfil.value = {
      nome: usuario.nome,
      email: usuario.email,
      tipoCliente: usuario.tipoCliente,
      dataCadastro: usuario.dataCadastro || '2025-01-01' 
    }
  }
}

const editarPerfil = () => {
  alert('Funcionalidade de edição ainda será implementada.')
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
