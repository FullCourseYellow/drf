import { createRootRoute, Link, Outlet } from '@tanstack/react-router'

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
