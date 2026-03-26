import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/')({
  component: HomePage,
})

function HomePage() {
  return (
    <div>
      <h1 className="text-3xl font-bold tracking-tight">Company.ProjectName</h1>
      <p className="text-muted-foreground mt-2">
        Your full-stack app is ready. See the{' '}
        <a
          href="/scalar/v1"
          target="_blank"
          rel="noopener noreferrer"
          className="underline underline-offset-4"
        >
          API reference
        </a>
        .
      </p>
    </div>
  )
}
