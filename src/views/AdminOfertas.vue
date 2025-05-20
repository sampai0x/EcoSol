<template>
    <nav class="admin-nav">
        <button @click="voltarPainel">← Voltar ao Painel da Empresa</button>
    </nav>
    <div class="admin-ofertas">


        <h1>Gestão de Ofertas de Energia</h1>

        <!-- Ofertas Disponíveis -->
        <section class="ofertas-bloco">
            <h2>Ofertas Disponíveis</h2>

            <div v-if="ofertas.length" class="card-container">
                <div v-for="(oferta, index) in ofertas" :key="index" class="card oferta">
                    <div class="card-header">
                        <h3>{{ oferta.nome }}</h3>
                        <span class="email">{{ oferta.email }}</span>
                    </div>
                    <p><strong>{{ oferta.quantidade }} kWh</strong> a <strong>R$ {{ oferta.preco }}</strong> por kWh</p>
                    <p><span class="label">Disponível em:</span> {{ oferta.dataDisponivel }}</p>

                    <div class="card-actions">
                        <button class="contratar" @click="firmarContrato(oferta.email)">Firmar Contrato</button>
                        <button class="excluir" @click="excluirOferta(oferta.email)">Excluir</button>
                    </div>
                </div>
            </div>
            <p v-else class="mensagem-vazia">Nenhuma oferta disponível.</p>
        </section>

        <!-- Ofertas Contratadas -->
        <section class="ofertas-bloco">
            <h2>Ofertas Contratadas</h2>

            <div v-if="ofertasContratadas.length" class="card-container">
                <div v-for="(oferta, index) in ofertasContratadas" :key="'c' + index" class="card contratada">
                    <div class="card-header">
                        <h3>{{ oferta.nome }}</h3>
                        <span class="email">{{ oferta.email }}</span>
                    </div>
                    <p><strong>{{ oferta.quantidade }} kWh</strong> a <strong>R$ {{ oferta.preco }}</strong> por kWh</p>
                    <p v-if="oferta.dataDisponivel">
                        <span class="label">Disponível em:</span> {{ oferta.dataDisponivel }}
                    </p>
                    <p class="status">✅ Contrato firmado</p>
                </div>
            </div>
            <p v-else class="mensagem-vazia">Nenhuma oferta contratada.</p>
        </section>
    </div>
</template>

<script>
export default {
    data() {
        return {
            ofertas: [],
            ofertasContratadas: []
        };
    },
    mounted() {
        this.carregarOfertas();
    },
    methods: {
        voltarPainel() {
            this.$router.push('/painelempresa');
        },
        carregarOfertas() {
            const dados = JSON.parse(localStorage.getItem('ofertasEnergia')) || {};
            const todas = Object.values(dados);
            this.ofertas = todas.filter(oferta => !oferta.contratada);
            this.ofertasContratadas = todas.filter(oferta => oferta.contratada);
        },
        excluirOferta(email) {
            const dados = JSON.parse(localStorage.getItem('ofertasEnergia')) || {};
            delete dados[email];
            localStorage.setItem('ofertasEnergia', JSON.stringify(dados));
            this.carregarOfertas();
        },
        firmarContrato(email) {
            const dados = JSON.parse(localStorage.getItem('ofertasEnergia')) || {};
            if (dados[email]) {
                dados[email].contratada = true;
                localStorage.setItem('ofertasEnergia', JSON.stringify(dados));
                this.carregarOfertas();
            }
        }
    }
};
</script>

<style scoped>
.admin-nav {
    background-color: #fff8e1;
    padding: 1rem 2rem;
    border-bottom: 1px solid #ffd95c;
    display: flex;
    justify-content: flex-start;
}

.admin-nav button {
    background-color: #ff9800;
    color: white;
    font-weight: bold;
    border: none;
    padding: 0.5rem 1rem;
    border-radius: 8px;
    cursor: pointer;
    transition: background-color 0.3s ease;
}

.admin-nav button:hover {
    background-color: #e68900;
}

.admin-ofertas {
    max-width: 1000px;
    margin: 2rem auto;
    background-color: white;
    font-family: 'Segoe UI', sans-serif;
    padding: 2rem;
    color: #333;
    border-radius: 10px;
}

h1 {
    text-align: center;
    font-size: 2rem;
    margin-bottom: 2rem;
    color: #2c3e50;
}

h2 {
    font-size: 1.5rem;
    color: #ff8800;
    margin-bottom: 1rem;
}

.ofertas-bloco {
    margin-bottom: 3rem;
}

.card-container {
    display: grid;
    gap: 1.5rem;
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
}

.card {
    background: #fff;
    border-radius: 16px;
    padding: 1.2rem 1.5rem;
    box-shadow: 0 4px 14px rgba(0, 0, 0, 0.08);
    transition: transform 0.2s ease;
}

.card:hover {
    transform: translateY(-4px);
}

.card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 0.5rem;
}

.card-header h3 {
    font-size: 1.2rem;
    margin: 0;
    color: #333;
}

.email {
    font-size: 0.9rem;
    color: #888;
}

.label {
    font-weight: bold;
    color: #555;
}

.card-actions {
    margin-top: 1rem;
    display: flex;
    justify-content: flex-end;
    gap: 0.5rem;
}

button {
    padding: 0.5rem 0.9rem;
    border-radius: 8px;
    font-weight: bold;
    border: none;
    cursor: pointer;
    font-size: 0.9rem;
    transition: background-color 0.2s ease;
}

.contratar {
    background-color: #2ecc71;
    color: white;
}

.contratar:hover {
    background-color: #27ae60;
}

.excluir {
    background-color: #e74c3c;
    color: white;
}

.excluir:hover {
    background-color: #c0392b;
}

.contratada {
    background-color: #ecfdf5;
    border: 1px solid #b2f2bb;
}

.status {
    margin-top: 0.8rem;
    font-weight: bold;
    color: #2ecc71;
}

.mensagem-vazia {
    color: #999;
    font-style: italic;
    margin-top: 1rem;
}
</style>
