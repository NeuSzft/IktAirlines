<template>
    <div :id="flightId" class="card w-100" v-if="flight !== null">
        <h4 class="card-header fw-bold">{{ flight.origin_city }} → {{ flight.destination_city }}</h4>
        <div class="card-body">
            <h5 class="card-title mb-3">{{ flight.airline }}</h5>
            <div class="row d-flex">
                <div class="col text-start">
                    <p class="card-text">Distance:</p>
                    <p class="card-text">Price/km:</p>
                    <p class="card-text">Passengers:</p>
                    <p class="card-text">Adults:</p>
                    <p class="card-text">Children: </p>
                </div>
                <div class="col text-end fw-bold">
                    <p class="card-text distance">{{ flight.distance }} km</p>
                    <p class="card-text price-per-km">{{ flight.huf_per_km }} HUF</p>
                    <p class="card-text">{{ adults + children }}</p>
                    <p class="card-text adults-count">{{ adults }}</p>
                    <p class="card-text children-count">{{ children }}</p>
                </div>
            </div>
        </div>
        <p class="card-footer fw-bold mb-0">Price: <span class="ticket-price">{{ price }}</span> HUF</p>
    </div>
</template>
  
<script>
import { http } from "@utils/http"
import { ref } from 'vue'

export default {
    props: [
        'flightId',
        'key',
        'adults',
        'children'
    ],
    mounted() {
        this.getFlight()
        this.getPrice()
    },
    data() {
        return {
            flight: null,
            price: ref(0),
        }
    },
    watch: {
        adults() {
            this.getPrice()
        },
        children() {
            this.getPrice()
        }
    },
    methods: {
        getFlight() {
            http.get(`/flights/${this.flightId}/joined`)
                .then(response => {
                    this.flight = response.data
                })
        },
        getPrice() {
            if (this.adults + this.children < 1) {
                this.price = 0
                return
            }
            
            http.post('/price', {
                id: this.flightId,
                adults: this.adults,
                children: this.children
            })
            .then(response => {
                this.price = response.data
            })
        }
    }
}
</script>
  