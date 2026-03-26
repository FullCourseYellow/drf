import { createFileRoute } from '@tanstack/react-router'
import { useEffect, useState } from 'react'
import { type ColumnDef } from '@tanstack/react-table'
import { DataTable } from '@/components/data-table'
import { getWeatherForecasts } from '@/api/generated/weather-forecasts/weather-forecasts'

const PAGE_SIZE = 10

interface WeatherForecast {
  id: number
  date: string
  temperatureC: number
  summary: string | null
}

interface PagedResult {
  count: number
  data: WeatherForecast[]
}

const columns: ColumnDef<WeatherForecast>[] = [
  { accessorKey: 'id', header: 'ID' },
  { accessorKey: 'date', header: 'Date' },
  { accessorKey: 'temperatureC', header: 'Temp (°C)' },
  {
    accessorKey: 'summary',
    header: 'Summary',
    cell: ({ getValue }) => getValue<string>() ?? '—',
  },
]

export const Route = createFileRoute('/weather-forecasts')({
  component: WeatherForecastsPage,
})

function WeatherForecastsPage() {
  const [page, setPage] = useState(1)
  const [data, setData] = useState<WeatherForecast[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    setLoading(true)
    getWeatherForecasts({ Page: page, PageSize: PAGE_SIZE })
      .then((result) => {
        const paged = result.data as unknown as PagedResult
        setData(paged?.data ?? [])
        setTotalCount(paged?.count ?? 0)
      })
      .finally(() => setLoading(false))
  }, [page])

  return (
    <div>
      <h1 className="text-2xl font-bold tracking-tight mb-6">Weather Forecasts</h1>
      {loading ? (
        <p className="text-muted-foreground">Loading...</p>
      ) : (
        <DataTable
          columns={columns}
          data={data}
          totalCount={totalCount}
          page={page}
          pageSize={PAGE_SIZE}
          onPageChange={setPage}
        />
      )}
    </div>
  )
}
