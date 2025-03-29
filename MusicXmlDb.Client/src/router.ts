import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router';
import { keycloak } from './services/auth';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    component: () => import('./components/Home.vue')
  },
  {
    path: '/about',
    component: () => import('./components/About.vue')
  },
  {
    path: '/preview',
    component: () => import('./components/Preview.vue')
  },
  {
    path: '/scores',
    meta:
    {
      requiresAuthentication: true
    },
    children: [
      {
        path: "",
        component: () => import("./components/scores/Scores.vue")
      },
      {
        path: "view",
        component: () => import('./components/scores/View.vue')
      }
    ]
  },
  {
    path: '/:pathMatch(.*)*',
    component: () => import('./components/NotFound.vue')
  }
];

const initializeRouter = () => {
  const router = createRouter({
    history: createWebHistory(),
    routes,
  });

  router.beforeEach((to, from, next) => {
    if (!(to.meta?.requiresAuthentication ?? false)) {
      return next();
    }

    if (keycloak.authenticated) {
      return next()
    }

    const redirectUri = window.location.origin + to.path
    keycloak.login({ redirectUri: redirectUri })
  })

  return router;
}

export default initializeRouter;
