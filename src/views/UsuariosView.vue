<template>
    <div class="container">
        <div class="top-bar">
            <button class="voltar-btn" @click="voltarParaPainel">← Voltar ao Painel da Empresa</button>
        </div>
        <h1 class="titulo">Painel de Usuários</h1>

        <div class="cards-container">
            <div class="card">
                <h2 class="card-title">Clientes Cadastrados</h2>
                <table v-if="clientes.length" class="tabela">
                    <thead>
                        <tr>
                            <th>Nome</th>
                            <th>Email</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="cliente in clientes" :key="cliente.email">
                            <td>{{ cliente.nome }}</td>
                            <td>{{ cliente.email }}</td>
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
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="fornecedor in fornecedores" :key="fornecedor.email">
                            <td>{{ fornecedor.nome }}</td>
                            <td>{{ fornecedor.email }}</td>
                        </tr>
                    </tbody>
                </table>
                <p v-else class="mensagem-vazia">Nenhum fornecedor cadastrado.</p>
            </div>
        </div>
    </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { ref, onMounted } from 'vue'

const clientes = ref([])
const fornecedores = ref([])
const router = useRouter()

function voltarParaPainel() {
  router.push('/painelempresa') 
}
onMounted(() => {
    const usuarios = JSON.parse(localStorage.getItem('usuarios')) || []
    clientes.value = usuarios.filter(u => u.tipo === 'Cliente')
    fornecedores.value = usuarios.filter(u => u.tipo === 'Fornecedor')
})
</script>

<style scoped>
/* Tipografia base */
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
</style>
