/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,jsx}'],
  theme: {
    extend: {
      colors: {
        navy: {
          DEFAULT: '#1B2A4A',
          50: '#EEF1F7',
          100: '#D7DEEC',
          600: '#22335A',
          700: '#1B2A4A',
          800: '#141F38',
          900: '#0E1526',
        },
        blush: {
          50: '#FDF3F3',
          100: '#FBE6E6',
          200: '#F6CFCF',
          400: '#E8949B',
          500: '#DE7A85',
          600: '#C85D6B',
        },
        cream: '#FBF7F4',
        sand: '#F6EEE7',
      },
      fontFamily: {
        display: ['"Cormorant Garamond"', '"Noto Naskh Arabic"', 'serif'],
        body: ['"Tajawal"', '"Noto Kufi Arabic"', 'sans-serif'],
      },
      boxShadow: {
        card: '0 4px 18px -6px rgba(27, 42, 74, 0.12)',
      },
      borderRadius: {
        xl2: '1.25rem',
      },
    },
  },
  plugins: [],
}
