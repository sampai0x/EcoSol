import axios from "axios"

// Configuração da URL base dependendo do ambiente
const getBaseURL = () => {
  if (import.meta.env.PROD) {
    return "https://localhost:7272" // HTTPS em produção
  }
  // Em desenvolvimento, usar o proxy do Vite
  return ""
}

const api = axios.create({
  baseURL: getBaseURL(),
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
  timeout: 10000,
})

// Interceptor para log de requisições (apenas em desenvolvimento)
if (import.meta.env.DEV) {
  api.interceptors.request.use(
    (config) => {
      console.log(`🚀 Fazendo requisição: ${config.method?.toUpperCase()} ${config.url}`)
      console.log("Dados:", config.data)
      return config
    },
    (error) => {
      console.error("❌ Erro na requisição:", error)
      return Promise.reject(error)
    },
  )
}

// Interceptor para tratamento global de erros
api.interceptors.response.use(
  (response) => {
    if (import.meta.env.DEV) {
      console.log(`✅ Resposta recebida: ${response.status} ${response.config.url}`)
    }
    return response
  },
  (error) => {
    if (import.meta.env.DEV) {
      console.error("❌ Erro na resposta:", error.response?.status, error.response?.data)
    }

    if (error.response) {
      if (error.response.status === 401) {
        // Não autenticado - redirecionar para login se não estiver na home
        if (window.location.pathname !== "/") {
          window.location.href = "/"
        }
      }
    }
    return Promise.reject(error)
  },
)

export default api
