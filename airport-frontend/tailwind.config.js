/** @type {import('tailwindcss').Config} */
export default {
    content: ['./src/**/*.{html,js,svelte,ts}'],
    theme: {
        extend: {},
    },
    plugins: [require("daisyui")],
    daisyui: {
        themes: [
            "dark",
            {
                light: {
                    ...require("daisyui/src/theming/themes")["light"],
                    primary: "lightblue",
                    secondary: "teal",
                },
            },
        ],
    },
}

