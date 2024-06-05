import { Ref } from 'vue';
import { GraphData } from './graph.service';
import * as d3 from 'd3';
export interface GraphRenderer {
    drawGraph:(data:GraphData) => void
}
export interface IGraphRendererArgs {
    graphContainer:Ref<HTMLDivElement | null>
}

export function DefaultGraphRenderer(args:IGraphRendererArgs) {
    const { graphContainer } = args
    const createGraph = (data: GraphData) => {                
        const width = screen.availWidth;
        const height = screen.availWidth;
        
        const svg = d3.select(graphContainer.value)
        .append('svg')
        .attr('width', width)
        .attr('height', height);

        const nodes = Object.keys(data).map((id,index) => ({ id}));
        const links = Object.entries(data).flatMap(([source, targets]) =>
            targets.map(target => ({ source, target}))
        );        
        const simulation = d3.forceSimulation(nodes)
        .force('link', d3.forceLink(links).id((d: any) => d.id))
        .force('charge', d3.forceManyBody())          
        .force('center',d3.forceCenter(width / 2,height / 2))
        .force('collide', d3.forceCollide().radius(32));
        // .force('mouse',mouseRepulsionForce());
        // this is too caotic, need a way to update without tearing everything down
        // svg.on("mousemove",function (event:any) {
        //   const [mouseX,mouseY] = d3.pointer(event)
        //   simulation.force("mouse").strength(1).x(mouseX).y(mouseY)
        //   simulation.alphaTarget(0.3).restart()
        // })
        const link = svg.append('g')
        .attr('class', 'links')
        .selectAll('line')
        .data(links)
        .enter().append('line')
        .attr('stroke-width', 1)
        .attr('stroke',d3.color('hsl(120, 50%, 20%)').rgb());
        // .attr('color');

        const node = svg.append('g')
        .attr('class', 'nodes')
        .selectAll('circle')
        .data(nodes)
        .enter().append('circle')
        .attr('r', 10)
        .attr('fill', 'blue');

        node.append('title')
        .text((d: any) => d.id);
        
        simulation.on('tick', () => {
        
        link
            .attr('x1', (d: any) => d.source.x)
            .attr('y1', (d: any) => d.source.y)
            .attr('x2', (d: any) => d.target.x)
            .attr('y2', (d: any) => d.target.y);

        node
            .attr('cx', (d: any) => d.x)
            .attr('cy', (d: any) => d.y);
        
        });
    };
    return {
        drawGraph:createGraph
    };
}

// interface Node {
//     id: number;
//     x?: number;
//     y?: number;
//     vx?: number;
//     vy?: number;
//   }
//   function mouseRepulsionForce() {
//     let nodes: Node[];
//     let mouseX: number = 0;
//     let mouseY: number = 0;
//     let strength: number = 1;

//     function force(alpha: number) {
//       for (const node of nodes) {
//         const dx = node.x! - mouseX;
//         const dy = node.y! - mouseY;
//         const distance = Math.sqrt(dx * dx + dy * dy);
//         const forceStrength = strength * alpha;

//         if (distance < 100) {
//           const force = (100 - distance) * forceStrength;
//           node.vx! += dx / distance * force;
//           node.vy! += dy / distance * force;
//         }
//       }
//     }

//     force.initialize = function(_: Node[]) {
//       nodes = _;
//     };

//     force.x = function(_: number) {
//       mouseX = _;
//       return force;
//     };

//     force.y = function(_: number) {
//       mouseY = _;
//       return force;
//     };

//     force.strength = function(_: number) {
//       strength = _;
//       return force;
//     };

//     return force;
//   }