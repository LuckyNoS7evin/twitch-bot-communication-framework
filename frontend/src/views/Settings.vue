<template>
  <v-container fluid v-if="userInformation != null">
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
            <v-toolbar-title> API testing &amp; documentation</v-toolbar-title>
            <v-spacer></v-spacer>
          </v-toolbar>
          <v-container>
            <v-row>
              <v-col cols="4">
                Swagger UI URL
              </v-col>
              <v-col cols="8">
                <a :href="swaggerUrl" target="_blank">{{swaggerUrl}}</a>
              </v-col>
            </v-row>
            <v-row>
              <v-col cols="4">
                JWT (this test token lasts 30 minutes from page refresh)
              </v-col>
              <v-col cols="6">
                <v-text-field
                  :value="apiJwt"
                  type="password"
                  single-line
                  disabled
                ></v-text-field>
              </v-col>
              <v-col cols="2">
                <v-btn block color="primary" v-clipboard:copy="apiJwt">copy</v-btn>
              </v-col>
            </v-row>
            <v-row>
              <v-col cols="4">
                Your JWT Header
              </v-col>
              <v-col cols="8">
                <code>{<br/>
                    "alg": "HS256",<br/>
                    "typ": "JWT",<br/>
                    "kid": "{{userInformation.clientId}}"<br/>
                  }
                </code>
              </v-col>
            </v-row>
             <v-row>
              <v-col cols="4">
                Example JWT Body
              </v-col>
              <v-col cols="8">
                <code>{<br/>
                    "iss": "https://botcommunicationframework.com",<br/>
                    "aud": "{{userInformation.clientId}}",<br/>
                    "sub": "{{userInformation.userId}}",<br/>
                    "iat": {{Math.floor(Date.now() / 1000)}},<br/>
                    "exp": {{Math.floor(Date.now() / 1000) + 60}}<br/>
                  }
                </code>
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
                <v-alert type="info" text>
                  <p>You only need to connect a bot to a channel account if you are using the whisper system of BCF, this is to avoid your account getting too many whispers from other bot accounts.</p>
                  <p>Twitch have changed the policy on bot initiated whispers, which restricts whispers initiated from 3rd party chat clients (or seems to have). Bot initiated whispers from 3rd party chat clients are only available to known or verified bots.</p>
                  <p>S7evbot will act as a middleman bot in the BCF as it is a verified bot. This will happen when we build the whisper system</p>
                </v-alert>
              </v-col>
            </v-row>
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
import { mapGetters, mapState } from 'vuex'

export default {
  data () {
    return {
      links: [],
      username: ''
    }
  },
  computed: {
    ...mapState([
      'swaggerUrl'
    ]),
    ...mapGetters([
      'user',
      'userInformation',
      'apiJwt'
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
      axios.get(`${process.env.VUE_APP_API_ENDPOINT}bot`, {
        headers: {
          'Authorization': `Bearer ${this.user.id_token}`
        }
      })
        .then(response => {
          this.links = response.data
        })
    },
    linkRequest (model) {
      axios.post(`${process.env.VUE_APP_API_ENDPOINT}bot`, model, {
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
      axios.put(`${process.env.VUE_APP_API_ENDPOINT}bot`, {
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
      axios.put(`${process.env.VUE_APP_API_ENDPOINT}/bot`, {
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

<style scoped>
code {
  font-family: monospace;
}
</style>
