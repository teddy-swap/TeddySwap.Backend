/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./Pages/**/*.{html,js,cshtml,razor}", "./Shared/**/*.{html,js,cshtml,razor}"],
  theme: {
    screens: {
      "xs": "420px",
      "sm": "640px",
      "md": "768px",
      "lg": "1024px",
      "xl": "1280px",
      "2xl": "1536px"
    },
    extend: {},
  },
  plugins: [],
}
