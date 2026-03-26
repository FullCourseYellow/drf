import { createFileRoute } from '@tanstack/react-router'

// Full implementation added in Task 9 (Orval + DataTable)
export const Route = createFileRoute('/weather-forecasts')({
  component: () => <div>Weather Forecasts</div>,
})
