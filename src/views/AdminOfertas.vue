<template>
    <nav class="admin-nav">
        <button @click="voltarPainel">← Voltar ao Painel da Empresa</button>
    </nav>
    <div class="admin-ofertas">
        <h1>Gestão de Ofertas de Energia</h1>

        <div v-if="loading" class="loading">
            Carregando ofertas...
        </div>

        <div v-else>
            <!-- Ofertas Disponíveis -->
            <section class="ofertas-bloco">
                <h2>Contratos de Energia</h2>

                <div v-if="ofertas.length" class="card-container">
                    <div v-for="(oferta, index) in ofertas" :key="oferta.id" class="card oferta">
                        <div class="card-header">
                            <h3>{{ oferta.fornecedorNome }}</h3>
                            <span :class="getStatusClass(oferta.status)">{{ getStatusText(oferta.status) }}</span>
                        </div>
                        <p><strong>{{ oferta.quantidadeEnergia.toLocaleString('pt-BR') }} kWh</strong></p>
                        <p><span class="label">Valor:</span> R$ {{ formatCurrency(oferta.valorContrato) }} por kWh</p>
                        <p><span class="label">Data do Contrato:</span> {{ formatDate(oferta.dataContrato) }}</p>

                        <div class="card-actions" v-if="oferta.status === StatusContrato.EM_ANALISE">
                            <button class="contratar" 
                                    @click="firmarContrato(oferta.id)" 
                                    :disabled="processando">
                                {{ processando ? 'Processando...' : 'Aprovar Contrato' }}
                            </button>
                            <button class="excluir" 
                                    @click="excluirOferta(oferta.id)" 
                                    :disabled="processando">
                                {{ processando ? 'Processando...' : 'Cancelar' }}
                            </button>
                        </div>
                    </div>
                </div>
                <p v-else class="mensagem-vazia">Nenhum contrato disponível.</p>
            </section>
        </div>

        <div v-if="erro" class="erro-message">{{ erro }}</div>
    </div>
</template>

<script>
import { adminService } from '@/services/admin'

const StatusContrato = {
    EM_ANALISE: 0,
    EM_VIGOR: 1,
    CANCELADO: 2,
    CONCLUIDO: 3
}

