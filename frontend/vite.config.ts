import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react-swc';

export default defineConfig({
  base: '/specula-pl/',
  plugins: [react()],
  server: {
    port: 5173,
    host: '127.0.0.1'
  },
  build: {
    target: 'esnext'
  }
});
