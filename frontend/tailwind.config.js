/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      // 🎨 Custom colors for transparent backgrounds and borders
      colors: {
        glass: {
          100: 'rgba(255, 255, 255, 0.1)',
          200: 'rgba(255, 255, 255, 0.2)',
          300: 'rgba(255, 255, 255, 0.3)',
          border: 'rgba(255, 255, 255, 0.2)',
        }
      },
      // 🌫️ Custom blur level (optional, but good for fine-tuning)
      backdropBlur: {
        xs: '2px',
      }
    },
  },
  plugins: [],
}