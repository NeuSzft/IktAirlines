import { createRouter, createWebHistory } from 'vue-router'
import Booking from '@pages/Booking.vue'
import Summary from '@pages/Summary.vue'
import Home from '@pages/Home.vue'

const routes = [
  { path: '/', component: Home, meta: { title: 'Home' } },
  { path: '/booking', component: Booking, meta: { title: 'Booking' } },
  { path: '/summary', component: Summary, meta: { title: 'Summary' } },
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

router.beforeEach((to, from, next) => {
  document.title = 'Airlines | ' + to.meta.title || 'Airlines';
  next();
});

export default router;
