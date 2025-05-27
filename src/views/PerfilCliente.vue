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

    <div class="perfil-card">
      <div v-if="!editando">
        <p><strong>Nome:</strong> {{ perfil.nome }}</p>
        <p><strong>E-mail:</strong> {{ perfil.email }}</p>
        <p><strong>Tipo de Conta:</strong> {{ perfil.tipo }}</p>
        <p><strong>CPF/CNPJ:</strong> {{ perfil.cpfCnpj }}</p>
        <p><strong>Endereços:</strong></p>
        <ul>
          <li v-for="(end, index) in perfil.enderecos" :key="index">{{ end.texto }}</li>
        </ul>
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
          <label>Endereço Principal:</label>
          <div class="endereco-principal">
            <input v-if="editando" v-model="perfil.enderecos[0].texto" @input="comprovanteRequisitado = true"
              class="input-endereco" />
            <span v-else>{{ perfil.enderecos[0]?.texto || 'Nenhum endereço cadastrado' }}</span>
          </div>
          <input v-if="editando" type="file" @change="handleFileUpload" accept=".pdf,image/*"
            class="input-comprovante" />
          <p v-if="comprovanteRequisitado && comprovante">
            Arquivo selecionado: {{ comprovante.nome }}
          </p>
        </div>

        <div class="form-group">
          <label>Endereços Secundários:</label>
          <ul class="lista-enderecos">
            <li v-for="(end, index) in perfil.enderecos.slice(1)" :key="end.id">
              <div>{{ end.texto }}</div>
              <button v-if="editando" @click="removerEndereco(index + 1)" type="button"
                class="btn-remover">Remover</button>
            </li>
          </ul>
          <input v-if="editando" type="text" v-model="novoEndereco" placeholder="Novo endereço"
            class="input-endereco" />
          <input v-if="editando" type="file" @change="handleFileUpload" accept=".pdf,image/*"
            class="input-comprovante" />
          <p v-if="comprovante && !comprovanteRequisitado">
            Arquivo selecionado: {{ comprovante.nome }}
          </p>
          <button v-if="editando" @click="adicionarEndereco" type="button" class="btn-adicionar">Adicionar
            Endereço</button>
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
  enderecos: []
})

const originalPerfil = ref({})
const editando = ref(false)
const router = useRouter()
const comprovante = ref(null)
const comprovanteRequisitado = ref(false)


function handleFileUpload(event) {
  const file = event.target.files[0]
  if (file && file.type === "application/pdf") {
    const reader = new FileReader()
    reader.onload = function (e) {
      comprovante.value = {
        nome: file.name,
        conteudo: e.target.result
      }
    }
    reader.readAsDataURL(file)
  } else {
    alert("Por favor, envie um arquivo PDF válido.")
    event.target.value = null
    comprovante.value = null
  }
}

const voltarDashboard = () => {
  router.push('/DashboardCliente')
}

const carregarPerfil = () => {
  const usuario = JSON.parse(localStorage.getItem('usuarioLogado'))
  if (usuario) {
    let enderecos = JSON.parse(localStorage.getItem('enderecos')) || []
    // filtra os endereços deste usuário
    enderecos = enderecos.filter(e => e.emailUsuario === usuario.email)

    perfil.value = {
      nome: usuario.nome,
      email: usuario.email,
      tipo: usuario.tipo,
      cpfCnpj: usuario.cpfCnpj,
      enderecos: usuario.enderecos || []
    }
  }
}

const editarPerfil = () => {
  originalPerfil.value = JSON.parse(JSON.stringify(perfil.value))
  editando.value = true
}

const novoEndereco = ref('')

const adicionarEndereco = () => {
  const enderecoTexto = novoEndereco.value.trim()
  if (
    enderecoTexto &&
    !perfil.value.enderecos.some(e => e.texto.toLowerCase() === enderecoTexto.toLowerCase())
  ) {

    if (!comprovante.value) {
      alert('Por favor, envie um comprovante antes de adicionar o endereço.')
      return
    }


    const novoEndObj = {
      id: Date.now().toString(),
      texto: enderecoTexto,
      validado: false,
      emailUsuario: perfil.value.email,
      comprovante: comprovante.value
    }

    perfil.value.enderecos.push(novoEndObj)
    novoEndereco.value = ''

    let enderecos = JSON.parse(localStorage.getItem('enderecos')) || []

    const jaExiste = enderecos.some(e =>
      e.texto.toLowerCase() === enderecoTexto.toLowerCase() &&
      e.emailUsuario === perfil.value.email
    )

    if (!jaExiste) {
      enderecos.push(novoEndObj)
      localStorage.setItem('enderecos', JSON.stringify(enderecos))
      comprovante.value = null
    }
  }
}




