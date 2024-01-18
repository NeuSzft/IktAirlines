import { createRouter, createWebHistory } from 'vue-router';
import Booking from '@components/Booking.vue';

const routes = [
  { path: '/booking', component: Booking },
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;
