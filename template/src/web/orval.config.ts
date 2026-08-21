import { defineConfig } from 'orval'

export default defineConfig({
  api: {
    input: {
      target: './openapi.json',
    },
    output: {
      mode: 'tags-split',
      target: 'src/api/generated',
      client: 'fetch',
      tsconfig: './tsconfig.app.json',
      override: {
        mutator: {
          path: './src/api/authenticated-fetch.ts',
          name: 'authenticatedFetch',
        },
      },
    },
  },
})
