<template>
    <div id="booking" class="container mt-5">
        <h1 class="text-center mb-4">Flight Booking</h1>
        <form id="bookingForm" @submit.prevent="bookTicket">
            <div class="row">
                <div class="col-12 col-md-6 mb-3">
                    <label for="originCity" class="form-label">Origin City</label>
                    <select class="form-select" id="originCity" v-model="origin" required>
                        <option v-for="country of countriesOrigin" :key="country" :value="country">{{ country }}</option>
                    </select>
                </div>
                <div class="col-12 col-md-6 mb-3">
                    <label for="destinationCity" class="form-label">Destination City</label>
                    <select class="form-select" id="destinationCity" v-model="destination" :disabled="origin === ''"
                        required>
                        <option v-for="country of countriesDestination" :key="country" :value="country">{{ country }}
                        </option>
                    </select>
                </div>
            </div>
            <div class="row mt-5">
                <div class="col-12 col-md-6 mb-3">
                    <label for="passengers" class="form-label">Number of adults</label>
                    <input type="number" class="form-control" id="passengers" v-model="adultsCount" min="0" required>
                </div>
                <div class="col-12 col-md-6 mb-3">
                    <label for="passengers" class="form-label">Number of children</label>
                    <input type="number" class="form-control" id="passengers" v-model="childrenCount" min="0" required>
                </div>
            </div>
            <button type="submit" class="btn btn-primary d-block float-end my-3">Book a flight</button>
        </form>

        <div id="flight" class="container d-flex flex-wrap justify-content-around" v-if="destination !== null">
            <template v-for="route in routes.filter(x => x.final === destination)" :key="route.final">
                <Ticket v-for="id in route.flightIds" :key="id" :flightId="id" :adults="adultsCount"
                    :children="childrenCount" />
            </template>
        </div>
    </div>
</template>
  
<script>
import { ref, toRaw } from 'vue'
import { http } from "@utils/http"
import Ticket from "@components/Ticket.vue"

export default {
    mounted() {
        this.getFlights()
    },
    data() {
        return {
            flights: new Array(),
            countriesOrigin: new Set(),
            countriesDestination: new Set(),
            origin: ref(''),
            destination: ref(''),
            adultsCount: ref(1),
            childrenCount: ref(0),
            passengerCount: ref(0),
            price: ref(0),
            routes: new Array()
        }
    },
    watch: {
        adultsCount(value) {
            this.adultsCount = value
            this.passengerCount = this.adultsCount + this.childrenCount
        },
        childrenCount(value) {
            this.childrenCount = value
            this.passengerCount = this.adultsCount + this.childrenCount
        },
        passengerCount(value) {
            this.passengerCount = value
        },
        origin(value) {
            this.origin = value
            this.destination = null
            this.filteredDestinations()
        },
        destination(value) {
            this.destination = value
        }
    },
    methods: {
        filteredDestinations() {
            const destinations = new Array();
            const routes = new Array();

            for (const flight of this.flights) {
                if (flight.origin_city === this.origin) {
                    destinations.push(flight.destination_city);
                    routes.push({ final: flight.destination_city, transfers: [], flightIds: [flight.id] });
                }
            }

            for (const flight1 of this.flights) {
                if (flight1.origin_city === this.origin) {
                    for (const flight2 of this.flights) {
                        if (flight1.destination_city === flight2.origin_city) {

                            destinations.push(flight2.destination_city);
                            routes.push({ final: flight2.destination_city, transfers: [flight1.destination_city], flightIds: [flight1.id, flight2.id] });

                            for (const flight3 of this.flights) {
                                if (flight2.destination_city === flight3.origin_city) {
                                    destinations.push(flight3.destination_city);
                                    routes.push({ final: flight3.destination_city, transfers: [flight1.destination_city, flight2.destination_city], flightIds: [flight1.id, flight2.id, flight3.id] });
                                }
                            }
                        }
                    }
                }
            }

            this.countriesDestination = new Set([...destinations].sort());
            this.routes = routes;
            if (this.destination !== null) {
                this.routes = routes.filter(route => route.final === this.destination);
            }
        },
        calculateCost(flights) {
            let totalCost = 0
            for (const id of toRaw(flights.flightIds)) {
                const flight = toRaw(this.flights.find(x => x.id === id))

            let baseCostPerPassenger = flight.distance * flight.huf_per_km

                let totalBaseCostAdult = baseCostPerPassenger * this.adultsCount
                let totalBaseCostChild = baseCostPerPassenger * this.childrenCount

                let destinationPop = flight.destination_city_population
                let tourismTaxRate = destinationPop < 2000000 ? 0.05 : destinationPop < 10000000 ? 0.075 : 0.10

                let vatAdult = totalBaseCostAdult * 0.27
                let vatChild = totalBaseCostChild * 0.27
            let keroseneTax = flight.distance * 0.10
                let tourismTaxAdult = totalBaseCostAdult * tourismTaxRate
                let tourismTaxChild = totalBaseCostChild * tourismTaxRate

                let flightCostAdult = totalBaseCostAdult + vatAdult + keroseneTax + tourismTaxAdult
                let flightCostChild = totalBaseCostChild + vatChild + keroseneTax + tourismTaxChild

                if (flight.passengerCount > 10) {
                    flightCostAdult *= 0.90
                    flightCostChild *= 0.90
                }

                totalCost += flightCostAdult + (flightCostChild * 0.8)
            }

            return (this.adultsCount + this.childrenCount > 0) ? Math.round(totalCost) : 0
        },
        getFlights() {
            http.get("/flights/joined")
                .then(response => {
                    this.flights = response.data
                    const countriesOrigin = []
                    const countriesDestination = []

                    for (const flight of response.data) {
                        countriesOrigin.push(flight.origin_city)
                        countriesDestination.push(flight.destination_city)
                    }

                    this.countriesOrigin = new Set([...countriesOrigin].sort())
                    this.countriesDestination = new Set([...countriesDestination].sort())
                })
        },
        bookTicket() {

        }
    },
    components: {
        Ticket
    }
}
</script>