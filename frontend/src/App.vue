<template>
  <v-app>
    <v-app-bar
      app
      color="primary"
      dark
    >
      <router-link  to="/">Home</router-link>
      <router-link v-if="user !== null" to="/settings">Settings</router-link>
      <v-spacer></v-spacer>
      <v-btn
        text
        @click="login"
        v-if="user === null"
      >
        <span class="mr-2">Login</span>
        <font-awesome-icon :icon="['fab','twitch']" />
      </v-btn>
      <v-btn
        text
        @click="doLogout"
        v-else
      >
        <span class="mr-2">{{user.profile.preferred_username}}</span>
      </v-btn>
    </v-app-bar>

    <v-content>
      <router-view />
    </v-content>
  </v-app>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'

export default {
  name: 'App',
  data: () => ({
    //
  }),
  computed: {
    ...mapGetters([
      'user'
    ])
  },
  methods: {
    ...mapActions([
      'login',
      'logout',
      'load'
    ]),
    doLogout () {
      this.logout()
        .then(() => {
          this.$router.push({ name: 'home' })
        })
    }
  },
  mounted () {
    this.load()
  }
}
</script>
