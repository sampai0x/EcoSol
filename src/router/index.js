import { createRouter, createWebHistory } from "vue-router"
import { authService } from "@/services/auth"

const routes = [
  {
    path: "/",
    name: "Home",
    component: () => import("@/views/Home.vue"),
  },
  {
    path: "/cliente",
    name: "CadastroCliente",
    component: () => import("@/views/CadastroCliente.vue"),
  },
  {
    path: "/fornecedor",
    name: "CadastroFornecedor",
    component: () => import("@/views/CadastroFornecedor.vue"),
  },
  {
    path: "/dashboardcliente",
    name: "DashboardCliente",
    component: () => import("@/views/DashboardCliente.vue"),
    meta: { requiresAuth: true, role: "Cliente" },
  },
  {
    path: "/dashboardfornecedor",
    name: "DashboardFornecedor",
    component: () => import("@/views/DashboardFornecedor.vue"),
    meta: { requiresAuth: true, role: "Fornecedor" },
  },
  {
    path: "/painelempresa",
    name: "PainelEmpresa",
    component: () => import("@/views/PainelEmpresa.vue"),
    meta: { requiresAuth: true, role: ["Empresa", "Admin"] },
  },
  {
    path: "/admin",
    redirect: "/painelempresa",
  },
  {
    path: "/perfilcliente",
    name: "PerfilCliente",
    component: () => import("@/views/PerfilCliente.vue"),
    meta: { requiresAuth: true, role: "Cliente" },
  },
  {
    path: "/perfilfornecedor",
    name: "PerfilFornecedor",
    component: () => import("@/views/PerfilFornecedor.vue"),
    meta: { requiresAuth: true, role: "Fornecedor" },
  },
  {
    path: "/ofertaenergia",
    name: "OfertaEnergia",
    component: () => import("@/views/OfertaEnergia.vue"),
    meta: { requiresAuth: true, role: "Fornecedor" },
  },
  {
    path: "/pedidos",
    name: "AdminPedidos",
    component: () => import("@/views/AdminPedidos.vue"),
    meta: { requiresAuth: true, role: ["Empresa", "Admin"] },
  },
  {
    path: "/ofertas",
    name: "AdminOfertas",
    component: () => import("@/views/AdminOfertas.vue"),
    meta: { requiresAuth: true, role: ["Empresa", "Admin"] },
  },
  {
    path: "/usuariosview",
    name: "UsuariosView",
    component: () => import("@/views/UsuariosView.vue"),
    meta: { requiresAuth: true, role: ["Empresa", "Admin"] },
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

// Guarda de navegação otimizada
router.beforeEach(async (to, from, next) => {
  console.log("🚀 Navegando para:", to.path, "Meta:", to.meta)

  // Se não é uma rota protegida, permite acesso imediatamente
  if (!to.matched.some((record) => record.meta.requiresAuth)) {
    console.log("✅ Rota pública - permitindo acesso")
    next()
    return
  }

  console.log("🔐 Rota protegida - verificando autenticação no backend...")

  try {
    const user = await authService.getCurrentUser()
    console.log("👤 Usuário atual:", user)

    if (!user) {
      console.log("❌ Usuário não autenticado, redirecionando para home")
      next({ path: "/" })
      return
    }

    // Verificar permissões de role
    if (to.meta.role) {
      const allowedRoles = Array.isArray(to.meta.role) ? to.meta.role : [to.meta.role]
      const userType = user.tipo

      console.log("🔐 Verificando permissões - Tipo do usuário:", userType, "Roles permitidas:", allowedRoles)

      if (allowedRoles.includes(userType)) {
        console.log("✅ Usuário autorizado para a rota")
        next()
      } else {
        console.log("❌ Usuário não autorizado para a rota")
        next({ path: "/" })
      }
    } else {
      console.log("✅ Rota protegida sem role específica - permitindo acesso")
      next()
    }
  } catch (error) {
    console.error("❌ Erro na verificação de autenticação:", error)
    next({ path: "/" })
  }
})

export default router
