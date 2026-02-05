# Intalio Knowledge Hub

A modern knowledge management system built with Vue 3, TypeScript, and Vite.

## Features

- **Content Management**: Articles, documents, events, and media management
- **AI Integration**: AI-powered summarization, translation, and content analysis
- **Collaboration**: Real-time collaboration, comments, and social sharing
- **Search**: Full-text search with filters and AI-enhanced results
- **Analytics**: Content analytics and user engagement tracking
- **Internationalization**: Multi-language support (English, Arabic)
- **Responsive Design**: Mobile-first design with RTL support

## Tech Stack

- **Framework**: Vue 3 with Composition API
- **Language**: TypeScript
- **Build Tool**: Vite
- **State Management**: Pinia
- **Routing**: Vue Router
- **Styling**: Tailwind CSS
- **Testing**: Vitest + Vue Test Utils
- **Internationalization**: vue-i18n

## Prerequisites

- Node.js 18.x or higher (20.x recommended)
- npm 9.x or higher

## Getting Started

### Installation

```bash
# Clone the repository
git clone https://github.com/your-org/intalio-knowledge-hub.git
cd intalio-knowledge-hub

# Install dependencies
npm install
```

### Development

```bash
# Start development server
npm run dev
```

The application will be available at `http://localhost:5173`.

### Building for Production

```bash
# Type check and build
npm run build

# Preview production build
npm run preview
```

## Available Scripts

| Script | Description |
|--------|-------------|
| `npm run dev` | Start development server |
| `npm run build` | Build for production |
| `npm run preview` | Preview production build |
| `npm run test` | Run tests in watch mode |
| `npm run test:run` | Run tests once |
| `npm run test:coverage` | Run tests with coverage |
| `npm run lint` | Lint and fix files |
| `npm run lint:check` | Check linting without fixing |
| `npm run format` | Format files with Prettier |
| `npm run format:check` | Check formatting |
| `npm run type-check` | TypeScript type checking |

## Project Structure

```
src/
├── api/                 # API client and endpoints
├── components/          # Vue components
│   ├── ai/             # AI-related components
│   ├── common/         # Reusable components
│   ├── layout/         # Layout components
│   └── unified/        # Unified design system
├── composables/         # Vue composables
├── i18n/               # Internationalization
│   └── locales/        # Translation files
├── mocks/              # MSW mock handlers
├── pages/              # Page components
├── router/             # Vue Router configuration
├── stores/             # Pinia stores
├── styles/             # Global styles
├── types/              # TypeScript types
└── utils/              # Utility functions
```

## Environment Variables

Create a `.env` file based on `.env.example`:

```env
# API Configuration
VITE_API_BASE_URL=http://localhost:5010/api
VITE_API_GATEWAY_URL=http://localhost:5010

# Feature Flags
VITE_ENABLE_MOCKS=false

# SignalR
VITE_SIGNALR_HUB_URL=http://localhost:5010/hubs

# App Configuration
VITE_APP_TITLE=Intalio Knowledge Hub
```

## Testing

```bash
# Run all tests
npm run test:run

# Run tests with coverage
npm run test:coverage

# Run tests in watch mode
npm run test
```

Test files are located in `src/__tests__/` following the same structure as source files.

## Code Quality

### Linting

ESLint is configured with TypeScript and Vue support:

```bash
npm run lint:check  # Check for issues
npm run lint        # Fix issues automatically
```

### Formatting

Prettier is used for code formatting:

```bash
npm run format:check  # Check formatting
npm run format        # Fix formatting
```

## Security

- **XSS Protection**: All user-generated HTML is sanitized using DOMPurify
- **Error Handling**: Global error boundary and error logging
- **Input Validation**: Form validation on all user inputs
- **Dependency Auditing**: Regular `npm audit` checks

## Deployment

### GitHub Actions

CI/CD is configured with GitHub Actions:

- **CI Pipeline**: Runs on all pull requests
  - Linting
  - Type checking
  - Tests with coverage
  - Build verification
  - Security audit

- **Deploy Pipeline**: Runs on merge to main/master
  - Full test suite
  - Production build
  - Deployment to GitHub Pages

### Manual Deployment

```bash
# Build the application
npm run build

# The dist/ folder contains the production build
# Deploy to your preferred hosting service
```

### Docker (Optional)

```dockerfile
FROM node:20-alpine as build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is proprietary software. All rights reserved.
