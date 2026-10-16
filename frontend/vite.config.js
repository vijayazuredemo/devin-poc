import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// During development, requests to /api are forwarded to the .NET backend,
// so the browser sees a single origin and no CORS setup is needed.
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': 'http://localhost:5211',
    },
  },
})
