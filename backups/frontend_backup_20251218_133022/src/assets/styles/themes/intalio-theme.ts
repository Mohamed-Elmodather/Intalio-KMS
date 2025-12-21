import { definePreset } from '@primeuix/themes'
import Aura from '@primeuix/themes/aura'

/**
 * Intalio KMS Custom PrimeVue Theme
 * Based on Aura preset with Intalio brand colors
 */
export const IntalioTheme = definePreset(Aura, {
  primitive: {
    // Intalio Teal (Primary)
    intalioTeal: {
      50: '#e6f7f4',
      100: '#b3e8df',
      200: '#80d9ca',
      300: '#4dcbb4',
      400: '#26c0a3',
      500: '#00ae8d', // Primary brand teal
      600: '#009a7d',
      700: '#008f75',
      800: '#007a64',
      900: '#005a4a'
    },
    // Intalio Gray (Secondary/Accent)
    intalioGray: {
      50: '#f5f5f5',
      100: '#e0e0e0',
      200: '#c0c0c0',
      300: '#a0a0a0',
      400: '#909090',
      500: '#808080', // Primary gray
      600: '#707070',
      700: '#606060',
      800: '#505050',
      900: '#404040'
    }
  },
  semantic: {
    primary: {
      50: '{intalioTeal.50}',
      100: '{intalioTeal.100}',
      200: '{intalioTeal.200}',
      300: '{intalioTeal.300}',
      400: '{intalioTeal.400}',
      500: '{intalioTeal.500}',
      600: '{intalioTeal.600}',
      700: '{intalioTeal.700}',
      800: '{intalioTeal.800}',
      900: '{intalioTeal.900}'
    },
    colorScheme: {
      light: {
        surface: {
          0: '#ffffff',
          50: '#FAFAFA',
          100: '#F5F5F5',
          200: '#EEEEEE',
          300: '#E0E0E0',
          400: '#BDBDBD',
          500: '#9E9E9E',
          600: '#757575',
          700: '#616161',
          800: '#424242',
          900: '#212121'
        },
        primary: {
          color: '{primary.500}',
          contrastColor: '#ffffff',
          hoverColor: '{primary.600}',
          activeColor: '{primary.700}'
        },
        highlight: {
          background: '{primary.50}',
          focusBackground: '{primary.100}',
          color: '{primary.700}',
          focusColor: '{primary.800}'
        }
      },
      dark: {
        surface: {
          0: '#121212',
          50: '#1E1E1E',
          100: '#2D2D2D',
          200: '#3D3D3D',
          300: '#4D4D4D',
          400: '#5D5D5D',
          500: '#6D6D6D',
          600: '#7D7D7D',
          700: '#8D8D8D',
          800: '#9D9D9D',
          900: '#ADADAD'
        },
        primary: {
          color: '{primary.400}',
          contrastColor: '#121212',
          hoverColor: '{primary.300}',
          activeColor: '{primary.200}'
        }
      }
    }
  },
  components: {
    button: {
      root: {
        borderRadius: '{border.radius.lg}',
        paddingX: '1.25rem',
        paddingY: '0.75rem'
      }
    },
    card: {
      root: {
        borderRadius: '{border.radius.xl}',
        shadow: '0 2px 8px rgba(0, 0, 0, 0.08)'
      }
    },
    inputtext: {
      root: {
        borderRadius: '{border.radius.md}'
      }
    },
    panel: {
      header: {
        borderRadius: '{border.radius.lg} {border.radius.lg} 0 0'
      }
    },
    datatable: {
      headerCell: {
        background: '{surface.50}'
      }
    },
    menu: {
      root: {
        borderRadius: '{border.radius.lg}'
      }
    },
    toast: {
      root: {
        borderRadius: '{border.radius.lg}'
      }
    }
  }
})
