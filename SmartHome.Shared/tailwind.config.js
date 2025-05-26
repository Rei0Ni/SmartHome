module.exports = {
    content: [
        './**/*.razor',          // Files within SmartHome.Shared itself
        '../SmartHome.Web/**/*.razor', // Files in SmartHome.Web
        '../SmartHome.App/**/*.razor',           // Files in SmartHome.App
    ],
    darkMode: 'class',
    theme: {
        extend: {
            colors: {
                // Map Tailwind's semantic colors to your CSS variables
                primary: 'var(--primary)',
                'on-primary': 'var(--on-primary)',
                secondary: 'var(--secondary)',
                'on-secondary': 'var(--on-secondary)',
                background: 'var(--background)', // For elements that should use main background
                surface: 'var(--surface)',       // For card-like elements
                // 'surface-variant' is tricky if it's a gradient.
                // Tailwind's bg-* utilities expect a color.
                // You'd use `bg-[var(--surface-variant)]` in HTML for gradients.
                // If you want a solid color for surface-variant in Tailwind, ensure the CSS var is a solid color.
                'on-surface': 'var(--on-surface)',
                error: 'var(--error)',
                'on-error': 'var(--on-error)',
                body: 'var(--body)',             // For the main body background

                // Device-specific colors
                'fan-card-bg-on': 'var(--fan-card-bg-on)',
                'fan-card-bg-off': 'var(--fan-card-bg-off)',
                'fan-indicator-on': 'var(--fan-indicator-on)',
                'fan-indicator-off': 'var(--fan-indicator-off)',
                // Add other device colors (lamp, ipcamera) similarly
                'lamp-card-bg-on': 'var(--lamp-card-bg-on)',
                'lamp-card-bg-off': 'var(--lamp-card-bg-off)',
                'lamp-indicator-on': 'var(--lamp-indicator-on)',
                'lamp-indicator-off': 'var(--lamp-indicator-off)',

                'ipcamera-card-bg-on': 'var(--ipcamera-card-bg-on)',
                'ipcamera-card-bg-off': 'var(--ipcamera-card-bg-off)',
            },
            // For properties like stroke, if you use Tailwind utilities for them
            stroke: {
                'fan-on': 'var(--fan-on-stroke)',
                'fan-off': 'var(--fan-off-stroke)', // This will resolve to var(--fan-indicator-on)
            },
            // For slider thumb, it's usually controlled by accent-color or specific input styling
            // If you use `accent-color` CSS property:
            // accentColor: {
            //   'fan-slider': 'var(--fan-slider-thumb-color)',
            // }
        },
    },
    plugins: [],
    safelist: ['bg-surface', 'text-on-surface']
}