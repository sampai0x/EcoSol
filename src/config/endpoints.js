// Mapeamento dos endpoints da API baseado no Swagger real
export const endpoints = {
  // Base URL
  baseURL: "https://localhost:7272",

  // ✅ Endpoints que funcionam (confirmados)
  home: {
    stats: "/api/Home/stats", // ✅ Funcionando
  },

  // 🔐 Endpoints de autenticação
  auth: {
    loginCliente: "/api/auth/login/cliente",
    loginFornecedor: "/api/auth/login/fornecedor",
    loginAdmin: "/api/auth/login/admin",
    logout: "/api/auth/logout",
    unauthorized: "/api/auth/unauthorized",
    forbidden: "/api/auth/forbidden",
  },

  // 👤 Endpoints de cliente
  cliente: {
    cadastrar: "/api/Clientes/me/cadastro/cliente", // ✅ Endpoint correto do Swagger
    perfil: "/api/Clientes/me", // 🔒 Precisa autenticação
    atualizarPerfil: "/api/Clientes/me", // PUT
    uploadComprovante: "/api/Clientes/me/endereco/comprovante",
  },

  // 🏭 Endpoints de fornecedor
  fornecedor: {
    cadastrar: "/api/Fornecedores", // ✅ Endpoint correto do Swagger
    perfil: "/api/Fornecedores/me", // 🔒 Precisa autenticação
    atualizarPerfil: "/api/Fornecedores/me", // PUT
  },

  // 📊 Endpoints de dashboard
  dashboard: {
    cliente: "/api/Dashboard/cliente", // 🔒 Precisa autenticação
    fornecedor: "/api/Dashboard/fornecedor", // 🔒 Precisa autenticação
    empresa: "/api/Dashboard/dashboard/empresa/stats", // ✅ Endpoint correto
    graficoCliente: "/api/Dashboard/dashboard/cliente/grafico",
    graficoFornecedor: "/api/Dashboard/dashboard/fornecedor/grafico",
  },

  // 📋 Endpoints de pedidos
  pedidos: {
    criar: "/api/Pedidos", // POST
    meus: "/api/Pedidos/me", // GET
  },

  // 🤝 Endpoints de contratos (fornecedores)
  contratos: {
    criar: "/api/fornecedores/me/contratos", // POST
    meus: "/api/fornecedores/me/contratos", // GET
    obter: "/api/fornecedores/me/contratos/{id}", // GET
  },

  // 👨‍💼 Endpoints de admin
  admin: {
    contratos: "/api/admin/contratos",
    contrato: "/api/admin/contratos/{id}",
    atualizarStatusContrato: "/api/admin/contratos/{id}/status", // PUT
    ofertas: "/api/admin/ofertasenergia",
    pedidos: "/api/admin/pedidos",
    pedido: "/api/admin/pedidos/{id}",
    atualizarStatusPedido: "/api/admin/pedidos/{id}/status", // PUT
    clientes: "/api/admin/AdminUsuarios/clientes",
    cliente: "/api/admin/AdminUsuarios/clientes/{id}",
    aprovarEndereco: "/api/admin/AdminUsuarios/clientes/{id}/endereco/aprovar", // PUT
    rejeitarEndereco: "/api/admin/AdminUsuarios/clientes/{id}/endereco/rejeitar", // PUT
    fornecedores: "/api/admin/AdminUsuarios/fornecedores",
    fornecedor: "/api/admin/AdminUsuarios/fornecedores/{id}",
  },

  // 🧪 Endpoints para teste
  swagger: "/swagger/index.html",
}
