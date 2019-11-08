
import oidc from 'oidc-client'

export const oidcManager = new oidc.UserManager({
  authority: 'https://id.twitch.tv/oauth2',
  client_id: process.env.VUE_APP_CLIENT_ID,
  redirect_uri: window.location.origin + process.env.VUE_APP_REDIRECT_URI,
  popup_redirect_uri: window.location.origin + process.env.VUE_APP_POPUP_REDIRECT_URI,
  response_type: 'token id_token',
  scope: 'openid',
  post_logout_redirect_uri: window.location.origin,
  silentRedirectUri: window.location.origin + process.env.VUE_APP_SILENT_REDIRECT_URI,
  automaticSilentRenew: true,
  loadUserInfo: true,
  prompt: 'login'

  // scope: 'openid profile address roles identityserver4api country subscriptionlevel offline_access',
  // accessTokenExpiringNotificationTime: 10,
})
