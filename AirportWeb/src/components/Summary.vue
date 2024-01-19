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
                <th>Price</th>
            </thead>
            <tbody>
                <tr v-for="item of flights" :key="item" :id="item.airline">
                    <td>{{ item.airline }}</td>
                    <td>{{ item.origin }}</td>
                    <td>{{ item.destination }}</td>
                    <td>{{ item.distance }} km</td>
                    <td>{{ item.passengers }}</td>
                    <td>{{ calculateCost(item) }} Ft</td>
                </tr>
            </tbody>
        </table>
    </div>
</template>
  
<script>
const cities = {
    "Budapest": 1800000,
    "London": 8800000,
}

const flights = [
    { airline: "Wizz Air", origin: "Budapest", destination: "London", distance: 1450, flightTime: 145, costPerKm: 6, passengers: 0 },
    { airline: "British Airways", origin: "Budapest", destination: "London", distance: 1200, flightTime: 155, costPerKm: 7, passengers: 0 },
]


export default {
    name: "Summary",
    data() {
        return {
            flights
        }
    },
    methods: {
        calculateCost(flight) {
            let baseCostPerPassenger = flight.distance * flight.costPerKm
            let totalBaseCost = baseCostPerPassenger * flight.passengers

            let vat = totalBaseCost * 0.27
            let keroseneTax = flight.distance * 0.10
            let destinationPop = cities[flight.destination]
            let tourismTaxRate

            if (destinationPop < 2000000) tourismTaxRate = 0.05
            else if (destinationPop < 10000000) tourismTaxRate = 0.075
            else tourismTaxRate = 0.10

            let tourismTax = totalBaseCost * tourismTaxRate
            let totalCost = totalBaseCost + vat + keroseneTax + tourismTax

            // Applying group discount if applicable
            if (flight.passengers > 10) totalCost *= 0.90

            // Assuming a fixed children discount rate for simplicity
            // In a real application, you should calculate this based on the actual number of children

            //totalCost *= 0.80 // Children discount

            return Math.round(totalCost)
        }
    }
}
</script>