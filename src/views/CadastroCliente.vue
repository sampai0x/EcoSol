<template>
  <div class="cadastro">
    <form @submit.prevent="enviarFormulario">
      <h2>Cadastro do Cliente</h2>
      <div class="form-group">
        <label for="nome">Nome:</label>
        <input type="text" id="nome" v-model="form.nome" required />
      </div>

      <div class="form-group">
        <label for="email">Email:</label>
        <input type="email" id="email" v-model="form.email" required />
      </div>

      <div class="form-group">
        <label for="senha">Senha:</label>
        <input type="password" id="senha" v-model="form.senha" required />
      </div>

      <div class="form-group">
        <label for="endereco">Endereço:</label>
        <input type="text" id="endereco" v-model="form.endereco" required />
      </div>

      <button type="submit">Cadastrar</button>
    </form>
  </div>
</template>


<script>
export default {
  name: 'CadastroCliente',
  data() {
    return {
      form: {
        nome: '',
        email: '',
        senha: '',
        endereco: ''
      }
    };
  },
  methods: {
    enviarFormulario() {
      alert(`Cliente ${this.form.nome} cadastrado com sucesso!`);
    }
  }
};
</script>

<style scoped>
.cadastro {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(to bottom, #fef3c7, #fde68a);; 
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

@media (max-width: 480px) {
  form {
    padding: 1.2rem;
    width: 90%;
  }

  input, button {
    font-size: 0.95rem;
  }
}
</style>