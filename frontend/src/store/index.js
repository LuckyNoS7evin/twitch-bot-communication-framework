import Vue from 'vue'
import Vuex from 'vuex'
import { oidcManager } from '@/plugins/oidc'
import axios from 'axios'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    user: null,
    userInformation: null
  },
  getters: {
    user (state) {
      return state.user
    },
    userInformation (state) {
      return state.userInformation
    }
  },
  mutations: {
    user (state, user) {
      state.user = user
    },
    userInformation (state, userInformation) {
      state.userInformation = userInformation
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
          return axios.get(`https://localhost:5001/user`, {
            headers: {
              'Authorization': `Bearer ${user.id_token}`
            }
          })
        })
        .then(userInfo => {
          context.commit('userInformation', userInfo.data)
          return true
        })
    },
    load (context) {
      oidcManager.getUser()
        .then(user => {
          context.commit('user', user)
          return axios.get(`https://localhost:5001/user`, {
            headers: {
              'Authorization': `Bearer ${user.id_token}`
            }
          })
        })
        .then(userInfo => {
          context.commit('userInformation', userInfo.data)
          return true
        })
    }
  },
  modules: {
  }
})
