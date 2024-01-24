<template>
    <div :id="flightId" class="card" v-if="flight !== null">
        <h4 class="card-header">{{ flight.origin_city }} → {{ flight.destination_city }}</h4>
        <div class="card-body">
            <h5 class="card-title mb-3">{{ flight.airline }}</h5>
            <p class="card-text">Distance: {{ flight.distance }} km</p>
            <p class="card-text">Passengers: {{ adults + children }}</p>
            <p class="card-text">Adults: {{ adults }}</p>
            <p class="card-text">Children: {{ children }}</p>
        </div>
        <p class="card-footer mb-0">Price: {{ calculateCost() }} Ft</p>
    </div>
</template>
  
<script>
import { http } from "@utils/http"
import { ref } from "vue"

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
                    this.flight = response.data[0]
                })
        },
        calculateCost() {
            let baseCostPerPassenger = this.flight.distance * this.flight.huf_per_km
            let totalBaseCost = baseCostPerPassenger * (this.adults + this.children)

            let vat = totalBaseCost * 0.27
            let keroseneTax = this.flight.distance * 0.10
            let destinationPop = this.flight.destination_city_population
            let tourismTaxRate

            if (destinationPop < 2000000) tourismTaxRate = 0.05
            else if (destinationPop < 10000000) tourismTaxRate = 0.075
            else tourismTaxRate = 0.10

            let tourismTax = totalBaseCost * tourismTaxRate
            let totalCost = totalBaseCost + vat + keroseneTax + tourismTax

            if (flight.passengers > 10) totalCost *= 0.90
            if (this.children > 0) totalCost *= 0.80

            return Math.round(totalCost)
        }
    },
    computed: {
        
    }
}
</script>
  