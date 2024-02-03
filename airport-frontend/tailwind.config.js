/** @type {import('tailwindcss').Config} */
export default {
    content: ['./src/**/*.{html,js,svelte,ts}'],
    theme: {
        extend: {},
    },
    plugins: [require("daisyui")],
    daisyui: {
        themes: [
            {
                corporate: {
                    ...require("daisyui/src/theming/themes")["corporate"],
                    primary: '#3B82F6',
                    'primary-content': '#ffffff',
                    'accent-content': '#ffffff',
                    'success-content': '#ffffff',
                    'warning-content': '#ffffff',
                    'error-content': '#ffffff'
                },
            },
            {
                business: {
                    ...require("daisyui/src/theming/themes")["business"],
                    primary: '#3B82F6',
                    'primary-content': '#ffffff',
                    'accent-content': '#ffffff',
                    'success-content': '#ffffff',
                    'warning-content': '#ffffff'
                },
            },
        ],
    },
}

