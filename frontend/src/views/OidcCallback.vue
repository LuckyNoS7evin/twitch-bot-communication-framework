<template>
  <v-overlay :value="true">
    <v-progress-circular indeterminate size="64"></v-progress-circular>
  </v-overlay>
</template>

<script>
import { mapActions } from 'vuex'

export default {
  name: 'home',
  methods: {
    ...mapActions([
      'signinCallback'
    ])
  },
  mounted () {
    this.signinCallback()
      .then(result => {
        if (result) {
          this.$router.push({ name: 'settings' })
        }
      })
  }
}
</script>
