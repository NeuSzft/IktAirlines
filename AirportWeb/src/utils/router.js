import { createRouter, createWebHistory } from 'vue-router'
import Booking from '@components/Booking.vue'
import Summary from '@components/Summary.vue'

const routes = [
  { path: '/booking', component: Booking },
  { path: '/summary', component: Summary },
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;
