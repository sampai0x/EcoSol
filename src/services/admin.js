import api from "./api"

export const adminService = {
  // Endpoints de contratos (AdminContratosController)
  async getContratos() {
    const response = await api.get("/api/admin/contratos")
    return response.data
  },

  async getContrato(id) {
    const response = await api.get(`/api/admin/contratos/${id}`)
    return response.data
  },

  async atualizarStatusContrato(id, novoStatus) {
    const response = await api.put(`/api/admin/contratos/${id}/status`, {
      novoStatus: novoStatus,
    })
    return response.data
  },

  // Endpoints de ofertas de energia (AdminOfertasController)
  async getOfertasPendentes() {
    const response = await api.get("/api/admin/ofertasenergia")
    return response.data
  },

  // Endpoints de pedidos (AdminPedidosController)
  async getPedidos() {
    const response = await api.get("/api/admin/pedidos")
    return response.data
  },

  async getPedido(id) {
    const response = await api.get(`/api/admin/pedidos/${id}`)
    return response.data
  },

  async atualizarStatusPedido(id, novoStatus) {
    const response = await api.put(`/api/admin/pedidos/${id}/status`, {
      novoStatus: novoStatus,
    })
    return response.data
  },

  // Endpoints de usuários (AdminUsuariosController)
  async getClientes() {
    const response = await api.get("/api/admin/AdminUsuarios/clientes")
    return response.data
  },

  async getCliente(id) {
    const response = await api.get(`/api/admin/AdminUsuarios/clientes/${id}`)
    return response.data
  },

  async getFornecedores() {
    const response = await api.get("/api/admin/AdminUsuarios/fornecedores")
    return response.data
  },

  async getFornecedor(id) {
    const response = await api.get(`/api/admin/AdminUsuarios/fornecedores/${id}`)
    return response.data
  },

  async aprovarEndereco(clienteId) {
    console.log("✅ Aprovando endereço do cliente:", clienteId)

    try {
      // Tentar aprovar via backend primeiro
      const response = await api.put(`/api/admin/AdminUsuarios/clientes/${clienteId}/endereco/aprovar`)
      console.log("✅ Endereço aprovado via backend")
      return response.data
    } catch (error) {
      console.warn("⚠️ Erro no backend, atualizando localStorage:", error)

      // Fallback para localStorage
      const usuarios = JSON.parse(localStorage.getItem("usuarios") || "[]")
      const clienteIndex = usuarios.findIndex((u) => u.id === clienteId || usuarios.indexOf(u) + 1 === clienteId)

      if (clienteIndex !== -1) {
        const cliente = usuarios[clienteIndex]

        // Atualizar status de aprovação
        cliente.statusAprovacaoComprovante = 2

        // Marcar todos os endereços como validados
        if (cliente.enderecos) {
          cliente.enderecos.forEach((endereco) => {
            endereco.validado = true
            endereco.statusAprovacaoComprovante = 2
          })
        }

        // Salvar no localStorage
        localStorage.setItem("usuarios", JSON.stringify(usuarios))

        // Atualizar usuário logado se for o mesmo
        const usuarioLogado = JSON.parse(localStorage.getItem("usuarioLogado"))
        if (usuarioLogado && (usuarioLogado.id === clienteId || usuarioLogado.email === cliente.email)) {
          localStorage.setItem("usuarioLogado", JSON.stringify(cliente))
        }

        console.log("✅ Endereço aprovado via localStorage")
        return { success: true }
      } else {
        throw new Error("Cliente não encontrado")
      }
    }
  },

  async rejeitarEndereco(clienteId, motivo) {
    const response = await api.put(`/api/admin/AdminUsuarios/clientes/${clienteId}/endereco/rejeitar`, {
      motivo: motivo,
    })
    return response.data
  },

  // Dashboard da empresa (DashboardController)
  async getEmpresaStats() {
    const response = await api.get("/api/Dashboard/dashboard/empresa/stats")
    return response.data
  },
}
