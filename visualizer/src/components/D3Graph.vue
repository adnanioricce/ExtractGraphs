<template>
    <div ref="graphContainer" class="graph-container"></div>
  </template>
  
  <script lang="ts">
  import { defineComponent, onMounted, ref } from 'vue';
  import * as d3 from 'd3';
  import { fetchGraphData } from '../services/graph.service';
  import { DefaultGraphRenderer } from "../services/graph.d3renderer";
  
  
  export default defineComponent({
    name: 'D3Graph',
    setup() {
      const graphContainer = ref<HTMLDivElement | null>(null);
      onMounted(async () => {
        const _renderer = DefaultGraphRenderer({
          graphContainer
        })
        const data = await fetchGraphData();        
        _renderer.drawGraph(data);
      });
  
      return {
        graphContainer,
      };
    },
  });
  </script>
  
  <style scoped>
  .graph-container {
    width: 100%;
    height: 100%;
  }
  /* .links {
    
  } */
  </style>
  