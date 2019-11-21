<template>
  <v-container fluid>
    <v-row>
      <v-col cols="12">
        <v-card>
          <v-toolbar>
            <v-toolbar-title>API Access</v-toolbar-title>
            <v-spacer></v-spacer>
          </v-toolbar>
          <v-container>
            <v-row>
              <v-col cols="4">
                Client Id
              </v-col>
              <v-col cols="8">
                {{userInformation.clientId}}
              </v-col>
            </v-row>
            <v-row>
              <v-col cols="4">
                Secret
              </v-col>
              <v-col cols="6">
                <v-text-field
                  :value="userInformation.secret"
                  type="password"
                  single-line
                  disabled
                ></v-text-field>
              </v-col>
              <v-col cols="2">
                <v-btn block color="primary"
                  v-clipboard:copy="userInformation.secret">copy</v-btn>
              </v-col>
            </v-row>
          </v-container>
        </v-card>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <v-card>
          <v-toolbar>
            <v-toolbar-title>Link accounts</v-toolbar-title>
            <v-spacer></v-spacer>
          </v-toolbar>
          <v-container>
            <v-row>
              <v-col cols="12">
                <v-text-field v-model="username" label="Twitch Account to link to"></v-text-field>
              </v-col>
            </v-row>
          </v-container>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn @click="linkChannel()">
              Link to channel
            </v-btn>
            <v-btn @click="linkBot()">
              Link to bot
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="6" v-for="link in links" :key="link.channelId + link.botId">
        <v-card>
          <v-toolbar :color="linkStatusColour(link)">
            <v-toolbar-title>{{linkStatus(link)}}</v-toolbar-title>
          </v-toolbar>
          <v-container>
            <v-row>
              <v-col cols="6">
                Channel
              </v-col>
              <v-col cols="6">
                {{link.channelDisplayName}}
              </v-col>
            </v-row>
            <v-row>
              <v-col cols="6">
                Bot
              </v-col>
              <v-col cols="6">
                {{link.botDisplayName}}
              </v-col>
            </v-row>
          </v-container>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn @click="decline(link)" v-if="showButtons(link)">
              Decline
            </v-btn>
            <v-btn @click="accept(link)"  v-if="showButtons(link)">
              Accept
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import axios from 'axios'
import { mapGetters } from 'vuex'

export default {
  data () {
    return {
      links: [],
      username: ''
    }
  },
  computed: {
    ...mapGetters([
      'user',
      'userInformation'
    ])
  },
  methods: {
    linkStatusColour (link) {
      return link.pendingResponse ? 'amber lighten-2' : (link.botAccepted && link.channelAccepted) ? 'light-green lighten-2' : 'red lighten-2'
    },
    linkStatus (link) {
      return link.pendingResponse ? 'Pending' : (link.botAccepted && link.channelAccepted) ? 'Active' : 'Rejected'
    },
    showButtons (link) {
      if (this.user.profile.sub === link.channelId && !link.channelAccepted) {
        return true
      }
      if (this.user.profile.sub === link.botId && !link.botAccepted) {
        return true
      }
      return false
    },
    getUser (user) {
      return axios.get(`https://api.twitch.tv/helix/users?login=${user}`, {
        headers: {
          'Client-ID': process.env.VUE_APP_CLIENT_ID
        }
      })
        .then(response => {
          if (response.data.data.length === 0) {
            return null
          }
          return response.data.data[0]
        })
    },
    getLinks () {
      axios.get(`https://localhost:5001/bot`, {
        headers: {
          'Authorization': `Bearer ${this.user.id_token}`
        }
      })
        .then(response => {
          this.links = response.data
        })
    },
    linkRequest (model) {
      axios.post(`https://localhost:5001/bot`, model, {
        headers: {
          'Authorization': `Bearer ${this.user.id_token}`
        }
      })
        .then(response => {
          this.links.push(response.data)
        })
    },
    linkBot () {
      this.getUser(this.username)
        .then(user => {
          if (user !== undefined && user !== null) {
            this.linkRequest({
              channelId: this.user.profile.sub,
              botId: user.id
            })
          }
        })
    },
    linkChannel () {
      this.getUser(this.username)
        .then(user => {
          if (user !== undefined && user !== null) {
            this.linkRequest({
              channelId: user.id,
              botId: this.user.profile.sub
            })
          }
        })
    },
    accept (link) {
      axios.put(`https://localhost:5001/bot`, {
        channelId: link.channelId,
        botId: link.botId,
        accepted: true
      }, {
        headers: {
          'Authorization': `Bearer ${this.user.id_token}`
        }
      })
        .then(response => {
          this.getLinks()
        })
    },
    decline (link) {
      axios.put(`https://localhost:5001/bot`, {
        channelId: link.channelId,
        botId: link.botId,
        accepted: false
      }, {
        headers: {
          'Authorization': `Bearer ${this.user.id_token}`
        }
      })
        .then(response => {
          this.getLinks()
        })
    }
  },
  mounted () {
    this.getLinks()
  }
}
</script>