const removerEndereco = (index) => {
  const enderecoRemovido = perfil.value.enderecos.splice(index, 1)[0]

  if (enderecoRemovido) {
    let enderecos = JSON.parse(localStorage.getItem('enderecos')) || []

    enderecos = enderecos.filter(e =>
      !(e.id === enderecoRemovido.id && e.emailUsuario === perfil.value.email)
    )

    localStorage.setItem('enderecos', JSON.stringify(enderecos))
  }
}


const salvarEdicao = () => {
  console.log("Botão salvar clicado");
  const enderecoOriginal = originalPerfil.value.enderecos[0]?.texto?.trim() || ''
  const enderecoAtual = perfil.value.enderecos[0]?.texto?.trim() || ''

  const enderecoAlterado = enderecoOriginal !== enderecoAtual

  if (enderecoAlterado && !comprovante.value) {
    alert('Você alterou o endereço principal. Envie um comprovante antes de salvar.')
    return
  }

  if (enderecoAlterado && comprovante.value) {
    perfil.value.enderecos[0].comprovante = comprovante.value
    perfil.value.enderecos[0].validado = false 

    let enderecos = JSON.parse(localStorage.getItem('enderecos')) || []

    const indexExistente = enderecos.findIndex(e =>
      e.id === perfil.value.enderecos[0].id &&
      e.emailUsuario === perfil.value.email
    )

    if (indexExistente !== -1) {
      enderecos[indexExistente] = perfil.value.enderecos[0]
    } else {
      enderecos.push(perfil.value.enderecos[0])
    }

    localStorage.setItem('enderecos', JSON.stringify(enderecos))
  }



  localStorage.setItem('usuarioLogado', JSON.stringify(perfil.value))

  const usuarios = JSON.parse(localStorage.getItem('usuarios') || '[]')
  const index = usuarios.findIndex(u => u.email === originalPerfil.value.email)

  if (index !== -1) {
    usuarios[index] = { ...perfil.value }
    localStorage.setItem('usuarios', JSON.stringify(usuarios))
  }

  editando.value = false
  comprovante.value = null
}

const cancelarEdicao = () => {
  perfil.value = JSON.parse(JSON.stringify(originalPerfil.value))
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

.perfil-card ul {
  list-style: none;
  padding: 0;
  margin-top: 0.5rem;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.perfil-card ul li {
  background-color: #fff3e0;
  padding: 10px 14px;
  border-radius: 8px;
  font-size: 1rem;
  border: 1px solid #ffd8a8;
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

.endereco-principal {
  display: flex;
  background-color: #fff3e0;
  padding: 10px 14px;
  border-radius: 8px;
  border: 1px solid #ffd8a8;
  margin-bottom: 0.5rem;
  font-size: 1rem;
}

.endereco-principal input {
  width: 100%;
  max-width: 100%;
  padding: 0.6rem;
  border: 1px solid #ccc;
  border-radius: 8px;
  font-size: 1rem;
}



.lista-enderecos {
  list-style: none;
  padding: 0;
  margin-top: 0.5rem;
  margin-bottom: 1rem;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.lista-enderecos li {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: #fff3e0;
  padding: 10px 14px;
  border-radius: 8px;
  margin-bottom: 8px;
  font-size: 1rem;
}

.btn-remover {
  background-color: #ff0000;
  color: white;
  margin-top: 0px;
  border: none;
  font-weight: bold;
  cursor: pointer;
  transition: color 0.2s ease;
}

.btn-remover:hover {
  background-color: #b90303;
}

.input-comprovante {
  display: flex;
  padding: 0.6rem;
  border: 1px solid #ccc;
  border-radius: 8px;
  margin-bottom: 0.5rem;
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

.btn-adicionar {
  background-color: #ff8800;
  color: white;
  padding: 10px 18px;
  border: none;
  border-radius: 8px;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.btn-adicionar:hover {
  background-color: #e67300;
}
</style>
