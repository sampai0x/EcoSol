import api from "./api"

export const homeService = {
  async getStats() {
    // Endpoint correto do Swagger: GET /api/Home/stats
    const response = await api.get("/api/Home/stats")
    return response.data
  },
}
