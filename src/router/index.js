import { createRouter, createWebHistory } from 'vue-router'

// Importar as páginas
import Home from '../views/Home.vue'
import CadastroFornecedor from '../views/CadastroFornecedor.vue'
import CadastroCliente from '../views/CadastroCliente.vue'
import OfertaEnergia from '../views/OfertaEnergia.vue'
import DashboardFornecedor from '../views/DashboardFornecedor.vue'
import PerfilFornecedor from '../views/PerfilFornecedor.vue'


// Criar a lista de páginas
const routes = [
  { path: '/', name: 'Home', component: Home },
  { path: '/fornecedor', name: 'CadastroFornecedor', component: CadastroFornecedor },
  { path: '/cliente', name: 'CadastroCliente', component: CadastroCliente },
  { path: '/OfertaEnergia', name: 'OfertaEnergia', component: OfertaEnergia },
  { path: '/DashboardFornecedor', name: 'DashboardFornecedor', component: DashboardFornecedor, meta: { requiresAuth: true } },
  { path: '/PerfilFornecedor', name: 'PerfilFornecedor', component: PerfilFornecedor}
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to, from, next) => {
  const isAuthenticated = !!localStorage.getItem('usuarioLogado');

  if (to.meta.requiresAuth && !isAuthenticated) {
    next('/');
  } else {
    next();
  }
});

export default router
