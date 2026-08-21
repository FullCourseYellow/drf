import { createFileRoute } from '@tanstack/react-router'
import { useEffect, useState } from 'react'
import { createColumnHelper } from '@tanstack/react-table'
import { DataTable, dataTableFeatures } from '@/components/data-table'
import { getWeatherForecasts } from '@/api/generated/weather-forecasts/weather-forecasts'
import type { WeatherForecast } from '@/api/generated/companyProjectNameApiV1.schemas'

const PAGE_SIZE = 10

const columnHelper = createColumnHelper<typeof dataTableFeatures, WeatherForecast>()

const columns = columnHelper.columns([
  columnHelper.accessor('id', { header: 'ID' }),
  columnHelper.accessor('date', { header: 'Date' }),
  columnHelper.accessor('temperatureC', { header: 'Temp (°C)' }),
  columnHelper.accessor('summary', {
    header: 'Summary',
    cell: ({ getValue }) => getValue() ?? '—',
  }),
])

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
        if (result.status === 200) {
          setData(result.data.data ?? [])
          setTotalCount(Number(result.data.count ?? 0))
        }
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
