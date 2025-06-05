import api from "./api"

export const clienteService = {
  async cadastrar(dadosCliente) {
    // Endpoint correto: POST /api/Clientes/me/cadastro/cliente
    const response = await api.post("/api/Clientes/me/cadastro/cliente", {
      nome: dadosCliente.nome,
      email: dadosCliente.email,
      senha: dadosCliente.senha,
      enderecoCompleto: dadosCliente.endereco,
      cpfCnpj: dadosCliente.cpfCnpj,
    })

    return response.data
  },

  async uploadComprovante(arquivo) {
    const formData = new FormData()
    formData.append("arquivo", arquivo)

    // Endpoint correto: POST /api/Clientes/me/endereco/comprovante
    const response = await api.post("/api/Clientes/me/endereco/comprovante", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    })

    return response.data
  },

  async getPerfil() {
    // Endpoint correto: GET /api/Clientes/me
    const response = await api.get("/api/Clientes/me")
    return response.data
  },

  async atualizarPerfil(dados) {
    // Endpoint correto: PUT /api/Clientes/me
    const response = await api.put("/api/Clientes/me", dados)
    return response.data
  },

  async getDashboard() {
    // Endpoint correto: GET /api/Dashboard/cliente
    const response = await api.get("/api/Dashboard/cliente")
    return response.data
  },

  async getGrafico() {
    // Endpoint correto: GET /api/Dashboard/dashboard/cliente/grafico
    const response = await api.get("/api/Dashboard/dashboard/cliente/grafico")
    return response.data
  },

  async criarPedido(dadosPedido) {
    // Endpoint correto: POST /api/Pedidos
    const response = await api.post("/api/Pedidos", dadosPedido)
    return response.data
  },

  async getMeusPedidos() {
    // Endpoint correto: GET /api/Pedidos/me
    const response = await api.get("/api/Pedidos/me")
    return response.data
  },

  // Método para atualizar status de pedido (se for implementado no backend)
  async atualizarStatusPedido(id, status) {
    // Este endpoint não existe no Swagger atual, mas seria algo como:
    // PUT /api/Pedidos/{id}/status
    const response = await api.put(`/api/Pedidos/${id}/status`, { status })
    return response.data
  },
}
