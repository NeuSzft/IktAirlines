<template>
    <div id="booking" class="container mt-5">
        <h1 class="text-center mb-4">Flight Booking</h1>
        <form id="bookingForm">
            <div class="row">
                <div class="col-12 col-md-6 mb-3">
                    <label for="originCity" class="form-label">Origin City</label>
                    <select class="form-select" id="originCity" required>
                        <!-- Options -->
                    </select>
                </div>
                <div class="col-12 col-md-6 mb-3">
                    <label for="destinationCity" class="form-label">Destination City</label>
                    <select class="form-select" id="destinationCity" required>
                        <!-- Options -->
                    </select>
                </div>
            </div>
            <div class="mb-3">
                <label for="passengers" class="form-label">Number of Passengers</label>
                <input type="number" class="form-control" id="passengers" min="1" value="1" required>
            </div>
            <button type="submit" class="btn btn-primary">Book Flight</button>
        </form>
    </div>
</template>
  
<script>
export default {
    name: "Booking",
    // Script content like data, methods, etc.
}
</script>