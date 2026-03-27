// #if (includeAuth)
import { User } from 'oidc-client-ts'

function getUser(): User | null {
  const oidcStorage = sessionStorage.getItem(
    `oidc.user:${import.meta.env.VITE_OIDC_AUTHORITY}:${import.meta.env.VITE_OIDC_CLIENT_ID}`,
  )
  if (!oidcStorage) return null
  return User.fromStorageString(oidcStorage)
}

export async function authenticatedFetch(
  input: RequestInfo | URL,
  init?: RequestInit,
): Promise<Response> {
  const user = getUser()
  const headers = new Headers(init?.headers)
  if (user?.access_token) {
    headers.set('Authorization', `Bearer ${user.access_token}`)
  }
  return fetch(input, { ...init, headers })
}
// #endif
