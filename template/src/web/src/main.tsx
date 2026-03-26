import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { RouterProvider, createRouter } from '@tanstack/react-router'
import { routeTree } from './routeTree.gen'
// #if (includeAuth)
import { AuthProvider } from 'react-oidc-context'
import { oidcConfig } from './config'
// #endif
import './index.css'

const router = createRouter({ routeTree })

declare module '@tanstack/react-router' {
  interface Register {
    router: typeof router
  }
}

createRoot(document.getElementById('root')!).render(
  <StrictMode>
// #if (includeAuth)
    <AuthProvider {...oidcConfig}>
      <RouterProvider router={router} />
    </AuthProvider>
// #else
    <RouterProvider router={router} />
// #endif
  </StrictMode>,
)
