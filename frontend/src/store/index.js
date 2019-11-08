import Vue from 'vue'
import Vuex from 'vuex'
import { oidcManager } from '@/plugins/oidc'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    user: null
  },
  getters: {
    user (state) {
      return state.user
    }
  },
  mutations: {
    user (state, user) {
      state.user = user
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
          return true
        })
    },
    load (context) {
      oidcManager.getUser()
        .then(user => {
          context.commit('user', user)
          return true
        })
    }
  },
  modules: {
  }
})
