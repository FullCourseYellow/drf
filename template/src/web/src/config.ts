// #if (includeAuth)
export const oidcConfig = {
  authority: import.meta.env.VITE_OIDC_AUTHORITY as string,
  client_id: import.meta.env.VITE_OIDC_CLIENT_ID as string,
  redirect_uri: import.meta.env.VITE_OIDC_REDIRECT_URI ?? window.location.origin,
}
// #endif
