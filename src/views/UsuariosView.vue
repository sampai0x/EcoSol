<template>
  <div class="container">
    <div class="top-bar">
      <button class="voltar-btn" @click="voltarParaPainel">← Voltar ao Painel da Empresa</button>
    </div>

    <h1 class="titulo">Painel de Usuários</h1>

    <div class="cards-container">

      <!-- Clientes -->
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

      <!-- Endereços Pendentes -->
      <div class="card">
        <h2 class="card-title">Endereços Pendentes de Aprovação</h2>
        <table v-if="enderecosPendentes.length" class="tabela">
          <thead>
            <tr>
              <th>Rua / Logradouro</th>
              <th>Comprovante</th>
              <th>Cliente</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="endereco in enderecosPendentes" :key="endereco.id">
              <td>{{ endereco.texto }}</td>
              <td>
                <a v-if="endereco.comprovante && endereco.comprovante.conteudo" :href="endereco.comprovante.conteudo"
                  :download="endereco.comprovante.nome" class="comprovante-btn">
                  Baixar
                </a>
                <span v-else style="color: gray;">Não enviado</span>
              </td>
              <td>{{ getClienteNome(endereco.emailUsuario) }}</td>
              <td>
                <button class="aprovar-btn" @click="aprovarEndereco(endereco.id)">Aprovar</button>
                <button class="rejeitar-btn" @click="rejeitarEndereco(endereco.id)">Rejeitar</button>
              </td>
            </tr>
          </tbody>
        </table>
        <p v-else class="mensagem-vazia">Nenhum endereço pendente.</p>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const clientes = ref([])
const fornecedores = ref([])
const enderecosPendentes = ref([])

const router = useRouter()

function voltarParaPainel() {
  router.push('/painelempresa')
}

function getClienteNome(email) {
  if (!email) return 'Desconhecido'
  const emailClean = email.toLowerCase().trim()
  const cliente = clientes.value.find(c => c.email.toLowerCase().trim() === emailClean)
  return cliente ? cliente.nome : 'Desconhecido'
}


function aprovarEndereco(id) {
  const enderecos = JSON.parse(localStorage.getItem('enderecos')) || [];
  const idx = enderecos.findIndex(e => e.id === id);

  if (idx === -1) return;

  // Atualiza o endereço
  enderecos[idx].status = 'aprovado';
  enderecos[idx].validado = true;

  // Salva no localStorage
  localStorage.setItem('enderecos', JSON.stringify(enderecos));

  // Atualiza também no usuário correspondente
  const usuarios = JSON.parse(localStorage.getItem('usuarios')) || [];
  const usuarioIdx = usuarios.findIndex(u => u.email === enderecos[idx].emailUsuario);

  if (usuarioIdx !== -1) {
    const usuario = usuarios[usuarioIdx];
    const enderecoUsuarioIdx = usuario.enderecos.findIndex(e => e.id === id);
    if (enderecoUsuarioIdx !== -1) {
      usuario.enderecos[enderecoUsuarioIdx].validado = true;
      usuario.enderecos[enderecoUsuarioIdx].status = 'aprovado';
    }

    usuarios[usuarioIdx] = usuario;
    localStorage.setItem('usuarios', JSON.stringify(usuarios));
  }

  // Atualiza a lista reativa
  carregarEnderecosPendentes();
}




function rejeitarEndereco(id) {
  const enderecos = JSON.parse(localStorage.getItem('enderecos')) || []
  const enderecoRemover = enderecos.find(e => e.id === id)
  const novosEnderecos = enderecos.filter(e => e.id !== id)
  localStorage.setItem('enderecos', JSON.stringify(novosEnderecos))


  // Remove do usuário
  if (enderecoRemover) {
    const usuarios = JSON.parse(localStorage.getItem('usuarios')) || []
    const usuarioIdx = usuarios.findIndex(u => u.email === enderecoRemover.clienteEmail)
    if (usuarioIdx !== -1) {
      const usuario = usuarios[usuarioIdx]
      usuario.enderecos = usuario.enderecos.filter(e => e.id !== id)
      usuarios[usuarioIdx] = usuario
      localStorage.setItem('usuarios', JSON.stringify(usuarios))
    }
  }

  carregarEnderecosPendentes()
}

function carregarEnderecosPendentes() {
  const usuarios = JSON.parse(localStorage.getItem('usuarios')) || []
  clientes.value = usuarios.filter(u => u.tipo === 'Cliente')
  fornecedores.value = usuarios.filter(u => u.tipo === 'Fornecedor')

  const enderecos = JSON.parse(localStorage.getItem('enderecos')) || []
  enderecosPendentes.value = enderecos.filter(e => !e.validado)

  console.log('Endereços pendentes:', enderecosPendentes.value)
}

onMounted(() => {
  carregarEnderecosPendentes()
  localStorage.removeItem('enderecosPendentes')

  setTimeout(() => {
    console.log('Clientes:', clientes.value)
    console.log('Endereços Pendentes:', enderecosPendentes.value)
  }, 300)
})

import { watch } from 'vue'

watch(clientes, (novosClientes) => {
  console.log('Clientes atualizados:', novosClientes)
  console.log('Recalculando nomes dos clientes pendentes...')
})

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

.aprovar-btn:hover {
  background-color: #45a049;
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

.rejeitar-btn:hover {
  background-color: #da190b;
}
</style>
