import axios from 'axios'

const API_BASE = 'http://localhost:5069/api'

const api = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' }
})

// Attach JWT token to every request
api.interceptors.request.use(config => {
  const token = localStorage.getItem('smartpos_token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

// Handle 401 - redirect to login
api.interceptors.response.use(
  res => res,
  err => {
    if (err.response?.status === 401) {
      localStorage.removeItem('smartpos_token')
      window.location.href = '/login'
    }
    return Promise.reject(err)
  }
)

export default api