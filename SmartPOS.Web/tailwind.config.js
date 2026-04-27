/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary:    "#1E40AF",
        secondary:  "#3B82F6",
        accent:     "#F59E0B",
        background: "#F8FAFC",
        surface:    "#FFFFFF",
        surface2:   "#F1F5F9",
        border:     "#E2E8F0",
        text:       "#1E3A8A",
        muted:      "#64748B",
        success:    "#22C55E",
        warning:    "#F59E0B",
        danger:     "#EF4444",
        info:       "#3B82F6",
      },
      fontFamily: {
        sans: ["Fira Sans", "system-ui", "sans-serif"],
        mono: ["Fira Code", "monospace"],
      },
    },
  },
  plugins: [],
}