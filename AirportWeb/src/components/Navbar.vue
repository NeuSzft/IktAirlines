<template>
    <nav class="navbar navbar-expand-lg bg-body-tertiary">
        <div class="container-fluid">
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav"
                aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav">
                    <li class="nav-item">
                        <router-link to="/" aria-current="page" class="nav-link active">Home</router-link>
                    </li>
                    <li class="nav-item">
                        <router-link to="/booking" aria-current="page" class="nav-link active">Booking</router-link>
                    </li>
                    <li class="nav-item">
                        <router-link to="/summary" aria-current="page" class="nav-link active">Summary</router-link>
                    </li>
                </ul>
            </div>
        </div>
    </nav>
</template>