<template>
    <div id="booking" class="container mt-5">
        <h1 class="text-center mb-4">Summary</h1>
        <table class="table table-striped">
            <thead>
                <th>Airline</th>
                <th>Origin</th>
                <th>Destination</th>
                <th>Distance</th>
                <th>Passengers</th>
                <th>Children</th>
                <th>Price</th>
            </thead>
            <tbody>
                <tr v-for="item of flights" :key="item" :id="item.id">
                    <td>{{ item.airline }}</td>
                    <td>{{ item.origin_city }}</td>
                    <td>{{ item.destination_city }}</td>
                    <td>{{ item.distance }} km</td>
                    <td>{{ 0 }}</td>
                    <td>{{ 0 }}</td>
                    <td>{{ calculateCost(item) }} Ft</td>
                </tr>
            </tbody>
        </table>
    </div>
</template>
  
<script>
import { ref } from 'vue'

export default {
    data() {
        return {
            flights: ref([])
        }
    }
}
</script>