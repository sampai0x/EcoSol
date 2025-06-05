import api from "./api"

// Cache para evitar múltiplas requisições simultâneas
let currentUserCache = null
let currentUserPromise = null

export const authService = {
  async loginCliente(email, senha) {
    // Endpoint correto do Swagger: POST /api/auth/login/cliente
    const response = await api.post("/api/auth/login/cliente", { email, senha })
    // Limpar cache após login
    currentUserCache = null
    currentUserPromise = null
    return response.data
  },

  async loginFornecedor(email, senha) {
    // Endpoint correto do Swagger: POST /api/auth/login/fornecedor
    const response = await api.post("/api/auth/login/fornecedor", { email, senha })
    // Limpar cache após login
    currentUserCache = null
    currentUserPromise = null
    return response.data
  },

  async loginAdmin(email, senha) {
    // Endpoint correto do Swagger: POST /api/auth/login/admin
    const response = await api.post("/api/auth/login/admin", { email, senha })
    // Limpar cache após login
    currentUserCache = null
    currentUserPromise = null
    return response.data
  },

  // Login sem salvar no localStorage - apenas retorna os dados
  async login(email, senha) {
    console.log("Tentando login para:", email)

    // Tenta todos os tipos de login em sequência
    try {
      console.log("Tentando login como cliente...")
      const response = await this.loginCliente(email, senha)
      console.log("Login como cliente bem-sucedido:", response)
      const user = { ...response, tipo: "Cliente" }
      // Cachear o usuário após login bem-sucedido
      currentUserCache = user
      return user
    } catch (error) {
      console.log("Login como cliente falhou:", error.message)

      try {
        console.log("Tentando login como fornecedor...")
        const response = await this.loginFornecedor(email, senha)
        console.log("Login como fornecedor bem-sucedido:", response)
        const user = { ...response, tipo: "Fornecedor" }
        // Cachear o usuário após login bem-sucedido
        currentUserCache = user
        return user
      } catch (error) {
        console.log("Login como fornecedor falhou:", error.message)

        try {
          console.log("Tentando login como admin...")
          const response = await this.loginAdmin(email, senha)
          console.log("Login como admin bem-sucedido:", response)
          const user = { ...response, tipo: "Empresa" }
          // Cachear o usuário após login bem-sucedido
          currentUserCache = user
          return user
        } catch (error) {
          console.log("Login como admin falhou:", error.message)
          throw new Error("Credenciais inválidas")
        }
      }
    }
  },

  // Logout apenas no backend
  async logout() {
    console.log("Fazendo logout...")

    try {
      // Endpoint correto do Swagger: POST /api/auth/logout
      const response = await api.post("/api/auth/logout")
      console.log("Logout no backend bem-sucedido")
      // Limpar cache após logout
      currentUserCache = null
      currentUserPromise = null
      return response.data
    } catch (error) {
      console.warn("Erro no logout do backend:", error.message)
      // Limpar cache mesmo com erro
      currentUserCache = null
      currentUserPromise = null
      // Relança o erro para que o componente saiba que houve problema
      throw error
    }
  },

  // Verifica autenticação sempre pelo backend com cache
  async getCurrentUser() {
    console.log("🔍 Verificando usuário atual no backend...")

    // Se já temos um cache válido, retorna imediatamente
    if (currentUserCache) {
      console.log("✅ Retornando usuário do cache:", currentUserCache)
      return currentUserCache
    }

    // Se já há uma requisição em andamento, aguarda ela
    if (currentUserPromise) {
      console.log("⏳ Aguardando requisição em andamento...")
      return await currentUserPromise
    }

    // Cria uma nova promise para evitar múltiplas requisições
    currentUserPromise = this._fetchCurrentUser()

    try {
      const user = await currentUserPromise
      currentUserCache = user // Cachear o resultado
      return user
    } catch (error) {
      currentUserCache = null
      throw error
    } finally {
      currentUserPromise = null
    }
  },

  async _fetchCurrentUser() {
    try {
      // Tenta verificar como cliente primeiro
      try {
        const response = await api.get("/api/Clientes/me")
        console.log("✅ Usuário é cliente:", response.data)
        return { ...response.data, tipo: "Cliente" }
      } catch (clienteError) {
        console.log("Não é cliente:", clienteError.response?.status)
      }

      // Tenta verificar como fornecedor
      try {
        const response = await api.get("/api/Fornecedores/me")
        console.log("✅ Usuário é fornecedor:", response.data)
        return { ...response.data, tipo: "Fornecedor" }
      } catch (fornecedorError) {
        console.log("Não é fornecedor:", fornecedorError.response?.status)
      }

      // Tenta verificar como admin/empresa
      try {
        const response = await api.get("/api/Dashboard/dashboard/empresa/stats")
        console.log("✅ Usuário é admin/empresa")
        // Para admin, retornamos dados básicos já que não temos endpoint específico
        return {
          nome: "Administrador",
          email: "admin@ecosol.com",
          tipo: "Empresa",
        }
      } catch (adminError) {
        console.log("Não é admin:", adminError.response?.status)
      }

      // Se chegou até aqui, não está autenticado
      console.log("❌ Usuário não autenticado")
      return null
    } catch (error) {
      console.error("❌ Erro na verificação de autenticação:", error)
      return null
    }
  },

  // Método para verificar se está logado (mais leve)
  async isAuthenticated() {
    try {
      const user = await this.getCurrentUser()
      return !!user
    } catch (error) {
      return false
    }
  },

  // Método para limpar cache (útil para forçar nova verificação)
  clearCache() {
    currentUserCache = null
    currentUserPromise = null
  },
}
