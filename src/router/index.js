import { createRouter, createWebHistory } from 'vue-router'

// Importar as páginas
import Home from '../views/Home.vue'
import CadastroFornecedor from '../views/CadastroFornecedor.vue'
import CadastroCliente from '../views/CadastroCliente.vue'
import OfertaEnergia from '../views/OfertaEnergia.vue'
import DashboardFornecedor from '../views/DashboardFornecedor.vue'
import PerfilFornecedor from '../views/PerfilFornecedor.vue'
import PerfilCliente from '../views/PerfilCliente.vue'
import DashboardCliente from '../views/DashboardCliente.vue'
import AdminPedidos from '../views/AdminPedidos.vue'
import PainelEmpresa from '../views/PainelEmpresa.vue'
import UsuariosView from '../views/UsuariosView.vue'




// Criar a lista de páginas
const routes = [
  { path: '/', name: 'Home', component: Home },
  { path: '/fornecedor', name: 'CadastroFornecedor', component: CadastroFornecedor },
  { path: '/cliente', name: 'CadastroCliente', component: CadastroCliente },
  { path: '/OfertaEnergia', name: 'OfertaEnergia', component: OfertaEnergia },
  { path: '/DashboardFornecedor', name: 'DashboardFornecedor', component: DashboardFornecedor, meta: { requiresAuth: true } },
  { path: '/PerfilFornecedor', name: 'PerfilFornecedor', component: PerfilFornecedor },
  { path: '/PerfilCliente', name: 'PerfilCliente', component: PerfilCliente},
  { path: '/DashboardCliente', name: 'DashboardCliente', component: DashboardCliente },
  { path: '/AdminPedidos', name: 'AdminPedidos', component: AdminPedidos },
  { path: '/painelempresa', name: 'PainelEmpresa', component: PainelEmpresa, meta: { requiresAuth: true } },
  { path: '/usuariosview', name: 'Usuarios', component: UsuariosView}
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to, from, next) => {
  const user = JSON.parse(localStorage.getItem('usuarioLogado'));
  const isAuthenticated = !!localStorage.getItem('usuarioLogado');

  if (to.meta.requiresAuth && !isAuthenticated) {
    next('/');
  } else if (to.path.startsWith('/painelempresa') || to.path.startsWith('/pedidos') || to.path.startsWith('/ofertas') || to.path.startsWith('/usuarios')) {
    if (user?.tipo !== 'Empresa') {
      next('/');
    } else {
      next();
    }
  } else {
    next();
  }
});

export default router
