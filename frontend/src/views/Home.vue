<template>
  <v-container fluid >
    <v-row>
      <v-col cols="12">
        <v-card>
          <v-toolbar>
            <v-toolbar-title>Welcome</v-toolbar-title>
            <v-spacer></v-spacer>
          </v-toolbar>
          <v-container>
            <p>Welcome to the Bot Communication Framework (BCF), a very simple service to allow you as a developer to
               connect your twitch bot to others. Send Messages to individual twitch channels, and if their bot is using
               the BCF they will get your message(s).</p>
          </v-container>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>

export default {
  name: 'home'
}
</script>