export default {
    data() {
        return {
            todasOfertas: [],
            loading: false,
            processando: false,
            erro: '',
            StatusContrato
        };
    },
    computed: {
        ofertas() {
            // Todas as ofertas dos fornecedores estão em análise
            return this.todasOfertas;
        },
        ofertasContratadas() {
            // Não temos mais ofertas contratadas aqui
            return [];
        }
    },
    async mounted() {
        await this.carregarOfertas();
    },
    methods: {
        voltarPainel() {
            this.$router.push('/painelempresa');
        },

        async carregarOfertas() {
            console.log('🔄 Iniciando carregamento de contratos...');
            this.loading = true;
            this.erro = '';
            
            try {
                // Carregar todos os contratos
                console.log('📡 Fazendo requisição para contratos...');
                const contratos = await adminService.getContratos();
                console.log('✅ Contratos recebidos:', contratos);

                // Mapear os contratos
                this.todasOfertas = contratos.map(contrato => ({
                    id: contrato.id,
                    fornecedorNome: contrato.fornecedorNome,
                    quantidadeEnergia: contrato.quantidadeEnergia,
                    valorContrato: contrato.valorContrato,
                    dataContrato: contrato.dataContrato || contrato.createdAt,
                    status: contrato.status
                }));

                console.log('🔍 Contratos mapeados:', this.todasOfertas);
            } catch (error) {
                console.error('❌ Erro ao carregar contratos:', error);
                console.error('Detalhes do erro:', {
                    message: error.message,
                    response: error.response?.data,
                    status: error.response?.status
                });
                this.erro = 'Erro ao carregar contratos. Tente novamente.';
                this.todasOfertas = [];
            } finally {
                this.loading = false;
                console.log('🏁 Carregamento finalizado. Total de contratos:', this.todasOfertas.length);
            }
        },

        getStatusClass(status) {
            switch (status) {
                case StatusContrato.EM_ANALISE:
                    return 'status-analise'
                case StatusContrato.EM_VIGOR:
                    return 'status-vigor'
                case StatusContrato.CANCELADO:
                    return 'status-cancelado'
                case StatusContrato.CONCLUIDO:
                    return 'status-concluido'
                default:
                    return ''
            }
        },

        getStatusText(status) {
            switch (status) {
                case StatusContrato.EM_ANALISE:
                    return '⏳ Em Análise'
                case StatusContrato.EM_VIGOR:
                    return '✅ Em Vigor'
                case StatusContrato.CANCELADO:
                    return '❌ Cancelado'
                case StatusContrato.CONCLUIDO:
                    return '🏆 Concluído'
                default:
                    return 'Status Desconhecido'
            }
        },

        async excluirOferta(id) {
            if (!confirm('Tem certeza que deseja cancelar este contrato?')) return;
            
            this.processando = true;
            this.erro = '';
            
            try {
                await adminService.atualizarStatusContrato(id, StatusContrato.CANCELADO);
                const oferta = this.todasOfertas.find(o => o.id === id);
                if (oferta) {
                    oferta.status = StatusContrato.CANCELADO;
                }
            } catch (error) {
                console.error('Erro ao cancelar contrato:', error);
                this.erro = 'Erro ao cancelar contrato. Tente novamente.';
            } finally {
                this.processando = false;
            }
        },

        async firmarContrato(id) {
            if (!confirm('Tem certeza que deseja aprovar este contrato?')) return;
            
            this.processando = true;
            this.erro = '';
            
            try {
                console.log('📝 Iniciando aprovação do contrato:', id);
                await adminService.atualizarStatusContrato(id, StatusContrato.EM_VIGOR);
                
                // Recarregar ofertas
                await this.carregarOfertas();
                
            } catch (error) {
                console.error('❌ Erro ao aprovar contrato:', error);
                this.erro = 'Erro ao aprovar contrato. Tente novamente.';
            } finally {
                this.processando = false;
            }
        },

        formatCurrency(value) {
            if (!value) return '0,00';
            return new Intl.NumberFormat('pt-BR', { 
                minimumFractionDigits: 2, 
                maximumFractionDigits: 2 
            }).format(value);
        },

        formatDate(dateString) {
            if (!dateString) return 'N/A';
            try {
                const date = new Date(dateString);
                return date.toLocaleDateString('pt-BR');
            } catch (error) {
                return dateString;
            }
        },
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

.loading {
    text-align: center;
    padding: 2rem;
    color: #6b7280;
    font-style: italic;
}

.erro-message {
    color: #dc2626;
    font-size: 0.875rem;
    margin-top: 1rem;
    padding: 0.75rem;
    background-color: #fef2f2;
    border-radius: 8px;
    border: 1px solid #fecaca;
    text-align: center;
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
    gap: 1rem;
    margin-bottom: 1rem;
}

.card-header h3 {
    font-size: 1.2rem;
    margin: 0;
    color: #333;
    flex: 1;
}

.preco {
    font-size: 1.1rem;
    color: #2ecc71;
    font-weight: bold;
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

button:disabled {
    background-color: #6c757d;
    cursor: not-allowed;
}

.contratar {
    background-color: #2ecc71;
    color: white;
}

.contratar:hover:not(:disabled) {
    background-color: #27ae60;
}

.excluir {
    background-color: #e74c3c;
    color: white;
}

.excluir:hover:not(:disabled) {
    background-color: #c0392b;
}

.contratada {
    background-color: #ecfdf5;
    border: 1px solid #b2f2bb;
}

.status-analise {
    color: #f39c12;
    background-color: #fef3c7;
    padding: 0.2rem 0.5rem;
    border-radius: 4px;
    font-weight: bold;
}

.status-vigor {
    color: #2ecc71;
    background-color: #ecfdf5;
    padding: 0.2rem 0.5rem;
    border-radius: 4px;
    font-weight: bold;
}

.status-cancelado {
    color: #e74c3c;
    background-color: #fef2f2;
    padding: 0.2rem 0.5rem;
    border-radius: 4px;
    font-weight: bold;
}

.status-concluido {
    color: #3498db;
    background-color: #eff6ff;
    padding: 0.2rem 0.5rem;
    border-radius: 4px;
    font-weight: bold;
}

.mensagem-vazia {
    color: #999;
    font-style: italic;
    margin-top: 1rem;
}
</style>
