<template>
    <div class="oferta">
        <form @submit.prevent="enviarOferta">
            <h2>Cadastro de Oferta de Energia</h2>

            <div class="form-group">
                <label for="quantidade">Quantidade (kWh):</label>
                <input type="number" id="quantidade" v-model="oferta.quantidade" required />
            </div>

            <div class="form-group">
                <label for="preco">Preço por kWh (R$):</label>
                <input type="number" step="0.01" id="preco" v-model="oferta.preco" required />
            </div>

            <div class="form-group">
                <label for="disponivel">Data de Disponibilidade:</label>
                <input type="date" id="disponivel" v-model="oferta.dataDisponivel" required />
            </div>

            <button type="submit">Enviar Oferta</button>
        </form>
    </div>
</template>

<script>
export default {
  name: 'OfertaEnergia',
  data() {
    return {
      oferta: {
        quantidade: '',
        preco: '',
        dataDisponivel: ''
      }
    };
  },
  methods: {
    enviarOferta() {
      const newOffer = {
        quantidade: this.oferta.quantidade,
        preco: this.oferta.preco,
        dataDisponivel: this.oferta.dataDisponivel
      };

      const offers = JSON.parse(localStorage.getItem('userOffers') || '[]');
      offers.push(newOffer);
      localStorage.setItem('userOffers', JSON.stringify(offers));

      console.log('Ofertas após adição:', offers);

     
      this.$router.push({ name: 'Dashboard' }).then(() => {
        this.$router.go();
      });
    }
  }
};
</script>

<style scoped>
.oferta {
    min-height: 100vh;
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
</style>
