<template>
    <div class="card">
        <div class="card-header">
            <b>{{ name }}</b>
        </div>
        <div class="card-body">
            <div :style="multiLine">{{ text }}</div>
        </div>
        <div class="card-footer">
            <i v-for="_ in stars" class="bi bi-star-fill text-warning"></i>
            <i v-for="_ in 5 - stars" class="bi bi-star-fill"></i>
        </div>
    </div>
</template>
  
<script>
export default {
    props: [
        'name',
        'text',
        'stars'
    ],    
    computed: {
        multiLine() {
            return {
                'white-space': 'pre-line'
            }
        }
    }
}
</script>