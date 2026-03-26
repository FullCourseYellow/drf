import * as React from 'react'
import { createRootRoute, Link, Outlet } from '@tanstack/react-router'
// #if (includeAuth)
import { useAuth } from 'react-oidc-context'
// #endif

export const Route = createRootRoute({
  component: RootLayout,
  errorComponent: ({ error }) => (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-xl font-bold text-destructive">Something went wrong</h1>
      <p className="text-muted-foreground mt-2">{error.message}</p>
    </div>
  ),
})

function RootLayout() {
  return (
    <div className="min-h-screen bg-background">
      <nav className="border-b">
        <div className="container mx-auto px-4 h-14 flex items-center gap-6">
          <Link to="/" className="font-semibold text-foreground">
            Company.ProjectName
          </Link>
          <Link
            to="/weather-forecasts"
            className="text-sm text-muted-foreground hover:text-foreground transition-colors"
            activeProps={{ className: 'text-foreground font-medium' }}
          >
            Weather Forecasts
          </Link>
        </div>
      </nav>
      <main className="container mx-auto px-4 py-8">
        <Outlet />
      </main>
    </div>
  )
}

// #if (includeAuth)
/** Wrap any route component with this to require authentication. */
export function RequireAuth({ children }: { children: React.ReactNode }) {
  const auth = useAuth()
  if (auth.isLoading) return <p className="p-8 text-muted-foreground">Authenticating…</p>
  if (!auth.isAuthenticated) {
    auth.signinRedirect()
    return null
  }
  return <>{children}</>
}
// #endif
