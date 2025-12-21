import { definePreset } from '@primeuix/themes'
import Aura from '@primeuix/themes/aura'

/**
 * AFC Asian Cup 2027 Custom PrimeVue Theme
 * Based on Aura preset with AFC27 brand colors
 */
export const AFC27Theme = definePreset(Aura, {
  primitive: {
    // AFC27 Brand Colors - Green (Primary)
    afc27Green: {
      50: '#E8F5E9',
      100: '#C8E6C9',
      200: '#A5D6A7',
      300: '#81C784',
      400: '#66BB6A',
      500: '#2E7D32', // Primary brand green
      600: '#388E3C',
      700: '#2E7D32',
      800: '#1B5E20',
      900: '#0D3D15'
    },
    // AFC27 Gold (Accent)
    afc27Gold: {
      50: '#FFF8E1',
      100: '#FFECB3',
      200: '#FFE082',
      300: '#FFD54F',
      400: '#FFCA28',
      500: '#D4AF37', // Primary gold accent
      600: '#FFB300',
      700: '#FFA000',
      800: '#FF8F00',
      900: '#FF6F00'
    },
    // Saudi Arabia colors
    saudiGreen: {
      500: '#006C35'
    }
  },
  semantic: {
    primary: {
      50: '{afc27Green.50}',
      100: '{afc27Green.100}',
      200: '{afc27Green.200}',
      300: '{afc27Green.300}',
      400: '{afc27Green.400}',
      500: '{afc27Green.500}',
      600: '{afc27Green.600}',
      700: '{afc27Green.700}',
      800: '{afc27Green.800}',
      900: '{afc27Green.900}'
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
