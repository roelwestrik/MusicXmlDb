import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import { keycloak } from "./services/auth"
import initializeRouter from './router'

const app = createApp(App)

keycloak.init({ onLoad: "check-sso", checkLoginIframe: false }).then(() => {

    app.use(initializeRouter())
    app.mount('#app')
    
});
