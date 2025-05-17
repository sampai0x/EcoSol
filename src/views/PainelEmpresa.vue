<template>
  <div class="painel-empresa">
    <div class="header">
      <h1>Painel Administrativo</h1>
      <p class="subtitulo">Bem-vindo, administrador da EcoSol</p>
    </div>

    <div class="cards">
      <div class="card" @click="irParaPedidos">
        <h2>Pedidos de Energia</h2>
        <p>Visualize e gerencie os pedidos dos clientes.</p>
      </div>

      <div class="card" @click="irParaOfertas">
        <h2>Ofertas de Fornecedores</h2>
        <p>Veja as ofertas disponíveis de cada fornecedor.</p>
      </div>

      <div class="card" @click="irParaUsuarios">
        <h2>Usuários Cadastrados</h2>
        <p>Consulte clientes e fornecedores registrados.</p>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'PainelEmpresa',
  methods: {
    irParaPedidos() {
      this.$router.push('/pedidos');
    },
    irParaOfertas() {
      this.$router.push('/ofertas');
    },
    irParaUsuarios() {
      this.$router.push('/usuariosview');
    }
  }
}
</script>

<style scoped>
.painel-empresa {
  padding: 2rem;
  max-width: 1000px;
  margin: auto;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  background: linear-gradient(to right, #fff7ed, #ffedd5);
  min-height: 100vh;
}

.header {
  text-align: center;
  margin-bottom: 2rem;
}

.header h1 {
  font-size: 2.5rem;
  color: #b45309; 
}

.subtitulo {
  color: #78350f; 
  font-size: 1.1rem;
  margin-top: 0.5rem;
}

.cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.8rem;
}

.card {
  background: #ffffff;
  border-radius: 1rem;
  padding: 2rem;
  text-align: center;
  cursor: pointer;
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
  border: 1px solid #fcd34d;
}

.card h2 {
  color: #92400e; 
  font-size: 1.4rem;
  margin-bottom: 0.8rem;
}

.card p {
  color: #78350f;
  font-size: 0.95rem;
}

.card:hover {
  background-color: #fef3c7; 
  transform: translateY(-4px);
  box-shadow: 0 10px 16px rgba(0, 0, 0, 0.12);
}
</style>
