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
    path: "/view",
    name: "View",
    component: () => import("./components/View.vue"),
    props: route => ({ id: route.query.id })
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
        component: () => import("./components/manage/Scores.vue")
      },
      {
        path: "edit",
        component: () => import('./components/manage/Edit.vue')
      },
      {
        path: "create",
        component: () => import('./components/manage/Create.vue')
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
    if (to.meta === undefined || !to.meta.requiresAuthentication) {
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
