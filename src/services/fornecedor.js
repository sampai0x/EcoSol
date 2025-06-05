import api from "./api"

export const fornecedorService = {
  async cadastrar(dadosFornecedor) {
    // Endpoint correto do Swagger: POST /api/Fornecedores
    const response = await api.post("/api/Fornecedores", {
      nome: dadosFornecedor.nome,
      email: dadosFornecedor.email,
      senha: dadosFornecedor.senha,
      telefone: dadosFornecedor.telefone || "",
      endereco: dadosFornecedor.endereco,
      cidade: dadosFornecedor.cidade || "",
      estado: dadosFornecedor.estado || "",
      cpfCnpj: dadosFornecedor.cpfCnpj,
    })

    return response.data
  },

  async getPerfil() {
    // Endpoint correto do Swagger: GET /api/Fornecedores/me
    const response = await api.get("/api/Fornecedores/me")
    return response.data
  },

  async atualizarPerfil(dados) {
    // Endpoint correto do Swagger: PUT /api/Fornecedores/me
    const response = await api.put("/api/Fornecedores/me", {
      nome: dados.nome,
      email: dados.email,
      senha: dados.senha || "",
      telefone: dados.telefone || "",
      endereco: dados.endereco,
      cidade: dados.cidade || "",
      estado: dados.estado || "",
      cpfCnpj: dados.cpfCnpj
    })
    return response.data
  },

  async getDashboard() {
    // Endpoint correto do Swagger: GET /api/Dashboard/fornecedor
    const response = await api.get("/api/Dashboard/fornecedor")
    return response.data
  },

  async getGrafico() {
    // Endpoint correto do Swagger: GET /api/Dashboard/dashboard/fornecedor/grafico
    const response = await api.get("/api/Dashboard/dashboard/fornecedor/grafico")
    return response.data
  },

  async criarOferta(dadosOferta) {
    // Endpoint correto do Swagger: POST /api/fornecedores/me/contratos
    const response = await api.post("/api/fornecedores/me/contratos", {
      quantidadeEnergia: dadosOferta.quantidade,
      valorContrato: dadosOferta.preco,
    })

    return response.data
  },

  async getMinhasOfertas() {
    // Endpoint correto do Swagger: GET /api/fornecedores/me/contratos
    const response = await api.get("/api/fornecedores/me/contratos")
    return response.data
  },

  async getOferta(id) {
    // Endpoint correto do Swagger: GET /api/fornecedores/me/contratos/{id}
    const response = await api.get(`/api/fornecedores/me/contratos/${id}`)
    return response.data
  },

  async removerOferta(id) {
    // Atualiza o status do contrato para CANCELADO (2)
    const response = await api.put(`/api/fornecedores/me/contratos/${id}/status`, {
      novoStatus: 2 // StatusContrato.CANCELADO
    })
    return response.data
  },
}
