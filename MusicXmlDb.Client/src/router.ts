import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router';

const routes: Array<RouteRecordRaw> = [
  { path: '/', component: () => import('./components/Home.vue') },
  { path: '/about', component: () => import('./components/About.vue') },
  { path: '/:pathMatch(.*)*', component: () => import('./components/NotFound.vue')}
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;