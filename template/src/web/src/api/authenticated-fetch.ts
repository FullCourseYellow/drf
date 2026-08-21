// #if (includeAuth)
import { User } from 'oidc-client-ts'

function getUser(): User | null {
  const oidcStorage = sessionStorage.getItem(
    `oidc.user:${import.meta.env.VITE_OIDC_AUTHORITY}:${import.meta.env.VITE_OIDC_CLIENT_ID}`,
  )
  if (!oidcStorage) return null
  return User.fromStorageString(oidcStorage)
}
// #endif

export async function authenticatedFetch<T>(
  input: RequestInfo | URL,
  init?: RequestInit,
): Promise<T> {
  const headers = new Headers(init?.headers)
// #if (includeAuth)
  const user = getUser()
  if (user?.access_token) {
    headers.set('Authorization', `Bearer ${user.access_token}`)
  }
// #endif
  const response = await fetch(input, { ...init, headers })
  const body = [204, 205, 304].includes(response.status) ? null : await response.text()
  const data = body ? JSON.parse(body) : {}

  return { data, status: response.status, headers: response.headers } as T
}
