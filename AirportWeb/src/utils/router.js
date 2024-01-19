import { createRouter, createWebHistory } from 'vue-router'
import Booking from '@components/Booking.vue'
import Summary from '@components/Summary.vue'

const routes = [
  { path: '/booking', component: Booking, meta: { title: 'Booking' } },
  { path: '/summary', component: Summary, meta: { title: 'Summary' } },
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

router.beforeEach((to, from, next) => {
  document.title = to.meta.title || 'Airlines';
  next();
});

export default router;
