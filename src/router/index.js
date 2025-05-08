import { createRouter, createWebHistory } from 'vue-router'

// Importar as páginas
import Home from '../views/Home.vue'
import CadastroFornecedor from '../views/CadastroFornecedor.vue'
import CadastroCliente from '../views/CadastroCliente.vue'

// Criar a lista de páginas
const routes = [
  { path: '/', name: 'Home', component: Home },
  { path: '/fornecedor', name: 'CadastroFornecedor', component: CadastroFornecedor },
  { path: '/cliente', name: 'CadastroCliente', component: CadastroCliente },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
