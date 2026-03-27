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
// #if (includeAuth)
      override: {
        mutator: {
          path: './src/api/authenticated-fetch.ts',
          name: 'authenticatedFetch',
        },
      },
// #endif
    },
  },
})
