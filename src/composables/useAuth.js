"use client"

import { ref, computed } from "vue"
import { authService } from "@/services/auth"

const user = ref(null)
const loading = ref(false)

export function useAuth() {
  const isLoggedIn = computed(() => !!user.value)

  const login = async (email, senha) => {
    loading.value = true
    try {
      const response = await authService.login(email, senha)
      user.value = response
      return response
    } catch (error) {
      throw error
    } finally {
      loading.value = false
    }
  }

  const logout = async () => {
    loading.value = true
    try {
      await authService.logout()
      user.value = null
    } catch (error) {
      console.error("Erro ao fazer logout:", error)
      user.value = null
    } finally {
      loading.value = false
    }
  }

  const checkAuth = async () => {
    try {
      const response = await authService.getCurrentUser()
      if (response) {
        user.value = response.usuario
      }
    } catch (error) {
      user.value = null
    }
  }

  return {
    user: computed(() => user.value),
    isLoggedIn,
    loading: computed(() => loading.value),
    login,
    logout,
    checkAuth,
  }
}
