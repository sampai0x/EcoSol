<template>
  <div class="lista-fornecedores">
    <h2>Fornecedores Cadastrados</h2>
    <ul v-if="fornecedores.length">
      <li v-for="(fornecedor, index) in fornecedores" :key="index">
        <strong>{{ fornecedor.nome }}</strong> — {{ fornecedor.email }}
      </li>
    </ul>
    <p v-else>Nenhum fornecedor cadastrado ainda.</p>
  </div>
</template>

<script>
export default {
  name: 'FornecedorLista',
  props: {
    fornecedores: {
      type: Array,
      required: true
    }
  }
};
</script>

<style scoped>
.lista-fornecedores {
  padding: 2rem;
  background-color: #f1f5f9;
  border-radius: 12px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

ul {
  list-style: none;
  padding-left: 0;
}

li {
  background: white;
  padding: 1rem;
  margin-bottom: 0.5rem;
  border-radius: 8px;
  border-left: 4px solid #00acc1;
}
</style>
