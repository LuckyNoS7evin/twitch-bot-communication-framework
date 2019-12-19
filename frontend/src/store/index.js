import Vue from 'vue'
import Vuex from 'vuex'
import { oidcManager } from '@/plugins/oidc'
import axios from 'axios'
import jwt from 'jsonwebtoken'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    swaggerUrl: process.env.VUE_APP_SWAGGER_TEST_ENDPOINT,
    apiUrl: process.env.VUE_APP_API_ENDPOINT,
    apiJwt: '',
    user: null,
    userInformation: null
  },
  getters: {
    user (state) {
      return state.user
    },
    userInformation (state) {
      return state.userInformation
    },
    apiJwt (state) {
      return state.apiJwt
    }
  },
  mutations: {
    user (state, user) {
      state.user = user
    },
    userInformation (state, userInformation) {
      state.userInformation = userInformation
    },
    apiJwt (state, apiJwt) {
      state.apiJwt = apiJwt
    }
  },
  actions: {
    login (context) {
      oidcManager.signinRedirect()
    },
    logout (context) {
      return oidcManager.removeUser().then(() => {
        context.commit('user', null)
      })
    },
    signinCallback (context) {
      return oidcManager.signinRedirectCallback()
        .then(() => oidcManager.getUser())
        .then(user => {
          context.commit('user', user)
          return axios.get(`${context.state.apiUrl}user`, {
            headers: {
              'Authorization': `Bearer ${user.id_token}`
            }
          })
        })
        .then(userInfo => {
          context.commit('userInformation', userInfo.data)
          context.commit('apiJwt', jwt.sign({}, Buffer.from(userInfo.data.secret, 'base64'), {
            keyid: userInfo.data.clientId,
            audience: userInfo.data.clientId,
            issuer: 'https://botcommunicationframework.com',
            subject: userInfo.data.userId,
            expiresIn: '30m'
          }))
          return true
        })
    },
    load (context) {
      oidcManager.getUser()
        .then(user => {
          if (user === null) return false
          context.commit('user', user)
          return axios.get(`${context.state.apiUrl}user`, {
            headers: {
              'Authorization': `Bearer ${user.id_token}`
            }
          })
        })
        .then(userInfo => {
          if (!userInfo) return true
          context.commit('userInformation', userInfo.data)
          context.commit('apiJwt', jwt.sign({}, Buffer.from(userInfo.data.secret, 'base64'), {
            keyid: userInfo.data.clientId,
            audience: userInfo.data.clientId,
            issuer: 'https://botcommunicationframework.com',
            subject: userInfo.data.userId,
            expiresIn: '30m'
          }))
          return true
        })
    }
  },
  modules: {
  }
})
