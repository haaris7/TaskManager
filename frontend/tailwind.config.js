/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        // Bright glass theme colors
        glass: {
          50: 'rgba(255, 255, 255, 0.4)',
          100: 'rgba(255, 255, 255, 0.6)',
          200: 'rgba(255, 255, 255, 0.75)',
          300: 'rgba(255, 255, 255, 0.9)',
          border: 'rgba(255, 255, 255, 0.8)',
        },
        sky: {
          50: '#f0f9ff',
          100: '#e0f2fe',
          400: '#38bdf8',
          500: '#0ea5e9',
          600: '#0284c7',
        },
        teal: {
          400: '#2dd4bf',
          500: '#14b8a6',
        }
      },
      boxShadow: {
        'glass': '0 8px 32px 0 rgba(31, 38, 135, 0.1)',
        'glow-green': '0 0 10px rgba(52, 211, 153, 0.7)',
        'glow-yellow': '0 0 10px rgba(251, 191, 36, 0.7)',
        'glow-red': '0 0 10px rgba(244, 63, 94, 0.7)',
        'glow-blue': '0 0 10px rgba(56, 189, 248, 0.7)',
      }
    },
  },
  plugins: [],
}