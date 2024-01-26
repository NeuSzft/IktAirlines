<template>
    <div :id="flightId" class="card w-100" v-if="flight !== null">
        <h4 class="card-header fw-bold">{{ flight.origin_city }} → {{ flight.destination_city }}</h4>
        <div class="card-body">
            <h5 class="card-title mb-3">{{ flight.airline }}</h5>
            <p class="card-text">Distance: {{ flight.distance }} km</p>
            <p class="card-text">Price/km: {{ flight.huf_per_km }} HUF</p>
            <p class="card-text">Passengers: {{ adults + children }}</p>
            <p class="card-text">Adults: {{ adults }}</p>
            <p class="card-text">Children: {{ children }}</p>
        </div>
        <p class="card-footer mb-0">Price: {{ calculateCost() }} HUF</p>
    </div>
</template>
  
<script>
import { http } from "@utils/http"

export default {
    props: [
        'flightId',
        'key',
        'adults',
        'children'
    ],
    mounted() {
        this.getFlight()
    },
    data() {
        return {
            flight: null
        }
    },
    methods: {
        getFlight() {
            http.get(`/flights/${this.flightId}/joined`)
                .then(response => {
                    this.flight = response.data
                })
        },
        calculateCost() {
            let totalCost = 0
            let baseCostPerPassenger = this.flight.distance * this.flight.huf_per_km

            let totalBaseCostAdult = baseCostPerPassenger * this.adults
            let totalBaseCostChild = baseCostPerPassenger * this.children 

            let destinationPop = this.flight.destination_city_population
            let tourismTaxRate = destinationPop < 2000000 ? 0.05 : destinationPop < 10000000 ? 0.075 : 0.10

            let vatAdult = totalBaseCostAdult * 0.27
            let vatChild = totalBaseCostChild * 0.27
            let keroseneTax = this.flight.distance * 0.10
            let tourismTaxAdult = totalBaseCostAdult * tourismTaxRate
            let tourismTaxChild = totalBaseCostChild * tourismTaxRate

            let flightCostAdult = totalBaseCostAdult + vatAdult + keroseneTax + tourismTaxAdult
            let flightCostChild = totalBaseCostChild + vatChild + keroseneTax + tourismTaxChild

            if (this.adults + this.children > 10) {
                flightCostAdult *= 0.90
                flightCostChild *= 0.90
            }

            totalCost += flightCostAdult + (flightCostChild * 0.8)

            return (this.adults + this.children > 0) ? Math.round(totalCost) : 0
        }
    },
    computed: {

    }
}
</script>
  