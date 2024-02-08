<template>
    <div :style="backgroundStyle"></div>
    <PageLayout>
        <div class="text-center rounded m-5">
            <span class="badge rounded-pill bg-body p-3 w-100">
                <h2><router-link to="/booking" class="nav-link text-warning">Book your flight now!</router-link></h2>
            </span>
        </div>
        <div class="border rounded bg-body m-5">
            <h3 class="text-body mt-3 mx-3">Latest Reviews</h3>
            <Review :name="'Tom Ska'" :text="'I like planes!\n\n(and trains)'" :stars="4" class="m-3" />
            <Review :name="'Bing Chat'" :text="'The booking process was incredibly smooth and intuitive. I\'ve never had an easier time planning a trip!'" :stars="5" class="m-3" />
            <Review :name="'John Doe'" :text="'Could be worse'" :stars="3" class="m-3" />
            <Review :name="'Tele Pete'" :text="'I do not care what anyone says! In my opinion the site is user-friendly and well designed!'" :stars="5" class="m-3" />
            <Review :name="'Tsavi'" :text="'not made with asp.net'" :stars="0" class="m-3" />
        </div>
    </PageLayout>
</template>

<script>
import PageLayout from '@layouts/PageLayout.vue'
import Review from '@components/Review.vue'
import BackgroundImage from '@images/clouds.jpg'

export default {
    components: {
        PageLayout,
        Review
    },
    computed: {
        backgroundStyle() {
            return {
                'background-image': `url(${BackgroundImage})`,
                'background-position': 'center',
                'background-repeat': 'no-repeat',
                'background-size': 'cover',
                'background-attachment': 'fixed',
                'position': 'fixed',
                'width': '100%',
                'height': '100%',
                'z-index': '-1'
            }
        }
    }
}
</script>